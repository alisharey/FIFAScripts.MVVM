using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                DataRowCollection _playersContractInfo = contractsTable.Rows;
                foreach (DataRow _player in _playersContractInfo)
                {
                    if (_player["teamid"].ToString() == MyTeamID)
                    {
                        string? playerID = _player["playerid"].ToString();
                        tempPlayerList.Add(playerID is not null ? playerID : "");
                        Console.WriteLine($"{playerID} is in Team {MyTeamID}");
                    }
                }

            }

            return tempPlayerList;

        }

        private Dictionary<string, string> GetMyTeamPLayerNames()
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();

            if(_playersTable is not null)
            {
                foreach (DataRow player in _playersTable)
                {
                    string playerID = player["playerid"].ToString() ?? "";
                    if (MyTeamPlayersIDs.Contains(playerID))
                    {

                        var tempNameID = player["commonnameid"].ToString();
                        if (tempNameID == "0")
                        {
                            tempNameID = player["lastnameid"].ToString();
                        }

                        foreach (DataRow name in PlayerNames.Rows)
                        {
                            var nameid = name["nameid"].ToString();

                            if (nameid == tempNameID)
                            {
                                if (nameid == "0") continue;
                                temp.Add(playerID, name["name"].ToString());
                            }

                        }
                    }


                }
            }
            

            return temp;
        }
    }
}
