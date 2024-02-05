using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClosedXML.Excel;

using CommunityToolkit.Mvvm.Messaging;

using DocumentFormat.OpenXml.Spreadsheet;

using FIFAScripts.MVVM.Enums;
using FIFAScripts.MVVM.Messages;

namespace FIFAScripts.MVVM.Models
{
    public class Util
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

        private static DataTable? PositionConverterTable { get; set; }

        public Util()
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

        public static DataTable? LoadSheetTable(string path)
        {          
            var datatabel = new DataTable();
            DataSet dataSet = new DataSet();
            // Write dataset to xml file or stream            
            dataSet.ReadXml(path);
            return dataSet.Tables[0];


        }


        public static DataSet? ConvertXLToDataSet(string filepath)
        {

            DataTable dt = new DataTable();
            //Open the Excel file using ClosedXML.
            using (XLWorkbook workBook = new XLWorkbook(filepath))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);

                //Create a new DataTable.

                //Loop through the Worksheet rows.
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            if (!string.IsNullOrEmpty(cell.Value.ToString()))
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            else
                            {
                                break;
                            }
                        }
                        firstRow = false;
                    }
                    else
                    {
                        int i = 0;
                        DataRow toInsert = dt.NewRow();
                        foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                        {
                            try
                            {
                                toInsert[i] = cell.Value.ToString();
                            }
                            catch (Exception ex)
                            {

                            }
                            i++;
                        }
                        dt.Rows.Add(toInsert);
                    }
                }

                DataSet? dataSet = new DataSet();
                dataSet.Tables.Add(dt);
                return dataSet;
            }
        }

        public static DataTable? GetPositionalRatings(Dictionary<string, string>? myTeamPlayersIDtoName, EASaveFile? squadSaveFile)
        {
            if(PositionConverterTable is null)
            {
                PositionConverterTable = LoadSheetTable(@"Data\FIFA23PosCoefficents.xml");
            }
           
            DataTable? positionalRatings = new();
            if (PositionConverterTable?.Rows is { } rows
                && myTeamPlayersIDtoName is { })
            {
                var stats = rows[0].Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Where(x => x != "position");
                var columnNames = PositionConverterTable.AsEnumerable().Select(row => row.Field<string>("position")).ToList();
                positionalRatings.Columns.Add("playername", typeof(string));
                positionalRatings.Columns.Add("playerid", typeof(string));
                positionalRatings.Columns.AddRange(columnNames.Select(name => new DataColumn(name, typeof(string))).ToArray());
                

                foreach (KeyValuePair<string, string> kvp in myTeamPlayersIDtoName)
                {
                    var playerid = kvp.Key;
                    var playername = kvp.Value;
                    var playersStats = squadSaveFile?.GetPlayerStats(playerid);

                    DataRow newRow = positionalRatings.NewRow();
                    newRow["playername"] = playername;
                    newRow["playerid"] = playerid;
                    foreach (DataRow posRow in rows)
                    {
                        string? Pos = posRow["position"].ToString();
                        float posRating = 0;
                        foreach (string stat in stats)
                        {
                            float value = float.Parse(playersStats?[stat] ?? "0");
                            float coefficient = float.Parse(posRow[stat].ToString() ?? "0");

                            posRating += value * coefficient;
                        }
                        newRow[Pos ?? ""] = posRating.ToString("0.00");

                    }

                    positionalRatings.Rows.Add(newRow);
                }

                XLWorkbook wb = new XLWorkbook();
                wb.Worksheets.Add(positionalRatings, "Positional Rating");
                wb.SaveAs(@"Data\PositionalRatings.xlsx");
               
            }

            return positionalRatings;
        }



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
