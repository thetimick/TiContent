// ⠀
// GameStatusDataSource.cs
// TiContent.Foundation.DataSources
// 
// Created by the_timick on 24.06.2025.
// ⠀

using AutoMapper;
using TiContent.Foundation.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.Api.GameStatus;
using TiContent.Foundation.Entities.ViewModel.GamesStatus;
using TiContent.Foundation.Services.Api.GameStatus;

namespace TiContent.Foundation.DataSources;

public interface IGameStatusDataSource
    : IDataSource<IGameStatusDataSource.InputEntity, IGameStatusDataSource.OutputEntity>
{
    public record InputEntity(
        int ContentType
    );

    public record OutputEntity(
        List<GamesStatusPageItemEntity> Items
    );
}

public partial class GameStatusDataSource(IGameStatusApiService api, IMapper mapper)
{
    private CancellationTokenSource? _tokenSource;
}

public partial class GameStatusDataSource : IGameStatusDataSource
{
    public bool InProgress => _tokenSource != null;
    public bool IsCompleted => !Cache.Items.IsEmpty();
    public IGameStatusDataSource.OutputEntity Cache { get; } = new([]);

    public async Task<IGameStatusDataSource.OutputEntity> ObtainAsync(
        IGameStatusDataSource.InputEntity input,
        bool pagination
    )
    {
        if (_tokenSource != null)
            await _tokenSource.CancelAsync();
        _tokenSource = new CancellationTokenSource();

        switch (input.ContentType)
        {
            case 0:
                var released = await api.ObtainReleasedAsync(_tokenSource.Token);
                var items = released.Data.Summer
                    .Concat(released.Data.Spring)
                    .Concat(released.Data.Winter)
                    .ToList();
                ApplyItems(items);
            break;
            case 1:
                var cracked = await api.ObtainLastCrackedAsync(_tokenSource.Token);
                ApplyItems(cracked.Items);
            break;
        }

        return Cache;
    }
}

public partial class GameStatusDataSource
{
    private void ApplyItems(List<GameStatusResponseItemEntity> items)
    {
        var mapped = mapper.Map<List<GameStatusResponseItemEntity>, List<GamesStatusPageItemEntity>>(items);
        Cache.Items.Clear();
        Cache.Items.AddRange(mapped);
    }
}