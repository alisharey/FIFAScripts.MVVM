using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.Models;

using Microsoft.Win32;

namespace FIFAScripts.MVVM.ViewModels;

public partial class SquadViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _header;

    [RelayCommand]
    private async Task ImportCareerToSquadAsync()
    {
        await Task.Run(() =>
        {
            SaveFile saveFile = new SaveFile(OpenFile("Select A file."));
        });
    }


    private string OpenFile(string title)
    {
        string _fileName = "";        
        OpenFileDialog openDialog = new();
        openDialog.Title = title;
        string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        openDialog.InitialDirectory = Path.Combine(userPath, @"OneDrive\Documents\FIFA 23\settings");
        openDialog.Title = title;
        var result = openDialog.ShowDialog();
        if (result == true)
        {

            _fileName = openDialog.FileName;
        }
        
        return _fileName;        


    }

}
