using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DeployBot.Features.Shared.Exceptions
{
    internal class HttpExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<HttpExceptionMiddleware> _logger;

        public HttpExceptionMiddleware(ILogger<HttpExceptionMiddleware> logger)
        {
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                _logger.LogWarning(httpException, "An unexpected occurred while trying to invoke {0}.", context.Request.Path);

                context.Response.StatusCode = (int)httpException.StatusCode;
                await context.Response.WriteAsync(httpException.Message);
                await context.Response.CompleteAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unexpected error occurred while trying to invoke {0}.", context.Request.Path);
                throw exception;
            }
        }
    }
}