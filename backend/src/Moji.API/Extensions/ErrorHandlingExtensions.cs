using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
                context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
                // Trích xuất lỗi thô từ bộ nhớ RAM của hệ thống
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionFeature.Error;
                var statusCode = StatusCodes.Status500InternalServerError;
                var message = "Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau!";
                var title = "Internal Server Error";
                IDictionary<string, string[]>? validationErrors = null;          
                string? stackTrace = null;

                switch (exception)
                {
                    case MojiBadRequestException badEx:
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Business Logic Error";
                        message = badEx.Message;
                        break;
                    
                    case MojiValidationException validationEx:
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Validation Error";
                        message = validationEx.Message;
                        validationErrors = validationEx.Errors;
                        break;
                    
                    case MojiUnauthorizedException unAuthEx:
                        statusCode = StatusCodes.Status401Unauthorized;
                        title = "Unauthorized";
                        message = unAuthEx.Message;
                        break;
                    
                    case MojiForbiddenException forbiddenEx:
                        statusCode = StatusCodes.Status403Forbidden;
                        title = "Forbidden";
                        message = forbiddenEx.Message;
                        break;
                    
                    case MojiConflictException conflictEx:
                        statusCode = StatusCodes.Status409Conflict;
                        title = "Data Conflict";
                        message = conflictEx.Message;
                        break;
                    
                    case MojiNotFoundException notFoundEx:
                        statusCode = StatusCodes.Status404NotFound;
                        title = "Not Found";
                        message = notFoundEx.Message;
                        break;
                    
                    default:
                        if (app.Environment.IsDevelopment())
                        {
                            message = exception
                                .Message;
                            stackTrace = exception.StackTrace;
                        }

                        // Luôn luôn in lỗi thật ra màn hình Console/Terminal của Server để tiện giám sát
                        Console.WriteLine($"[CRITICAL ERROR]: {exception.ToString()}");
                        break;
                }
                
                context.Response.StatusCode = statusCode;
                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = message,
                    Instance = exceptionFeature.Path
                };

                if (validationErrors != null)
                {
                    problemDetails.Extensions.Add("errors", validationErrors);
                }
                
                if (app.Environment.IsDevelopment() && stackTrace != null)
                {
                    problemDetails.Extensions.Add("stackTrace", stackTrace);
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
            });
        });
    }
}