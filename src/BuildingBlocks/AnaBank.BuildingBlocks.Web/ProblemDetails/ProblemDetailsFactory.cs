using Microsoft.AspNetCore.Mvc;

namespace AnaBank.BuildingBlocks.Web.ProblemDetails;

public static class ProblemDetailsFactory
{
    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateValidationProblem(string type, string title, string detail)
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = type,
            Title = title,
            Detail = detail,
            Status = 400
        };
    }

    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateUnauthorizedProblem()
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = ErrorTypes.UserUnauthorized,
            Title = "Unauthorized",
            Detail = "Invalid credentials",
            Status = 401
        };
    }

    public static Microsoft.AspNetCore.Mvc.ProblemDetails CreateForbiddenProblem()
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Type = "FORBIDDEN",
            Title = "Forbidden",
            Detail = "Invalid or expired token",
            Status = 403
        };
    }
}