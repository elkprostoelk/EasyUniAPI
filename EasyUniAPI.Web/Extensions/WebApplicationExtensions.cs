using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EasyUniAPI.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void SetupExceptionHandling(this WebApplication app)
        {
            var isDevelopment = app.Environment.IsDevelopment();
            app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                ProblemDetails problemDetails;

                switch (exception)
                {
                    case ValidationException e:
                        {
                            problemDetails = new ProblemDetails
                            {
                                Title = "Validation failed. Please review your data",
                                Status = StatusCodes.Status400BadRequest,
                                Detail = exception?.Message,
                                Instance = context.Request.Path
                            };

                            context.Response.StatusCode = StatusCodes.Status400BadRequest;

                            break;
                        }
                    default:
                        {
                            problemDetails = new ProblemDetails
                            {
                                Title = "An error occured while processing the request. Please try again later",
                                Status = StatusCodes.Status500InternalServerError,
                                Detail = isDevelopment ? exception?.Message : "See details in the application logs",
                                Instance = context.Request.Path
                            };

                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            break;
                        }
                }

                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            }));
        }
    }
}
