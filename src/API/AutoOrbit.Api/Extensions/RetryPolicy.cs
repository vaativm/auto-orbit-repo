using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace AutoOrbit.Api.Extensions;

public static class RetryPolicy
{
    public static void AddFaultHandlingPolicy(this IHttpClientBuilder builder)
    {
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5XX and 408
            .WaitAndRetryAsync(3, retryDelayInSeconds => TimeSpan.FromSeconds(3));

        AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError() // Handles 5XX and 408
            .CircuitBreakerAsync(4, TimeSpan.FromSeconds(15));

        AsyncPolicyWrap<HttpResponseMessage> policy = retryPolicy.WrapAsync(circuitBreakerPolicy);

        builder.AddPolicyHandler(policy);
    }
}
