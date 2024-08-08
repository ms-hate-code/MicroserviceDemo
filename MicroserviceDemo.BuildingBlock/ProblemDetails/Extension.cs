using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Polly.RateLimit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MicroserviceDemo.BuildingBlock.ProblemDetails;

public static class Extension
    {
        public static WebApplication UseCustomProblemDetails(this WebApplication app)
        {
            app.UseStatusCodePages(statusCodeApp =>
                {
                    statusCodeApp.Run(async ctx =>
                    {
                        ctx.Response.ContentType = "application/problem+json";
                        var problemDetailService = ctx.RequestServices.GetService<IProblemDetailsService>();

                        await problemDetailService.WriteAsync(new ProblemDetailsContext()
                        {
                            HttpContext = ctx,
                            ProblemDetails =
                            {
                                Status = ctx.Response.StatusCode,
                                Detail = $"[{ReasonPhrases.GetReasonPhrase(ctx.Response.StatusCode)}] An error occurred while processing your request.",
                                Title = ReasonPhrases.GetReasonPhrase(ctx.Response.StatusCode),
                                Extensions =
                                {
                                    ["traceId"] = ctx.TraceIdentifier
                                },
                                Instance = ctx.Request.Path,
                                Type = $"https://httpstatuses.com/{ctx.Response.StatusCode}"
                            }
                        });
                    });
                }
            );

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async ctx =>
                {
                    ctx.Response.ContentType = "application/problem+json";
                    var problemDetailService = ctx.RequestServices.GetService<IProblemDetailsService>();

                    var exceptionHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                    var exceptionType = exceptionHandlerFeature.Error;
                    var (statusCode, title) = exceptionType switch
                    {
                        BadHttpRequestException appException => (
                            StatusCodes.Status400BadRequest,
                            ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)
                        ),
                        KeyNotFoundException appException => (
                            StatusCodes.Status404NotFound,
                            ReasonPhrases.GetReasonPhrase(StatusCodes.Status404NotFound)
                        ),
                        UnauthorizedAccessException appException => (
                            StatusCodes.Status401Unauthorized,
                            ReasonPhrases.GetReasonPhrase(StatusCodes.Status401Unauthorized)
                        ),
                        ValidationException appException => (
                            StatusCodes.Status400BadRequest,
                            ReasonPhrases.GetReasonPhrase(StatusCodes.Status400BadRequest)
                        ),
                        RateLimitRejectedException appException => (
                            StatusCodes.Status429TooManyRequests,
                            ReasonPhrases.GetReasonPhrase(StatusCodes.Status429TooManyRequests)
                        ),
                        _ => (
                            StatusCodes.Status500InternalServerError,
                            ReasonPhrases.GetReasonPhrase(StatusCodes.Status500InternalServerError)
                        )
                    };

                    var problemDetailCtx = new ProblemDetailsContext()
                    {
                        HttpContext = ctx,
                        ProblemDetails =
                        {
                            Status = statusCode,
                            Detail = exceptionHandlerFeature.Error.Message,
                            Title = $"[{title}] An error occurred while processing your request.",
                            Extensions =
                            {
                                ["traceId"] = ctx.TraceIdentifier,
                            },
                            Instance = ctx.Request.Path,
                            Type = $"https://httpstatuses.com/{statusCode}",
                        }
                    };

                    if (app.Environment.IsDevelopment())
                    {
                        problemDetailCtx.ProblemDetails.Extensions.Add("exception", JsonSerializer.Serialize(
                            exceptionHandlerFeature.Error.StackTrace, 
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            }));
                    }

                    await problemDetailService.WriteAsync(problemDetailCtx);
                });
            });

            return app;
        }
    }