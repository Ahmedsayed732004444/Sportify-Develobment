using Microsoft.AspNetCore.Mvc;

namespace Sportiva.Abstractions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to a problem");

        var problemDetails = new ProblemDetails
        {
            Status = result.Error.StatusCode,
            Title = result.Error.Code,
            Detail = result.Error.Description
        };

        problemDetails.Extensions.Add("errors", new[]
        {
            result.Error.Code,
            result.Error.Description
        });

        return new ObjectResult(problemDetails)
        {
            StatusCode = result.Error.StatusCode
        };
    }
}