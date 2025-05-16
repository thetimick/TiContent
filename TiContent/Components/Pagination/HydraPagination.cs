// ⠀
// HydraPagination.cs
// TiContent
// 
// Created by the_timick on 12.05.2025.
// ⠀

namespace TiContent.Components.Pagination;

public class HydraPagination(int allItemsCount)
{
    // Constants
    
    public const int ItemsOnPage = 24;

    // Public Props
    
    public int AllItemsCount { get; init; } = allItemsCount;

    public int CurrentTakeValue { get; private set; } = ItemsOnPage;
    public int CurrentSkipValue { get; private set; }
    
    public bool InProgress;
    
    // Public Methods

    public void Next()
    {
        if (InProgress)
            return;

        if (CurrentTakeValue + ItemsOnPage > AllItemsCount)
        {
            CurrentTakeValue = AllItemsCount;
            CurrentSkipValue = AllItemsCount - ItemsOnPage;
            return;
        }
        
        CurrentTakeValue += ItemsOnPage;
        CurrentSkipValue = CurrentTakeValue - ItemsOnPage;

        InProgress = true;
    }

    public void Completed()
    {
        InProgress = false;
    }
}