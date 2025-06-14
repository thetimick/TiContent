// ⠀
// Inter.cs
// TiContent.UI.WPF
//
// Created by the_timick on 27.04.2025.
// ⠀

using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Interceptors;

namespace TiContent.UI.WPF.Components.Interceptors;

public class RestClientLoggerInterceptor(ILogger<RestClientLoggerInterceptor> logger) : Interceptor
{
    public override ValueTask BeforeRequest(RestRequest request, CancellationToken cancellationToken)
    {
        var resource = request.Resource;
        var query = GetQueryString(request);
        var url = $"{resource}{query}";

        logger.LogInformation(
            "REQUEST: {Method} {Url}{NL}Headers: {Headers}{NL}Body: {Body}",
            request.Method,
            url,
            Environment.NewLine,
            GetHeaders(request),
            Environment.NewLine,
            GetBody(request)
        );

        return base.BeforeRequest(request, cancellationToken);
    }

    public override ValueTask AfterRequest(RestResponse response, CancellationToken cancellationToken)
    {
        var request = response.Request;
        var resource = request.Resource;
        var queryString = GetQueryString(request);
        var url = $"{resource}{queryString}";

        logger.LogInformation("RESPONSE: {StatusCode} {Method} {Url}", (int)response.StatusCode, request.Method, url);

        if (!response.IsSuccessful)
        {
            logger.LogError(
                "{StatusCode} {ReasonPhrase}. Error: {ErrorMessage}",
                (int)response.StatusCode,
                response.StatusDescription,
                response.ErrorMessage ?? "No additional error message"
            );
        }

        return base.AfterRequest(response, cancellationToken);
    }

    private static string GetQueryString(RestRequest request)
    {
        var queryParams = request.Parameters.Where(p => p.Type == ParameterType.GetOrPost).Select(p => $"{p.Name}={p.Value}").ToList();

        return queryParams.Count != 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

    private static string GetHeaders(RestRequest request)
    {
        var headers = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader).Select(p => $"{p.Name}: {p.Value}").ToList();

        return headers.Count != 0 ? string.Join("; ", headers) : "None";
    }

    private static string GetBody(RestRequest request)
    {
        if (request.Method == Method.Get)
            return "None";
        var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value;
        return body?.ToString() ?? "None";
    }
}
