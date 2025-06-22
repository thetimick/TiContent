// ⠀
// JacredDataSource.cs
// TiContent.Foundation.DataSources
// 
// Created by the_timick on 22.06.2025.
// ⠀

using AutoMapper;
using TiContent.Foundation.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.Api.Jacred;
using TiContent.Foundation.Entities.ViewModel.FilmsSourcePage;
using TiContent.Foundation.Services.Api.Jacred;

namespace TiContent.Foundation.DataSources;

public partial class JacredDataSource(IJacredService api, IMapper mapper)
{
    private JacredDataSourceInputEntity? _previousInput;
    private CancellationTokenSource? _tokenSource;
};

public partial class JacredDataSource : IDataSource<JacredDataSourceInputEntity, JacredDataSourceOutputEntity>
{
    public bool InProgress => _tokenSource != null;
    public bool IsCompleted => !Cache.Items.IsEmpty();
    public JacredDataSourceOutputEntity Cache { get; } = new([]);

    public async Task<JacredDataSourceOutputEntity> ObtainAsync(
        JacredDataSourceInputEntity input,
        bool pagination
    )
    {
        if (_previousInput == input)
            return Cache;
        _previousInput = input;

        if (_tokenSource != null)
            await _tokenSource.CancelAsync();
        _tokenSource = new CancellationTokenSource();

        var items = await api.ObtainTorrentsAsync(input.Query, _tokenSource.Token);
        ApplyItems(items);

        return Cache;
    }
}

// Private Methods

public partial class JacredDataSource
{
    private void ApplyItems(List<JacredEntity> items)
    {
        Cache.Items.Clear();
        Cache.Items.AddRange(
            mapper.Map<List<JacredEntity>, List<FilmsSourcePageItemEntity>>(items)
        );
    }
}

// Input \ Output

public record JacredDataSourceInputEntity(
    string Query
);

public record JacredDataSourceOutputEntity(
    List<FilmsSourcePageItemEntity> Items
);