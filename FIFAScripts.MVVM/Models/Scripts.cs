using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FIFAScripts.MVVM.Enums;

namespace FIFAScripts.MVVM.Models
{
    public class Scripts
    {
        private readonly int _indexOffset;
        private DataSet[] DataSetCollection { get; set; }
        private readonly DataRowCollection _playersTable;
        private readonly DataRowCollection _teamplayerlinks;

        private string _myteamid = "";
        private List<string>? _myTeamPlayerIDs;
        private Dictionary<string, string>? _myTeamPlayersIDtoName;

        private readonly FileType _fileType;        
        private readonly SaveFile _file;
        public static List<string> PlayerStats { get => s_playerStats; private set => s_playerStats = value; }
        
        private static List<string> s_playerStats = new List<string>
            {
                   "overallrating",
                   "potential",
                   "birthdate",
                   //Pace 
                   "acceleration",
                   "sprintspeed",
                  
                  //Shooting
                   "positioning",
                   "finishing",
                   "shotpower",
                   "longshots",
                   "volleys",
                   "penalties",

                   //Passing
                    "vision",
                    "crossing",
                    "freekickaccuracy",
                    "shortpassing",
                    "longpassing",
                    "curve",

                   //Dribbling
                    "agility",
                    "balance",
                    "reactions",
                    "ballcontrol",
                    "dribbling",
                    "composure",
                  
                    //Defending
                     "interceptions",
                     "defensiveawareness",
                     "standingtackle",
                     "slidingtackle",
                     "headingaccuracy",
                    
                     //Physicality
                     "jumping",
                     "stamina",
                     "strength",
                     "aggression",


                    //GK
                     "gkdiving",
                     "gkhandling",
                     "gkkicking",
                     "gkpositioning",
                     "gkreflexes",
        };

        public Scripts(SaveFile _file)
        {
            this._file = _file;
            this.DataSetCollection = _file.DataSetEa;
            this._fileType = _file.Type;

            if (_fileType == FileType.Career)
            {
                _indexOffset = 1;
                this._myteamid = GetMyTeamID();
                this._myTeamPlayerIDs = GetMyTeamPlayerIDs(_myteamid);

            }
            else _indexOffset = 0;
            this._playersTable = GetPlayersTable();
            this._teamplayerlinks = GetTeamPlayerLinks();

            if (_fileType == FileType.Career) this._myTeamPlayersIDtoName = GetMyTeamPLayerNames();

        }


        #region  Getters
        private string GetMyTeamID()
        {
            return DataSetCollection[0].Tables["career_users"].Rows[0]["clubteamid"].ToString();       
        }

        private string? GetSeasonCount()
        {
            if (DataSetCollection[0].Tables["career_users"] is { } careerTable)
            {
                return careerTable.Rows[0]["seasoncount"].ToString();
            }

            return null;
                
        }
        private List<string>? GetMyTeamPlayerIDs(string myTeamID)
        {
            if (this._fileType != FileType.Career) return null;
            var tempPlayerList = new List<string>();

            if(DataSetCollection[0].Tables["career_playercontract"] is { }  contractsTable)
            {
                DataRowCollection _playersContractInfo = contractsTable.Rows;
                foreach (DataRow _player in _playersContractInfo)
                {
                    if (_player["teamid"].ToString() == myTeamID)
                    {
                        string playerID = _player["playerid"].ToString();
                        tempPlayerList.Add(playerID);
                        Console.WriteLine($"{playerID} is in Team {myTeamID}");
                    }
                }
                
            }

            return tempPlayerList;

        }
        private Dictionary<string, string> GetMyTeamPLayerNames()
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();

            foreach (DataRow _player in _playersTable)
            {
                string? playerID = _player["playerid"].ToString();
                if (_myTeamPlayerIDs.Contains(playerID))
                {

                    var tempNameID = _player["commonnameid"].ToString();
                    if (tempNameID == "0")
                    {
                        tempNameID = _player["lastnameid"].ToString();
                    }

                    foreach (DataRow name in _file.PlayerNames.Rows)
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

            return temp;
        }
        private DataRowCollection GetPlayersTable()
        {
            return DataSetCollection[_indexOffset].Tables["players"].Rows;


        }
        private DataRowCollection GetTeamPlayerLinks()
        {
            return DataSetCollection[_indexOffset].Tables["teamplayerlinks"].Rows;

        }
        #endregion





        public CareerInfo ExportCareerInfo()
        {

            var careerInfo = new CareerInfo(DataSetCollection[_indexOffset], _myteamid, _myTeamPlayerIDs, _myTeamPlayersIDtoName);

            return careerInfo;
        }

        public CareerInfo ExportCareerInfo(string temp)
        {

            var careerInfo = new CareerInfo(DataSetCollection[_indexOffset],
                string.Empty,
                new List<string>(),
                new Dictionary<string, string>());

            return careerInfo;
        }

        public int ImportCareerInfo(CareerInfo careerInfo)
        {

            this._myteamid = careerInfo.MyTeamID;
            this._myTeamPlayerIDs = careerInfo.MyTeamPlayerIDs;
            this._myTeamPlayersIDtoName = careerInfo.MyTeamPlayerNamesDict;
            ImportDataSet(careerInfo);
            return 0;
        }
        
        private void ImportDataSet(CareerInfo careerInfo)
        {


            foreach (DataTable savedTable in careerInfo.MainDataSet.Tables)
            {
                string tablename = savedTable.TableName;
                if (tablename == "manager") continue;


                var TargetTable = DataSetCollection[_indexOffset].Tables[tablename].Rows;

                foreach (DataRow row in savedTable.Rows)
                {
                    TargetTable.RemoveAt(0);
                    TargetTable.Add(row.ItemArray);
                }

            }




        }




        public void SetPlayerStats(string playerID, string stat, int value = 99)
        {
            if (stat == "birthdate") value = 154482;
            foreach (DataRow _player in _playersTable)
            {

                string? _playerID = _player["playerid"].ToString();
                if (_playerID == playerID) // try to merge with SetTeamStats in a clean way
                {
                    if (stat == "ALL")
                    {
                        foreach (string _stat in PlayerStats)
                        {
                            if (_stat == "birthdate")
                            {
                                //_player[_stat] = 154482;
                                continue;
                            }
                            _player[_stat] = value;
                        }
                    }
                    else
                    {
                        _player[stat] = value;
                    }
                }


            }

        }

        public void SetTeamStats(List<string> playerIDs, string stat, int value = 99)
        {
            if (stat == "birthdate") value = 154482;
            foreach (DataRow _player in _playersTable)
            {

                string? _playerID = _player["playerid"].ToString();
                if (playerIDs.Contains(_playerID))
                {

                    if (stat == "ALL") // all stats except birthdate for now
                    {
                        foreach (string _stat in PlayerStats)
                        {
                            if (_stat == "birthdate")
                            {
                                //_player[_stat] = 154482;
                                continue;
                            }
                            _player[_stat] = value;
                        }
                    }

                    else //single stat
                    {
                        _player[stat] = value;

                    }
                }
            }
        }


        public void SwapPSG(List<string> playerIDs, string myteamID)
        {
            var teamplayerslink = DataSetCollection[_indexOffset].Tables["teamplayerlinks"].Rows;
            var teamsheets = DataSetCollection[_indexOffset].Tables["default_teamsheets"].Rows;

            foreach (DataRow player in teamplayerslink)
            {
                string? teamid = player["teamid"].ToString();

                if (teamid == "73") player["teamid"] = "111592";
                else if (teamid == myteamID) player["teamid"] = "73";

            }




        }
    }
}
