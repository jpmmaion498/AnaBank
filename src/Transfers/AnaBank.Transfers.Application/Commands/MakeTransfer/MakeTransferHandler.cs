using MediatR;
using AnaBank.Transfers.Domain.Entities;
using AnaBank.Transfers.Domain.Interfaces;

namespace AnaBank.Transfers.Application.Commands.MakeTransfer;

public class MakeTransferHandler : IRequestHandler<MakeTransferCommand>
{
    private readonly ITransferRepository _transferRepository;
    private readonly IAccountsClient _accountsClient;

    public MakeTransferHandler(
        ITransferRepository transferRepository,
        IAccountsClient accountsClient)
    {
        _transferRepository = transferRepository ?? throw new ArgumentNullException(nameof(transferRepository));
        _accountsClient = accountsClient ?? throw new ArgumentNullException(nameof(accountsClient));
    }

    public async Task Handle(MakeTransferCommand request, CancellationToken cancellationToken)
    {
        // Valida��es iniciais
        if (request.Value <= 0)
            throw new InvalidOperationException("INVALID_VALUE");

        if (!int.TryParse(request.DestinationAccountNumber, out var destinationAccountNumber))
            throw new InvalidOperationException("INVALID_ACCOUNT");

        // Obter token do contexto HTTP (ser� injetado via middleware)
        var authToken = GetCurrentAuthToken();

        try
        {
            // 1. Realizar d�bito na conta de origem
            await _accountsClient.DebitAsync(request.OriginAccountId, request.Value, authToken);

            try
            {
                // 2. Realizar cr�dito na conta de destino
                await _accountsClient.CreditAsync(request.DestinationAccountNumber, request.Value, authToken);

                // 3. Persistir transfer�ncia com sucesso
                var transfer = new Transfer(request.OriginAccountId, request.DestinationAccountNumber, request.Value);
                await _transferRepository.CreateAsync(transfer);

                // TODO: Publicar evento no Kafka para Tarifas (opcional)
                // await PublishTransferCompletedEvent(transfer);
            }
            catch (Exception)
            {
                // Em caso de falha no cr�dito, fazer estorno (cr�dito de volta na origem)
                try
                {
                    await _accountsClient.CreditAsync(request.OriginAccountId, request.Value, authToken);
                }
                catch
                {
                    // Log da falha no estorno, mas n�o parar o fluxo
                }

                throw new InvalidOperationException("Falha na transfer�ncia - cr�dito n�o realizado");
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INVALID_ACCOUNT"))
        {
            throw new InvalidOperationException("INVALID_ACCOUNT");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INACTIVE_ACCOUNT"))
        {
            throw new InvalidOperationException("INACTIVE_ACCOUNT");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("INSUFFICIENT_FUNDS"))
        {
            throw new InvalidOperationException("INSUFFICIENT_FUNDS");
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Falha na transfer�ncia");
        }
    }

    private string GetCurrentAuthToken()
    {
        // Esta implementa��o ser� injetada via middleware ou contexto HTTP
        // Por enquanto, retorna uma string vazia que ser� substitu�da
        return string.Empty;
    }
}