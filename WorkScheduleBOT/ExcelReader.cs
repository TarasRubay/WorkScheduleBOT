using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;

namespace WorkScheduleBOT
{
    public class ExcelReader
    {
        private string excelFile { get; set; }
       
        public ExcelReader(string Path)
        {
            excelFile = Path;
        }
       
        public List<List<object>> readX(ref string table,string nameRead)
        {

            List<object> rowDataList = null;
            List<List<object>> allRowsList = new();
            try
            {

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = System.IO.File.Open(excelFile, FileMode.Open, FileAccess.Read))
                {
                    IExcelDataReader excelDataReader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
                    var conf = new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = a => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    DataSet dataSet = excelDataReader.AsDataSet(conf);
                    if(nameRead == "penunlimate") { 
                    
                       
                    try
                    {
                        DataRowCollection row = dataSet.Tables[dataSet.Tables.Count - 2].Rows;
                        foreach (DataRow item in row)
                        {
                            if (item is not null)
                            {
                                rowDataList = item.ItemArray.ToList(); //list of each rows
                                allRowsList.Add(rowDataList); //adding the above list of each row to another list
                            }
                        }
                            for (int i = 0; i < allRowsList[1].Count; i++)
                            {
                                if (allRowsList[1][i] is string) table += allRowsList[1][i].ToString() + ' ';
                            }
                            table += dataSet.Tables[dataSet.Tables.Count - 2].Columns[2].ToString();
                        }
                    catch (Exception)
                    {
                    }
                    }
                    else if (nameRead == "last")
                    {
                        
                        try
                        {
                            DataRowCollection row = dataSet.Tables[dataSet.Tables.Count - 1].Rows;
                            foreach (DataRow item in row)
                            {
                                if (item is not null)
                                {
                                    rowDataList = item.ItemArray.ToList(); //list of each rows
                                    allRowsList.Add(rowDataList); //adding the above list of each row to another list
                                }
                            }
                            for (int i = 0; i < allRowsList[1].Count; i++)
                            {
                                if (allRowsList[1][i] is string) table += allRowsList[1][i].ToString() + ' ';
                            }
                           
                            table += dataSet.Tables[dataSet.Tables.Count - 1].Columns[2].ToString();
                           
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return allRowsList;
        }
    }
}
