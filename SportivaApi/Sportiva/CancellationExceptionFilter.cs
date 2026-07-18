using Microsoft.AspNetCore.Mvc.Filters;

// CancellationExceptionFilter.cs
public class CancellationExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is OperationCanceledException)
        {
            context.Result = new StatusCodeResult(499);
            context.ExceptionHandled = true;
        }
        return Task.CompletedTask;
    }
}