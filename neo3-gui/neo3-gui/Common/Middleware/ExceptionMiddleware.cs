using Microsoft.AspNetCore.Http;
using Neo.Exceptions;
using Neo.Services.Abstractions;

namespace Neo.Common.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly INeoLogger _logger;

        public ExceptionMiddleware(RequestDelegate next, INeoLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NeoGuiException ex)
            {
                _logger.LogWarning("Business error: {Code} - {Message}", 
                    ex.ErrorCode, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = 500;
            
            var error = ex is NeoGuiException neoEx 
                ? neoEx.ToWsError() 
                : new Models.WsError { Code = 500, Message = ex.Message };
            
            await ctx.Response.WriteAsJsonAsync(error);
        }
    }
}
