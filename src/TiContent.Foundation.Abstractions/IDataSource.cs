// ⠀
// IDataSource.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 08.06.2025.
// ⠀

namespace TiContent.Foundation.Abstractions;

public interface IDataSource<in TInputEntity, TOutputEntity>
{
    public bool InProgress { get; }
    public bool IsCompleted { get; }
    public TOutputEntity Cache { get; }

    public Task<TOutputEntity> ObtainAsync(
        TInputEntity input,
        bool pagination
    );
}