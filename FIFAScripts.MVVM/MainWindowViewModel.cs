using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 
using DocumentFormat.OpenXml.Office2016.Excel;

using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.ViewModels;

using Microsoft.Win32;

namespace FIFAScripts.MVVM;

public partial class MainWindowViewModel : ObservableObject
{
    
    [ObservableProperty]
    private int _selectedTabIndex;
    

    public ObservableCollection<object> Tabs { get; } = new();
    
    public MainWindowViewModel()
    {
        Tabs.Add(new SquadViewModel()  {Header="Squad"});
        Tabs.Add(new CareerViewModel() {Header="Career"});
        Tabs.Add(new GitHubViewModel() { Header = "GitHub"});
        SelectedTabIndex = 0;

    }

    
    [RelayCommand(CanExecute = nameof(CanIncrementCount))]
    private void IncrementCount()
    {
        
    }

    private bool CanIncrementCount()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void OpenFile()
    {
       
    }
}
