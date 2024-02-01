using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using ClosedXML.Excel;

using DocumentFormat.OpenXml.Office2016.Excel;

using FifaLibrary;

using FIFAScripts.MVVM.Enums;

namespace FIFAScripts.MVVM.Models
{
    public class EASaveFile
    {

        readonly string _fifaDbFileName;
        readonly string _fifaDbXmlFileName;
        readonly string _internalFile;
        readonly DbFile _fifaDb;
        readonly DataSet _dataSet;
        readonly CareerFile _careerFile;

        public DataTable? PlayerNames { get; }
        public DataSet[] DataSetEa { get; }
        public FileType Type { get; }

        protected readonly DataTable? _playersTable;
        protected readonly DataTable? _teamplayerlinks;

        protected readonly int _indexOffset;



        public EASaveFile(string filename, int indexOffset = 0)
        {
            _indexOffset = indexOffset;
            _internalFile = filename;
            Type = GetFileType(Path.GetFileName(filename));
            _fifaDbFileName = Path.Combine(Environment.CurrentDirectory, @"Data\", "fifa_ng_db.db");
            _fifaDbXmlFileName = Path.Combine(Environment.CurrentDirectory, @"Data\", "fifa_ng_db-meta.xml");

            _careerFile = new CareerFile(_internalFile, _fifaDbXmlFileName);
            DataSetEa = _careerFile.ConvertToDataSet();
            _fifaDb = new DbFile(_fifaDbFileName, _fifaDbXmlFileName);

            _dataSet = this._fifaDb.ConvertToDataSet();
            PlayerNames = _dataSet.Tables["playernames"];

            _playersTable = GetPlayersTable();
            _teamplayerlinks = GetTeamPlayerLinks();

        }

        protected FileType GetFileType(string filename)
        {
            if (filename.StartsWith("Squad")) return FileType.Squad;
            else if (filename.StartsWith("Career")) return FileType.Career;
            else
            {
                throw new Exception("Wrong Squad File");
            }

        }

        protected DataTable GetPlayersTable()
        {
            return DataSetEa[_indexOffset].Tables["players"] ?? new DataTable();
        }

        protected DataTable GetTeamPlayerLinks()
        {
            return DataSetEa[_indexOffset].Tables["teamplayerlinks"] ?? new DataTable();

        }


        public DataSet GetMainDataSet()
        {
            return DataSetEa[_indexOffset];
        }        

        public int ImportCareerInfo(CareerInfo careerInfo)
        {
            return 0;

        }

        public void ImportDataSet(DataSet mainDataSet)
        {

            foreach (DataTable savedTable in mainDataSet.Tables)
            {
                string tablename = savedTable.TableName;
                if (tablename == "manager") continue;


                var TargetTable = DataSetEa[_indexOffset]?.Tables[tablename]?.Rows;

                foreach (DataRow row in savedTable.Rows)
                {
                    TargetTable?.RemoveAt(0);
                    TargetTable?.Add(row.ItemArray);
                }

            }

        }

        public Dictionary<string, string> GetPlayerStats(string playerID)
        {
            Dictionary<string, string> ret = new();
            if (string.IsNullOrEmpty(playerID)) return ret;

            DataRow? playerStats = _playersTable?.Select($"playerid= {playerID}").First();

            foreach (string _stat in Util.PlayerStats)
            {
                ret.Add(_stat, (playerStats?[_stat])?.ToString() ?? "0");
            }

            return ret;

        }

        public void SetPlayerStat(string playerID, string statName, int value = 99)
        {
            if(_playersTable?.Select($"playerid= {playerID}").First() is { } player)
            {
                player[statName] = value;
            }                
        }

        public void SetPlayerStats(string playerID, int value = 99)
        {            
            if(_playersTable?.Select($"playerid= {playerID}").First() is { } player)
            {
                foreach (string _stat in Util.PlayerStats)
                {
                    if (_stat == "birthdate")
                    {
                        //player[_stat] = 154482;
                        continue;
                    }

                    player[_stat] = value;
                }
            }
        }


        public string ExportToXL()
        {

            // EXPORT XL FILE FROM DataSet
            var filename = Path.GetFileName(_careerFile.FileName);
            foreach (DataSet dataSet in DataSetEa)
            {
                var wb = new XLWorkbook();
                wb.Worksheets.Add(dataSet);
                wb.SaveAs(filename + ".xlsx");
                filename += "1";
            }


            return filename;
        }

        public void Save()
        {
            SaveEA();
        }

        protected void SaveEA()
        {

            //Console.WriteLine(text);         
            this._careerFile.ConvertFromDataSet(this.DataSetEa);
            var directoryName = Path.GetDirectoryName(this._careerFile.FileName);
            var fileName = Path.GetFileName(this._careerFile.FileName);
            string backupFileName;
            var i = 0;
            do
            {
                backupFileName = directoryName + $"\\Backup{i}_" + fileName;
                i++;


            }
            while (File.Exists(backupFileName));

            File.Copy(this._careerFile.FileName, backupFileName, true);
            this._careerFile.SaveEa(this._careerFile.FileName);

        }

        
    }
}
