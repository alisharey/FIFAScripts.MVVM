using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFAScripts.MVVM.Messages;

public record class PlayersTableMessage(DataView? PlayersStats);
