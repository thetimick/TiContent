// ⠀
// TMDBPagination.cs
// TiContent
// 
// Created by the_timick on 16.05.2025.
// ⠀

namespace TiContent.Components.Pagination;

public class TMDBPagination
{
    // Public Props
    
    public long TotalPages { get; private set; }
    public long Page { get; private set; }
    
    public bool InProgress;
    
    public bool HasMorePage => Page < TotalPages;
    public bool HasBeenInit { get; private set; }
    
    // Lifecycle

    public TMDBPagination(long? totalPages = -1)
    {
        Init(totalPages, false);
    }
    
    // Public Methods

    public void Init(long? totalPages, bool hasBeenInit = true)
    {
        TotalPages = totalPages ?? -1;
        Page = 1;
        InProgress = false;
        HasBeenInit = hasBeenInit;
    }

    public void NextPage()
    {
        if (InProgress)
            return;
        if (Page < TotalPages)
            Page += 1;
        InProgress = true;
    }

    public void LoadingCompleted()
    {
        InProgress = false;
    }
}