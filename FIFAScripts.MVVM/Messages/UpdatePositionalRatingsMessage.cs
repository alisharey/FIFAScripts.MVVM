using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FIFAScripts.MVVM.Models;

namespace FIFAScripts.MVVM.Messages;

partial record class UpdatePositionalRatingsMessage(Dictionary<string, string>? MyTeamPlayersIDtoName, EASaveFile? SquadSaveFile);

