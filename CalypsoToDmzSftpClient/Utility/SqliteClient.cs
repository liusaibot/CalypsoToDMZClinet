using CalypsoToDmzSftpClient.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalypsoToDmzSftpClient.Utility
{
    public class SqliteClient
    {
        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public SqliteClient()
        {
            this.CreateDatabaseAndTable();

        }

        public async Task CreateDatabaseAndTable()
        {
            try
            {
                if (!File.Exists("checksumDB.sqlite"))
                {
                    SQLiteConnection.CreateFile("checksumDB.sqlite");

                    //string sql = @"CREATE TABLE Checksum(
                    //           ID INTEGER PRIMARY KEY AUTOINCREMENT ,
                    //           DataKey           TEXT      NOT NULL,
                    //           DataChecksum      TEXT       NOT NULL,
                    //           Filename          TEXT       NOT NULL
                    //        );";

                    string sql = @"CREATE TABLE ChecksumData(
                               Id INTEGER PRIMARY KEY AUTOINCREMENT ,
                               FileName           TEXT      NOT NULL,
                               FileCategory       TEXT      NOT NULL,
                               Checksum           TEXT       NOT NULL,
                               DateUploaded       TEXT       NOT NULL
                            );";
                    con = new SQLiteConnection("Data Source=checksumDB.sqlite;Version=3;");
                    con.Open();
                    cmd = new SQLiteCommand(sql, con);
                    await Task.FromResult<int>(cmd.ExecuteNonQuery());
                    con.Close();
                    //InitializeData();
                    //InitializeData();
                }
                else
                {
                    con = new SQLiteConnection("Data Source=checksumDB.sqlite;Version=3;");
                }
            }
            catch (Exception ex)
            {
                XLogger.AppendToLog($"Unable to Create Database. {ex.ToString()}");
                throw ex;
            }
        }

        public async Task InitializeData()
        {

            ChecksumItem productItem = new ChecksumItem
            {
                dataKey = DataKey.Maturity,
                dataChecksum = "",
                filename = ""
            };

            ChecksumItem holidayItem = new ChecksumItem
            {
                dataKey = DataKey.Holiday,
                dataChecksum = "",
                filename = ""
            };

            ChecksumItem fxItem = new ChecksumItem
            {
                dataKey = DataKey.Fx,
                dataChecksum = "",
                filename = ""
            };

            try
            {
                //await Task.FromResult(InsertData(productItem));
                //await Task.FromResult(InsertData(holidayItem));
                //await Task.FromResult(InsertData(fxItem));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreatetData(ChecksumData checksumData)
        {
            try
            {
                cmd = new SQLiteCommand();
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = $"insert into ChecksumData({ChecksumDataColumn.FileName}, {ChecksumDataColumn.FileCategory}, {ChecksumDataColumn.Checksum}, {ChecksumDataColumn.DateUploaded}) values ('{checksumData.FileName}','{checksumData.FileCategory}','{checksumData.Checksum}', '{checksumData.DateUploaded}')";
                //cmd.ExecuteNonQuery();
                await Task.FromResult<int>(cmd.ExecuteNonQuery());
                con.Close();
                string dataJson = JsonConvert.SerializeObject(checksumData);
                XLogger.AppendToLog($"Data added: {dataJson}");
            }
            catch (Exception ex)
            {
                string dataJson = JsonConvert.SerializeObject(checksumData);
                XLogger.AppendToLog($"Data added: {dataJson}. {ex.ToString()}");
            }
        }

        public async Task<List<ChecksumData>> SelectData()
        {
            XLogger.AppendToLog($"Fetching all records");
            Console.WriteLine($"Fetching all records");
            List<string> entries = new List<string>();
            List<ChecksumData> checksumDataList = new List<ChecksumData>();

            try
            {
                cmd = new SQLiteCommand();
                con.OpenAsync().Wait();
                cmd.Connection = con;
                cmd.CommandText = "Select * From ChecksumData";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader dataReader = await Task.FromResult<SQLiteDataReader>(cmd.ExecuteReader());
                while (dataReader.Read())
                {
                    ChecksumData checksumData = new ChecksumData()
                    {
                        Id = Int32.Parse(dataReader["Id"].ToString()),
                        Checksum = dataReader["Checksum"].ToString(),
                        DateUploaded = dataReader["DateUploaded"].ToString(),
                        FileCategory = dataReader["FileCategory"].ToString(),
                        FileName = dataReader["FileName"].ToString()
                    };
                    checksumDataList.Add(checksumData);
                }
                con.Close();

                return checksumDataList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Select Data Failed: {ex.ToString()}");
                XLogger.AppendToLog($"Select Data Failed: {ex.ToString()}");
                return checksumDataList;
            }
        }

        public async Task<ChecksumData> SelectDataByKey(string value, string column)
        {

            XLogger.AppendToLog($"Fetching record for {value} from {column} column in table");
            Console.WriteLine($"Fetching record for {value} from {column} column in table");
            List<string> entries = new List<string>();
            //List<ChecksumItem> items = new List<ChecksumItem>();
            List<ChecksumData> checksumDataList = new List<ChecksumData>();
            try
            {
                cmd = new SQLiteCommand();
                con.OpenAsync().Wait();
                cmd.Connection = con;
                cmd.CommandText = $"Select * from ChecksumData where {column} = '{value}'";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader dataReader = await Task.FromResult<SQLiteDataReader>(cmd.ExecuteReader());
                while (dataReader.Read())
                {
                    ChecksumData checksumData = new ChecksumData()
                    {
                        Id = Int32.Parse(dataReader["Id"].ToString()),
                        Checksum = dataReader["Checksum"].ToString(),
                        DateUploaded = dataReader["DateUploaded"].ToString(),
                        FileCategory = dataReader["FileCategory"].ToString(),
                        FileName = dataReader["FileName"].ToString()
                    };
                    checksumDataList.Add(checksumData);
                }
                con.Close();

                return checksumDataList.FirstOrDefault() ?? null;
            }
            catch (Exception ex)
            {
                XLogger.AppendToLog($"Data Could not be fetched {ex.ToString()}");
                Console.WriteLine($"Data Could not be fetched {ex.ToString()}");
                return null;                
            }
        }

        public async Task<List<ChecksumItem>> getItemsByKey(string datakey)
        {
            try
            {
                XLogger.AppendToLog($"Fetching record for {datakey}");
                Console.WriteLine($"Fetching record for {datakey}");
                List<string> entries = new List<string>();
                List<ChecksumItem> items = new List<ChecksumItem>();
                cmd = new SQLiteCommand();
                con.OpenAsync().Wait();
                cmd.Connection = con;
                cmd.CommandText = "Select * from Checksum where dataKey = '" + datakey + "'";
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader r = await Task.FromResult<SQLiteDataReader>(cmd.ExecuteReader());
                while (r.Read())
                {
                    ChecksumItem item = new ChecksumItem()
                    {
                        dataChecksum = r["DataChecksum"].ToString(),
                        dataKey = r["DataKey"].ToString(),
                        filename = r["Filename"].ToString()

                    };
                    XLogger.AppendToLog("Checksum Item " + JsonConvert.SerializeObject(item));
                    items.Add(item);

                }
                con.Close();

                return items;
            }
            catch (Exception ex)
            {
                XLogger.AppendToLog(ex.ToString());
                throw ex;
            }
        }

        public async Task UpdateDataByKey(ChecksumData checksumData, string column)
        {
            var currentItem = await SelectDataByKey(checksumData.FileName, column);

            try
            {
                if (currentItem == null)
                {
                    XLogger.AppendToLog($"No Record found for {checksumData.FileName} Not Found!");
                    Console.WriteLine($"No Record found for {checksumData.FileName} Not Found!");
                    return;
                }
                cmd = new SQLiteCommand();
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = $"update ChecksumData set " +
                    $"{ChecksumDataColumn.FileName} = '{checksumData.FileName}', " +
                    $"{ChecksumDataColumn.FileCategory} = '{checksumData.FileCategory}', " +
                    $"{ChecksumDataColumn.Checksum} = '{checksumData.Checksum}'" +
                    $"where Id = '{currentItem.Id}'";
                //cmd.ExecuteNonQuery();
                await Task.FromResult<int>(cmd.ExecuteNonQuery());
                con.Close();
                return;
            }

            catch(Exception ex)
            {
                var dataJson = JsonConvert.SerializeObject(checksumData);
                XLogger.AppendToLog($"Unable to Append {checksumData} to Log");
                Console.WriteLine($"Unable to Append {checksumData} to Log");
                return;
            }
        }

        public async Task AddData(ChecksumItem checksumItem)
        {
            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "insert into Checksum(dataKey,dataChecksum, filename) values ('" + checksumItem.dataKey + "','" + checksumItem.dataChecksum + "','" + checksumItem.filename + "')";
            //cmd.ExecuteNonQuery();
            await Task.FromResult<int>(cmd.ExecuteNonQuery());
            con.Close();
        }

        //public async Task UpdateData(ChecksumItem checksumItem)
        //{
        //    var currentItem = getItemsByKey(checksumItem.dataKey);
        //    cmd = new SQLiteCommand();
        //    con.Open();
        //    cmd.Connection = con;
        //    cmd.CommandText = $"update Checksum set dataChecksum = '{checksumItem.dataChecksum}', filename = '{checksumItem.filename}' where dataKey = '{checksumItem.dataKey}'";
        //    //cmd.ExecuteNonQuery();
        //    await Task.FromResult<int>(cmd.ExecuteNonQuery());
        //    con.Close();
        //}




    }
}
