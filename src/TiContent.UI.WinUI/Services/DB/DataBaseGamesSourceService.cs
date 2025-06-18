// ⠀
// DataBaseGamesSourceService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 04.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.Messaging;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.HydraLinks;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Services;
using TiContent.UI.WinUI.Services.Api.HydraLinks;
using TiContent.UI.WinUI.Services.UI;

namespace TiContent.UI.WinUI.Services.DB;

public interface IDataBaseGamesSourceService
{
    public record StateEventEntity(bool InProgress);

    public bool InProgress { get; }

    public Task ObtainIfNeededAsync(bool forceRefresh = false, CancellationToken token = default);

    public Task<List<DataBaseHydraLinkItemEntity>> SearchAsync(
        string query,
        CancellationToken token = default
    );
}

public partial class DataBaseGamesSourceService(
    App.AppDataBaseContext db,
    IStorageService storage,
    IHydraLinksService api,
    IMapper mapper,
    INotificationService notifications,
    ILogger<DataBaseGamesSourceService> logger
);

public partial class DataBaseGamesSourceService : IDataBaseGamesSourceService
{
    public bool InProgress { get; private set; }

    public async Task ObtainIfNeededAsync(bool forceRefresh, CancellationToken token)
    {
        await ObtainAllLinksAsync(forceRefresh, token);
    }

    public async Task<List<DataBaseHydraLinkItemEntity>> SearchAsync(
        string query,
        CancellationToken token
    )
    {
        var cleanQuery = RegexHelper.Clean().Replace(query.Trim().ToLower(), "");
        if (cleanQuery.IsNullOrEmpty())
            return [];
        var items = await db
            .HydraLinksItems.AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.CleanTitle, $"%{cleanQuery}%"))
            .ToListAsync(token);
        return items;
    }
}

public partial class DataBaseGamesSourceService
{
    private async Task ObtainAllLinksAsync(
        bool forceRefresh = false,
        CancellationToken token = default
    )
    {
        if (!IsEmptyOrExpiredDataBaseAsync() && !forceRefresh)
        {
            SetValueToInProgress(false);
            logger.LogInformation("HydraLinks will be refreshed");
            return;
        }

        try
        {
            SetValueToInProgress(true);

            await db.BulkDeleteAsync(
                await db.HydraLinksItems.ToHashSetAsync(token),
                cancellationToken: token
            );

            var items = (await api.ObtainSourcesAsync(token)).Items;
            var tasks = UseOnlyTrustedSourcesIfNeeded(items)
                .Select(entity => api.ObtainLinksAsync(entity.Url, token))
                .ToHashSet();
            await Task.WhenAll(tasks);

            var links = tasks
                .SelectMany(task => MapToDataBaseHydraLinkItemEntity(task.Result?.Name, task.Result?.Items))
                .ToHashSet();

            await db.BulkInsertOrUpdateAsync(links, cancellationToken: token);
            await db.SaveChangesAsync(token);

            storage.Cached.DataBaseTimestamp.HydraLinks = DateTime.Now;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "{msg}", ex.Message);
            SetValueToInProgress(false);
            throw;
        }

        logger.LogInformation("HydraLinks successfully refreshed");
        notifications.ShowNotification(
            "Обновление",
            "Обновлены источники HydraLinks!",
            InfoBarSeverity.Success,
            TimeSpan.FromSeconds(3)
        );

        SetValueToInProgress(false);
    }

    private bool IsEmptyOrExpiredDataBaseAsync()
    {
        return db.HydraLinksItems.AsNoTracking().IsEmpty() ||
               storage.Cached.DataBaseTimestamp.HydraLinks < DateTime.Now.AddHours(-3);
    }

    private IList<DataBaseHydraLinkItemEntity> MapToDataBaseHydraLinkItemEntity(
        string? owner,
        List<HydraLinksResponseEntity.ItemsEntity>? items
    )
    {
        if (owner.IsNullOrEmpty() || items.IsEmpty())
            return [];
        return items
            .Select(entity =>
                mapper
                    .Map<DataBaseHydraLinkItemEntity>(entity)
                    .Do(item => item.Owner = owner)
            )
            .ToList();
    }

    private List<HydraLinksSourcesResponseEntity.ItemEntity> UseOnlyTrustedSourcesIfNeeded(
        List<HydraLinksSourcesResponseEntity.ItemEntity> items
    )
    {
        if (!storage.Cached.GamesSource.UseOnlyTrustedSources)
            return items;

        return items
            .Where(entity =>
                !Enumerable.Contains(entity.Status, "Use At Your Own Risk") &&
                !Enumerable.Contains(entity.Status, "NSFW")
            )
            .ToList();
    }

    private void SetValueToInProgress(bool value)
    {
        InProgress = value;
        WeakReferenceMessenger.Default.Send(new IDataBaseGamesSourceService.StateEventEntity(value));
    }
}