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
    /// Efetua transferência entre contas da mesma instituição
    /// </summary>
    /// <param name="request">Dados da transferência</param>
    /// <returns>Sucesso sem conteúdo</returns>
    /// <response code="204">Transferência realizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="403">Token inválido ou expirado</response>
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
            var authToken = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            var command = new MakeTransferCommand(request.DestinationAccountNumber, request.Value, originAccountId, authToken);
            await _mediator.Send(command);
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            var problem = ex.Message switch
            {
                "INVALID_ACCOUNT" => CreateValidationProblem(
                    ErrorTypes.InvalidAccount, "Conta Inválida", "A conta não existe"),
                "INACTIVE_ACCOUNT" => CreateValidationProblem(
                    ErrorTypes.InactiveAccount, "Conta Inativa", "A conta está inativa"),
                "INVALID_VALUE" => CreateValidationProblem(
                    ErrorTypes.InvalidValue, "Valor Inválido", "O valor deve ser maior que zero"),
                "INSUFFICIENT_FUNDS" => CreateValidationProblem(
                    ErrorTypes.InsufficientFunds, "Saldo Insuficiente", "Saldo insuficiente para transferência"),
                _ => CreateValidationProblem(
                    "TRANSFER_ERROR", "Erro na Transferência", ex.Message)
            };
            
            return BadRequest(problem);
        }
    }

    private string GetCurrentAccountId()
    {
        var accountId = User.FindFirst("sub")?.Value ??
                       User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                       User.FindFirst("accountId")?.Value;
        
        if (string.IsNullOrEmpty(accountId))
        {
            throw new UnauthorizedAccessException("Token inválido - nenhum claim de identificação encontrado");
        }
        
        return accountId;
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

public record MakeTransferRequest(string DestinationAccountNumber, decimal Value);