using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FifaLibrary;

using FIFAScripts.MVVM.Enums;

namespace FIFAScripts.MVVM.Models
{
    public class CareerSaveFile : EASaveFile
    {
        public string? MyTeamID { get; }
        public List<string> MyTeamPlayersIDs { get; }
        public Dictionary<string, string> MyTeamPlayersIDtoName { get; }



        public CareerSaveFile(string filename) : base(filename, 1)
        {
            if (Type != FileType.Career) throw new Exception("Choose a career file");

            MyTeamID = DataSetEa[0].Tables["career_users"]?.Rows[0]["clubteamid"].ToString();
            MyTeamPlayersIDs = GetMyTeamPlayerIDs();
            MyTeamPlayersIDtoName = GetMyTeamPLayerNames();

        }

        private List<string> GetMyTeamPlayerIDs()
        {
            var tempPlayerList = new List<string>();

            if (DataSetEa[0].Tables["career_playercontract"] is { } contractsTable)
            {
                DataRow[] _playersContractInfo = contractsTable.Select($"teamid= {MyTeamID}");
                foreach (DataRow _player in _playersContractInfo)
                {
                    string? playerID = _player["playerid"].ToString();
                    tempPlayerList.Add(playerID is not null ? playerID : "");
                    
                }

            }

            return tempPlayerList;

        }

        private Dictionary<string, string> GetMyTeamPLayerNames()
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();

            if (_playersTable is not null)
            {                
                
                foreach (DataRow player in _playersTable.Rows)
                {
                    string playerID = player["playerid"].ToString() ?? "";
                    if (MyTeamPlayersIDs.Contains(playerID))
                    {
                        var tempNameID = player["commonnameid"].ToString();
                        if (tempNameID == "0")
                        {
                            tempNameID = player["lastnameid"].ToString();
                            if (tempNameID == "0") continue;
                        }
                        var name = PlayerNames?.Select($"nameid= {tempNameID}").FirstOrDefault()?["name"];
                        temp.Add(playerID, name?.ToString() ?? "");

                    }
                }
            }
            return temp;
        }
    }
}
