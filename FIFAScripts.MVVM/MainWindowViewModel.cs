using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2016.Excel;

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
    private SnackbarMessageQueue _messageQueue = new();
    


    public ObservableCollection<object> Tabs { get; } = new();

    public MainWindowViewModel()
    {       

        Tabs.Add(new SquadViewModel() { Header = "Squad" });
        Tabs.Add(new TableViewModel() { Header = "Table" });
        var gitHubVM = new GitHubViewModel() { Header = "Github" };

        Tabs.Add(gitHubVM);
        SelectedTabIndex = 0;

        Messenger.RegisterAll(this);

    }

    partial void OnSelectedTabIndexChanging(int value)
    {
        if (value == 2)
        {
            var sInfo = new System.Diagnostics.ProcessStartInfo(GitHubViewModel.GitHubLink)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);      



        }
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
