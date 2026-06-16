using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Formatters;
using Moji.BusinessLogic.Exceptions;

namespace Moji.API.Extensions;

public static class ErrorHandlingExtensions
{
    public static void UseApplicationExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exeptionHandlerApp =>
        {
            exeptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;
                // Trích xuất lỗi thô từ bộ nhớ RAM của hệ thống
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var statusCode = StatusCodes.Status500InternalServerError;
                var message = "Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau!";
                var title = "Internal Server Error";
                string? stackTrace = null;
                
                if (exceptionFeature?.Error != null)
                {
                    var exception = exceptionFeature.Error;
                    if (exceptionFeature?.Error is MojiBadRequestException badEx)
                    {
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Business Logic Error";
                        message = badEx.Message;
                    }
                    else if (exception is MojiUnauthorizedException unAuthEx)
                    {
                        statusCode = StatusCodes.Status401Unauthorized;
                        title = "Unauthorized";
                        message = unAuthEx.Message;
                    }
                    else if (exception is MojiForbiddenException forbiddenEx)
                    {
                        statusCode = StatusCodes.Status403Forbidden;
                        title = "Forbidden";
                        message = forbiddenEx.Message;
                    }
                    else if (exception is MojiConflictException conflictEx)
                    {
                        statusCode = StatusCodes.Status409Conflict;
                        title = "Data Conflict";
                        message = conflictEx.Message;
                    }
                    else if (exception is MojiNotFoundException notFoundEx)
                    {
                        statusCode = StatusCodes.Status404NotFound;
                        title = "Not Found";
                        message = notFoundEx.Message;
                    }
                    else
                    {
                        if (app.Environment.IsDevelopment())
                        {
                            message = exception
                                .Message;
                            stackTrace = exception.StackTrace;
                        }

                        // Luôn luôn in lỗi thật ra màn hình Console/Terminal của Server để tiện giám sát
                        Console.WriteLine($"[CRITICAL ERROR]: {exception.ToString()}");
                    }

                    context.Response.StatusCode = statusCode;
                    var problemDetail = new Dictionary<string, object?>
                    {
                        { "statusCode", statusCode },
                        { "title", title },
                        { "detail", message },
                        { "instance", exceptionFeature?.Path }
                    };
                    if (app.Environment.IsDevelopment() && stackTrace != null)
                    {
                        problemDetail.Add("stackTrace", stackTrace);
                    }

                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetail));
                }
            });
        });
    }
}