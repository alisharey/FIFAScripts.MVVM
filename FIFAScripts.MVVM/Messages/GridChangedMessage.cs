using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FIFAScripts.MVVM.Messages;

public record class GridChangedMessage(DataView? PlayersStats, string PlayerID, string Stat, int Value = 99);

