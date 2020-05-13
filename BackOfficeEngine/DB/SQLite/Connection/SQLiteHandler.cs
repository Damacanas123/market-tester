using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using BackOfficeEngine.AppConstants;
using BackOfficeEngine.Logger;
using BackOfficeEngine.Model;
using BackOfficeEngine.Helper;
using System.Collections.Concurrent;
using System.Threading;
using System.Globalization;

namespace BackOfficeEngine.DB.SQLite
{
    
    internal class SQLiteHandler : IDisposable
    {
        private SQLiteConnection conn;
        public void Dispose()
        {
            conn.Close();
        }

        internal SQLiteHandler()
        {
            try
            {
                this.conn = new SQLiteConnection("Data Source=" + CommonFolders.OrderDBFilePath + ";Version=3;", true);
                this.conn.Open();
            }
            catch (Exception ex)
            {
                ErrorLogger.DBError(ex);
            }         
        }

        internal void CreateTable(IDataBaseWritable writable)
        {
            string query = "CREATE TABLE IF NOT EXISTS " + writable.TableName + " (";
            foreach(TableField field in writable.Fields.Values)
            {
                query += field.Name + " " + TypeConvert(field.Type) +
                    (field.Length == 0 ? "(" + field.Length + ")" : "") + " " + field.Constraints + " ,";
            }
            query = query.Substring(0, query.Length - 1) + ");";
            ExecuteNonQuery(query);
            foreach (TableField field in writable.Fields.Values)
            {
                AddColumnToTableIfNotExists(field.Name, writable.TableName, field.DefaultValue, TypeConvert(field.Type), field.Length.ToString(CultureInfo.InvariantCulture));
            }
        }

        public bool FieldExists(string tableName, string columnName)
        {
            bool exists = false;
            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT * FROM " + tableName + " LIMIT 1", this.conn);
                var reader = command.ExecuteReader();
                for (var i = 0; i < reader.FieldCount && !exists; i++)
                {
                    if (reader.GetName(i).Equals(columnName))
                    {
                        exists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem fetching column names from " + tableName + "." + ex.ToString() + "\n\n");
                throw new Exception();
            }
            return exists;
        }

        public void AddColumnToTableIfNotExists(string columnName,string tableName, string defaultValue, string parameterType, string length = null)
        {
            string localType = length != null ? parameterType + "(" + length + ")" : parameterType;
            string valueEncloser = parameterType.Contains("CHAR") ? "'" : "";
            string sql = "ALTER TABLE " + tableName + " ADD " + columnName +
                            " " + localType + " NOT NULL DEFAULT " + valueEncloser +
                            defaultValue + valueEncloser + ";";

            try
            {
                if (!FieldExists(tableName, columnName))
                {
                    SQLiteCommand command = new SQLiteCommand(sql, this.conn);
                    command.ExecuteNonQuery();
                    Console.WriteLine(sql);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(sql);
                Console.WriteLine("Problem executing add column query to orders table.\n" + ex.ToString() + "\n\n");
            }
        }

        internal void Insert(IDataBaseWritable writable)
        {
            TransactionQueue.Enqueue((writable, TransactionType.Insert));
        }

        private void InsertPrivate(IDataBaseWritable writable)
        {
            string query = "INSERT INTO " + writable.TableName + " (";
            List<object> values = new List<object>();
            foreach (KeyValuePair<string, TableField> item in writable.Fields)
            {
                query += item.Value.Name + ",";
                values.Add(writable.Values[item.Key]);
            }
            query = query.Substring(0, query.Length - 1) + ") VALUES (";
            foreach (object value in values)
            {
                string repr;
                if (value is bool)
                {

                    repr = (bool)value ? "1" : "0";
                }
                else
                {
                    repr = value.ToString();
                }
                query += "'" + repr + "',";
            }
            query = query.Substring(0, query.Length - 1) + ")";
            ExecuteNonQuery(query);
        }
        internal void Update(IDataBaseWritable writable)
        {
            TransactionQueue.Enqueue((writable, TransactionType.Update));
        }
        private void UpdatePrivate(IDataBaseWritable writable)
        {
            string query = $"UPDATE {writable.TableName} SET ";
            Dictionary<string, object> values = writable.Values;

            foreach (KeyValuePair<string, TableField> entry in writable.Fields)
            {
                string valueEncloser = (entry.Value.Type == typeof(string) ? "'" : "");
                query += entry.Value.Name + " = " + valueEncloser + values[entry.Key] + valueEncloser + ",";
            }
            query = query.Substring(0, query.Length - 1) + " WHERE " + nameof(writable.DatabaseID) + " = '" + writable.DatabaseID + "'";
            ExecuteNonQuery(query);
        }
        internal List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            SQLiteDataReader reader = ExecuteReader($"SELECT * FROM {new Order().TableName}");
            if (reader != null)
            {
                while (reader.Read())
                {
                    orders.Add(new Order(reader));
                }
            }
            return orders;
        }

        internal List<Order> GetTodayOrders()
        {
            Order dummyOrder = new Order();
            List<Order> orders = new List<Order>();
            SQLiteDataReader reader = ExecuteReader($"SELECT * FROM {dummyOrder.TableName} WHERE {nameof(dummyOrder.Date)} = '{Util.GetTodayString()}'");
            if (reader != null)
            {
                while (reader.Read())
                {
                    orders.Add(new Order(reader));
                }
            }
            return orders;
        }

        internal List<Position> GetPositions()
        {
            Position dummyPosition = new Position();
            List<Position> positions = new List<Position>();
            SQLiteDataReader reader = ExecuteReader($"SELECT * FROM {dummyPosition.TableName}");
            if(reader != null)
            {
                while (reader.Read())
                {
                    positions.Add(new Position(reader));
                }
            }
            return positions;
        } 

        internal bool Truncate(IDataBaseWritable writable)
        {
            string query = $"DELETE FROM {writable.TableName};";
            return ExecuteNonQuery(query);
        }
        private static string TypeConvert(Type type)
        {
            if(type == typeof(string))
            {
                return "VARCHAR";
            }
            else if( type == typeof(bool))
            {
                return "SMALLINT";
            }
            throw new NotImplementedException("Unimplemented data type");
        }

        private bool ExecuteNonQuery(string query)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, this.conn);
                command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                ErrorLogger.DBError(ex);
                return false;
            }
        }

        private SQLiteDataReader ExecuteReader(string query)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, this.conn);
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                ErrorLogger.DBError(ex);
                return null;
            }
        }

        enum TransactionType
        {
            Insert,Update
        }
        static ConcurrentQueue<(IDataBaseWritable, TransactionType)> TransactionQueue { get; set; } = new ConcurrentQueue<(IDataBaseWritable, TransactionType)>();
        static bool IsTransactionThreadStarted = false;

        public static void StartTransactionThread()
        {
            if (IsTransactionThreadStarted)
            {
                return;
            }
            IsTransactionThreadStarted = true;
            Util.ThreadStart(() =>
            {
                while (true)
                {
                    if (TransactionQueue.Count > 0)
                    {
                        using (SQLiteHandler handler = new SQLiteHandler())
                        {
                            while (TransactionQueue.TryDequeue(out var item))
                            {
                                IDataBaseWritable writable = item.Item1;
                                TransactionType transType = item.Item2;
                                switch (transType)
                                {
                                    case TransactionType.Insert:
                                        handler.InsertPrivate(writable);
                                        break;
                                    case TransactionType.Update:
                                        handler.UpdatePrivate(writable);
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                }
            });
        }

    }
}
