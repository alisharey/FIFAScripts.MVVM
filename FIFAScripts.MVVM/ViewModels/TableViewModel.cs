using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using FIFAScripts.MVVM.Messages;

namespace FIFAScripts.MVVM.ViewModels;

public partial class TableViewModel : ObservableRecipient, IRecipient<PlayersTableMessage>
{
    [ObservableProperty]
    private string? _header;

    [ObservableProperty]
    private DataView? _playersStats;

    [RelayCommand]
    public void CellChanged(DataGridCellEditEndingEventArgs e)
    {

        int value = 99;
        int.TryParse(((TextBox)e.EditingElement).Text, out value);
        string stat = e.Column.Header.ToString() ?? "";
        string playerID = ((DataRowView)e.Row.Item).Row["playerid"].ToString() ?? "";
        
        Messenger.Send(new GridChangedMessage(PlayersStats, playerID, stat, value));

    }

    void IRecipient<PlayersTableMessage>.Receive(PlayersTableMessage message)
    {
        Task.Run(() => this.PlayersStats = message.PlayersStats);
    }

    public TableViewModel() 
    {
        Messenger.RegisterAll(this);
    }   
}
