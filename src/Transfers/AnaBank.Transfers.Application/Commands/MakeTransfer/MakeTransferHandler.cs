using AnaBank.Transfers.Application.Commands.MakeTransfer;
using AnaBank.Transfers.Domain.Entities;
using AnaBank.Transfers.Domain.Interfaces;
using AnaBank.Transfers.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using MediatR;

namespace AnaBank.Transfers.Application.Commands.MakeTransfer;

public class MakeTransferHandler : IRequestHandler<MakeTransferCommand>
{
    private readonly ITransferRepository _transferRepository;
    private readonly IAccountsClient _accountsClient;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<MakeTransferHandler> _logger;

    public MakeTransferHandler(
        ITransferRepository transferRepository,
        IAccountsClient accountsClient,
        IKafkaProducerService kafkaProducerService,
        ILogger<MakeTransferHandler> logger)
    {
        _transferRepository = transferRepository;
        _accountsClient = accountsClient;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

    public async Task Handle(MakeTransferCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing transfer from {request.OriginAccountId} to {request.DestinationAccountNumber} of {request.Value:C}");

        var transferId = Guid.NewGuid().ToString();

        try
        {
            // 1. Debitar da conta de origem
            await _accountsClient.DebitAsync(request.OriginAccountId, request.Value, request.AuthToken);
            _logger.LogInformation($"Debited {request.Value:C} from account {request.OriginAccountId}");

            // 2. Creditar na conta de destino
            await _accountsClient.CreditAsync(request.DestinationAccountNumber, request.Value, request.AuthToken);
            _logger.LogInformation($"Credited {request.Value:C} to account {request.DestinationAccountNumber}");

            // 3. Registrar transferência
            var transfer = new Transfer(
                request.OriginAccountId,
                request.DestinationAccountNumber,
                request.Value);

            await _transferRepository.CreateAsync(transfer);
            _logger.LogInformation($"Transfer registered with ID: {transfer.Id}");

            // 4. Publicar evento no Kafka para processamento de tarifas
            await _kafkaProducerService.PublishTransferCompletedAsync(
                transferId,
                request.OriginAccountId,
                request.Value,
                DateTime.UtcNow);
            
            _logger.LogInformation($"Transfer completed event published to Kafka for account {request.OriginAccountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing transfer from {request.OriginAccountId} to {request.DestinationAccountNumber}");

            // Em caso de erro, tentar estornar o débito
            try
            {
                await _accountsClient.CreditAsync(request.OriginAccountId, request.Value, request.AuthToken);
                _logger.LogInformation($"Reversal completed for account {request.OriginAccountId}");
            }
            catch (Exception reversalEx)
            {
                _logger.LogError(reversalEx, $"Failed to reverse debit for account {request.OriginAccountId}");
            }

            throw;
        }
    }
}