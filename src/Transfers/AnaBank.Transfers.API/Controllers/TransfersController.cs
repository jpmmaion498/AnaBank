using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AnaBank.Transfers.Application.Commands.MakeTransfer;
using AnaBank.BuildingBlocks.Web.ProblemDetails;
using System.Security.Claims;

namespace AnaBank.Transfers.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransfersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Efetua transfer�ncia entre contas da mesma institui��o
    /// </summary>
    /// <param name="request">Dados da transfer�ncia</param>
    /// <returns>Sucesso sem conte�do</returns>
    /// <response code="204">Transfer�ncia realizada com sucesso</response>
    /// <response code="400">Dados inv�lidos</response>
    /// <response code="403">Token inv�lido ou expirado</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MakeTransfer([FromBody] MakeTransferRequest request)
    {
        try
        {
            var originAccountId = GetCurrentAccountId();
            var command = new MakeTransferCommand(request.DestinationAccountNumber, request.Value, originAccountId);
            await _mediator.Send(command);
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            var problem = ex.Message switch
            {
                "INVALID_ACCOUNT" => CreateValidationProblem(
                    ErrorTypes.InvalidAccount, "Conta Inv�lida", "A conta n�o existe"),
                "INACTIVE_ACCOUNT" => CreateValidationProblem(
                    ErrorTypes.InactiveAccount, "Conta Inativa", "A conta est� inativa"),
                "INVALID_VALUE" => CreateValidationProblem(
                    ErrorTypes.InvalidValue, "Valor Inv�lido", "O valor deve ser maior que zero"),
                "INSUFFICIENT_FUNDS" => CreateValidationProblem(
                    ErrorTypes.InsufficientFunds, "Saldo Insuficiente", "Saldo insuficiente para transfer�ncia"),
                _ => CreateValidationProblem(
                    "TRANSFER_ERROR", "Erro na Transfer�ncia", ex.Message)
            };
            
            return BadRequest(problem);
        }
    }

    private string GetCurrentAccountId()
    {
        var subClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(subClaim))
            throw new UnauthorizedAccessException("Token inv�lido");
        
        return subClaim;
    }

    private Microsoft.AspNetCore.Mvc.ProblemDetails CreateValidationProblem(string type, string title, string detail)
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = type,
            Title = title,
            Detail = detail,
            Status = 400
        };
    }
}

// DTOs para requests
public record MakeTransferRequest(string DestinationAccountNumber, decimal Value);