// ⠀
// HydraLinksService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using RestSharp;
using TiContent.UI.WPF.Components.Helpers;
using TiContent.UI.WPF.Entities.Legacy.HydraLinks;
using TiContent.UI.WPF.Services.Storage;

namespace TiContent.UI.WPF.Services.HydraLinks;

public interface IHydraLinksService
{
    Task<List<HydraLinksResponseEntity?>> ObtainLinksAsync();
}

public partial class HydraLinksService(IRestClient client, IStorageService storage)
{
    // Constants

    private const string Types =
        "repack-games.json|atop-games.json|steamrip.json|dodi.json|empress.json|fitgirl.json|gog.json|kaoskrew.json|onlinefix.json|tinyrepacks.json|xatab.json";

    // Private Props

    private string HydraLinksBaseUrl => storage.Cached?.Urls.HydraLinksBaseUrl ?? string.Empty;
}

// IHydraLinksService

public partial class HydraLinksService : IHydraLinksService
{
    public async Task<List<HydraLinksResponseEntity?>> ObtainLinksAsync()
    {
        var requests = Types
            .Split('|')
            .Select(type => new RestRequest(UrlHelper.Combine(HydraLinksBaseUrl, "sources", type)));
        var responses = requests
            .Select(request => client.ExecuteAsync<HydraLinksResponseEntity>(request))
            .ToList();

        await Task.WhenAll(responses);

        return responses.Select(task => task.Result.Data).ToList();
    }
}
