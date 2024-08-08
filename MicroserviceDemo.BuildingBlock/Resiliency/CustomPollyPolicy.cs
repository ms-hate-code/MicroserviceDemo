using System.Net;
using Polly;
using Polly.Extensions.Http;
using TimeSpan = System.TimeSpan;

namespace MicroserviceDemo.BuildingBlock.Resiliency;

public static class CustomPollyPolicy
{
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        // break after 4 failed request
        var breakCount = 4;
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                breakCount, 
                TimeSpan.FromSeconds(10),
                onBreak: (result, breakDuration, context) => {
                    var policy = context.PolicyKey;
                    var message = $"Circuit Breaker open for duration {breakDuration.TotalSeconds}s after {breakCount} failed";
                    if (result.Exception is null)
                    {
                        Console.WriteLine(message);
                        return;
                    }

                    Console.WriteLine($"{message}, Exception = {result.Exception.Message}.");
                },
                onReset: (context) => {
                    Console.WriteLine($"{context.PolicyKey} => Circuit Breaker closed and is allowing requests through.");
                },
                onHalfOpen: () => {
                    Console.WriteLine($"Circuit Breaker is half-opened and will test the service with the next request");
                }
            );
    }
    
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        // wait 2s per retry time (total 2 retry time => wait 4s)
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                2, 
                retryAttempt => TimeSpan.FromSeconds(2),
                onRetry: (result, timeSpan, retryCount, context) => {
                    if (result.Exception is null)
                    {
                        Console.WriteLine($"{context.PolicyWrapKey} => Delaying for {timeSpan.TotalSeconds}s, then making retry {retryCount}");
                        return;
                    }

                    Console.WriteLine($"{context.PolicyWrapKey} => Has Error: Message = {result.Exception.Message}");
                }
            );
    }
    
    public static IAsyncPolicy<HttpResponseMessage> GetRateLimitingPolicy()
    {
        // allow only 2 request in 10s (5s per request)
        return Policy.RateLimitAsync<HttpResponseMessage>(2, TimeSpan.FromSeconds(10));
    }
}