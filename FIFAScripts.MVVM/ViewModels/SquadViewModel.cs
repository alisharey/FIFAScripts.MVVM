﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DocumentFormat.OpenXml.Math;


using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.Messages;
using FIFAScripts.MVVM.Models;



using Microsoft.Win32;
namespace FIFAScripts.MVVM.ViewModels;

public partial class SquadViewModel : ObservableRecipient, IRecipient<SaveFileMessage>, IRecipient<ExportMessage>, IRecipient<GridChangedMessage>
{


    [ObservableProperty]
    private string? _header;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeStatsCommand))]
    [NotifyCanExecuteChangedFor(nameof(ChangeAgeCommand))]
    private bool isFileLoaded;


    [ObservableProperty]
    private Dictionary<string, string> _currentPlayerStatsToValue = new();

    [ObservableProperty]
    private List<string>? _stats = Util.PlayerStats;

    [ObservableProperty]
    private string? _statValue;

    [ObservableProperty]
    private string? _selectedStat;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangeStatsCommand))]
    private object? _selectedPlayer;



    [ObservableProperty]

    private CareerInfo? _careerInfo;

    [ObservableProperty]
    private EASaveFile? _squadSaveFile;

    [ObservableProperty]
    private DataView? _playersStats;



    public SquadViewModel()
    {
        Messenger.RegisterAll(this);

    }



    [RelayCommand]
    private async Task ImportCareerToSquadAsync()
    {
        await Task.Run(() =>
        {

            if (new CareerSaveFile(OpenFile("Select a career file.")) is { } careerSaveFile)
            {
                CareerInfo = Util.ExportCareerInfo(careerSaveFile);

            }
            else return;


            SquadSaveFile = new EASaveFile(OpenFile("Select a squad file"));
            if (SquadSaveFile?.Type != FileType.Squad) return;


            SquadSaveFile?.ImportDataSet(CareerInfo?.MainDataSet ?? new System.Data.DataSet());
            UpdatePlayerStats();

        });


        IsFileLoaded = SquadSaveFile is not null;
        PopUpMessage("Done.");


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
                UpdatePlayerStats();
                PopUpMessage($"{SelectedPlayer}'s {SelectedStat} set to {statValue}");
            }
            else
            {
                PopUpMessage("Invalid stat value. Write a value between 1-99");
            }



        }


    }

    [RelayCommand(CanExecute = nameof(CanExecuteChangeStats))]
    public void ChangeStats()
    {

        int statValue = 0;
        int.TryParse(StatValue, out statValue);
        string playerID = GetSelectedPlayerID();

        if (statValue >= 1 && statValue <= 99 && !string.IsNullOrEmpty(playerID))
        {

            SquadSaveFile?.SetPlayerStats(playerID, statValue);
            OnSelectedPlayerChanged(SelectedPlayer);
            UpdatePlayerStats();
            PopUpMessage($"{SelectedPlayer}'s stats set to {statValue}");
        }
        else
        {
            PopUpMessage("Invalid stat value. Write a value between 1-99");
        }

    }

    [RelayCommand(CanExecute = nameof(SquadSaveFileLoaded))]
    public void ChangeAge()
    {

        if (CareerInfo?.MyTeamPlayersIDs is { } PlayersIDs)
        {

            //var columnNames = PlayersStats?[0].Table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();            
            //var datarows = SquadSaveFile?.GetPlayersStats(PlayersIDs)?.CopyToDataTable().DefaultView;

            foreach (string id in PlayersIDs)
            {
                SquadSaveFile?.SetPlayerStat(id, "birthdate", 154482);

            }
            UpdatePlayerStats();
            PopUpMessage($"Script done.");
        }

    }

    private void UpdatePlayerStats()
    {
        if (CareerInfo is { })
        {
            Task.Run(() =>
            {
                PlayersStats = SquadSaveFile?.GetPlayersStatsDefaultView(CareerInfo.MyTeamPlayersIDtoName);
                Messenger.Send(new PlayersTableMessage(PlayersStats));
            });
        }
    }



    private bool SquadSaveFileLoaded() => IsFileLoaded;
    private bool CanExecuteChangeStats() => IsFileLoaded && SelectedPlayer is not null;

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

    private void UpdateStatValue()
    {
        StatValue = !string.IsNullOrEmpty(SelectedStat) && CurrentPlayerStatsToValue.ContainsKey(SelectedStat)
            ? CurrentPlayerStatsToValue[SelectedStat]
            : "";

    }

    partial void OnSelectedPlayerChanged(object? value)
    {
        Task.Run(() =>
        {
            string playerID = CareerInfo?.MyTeamPlayersIDtoName.FirstOrDefault
            (x => x.Value == (string?)value).Key ?? "";

            CurrentPlayerStatsToValue = SquadSaveFile?.GetPlayerStats(playerID) ?? new Dictionary<string, string>();

            UpdateStatValue();
        });
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

    void IRecipient<GridChangedMessage>.Receive(GridChangedMessage message)
    {
        Task.Run(() =>
        {
            this.PlayersStats = message.PlayersStats;
            SquadSaveFile?.SetPlayerStat(message.PlayerID, message.Stat, message.Value);
            OnSelectedPlayerChanged(SelectedPlayer);
        });

    }
}
