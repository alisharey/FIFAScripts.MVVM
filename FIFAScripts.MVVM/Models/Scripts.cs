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

        public Scripts()
        {            

        }



        public static string? GetSeasonCount(DataSet[] dataSetEA)
        {
            if (dataSetEA[0].Tables["career_users"] is { } careerTable)
            {
                return careerTable.Rows[0]["seasoncount"].ToString();
            }

            return null;

        }

        public static CareerInfo ExportCareerInfo(CareerSaveFile csf)
        {

            var careerInfo = new CareerInfo(
                csf.GetMainDataSet(), 
                csf.MyTeamID ?? "",
                csf.MyTeamPlayersIDs, 
                csf.MyTeamPlayersIDtoName);

            return careerInfo;
        }

        public static CareerInfo ExportCareerInfo(CareerSaveFile csf, string temp)
        {

            var careerInfo = new CareerInfo(
                csf.GetMainDataSet(),
                string.Empty,
                new List<string>(),
                new Dictionary<string, string>());

            return careerInfo;
        }

        //public void setplayerstats(string playerid, string stat, int value = 99)
        //{
        //    if (stat == "birthdate") value = 154482;
        //    foreach (datarow _player in _playerstable)
        //    {

        //        string? _playerid = _player["playerid"].tostring();
        //        if (_playerid == playerid) // try to merge with setteamstats in a clean way
        //        {
        //            if (stat == "all")
        //            {
        //                foreach (string _stat in playerstats)
        //                {
        //                    if (_stat == "birthdate")
        //                    {
        //                        //_player[_stat] = 154482;
        //                        continue;
        //                    }
        //                    _player[_stat] = value;
        //                }
        //            }
        //            else
        //            {
        //                _player[stat] = value;
        //            }
        //        }


        //    }

        //}

       

        //public void SetTeamStats(List<string> playerIDs, string stat, int value = 99)
        //{
        //    if (stat == "birthdate") value = 154482;
        //    foreach (DataRow _player in _playersTable)
        //    {

        //        string? _playerID = _player["playerid"].ToString();
        //        if (playerIDs.Contains(_playerID))
        //        {

        //            if (stat == "ALL") // all stats except birthdate for now
        //            {
        //                foreach (string _stat in PlayerStats)
        //                {
        //                    if (_stat == "birthdate")
        //                    {
        //                        //_player[_stat] = 154482;
        //                        continue;
        //                    }
        //                    _player[_stat] = value;
        //                }
        //            }

        //            else //single stat
        //            {
        //                _player[stat] = value;

        //            }
        //        }
        //    }
        //}


        //public void SwapPSG(List<string> playerIDs, string myteamID)
        //{
        //    var teamplayerslink = DataSetEa[_indexOffset].Tables["teamplayerlinks"].Rows;
        //    var teamsheets = DataSetEa[_indexOffset].Tables["default_teamsheets"].Rows;

        //    foreach (DataRow player in teamplayerslink)
        //    {
        //        string? teamid = player["teamid"].ToString();

        //        if (teamid == "73") player["teamid"] = "111592";
        //        else if (teamid == myteamID) player["teamid"] = "73";

        //    }




        //}
    }
}
