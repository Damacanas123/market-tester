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
                Console.WriteLine("Problem connecting to database. \n" + ex.ToString() + "\n\n");
            }         
        }

        internal bool CreateTable(IDataBaseWritable writable)
        {
            string query = "CREATE TABLE IF NOT EXISTS " + writable.TableName + " (";
            foreach(TableField field in writable.Fields.Values)
            {
                query += field.Name + " " + TypeConvert(field.Type) +
                    (field.Length == 0 ? "(" + field.Length + ")" : "") + " " + field.Constraints + " ,";
            }
            query = query.Substring(0, query.Length - 1) + ");";
            return ExecuteNonQuery(query);
        }

        internal bool Insert(IDataBaseWritable writable)
        {
            string query = "INSERT INTO " + writable.TableName + " (";
            List<object> values = new List<object>();
            foreach (KeyValuePair<string,TableField> item in writable.Fields)
            {
                query += item.Value.Name + ",";
                values.Add(writable.Values[item.Key]);
            }
            query = query.Substring(0, query.Length - 1) + ") VALUES (";
            foreach(object value in values)
            {
                query += "'" + value.ToString() + "',";
            }
            query = query.Substring(0, query.Length - 1);
            return ExecuteNonQuery(query);
        }
        internal bool Update(IDataBaseWritable writable)
        {
            string query = $"UPDATE {writable.TableName} SET ";
            Dictionary<string,object> values = writable.Values;
            
            foreach(KeyValuePair<string,TableField> entry in writable.Fields)
            {
                string valueEncloser = (entry.Value.Type == typeof(string) ? "'" : "");
                query += entry.Value.Name + " = " + valueEncloser + values[entry.Key] + valueEncloser + ",";
            }
            query = query.Substring(0, query.Length - 1) + " WHERE " + nameof(writable.DatabaseID) + " = '" + writable.DatabaseID + "'";
            return ExecuteNonQuery(query);
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
        private string TypeConvert(Type type)
        {
            if(type == typeof(string))
            {
                return "VARCHAR";
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
    }
}
