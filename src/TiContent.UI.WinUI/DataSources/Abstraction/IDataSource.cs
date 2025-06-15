// ⠀
// IDataSource.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 08.06.2025.
// ⠀

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TiContent.UI.WinUI.DataSources.Abstraction;

public interface IDataSource<T1, in T2>
{
    public bool InProgress { get; }
    public bool IsCompleted { get; }
    public List<T1> Cache { get; }

    public Task<List<T1>> ObtainAsync(T2 @params, bool pagination);
}