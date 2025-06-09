// ⠀
// AboutPageViewModel.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 30.03.2025.
// ⠀

using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TiContent.UI.WPF.ViewModels.Main.Pages;

public partial class AboutPageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Version { get; set; } = $"v.{Assembly.GetExecutingAssembly().GetName().Version?.ToString(2)}";

    [ObservableProperty]
    public partial string Licences { get; set; } = string.Empty;

    [RelayCommand]
    private void OnLoaded()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TiContent.UI.WPF.Resources.Data.Licences.txt");
        if (stream is null)
            return;
        using var reader = new StreamReader(stream);
        Licences = reader.ReadToEnd();
    }
}