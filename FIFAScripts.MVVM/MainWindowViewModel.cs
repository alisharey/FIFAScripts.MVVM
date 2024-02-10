using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.Messages;
using FIFAScripts.MVVM.ViewModels;

using MaterialDesignThemes.Wpf;

using Microsoft.Win32;

namespace FIFAScripts.MVVM;

public partial class MainWindowViewModel : ObservableRecipient, IRecipient<PopUpMessage>
{

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private TabItem? _selectedTabItem;

    [ObservableProperty]
    private SnackbarMessageQueue _messageQueue = new();

    [ObservableProperty]
    private bool _isDarkTheme;

    partial void OnIsDarkThemeChanged(bool value)
    {
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();

        theme.SetBaseTheme(value ? Theme.Dark : Theme.Light);
        helper.SetTheme(theme);

       
    }


    public ObservableCollection<object> Tabs { get; } = new();

    public MainWindowViewModel()
    {
        PaletteHelper palette = new();
        ITheme theme = palette.GetTheme();
        var currentTheme = theme.GetBaseTheme();
        IsDarkTheme = currentTheme == BaseTheme.Dark;

        Tabs.Add(new PlayerViewModel() { Header = "Player" });
        Tabs.Add(new TableViewModel() { Header = "Table" });
        Tabs.Add(new PositionalRatingsViewModel() { Header = "Positional Ratings" });
        var gitHubVM = new GitHubViewModel() { Header = "Github" };

        Tabs.Add(gitHubVM);
        SelectedTabIndex = 0;

        Messenger.RegisterAll(this);

    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        var sInfo = new System.Diagnostics.ProcessStartInfo(GitHubViewModel.GitHubLink)
        {
            UseShellExecute = true,
        };
        //System.Diagnostics.Process.Start(sInfo);
    }

    [RelayCommand]
    public void MigrateCareerToSquad()
    {
        Messenger.Send(new MigrateMessage());

    }

    [RelayCommand]
    public async Task SaveFile()
    {
        await Task.Run(() => Messenger.Send(new SaveFileMessage()));
    }

    [RelayCommand]
    public async Task Export()
    {
        await Task.Run(() => Messenger.Send(new ExportMessage()));
    }


    void IRecipient<PopUpMessage>.Receive(PopUpMessage message)
    {
        MessageQueue.Enqueue(message.Message,
               "OK",
               param => Trace.WriteLine("Actioned: " + param),
               message.Message);
    }
}
