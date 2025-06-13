// ⠀
// NotificationService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 12.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace TiContent.UI.WinUI.Services.UI;

public interface INotificationService
{
    public void Setup(InfoBarPanel stack);

    public void ShowNotification(
        string title,
        string message,
        InfoBarSeverity severity,
        TimeSpan? duration = null
    );

    public void ShowErrorNotification(Exception ex, TimeSpan? duration = null);
}

public class NotificationService : INotificationService
{
    private InfoBarPanel? _stack;

    private readonly Dictionary<InfoBar, DispatcherTimer> _timers = new();

    public void Setup(InfoBarPanel stack)
    {
        _stack = stack;
    }

    public void ShowNotification(
        string title,
        string message,
        InfoBarSeverity severity,
        TimeSpan? duration
    )
    {
        var bar = MakeNotification(title, message, severity);
        if (duration is { } unwrapped)
        {
            var timer = MakeTimer(bar, unwrapped);
            _timers.Add(bar, timer);
        }
        _stack?.Children.Add(bar);
    }

    public void ShowErrorNotification(Exception ex, TimeSpan? duration = null)
    {
        ShowNotification("Ошибка", ex.Message, InfoBarSeverity.Error, duration);
    }

    private void TimerOnTick(InfoBar bar)
    {
        RemoveNotification(bar, _timers[bar]);
    }

    private void CloseButtonClick(InfoBar bar)
    {
        RemoveNotification(bar, null);
    }

    private InfoBar MakeNotification(string title, string message, InfoBarSeverity severity)
    {
        var infoBar = new InfoBar
        {
            Title = title,
            Message = message,
            Severity = severity,
            IsOpen = true,
        };
        infoBar.CloseButtonClick += (_, _) => CloseButtonClick(infoBar);
        return infoBar;
    }

    private DispatcherTimer MakeTimer(InfoBar bar, TimeSpan duration)
    {
        var timer = new DispatcherTimer { Interval = duration };
        timer.Tick += (_, _) => TimerOnTick(bar);
        timer.Start();
        return timer;
    }

    private void RemoveNotification(InfoBar bar, DispatcherTimer? timer)
    {
        if (timer != null)
        {
            timer.Stop();
            _timers.Remove(bar);
        }
        bar.IsOpen = false;
        _stack?.Children.Remove(bar);
    }
}
