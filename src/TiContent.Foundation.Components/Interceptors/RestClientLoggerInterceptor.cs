// ⠀
// RestClientLoggerInterceptor.cs
// TiContent.UI.WPF
//
// Created by the_timick on 27.04.2025.
// ⠀

using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Interceptors;

namespace TiContent.Foundation.Components.Interceptors;

public class RestClientLoggerInterceptor(ILogger<RestClientLoggerInterceptor> logger) : Interceptor
{
    public override ValueTask BeforeRequest(
        RestRequest request,
        CancellationToken cancellationToken
    )
    {
        var str = $"""
                   REQUEST {request.Method.ToString().ToUpperInvariant()}
                       {request.Resource}
                           Query: {GetQueryString(request)}
                           Headers: {GetHeaders(request)}
                           Body: {GetBody(request)}
                   """;
        logger.LogInformation("{str}", str);

        return base.BeforeRequest(request, cancellationToken);
    }

    public override ValueTask AfterRequest(
        RestResponse response,
        CancellationToken cancellationToken
    )
    {
        if (response.IsSuccessful)
        {
            var str = $"""
                       RESPONSE {response.Request.Method.ToString().ToUpperInvariant()} {(int)response.StatusCode} ({response.StatusCode.ToString()})
                           {response.Request.Resource}
                       """;
            logger.LogInformation("{str}", str);
        }
        else
        {
            var str = $"""
                       RESPONSE {response.Request.Method.ToString().ToUpperInvariant()} {(int)response.StatusCode} ({response.StatusCode.ToString()})
                           {response.Request.Resource}
                               {response.ErrorException?.Message}
                       """;
            logger.LogError("{str}", str);
        }

        return base.AfterRequest(response, cancellationToken);
    }

    #region Private Methods

    private static string GetQueryString(RestRequest request)
    {
        var queryParams = request
            .Parameters.Where(p => p.Type == ParameterType.GetOrPost)
            .Select(p => $"{p.Name}={p.Value}")
            .ToList();

        return queryParams.Count != 0 ? string.Join(", ", queryParams) : "null";
    }

    private static string GetHeaders(RestRequest request)
    {
        var headers = request
            .Parameters.Where(p => p.Type == ParameterType.HttpHeader)
            .Select(p => $"{p.Name}: {p.Value}")
            .ToList();

        return headers.Count != 0 ? string.Join(", ", headers) : "null";
    }

    private static string GetBody(RestRequest request)
    {
        if (request.Method == Method.Get)
            return "null";
        var body = request
            .Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)
            ?.Value;
        return body?.ToString() ?? "null";
    }

    #endregion
}