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
    public class SaveFile
    {

        readonly string _fifaDbFileName;
        readonly string _fifaDbXmlFileName;
        readonly string _internalFile;
        readonly DbFile _fifaDb;
        readonly DataSet _dataSet;
        readonly CareerFile _careerFile;

        public DataTable? PlayerNames;
        public DataSet[] DataSetEa;
        public FileType Type { get;}


        public SaveFile(string filename)

        {
            _internalFile = filename;
            Type = GetFileType(Path.GetFileName(filename));
            _fifaDbFileName = Path.Combine(Environment.CurrentDirectory, @"Data\", "fifa_ng_db.db");
            _fifaDbXmlFileName = Path.Combine(Environment.CurrentDirectory, @"Data\", "fifa_ng_db-meta.xml");            
            
            _careerFile = new CareerFile(_internalFile, _fifaDbXmlFileName);
            DataSetEa = _careerFile.ConvertToDataSet();
            _fifaDb = new DbFile(_fifaDbFileName, _fifaDbXmlFileName);
            
            _dataSet = this._fifaDb.ConvertToDataSet();
            PlayerNames = _dataSet.Tables["playernames"];

        }

        private FileType GetFileType(string filename)
        {
            if (filename.StartsWith("Squad")) return FileType.Squad;
            else if (filename.StartsWith("Career")) return FileType.Career;
            else
            {
                throw new Exception("Wrong Squad File");
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

        private void SaveEA()
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
