﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFormat.OpenXml.Math;

using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.Messages;
using FIFAScripts.MVVM.Models;

using Irony.Parsing;

using Microsoft.Win32;

namespace FIFAScripts.MVVM.ViewModels;

public partial class SquadViewModel : ObservableRecipient, IRecipient<SaveFileMessage>, IRecipient<ExportMessage>
{
    [ObservableProperty]
    private string? _header;

    [ObservableProperty]
    private Dictionary<string, string> _currentPlayerStatsToValue = new();

    [ObservableProperty]
    private List<string>? _stats = Util.PlayerStats;

    [ObservableProperty]
    private string? _statValue;

    [ObservableProperty]
    private string? _selectedStat;

    [ObservableProperty]
    private object? _selectedPlayer;

    private CareerSaveFile? _careerSaveFile;

    [ObservableProperty]   
    private CareerInfo? _careerInfo;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeStatsCommand))]
    [NotifyCanExecuteChangedFor(nameof(ChangeAgeCommand))]
    private EASaveFile? _squadSaveFile;

    [RelayCommand]
    private async Task ImportCareerToSquadAsync()
    {       

        await Task.Run(() => _careerSaveFile = new CareerSaveFile(OpenFile("Select a career file.")));

        if (_careerSaveFile != null)
        {
            CareerInfo = Util.ExportCareerInfo(_careerSaveFile);

        }

        await Task.Run(() =>
        {
            do
            {
                PopUpMessage("Select a target squad file.");
                SquadSaveFile = new EASaveFile(OpenFile("squad file"));
            }
            while (SquadSaveFile?.Type != FileType.Squad);
            

        });

        SquadSaveFile?.ImportDataSet(CareerInfo?.MainDataSet ?? new System.Data.DataSet());
        


    }



    private string GetSelectedPlayerID()
    {
        return CareerInfo?.MyTeamPlayersIDtoName.FirstOrDefault(x => x.Value == (string?)SelectedPlayer).Key ?? "";
       
    }
    
    [RelayCommand]
    public void ChangeStat()
    {

        if (!string.IsNullOrEmpty(SelectedStat) && CurrentPlayerStatsToValue.ContainsKey(SelectedStat))
        {

            int statValue = 0;
            int.TryParse(StatValue, out statValue);

            string playerID = GetSelectedPlayerID();

            if (statValue >= 1 && statValue <= 99 && !string.IsNullOrEmpty(playerID))
            {
                SquadSaveFile?.SetPlayerStat(playerID, SelectedStat, statValue);
                OnSelectedPlayerChanged(SelectedPlayer);
                PopUpMessage($"{SelectedPlayer}'s {SelectedStat} set to {statValue}");
            }
            else
            {
                PopUpMessage("Invalid stat value. Write a value between 1-99");
            }



        }


    }

    [RelayCommand(CanExecute = nameof(CareerFileLoaded))]
    public void ChangeStats()
    {

        if (!string.IsNullOrEmpty(SelectedStat) && CurrentPlayerStatsToValue.ContainsKey(SelectedStat))
        {

            int statValue = 0;
            int.TryParse(StatValue, out statValue);
            string playerID = GetSelectedPlayerID();


            if (statValue >= 1 && statValue <= 99 && !string.IsNullOrEmpty(playerID))
            {
                SquadSaveFile?.SetPlayerStats(playerID, statValue);
                OnSelectedPlayerChanged(SelectedPlayer);
                PopUpMessage($"{SelectedPlayer}'s stats set to {statValue}");
            }
            else
            {
                PopUpMessage("Invalid stat value. Write a value between 1-99");
            }



        }

    }

    
    [RelayCommand(CanExecute = nameof(CareerFileLoaded))]
    public void ChangeAge()
    {

    }


    private bool CareerFileLoaded() => CareerInfo is not null;


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


    public SquadViewModel()
    {
        Messenger.RegisterAll(this);
    }

    private void UpdateStatValue()
    {
        if (!string.IsNullOrEmpty(SelectedStat) && CurrentPlayerStatsToValue.ContainsKey(SelectedStat))
        {
            StatValue = CurrentPlayerStatsToValue[SelectedStat];
        }

    }

    partial void OnSelectedPlayerChanged(object? value)
    {


        string playerID = CareerInfo?.MyTeamPlayersIDtoName.FirstOrDefault
            (x => x.Value == (string?)value).Key ?? "";

        CurrentPlayerStatsToValue = SquadSaveFile?.GetPlayerStats(playerID) ?? new Dictionary<string, string>();

        UpdateStatValue();



    }

    partial void OnSelectedStatChanged(string? value)
    {
        UpdateStatValue();
    }

    void IRecipient<SaveFileMessage>.Receive(SaveFileMessage message)
    {
        SquadSaveFile?.Save();
        PopUpMessage("File Saved");
    }

    void IRecipient<ExportMessage>.Receive(ExportMessage message)
    {
        SquadSaveFile?.ExportToXL();
        PopUpMessage("File Exported");
        
    }

    private void PopUpMessage(string message)
    {
        Messenger.Send(new PopUpMessage(message));
    }
}
