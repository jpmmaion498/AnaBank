using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AnaBank.BuildingBlocks.Web.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verificar se é uma operação que deve ser idempotente (POST, PUT, PATCH)
        if (!ShouldCheckIdempotency(context.Request))
        {
            await _next(context);
            return;
        }

        var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            await _next(context);
            return;
        }

        // Tentar obter resposta cached
        var idempotencyService = context.RequestServices.GetService<IIdempotencyService>();
        if (idempotencyService != null)
        {
            var cachedResponse = await idempotencyService.GetResponseAsync(idempotencyKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            // Interceptar resposta para salvar
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Salvar resposta se foi bem-sucedida
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                
                await idempotencyService.SaveResponseAsync(idempotencyKey, responseText);
                
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            else
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private static bool ShouldCheckIdempotency(HttpRequest request)
    {
        return request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH";
    }
}

public interface IIdempotencyService
{
    Task<string?> GetResponseAsync(string key);
    Task SaveResponseAsync(string key, string response);
}

public static class IdempotencyMiddlewareExtensions
{
    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IdempotencyMiddleware>();
    }
}