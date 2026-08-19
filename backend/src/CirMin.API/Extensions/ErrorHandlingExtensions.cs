using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CirMin.BusinessLogic.Exceptions;
using CirMin.Contracts.Errors;

namespace CirMin.API.Extensions;

public static class ErrorHandlingExtensions
{
    public static void UseApplicationExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exeptionHandlerApp =>
        {
            exeptionHandlerApp.Run(async context =>
            {
                // Trích xuất lỗi thô từ bộ nhớ RAM của hệ thống
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionFeature?.Error ??
                                new InvalidOperationException("An unhandled exception occurred.");
                var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
                var statusCode = StatusCodes.Status500InternalServerError;
                var message = "Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau!";
                var title = "Internal Server Error";
                var code = ErrorCodes.System.InternalError;
                IReadOnlyDictionary<string, object?>? parameters = new Dictionary<string, object?>();
                object? validationErrors = null;
                string? stackTrace = null;
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalExceptionHandler");

                switch (exception)
                {
                    case CirMinBadRequestException badEx:
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Bad Request";
                        message = badEx.Message;
                        code = badEx.Code;
                        parameters = badEx.Params;
                        break;

                    case CirMinValidationException validationEx:
                        statusCode = StatusCodes.Status400BadRequest;
                        title = "Validation Failed";
                        message = validationEx.Message;
                        code = ErrorCodes.Validation.Failed;
                        validationErrors = validationEx.Errors;
                        break;

                    case CirMinUnauthorizedException unAuthEx:
                        statusCode = StatusCodes.Status401Unauthorized;
                        title = "Unauthorized";
                        message = unAuthEx.Message;
                        code = unAuthEx.Code;
                        parameters = unAuthEx.Params;
                        break;

                    case CirMinForbiddenException forbiddenEx:
                        statusCode = StatusCodes.Status403Forbidden;
                        title = "Forbidden";
                        message = forbiddenEx.Message;
                        code = forbiddenEx.Code;
                        parameters = forbiddenEx.Params;
                        break;

                    case CirMinConflictException conflictEx:
                        statusCode = StatusCodes.Status409Conflict;
                        title = "Conflict";
                        message = conflictEx.Message;
                        code = conflictEx.Code;
                        parameters = conflictEx.Params;
                        break;

                    case CirMinNotFoundException notFoundEx:
                        statusCode = StatusCodes.Status404NotFound;
                        title = "Not Found";
                        message = notFoundEx.Message;
                        code = notFoundEx.Code;
                        parameters = notFoundEx.Params;
                        break;

                    case MediaStorageException mediaStorageException:
                        statusCode = StatusCodes.Status502BadGateway;
                        title = "Media Storage Error";
                        message = "Cannot upload media to Cloudinary. Try again later.";
                        code = mediaStorageException.Code;
                        parameters = mediaStorageException.Params;
                        break;

                    default:
                        if (app.Environment.IsDevelopment())
                        {
                            message = exception
                                .Message;
                            stackTrace = exception.StackTrace;
                        }

                        // Luôn luôn in lỗi thật ra màn hình Console/Terminal của Server để tiện giám sát
                        Console.WriteLine($"[CRITICAL ERROR]: {exception}");
                        break;
                }

                if (statusCode >= 500)
                    logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
                        traceId, context.Request.Path);
                else if (exception is CirMinApplicationException cirMinAppEx)
                    logger.LogWarning(
                        "Request rejected. Code: {Code}, TraceId: {TraceId}, Path: {Path}",
                        cirMinAppEx.Code,
                        traceId,
                        context.Request.Path);

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title
                };

                problemDetails.Extensions["code"] = code;
                if (validationErrors is not null)
                    problemDetails.Extensions["errors"] =
                        validationErrors;
                else
                    problemDetails.Extensions["params"] =
                        parameters;

                problemDetails.Extensions["traceId"] = traceId;
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
 
                if (app.Environment.IsDevelopment() && stackTrace != null)
                    problemDetails.Extensions.Add("stackTrace", stackTrace);

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions),
                    context.RequestAborted);
            });
        });
    }
}