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
    public record InputEntity;

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

        var response = await api.ObtainMainAsync(_tokenSource.Token);
        ApplyItems(response);

        return Cache;
    }
}

public partial class GameStatusDataSource
{
    private void ApplyItems(GameStatusMainResponseEntity response)
    {
        var hotGames = mapper.Map<List<GameStatusMainResponseEntity.ItemEntity>, List<GamesStatusPageItemEntity>>(
            response.HotGames
        );

        Cache.Items.Clear();
        Cache.Items.AddRange(hotGames);
    }
}