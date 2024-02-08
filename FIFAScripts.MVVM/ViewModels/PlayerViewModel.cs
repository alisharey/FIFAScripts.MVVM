using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.Messages;
using FIFAScripts.MVVM.Models;
using FIFAScripts.MVVM.Wrappers;
using Microsoft.Win32;

namespace FIFAScripts.MVVM.ViewModels;

public partial class PlayerViewModel : ObservableRecipient,
    IRecipient<SaveFileMessage>,
    IRecipient<ExportMessage>,
    IRecipient<GridChangedMessage>,
    IRecipient<MigrateMessage>,
    IRecipient<ForceUpdateStatsMessage>
{
    private bool UpdateCurrentPlayerStats { get; set; } = true;



    [ObservableProperty]
    private string? _header;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPlayerStatsToValue))]
    private OverallStats? _averages;


    [ObservableProperty]
    private bool isFileLoaded = false;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string status = "No File Loaded";

    [ObservableProperty]
    private CurrentPlayerStatsWrapper<string, string> _currentPlayerStatsToValue;


    [ObservableProperty]
    private List<string>? _stats = Util.PlayerStats;

    [ObservableProperty]
    private object? _selectedPlayer;

    [ObservableProperty]

    private CareerInfo? _careerInfo;

    [ObservableProperty]
    private EASaveFile? _squadSaveFile;

    [ObservableProperty]
    private DataView? _playersStats;



    public PlayerViewModel()
    {
        Messenger.RegisterAll(this);
        CurrentPlayerStatsToValue = new(Messenger);

    }

    [RelayCommand]
    private async Task ApplyChangesAsync()
    {

        await Task.Run(() =>
        {
            if (GetSelectedPlayerID() is { Length: > 0 } playerID)
            {
                foreach (var kvp in CurrentPlayerStatsToValue)
                {
                    int value = 99;
                    int.TryParse(kvp.Value, out value);
                    if (value >= 0 && value <= 99)
                    {
                        SquadSaveFile?.SetPlayerStat(playerID, kvp.Key, value);


                    }
                }
                UpdatePlayerStats();

            }
        });

    }

    [RelayCommand]
    private async Task ImportCareerToSquadAsync()
    {
        IsLoading = true;
        Status = "Loading....";
        await Task.Run(() =>
        {

            if (new CareerSaveFile(OpenFile("Select a career file.")) is { } careerSaveFile)
            {
                CareerInfo = Util.ExportCareerInfo(careerSaveFile);

            }
            else
            {
                Status = "No File Loaded";
                IsLoading = IsFileLoaded = false;
                return;
            }


            SquadSaveFile = new EASaveFile(OpenFile("Select a squad file"));
            if (SquadSaveFile?.Type != FileType.Squad)
            {
                Status = "No File Loaded";
                IsLoading = IsFileLoaded = false;
                return;
            }


            SquadSaveFile?.ImportDataSet(CareerInfo?.MainDataSet ?? new System.Data.DataSet());
            UpdatePlayerStats();

        });
        IsLoading = false;
        IsFileLoaded = SquadSaveFile is not null;
        Status = "File Loaded";
        PopUpMessage("Done.");


    }



    private string GetSelectedPlayerID()
    {
        return CareerInfo?.MyTeamPlayersIDtoName.FirstOrDefault(x => x.Value == (string?)SelectedPlayer).Key ?? "";

    }

    private void IncDecOperation(int op, string group)
    {
        if (CurrentPlayerStatsToValue.ToList().Count == 0) return;
        Task.Run(() =>
        {
            string playerID = GetSelectedPlayerID();
            var stats = OverallStats.GroupedStats[group];
            if (!string.IsNullOrEmpty(playerID))
            {
                foreach (var stat in stats)
                {
                    int value = int.Parse(CurrentPlayerStatsToValue[stat]) + op;
                    if (value > 99) value = 99;
                    if (value < 0) value = 0;
                    CurrentPlayerStatsToValue.AddOrUpdateStat(stat, value.ToString());

                }
            }

            UpdateAverages();


        });





    }


    

    [RelayCommand]
    private void IncrementButtonClick(string group)
    {
        IncDecOperation(1, group);
    }



    [RelayCommand]
    private void DecrementButtonClick(string group)
    {
        IncDecOperation(-1, group);
    }




    //[RelayCommand]
    public void ChangeAge()
    {
        if (!IsFileLoaded) return;
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
                Messenger.Send(new UpdatePositionalRatingsMessage(CareerInfo?.MyTeamPlayersIDtoName, SquadSaveFile));
            });

        }
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

    partial void OnSelectedPlayerChanged(object? value)
    {
        Task.Run(() =>
        {
            string playerID = CareerInfo?.MyTeamPlayersIDtoName.FirstOrDefault
                  (x => x.Value == (string?)value).Key ?? "";

            if (UpdateCurrentPlayerStats && SquadSaveFile?.GetPlayerStats(playerID) is { } dict)
            {
                CurrentPlayerStatsToValue = new(Messenger, dict);
            }
            UpdateAverages();

        });


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
            if (!string.IsNullOrEmpty(message.PlayerID))
            {
                SquadSaveFile?.SetPlayerStat(message.PlayerID, message.Stat, message.Value);
                Messenger.Send(new UpdatePositionalRatingsMessage(CareerInfo?.MyTeamPlayersIDtoName, SquadSaveFile));
                OnSelectedPlayerChanged(SelectedPlayer);
            }

        });

    }


    async void IRecipient<MigrateMessage>.Receive(MigrateMessage message)
    {
        await ImportCareerToSquadAsync();
    }

    public void UpdateAverages()
    {
        Averages = !string.IsNullOrEmpty(GetSelectedPlayerID()) ? new OverallStats(CurrentPlayerStatsToValue.GetPlayerStatsDict()) : null;
    }

    void IRecipient<ForceUpdateStatsMessage>.Receive(ForceUpdateStatsMessage message)
    {
        Task.Run(() => UpdateAverages());
    }
}
