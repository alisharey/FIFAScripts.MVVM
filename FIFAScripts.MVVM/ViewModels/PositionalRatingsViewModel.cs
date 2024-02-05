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
using FIFAScripts.MVVM.Models;

namespace FIFAScripts.MVVM.ViewModels;

public partial class PositionalRatingsViewModel : ObservableRecipient, IRecipient<UpdatePositionalRatingsMessage>
{
    [ObservableProperty]
    private string? _header;

    [ObservableProperty]
    private DataView? _positionalRatings;

    void IRecipient<UpdatePositionalRatingsMessage>.Receive(UpdatePositionalRatingsMessage message)
    {
        Task.Run(() => this.PositionalRatings = Util.GetPositionalRatings(message.MyTeamPlayersIDtoName, message.SquadSaveFile)?.DefaultView);
    }

    public PositionalRatingsViewModel() 
    {
        Messenger.RegisterAll(this);
    }   
}
