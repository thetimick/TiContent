// ⠀
// HydraLinksService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using TiContent.Foundation.Entities.Api.HydraLinks;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.Api.HydraLinks;

public interface IHydraLinksService
{
    Task<List<HydraLinksResponseEntity?>> ObtainLinksAsync();
}

public partial class HydraLinksService(IRestClient client, IStorageService storage);

public partial class HydraLinksService : IHydraLinksService
{
    public async Task<List<HydraLinksResponseEntity?>> ObtainLinksAsync()
    {
        if (storage.Cached == null)
            return [];

        var sourceRequest = new RestRequest(storage.Cached.Urls.HydraLinksSources);
        var sourceResponse = await client.ExecuteAsync<HydraLinksSourcesResponseEntity>(sourceRequest);

        if (sourceResponse is not { IsSuccessful: true, Data: { } data })
            return [];

        var tasks = data
            .Items.Select(entity => new RestRequest(entity.Url))
            .Select(request => client.ExecuteAsync<HydraLinksResponseEntity>(request))
            .ToList();

        await Parallel.ForEachAsync(tasks, async (task, token) => await task.WaitAsync(token));

        return tasks.Where(task => task.Result.IsSuccessful).Select(task => task.Result.Data).ToList();
    }
}