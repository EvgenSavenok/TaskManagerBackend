using System.Net;
using Application.ErrorModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using TasksService.Domain.CustomExceptions;

namespace TasksService.Infrastructure.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var exception = contextFeature.Error;

                    int statusCode;
                    string message;

                    switch (exception)
                    {
                        case NotFoundException notFoundException:
                            statusCode = (int)HttpStatusCode.NotFound;
                            message = notFoundException.Message;
                            break;
                        case AlreadyExistsException conflictException:
                            statusCode = (int)HttpStatusCode.Conflict;
                            message = conflictException.Message;
                            break;
                        case UnauthorizedException unauthorizedException:
                            statusCode = (int)HttpStatusCode.Unauthorized;
                            message = unauthorizedException.Message;
                            break;
                        case BadRequestException badRequestException:
                            statusCode = (int)HttpStatusCode.BadRequest;
                            message = badRequestException.Message;
                            break;
                        default:
                            statusCode = (int)HttpStatusCode.InternalServerError;
                            message = "Internal Server Error.";
                            break;
                    }

                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = statusCode,
                        Message = message
                    }.ToString());
                }
            });
        });
    }
}
