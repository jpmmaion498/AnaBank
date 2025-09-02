using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AnaBank.Accounts.Application.Commands.RegisterAccount;
using AnaBank.Accounts.Application.Commands.Login;
using AnaBank.Accounts.Application.Commands.DeactivateAccount;
using AnaBank.Accounts.Application.Commands.MakeMovement;
using AnaBank.Accounts.Application.Queries.GetBalance;
using AnaBank.BuildingBlocks.Web.ProblemDetails;
using System.Security.Claims;

namespace AnaBank.Accounts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Cadastra uma nova conta corrente
    /// </summary>
    /// <param name="request">Dados da conta</param>
    /// <returns>Dados da conta criada</returns>
    /// <response code="201">Conta criada com sucesso</response>
    /// <response code="400">Dados inv�lidos - CPF inv�lido</response>
    [HttpPost]
    [ProducesResponseType(typeof(RegisterAccountResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
    {
        try
        {
            var command = new RegisterAccountCommand(request.Name, request.Cpf, request.Password);
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetBalance), new { }, result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "INVALID_DOCUMENT")
        {
            var problem = CreateValidationProblem(
                ErrorTypes.InvalidDocument,
                "CPF Inv�lido",
                "O CPF informado n�o � v�lido");
            return BadRequest(problem);
        }
        catch (InvalidOperationException ex) when (ex.Message == "CPF j� cadastrado")
        {
            var problem = CreateValidationProblem(
                ErrorTypes.InvalidDocument,
                "CPF j� cadastrado",
                "J� existe uma conta com este CPF");
            return BadRequest(problem);
        }
    }

    /// <summary>
    /// Realiza login na conta
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Credenciais inv�lidas</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var command = new LoginCommand(request.CpfOrNumber, request.Password);
            var result = await _mediator.Send(command);
            
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            var problem = CreateUnauthorizedProblem();
            return Unauthorized(problem);
        }
    }

    /// <summary>
    /// Desativa a conta corrente
    /// </summary>
    /// <param name="request">Senha para confirma��o</param>
    /// <returns>Sucesso sem conte�do</returns>
    /// <response code="204">Conta desativada com sucesso</response>
    /// <response code="403">Token inv�lido ou expirado</response>
    [HttpPost("deactivate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Deactivate([FromBody] DeactivateAccountRequest request)
    {
        var accountId = GetCurrentAccountId();
        var command = new DeactivateAccountCommand(accountId, request.Password);
        await _mediator.Send(command);
        
        return NoContent();
    }

    /// <summary>
    /// Realiza movimenta��o na conta (cr�dito ou d�bito)
    /// </summary>
    /// <param name="request">Dados da movimenta��o</param>
    /// <returns>Sucesso sem conte�do</returns>
    /// <response code="204">Movimenta��o realizada com sucesso</response>
    /// <response code="400">Dados inv�lidos</response>
    /// <response code="403">Token inv�lido ou expirado</response>
    [HttpPost("movements")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MakeMovement([FromBody] MakeMovementRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new MakeMovementCommand(request.AccountNumber, request.Type, request.Value, accountId);
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
                "INVALID_TYPE" => CreateValidationProblem(
                    ErrorTypes.InvalidType, "Tipo Inv�lido", "O tipo deve ser 'C' ou 'D'"),
                "INSUFFICIENT_FUNDS" => CreateValidationProblem(
                    ErrorTypes.InsufficientFunds, "Saldo Insuficiente", "Saldo insuficiente para d�bito"),
                _ => CreateValidationProblem(
                    "VALIDATION_ERROR", "Erro de Valida��o", ex.Message)
            };
            
            return BadRequest(problem);
        }
    }

    /// <summary>
    /// Consulta o saldo da conta
    /// </summary>
    /// <returns>Dados do saldo da conta</returns>
    /// <response code="200">Saldo consultado com sucesso</response>
    /// <response code="400">Conta inv�lida ou inativa</response>
    /// <response code="403">Token inv�lido ou expirado</response>
    [HttpGet("balance")]
    [Authorize]
    [ProducesResponseType(typeof(GetBalanceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetBalance()
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var query = new GetBalanceQuery(accountId);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            var problem = ex.Message switch
            {
                "INVALID_ACCOUNT" => CreateValidationProblem(
                    ErrorTypes.InvalidAccount, "Conta Inv�lida", "A conta n�o existe"),
                "INACTIVE_ACCOUNT" => CreateValidationProblem(
                    ErrorTypes.InactiveAccount, "Conta Inativa", "A conta est� inativa"),
                _ => CreateValidationProblem(
                    "VALIDATION_ERROR", "Erro de Valida��o", ex.Message)
            };
            
            return BadRequest(problem);
        }
    }

    private string GetCurrentAccountId()
    {
        try
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Usu�rio n�o autenticado");
            }

            var accountId = User.FindFirst("sub")?.Value ??
                           User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                           User.FindFirst("accountId")?.Value;
            
            if (string.IsNullOrEmpty(accountId))
            {
                var claims = User.Claims.ToList();
                throw new UnauthorizedAccessException($"Token inv�lido - nenhum claim de identifica��o encontrado. Claims dispon�veis: {string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}"))}");
            }
            
            return accountId;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"Token inv�lido: {ex.Message}");
        }
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

    private Microsoft.AspNetCore.Mvc.ProblemDetails CreateUnauthorizedProblem()
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = ErrorTypes.UserUnauthorized,
            Title = "Unauthorized",
            Detail = "Invalid credentials",
            Status = 401
        };
    }
}

// DTOs para requests
public record RegisterAccountRequest(string Name, string Cpf, string Password);
public record LoginRequest(string CpfOrNumber, string Password);
public record MakeMovementRequest(string? AccountNumber, string Type, decimal Value);
public record DeactivateAccountRequest(string Password);