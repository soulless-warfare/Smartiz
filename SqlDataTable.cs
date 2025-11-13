using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;

namespace Smartiz
{
    public class SqlDataTable
    {

        public DataTable DataTable;
        private string SqlServerTableName;
        private string SqlConncetionString;

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>Make a new Sql server data base</summary>
        public SqlDataTable(string sqlConncetionString, string sqlServerTableName)
        {
            SqlConncetionString = sqlConncetionString;
            SqlServerTableName = sqlServerTableName;
            DataTable = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>Connect to sql server database with certral connection string</summary>
        internal Exception Connect()
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");

            SqlConnection sqlConncetion = new SqlConnection(SqlConncetionString); ;
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            sqlConncetion.Open();

            try
            {
                sqlCommand = new SqlCommand($"SELECT * FROM {SqlServerTableName}", sqlConncetion);
                sqlDataReader = sqlCommand.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sqlDataReader);

                DataTable = dt;
                return new Exception("Successfuly");
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
                sqlCommand?.Cancel();
                sqlDataReader?.Close();
            }
        }

        /// <summary>Insert row to database</summary>
        public Exception Add(DataRow Row)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");

            SqlConnection sqlConncetion = null;
            SqlCommand sqlCommand = null;
            sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                string columns = string.Join(", ", Row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                string values = string.Join(", ", Row.Table.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));

                sqlCommand = new SqlCommand($"INSERT INTO {SqlServerTableName} ({columns}) VALUES ({values})", sqlConncetion);

                foreach (DataColumn column in Row.Table.Columns)
                {
                    sqlCommand.Parameters.AddWithValue($"@{column.ColumnName}", Row[column]);
                }

                if (sqlCommand.ExecuteNonQuery() == 1)
                {
                    DataTable.Rows.Add(Row);
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");

            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
                sqlCommand?.Cancel();
            }
        }

        /// <summary>Remove row from database</summary>
        internal Exception Remove(string ID)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");

            SqlConnection sqlConncetion = null;
            SqlCommand sqlCommand = null;
            sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                sqlCommand = new SqlCommand($"DELETE FROM {SqlServerTableName} WHERE ID = N'{ID}'", sqlConncetion);

                if (sqlCommand.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable()
                           .FirstOrDefault(row => row.Field<string>("ID") == ID);

                    DataTable.Rows.Remove(targetRow);
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
                sqlCommand?.Cancel();
            }
        }

        /// <summary>Remove row from database</summary>
        internal Exception Remove(string KeyColumnsName, string keyValue)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");

            SqlConnection sqlConncetion = null;
            SqlCommand sqlCommand = null;
            sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                sqlCommand = new SqlCommand($"DELETE FROM {SqlServerTableName} WHERE {KeyColumnsName} = N'{keyValue}'", sqlConncetion);

                if (sqlCommand.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable()
                           .FirstOrDefault(row => row.Field<string>(KeyColumnsName) == keyValue);

                    DataTable.Rows.Remove(targetRow);
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
                sqlCommand?.Cancel();
            }
        }

        /// <summary>Update row in data base with row id and eny value type except image</summary>
        internal Exception Update(string ID, string ColumnName, object Value)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");
            SqlConnection sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                SqlCommand sqlCommand = new SqlCommand($"UPDATE {SqlServerTableName} SET {ColumnName} = @Value WHERE ID = @ID", sqlConncetion);
                sqlCommand.Parameters.AddWithValue("@Value", Value);
                sqlCommand.Parameters.AddWithValue("@ID", ID);

                if (sqlCommand.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable()
                           .FirstOrDefault(row => row.Field<string>("ID") == ID);

                    if (targetRow != null)
                    {
                        targetRow[ColumnName] = Value;
                    }
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
            }
        }

        /// <summary>Update row in data base with row id and eny value type except image</summary>
        internal Exception Update(string KeyColumnsName, string keyValue, string ColumnName, object Value)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");
            SqlConnection sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                SqlCommand sqlCommand = new SqlCommand($"UPDATE {SqlServerTableName} SET {ColumnName} = @Value WHERE {KeyColumnsName} = @KeyValue", sqlConncetion);
                sqlCommand.Parameters.AddWithValue("@Value", Value);
                sqlCommand.Parameters.AddWithValue("@KeyValue", keyValue);

                if (sqlCommand.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable()
                           .FirstOrDefault(row => row.Field<string>(KeyColumnsName) == keyValue);

                    if (targetRow != null)
                    {
                        targetRow[ColumnName] = Value;
                    }
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
            }
        }

        /// <summary>Update image from data base with row id</summary>
        internal Exception UpdateImage(string ID, string ColumnName, byte[] Value)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return new Exception("((Sql Conncetion String)) or ((SqlServerTableName)) is null");
            SqlConnection sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                SqlCommand sqlCommand = new SqlCommand($"UPDATE [{SqlServerTableName}] SET [{ColumnName}] = @Value WHERE ID = @ID", sqlConncetion);
                sqlCommand.Parameters.Add("@ID", SqlDbType.NVarChar).Value = ID;
                sqlCommand.Parameters.Add("@Value", SqlDbType.Image).Value = Value;

                if (sqlCommand.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable()
                           .FirstOrDefault(row => row.Field<string>("ID") == ID);

                    if (targetRow != null)
                    {
                        targetRow[ColumnName] = Value;
                    }
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex)
            {
                return ex;
            }
            finally
            {
                sqlConncetion?.Close();
            }
        }

        /// <summary>Find row from data base with column and it value</summary>
        internal DataRow Find(string ColumnName, object Value)
        {
            if (SqlConncetionString == null || SqlServerTableName == null) return null;
            SqlConnection sqlConncetion = new SqlConnection(SqlConncetionString);
            sqlConncetion.Open();

            try
            {
                SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM {SqlServerTableName} WHERE {ColumnName} = N'{Value}'", sqlConncetion);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dt = new DataTable();

                sqlDataAdapter.Fill(dt);

                if (dt.Rows.Count > 0) return dt.Rows[0];
                else return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                sqlConncetion?.Close();
            }
        }

        /// <summary>sSwap tables columns with together</summary>
        internal DataTable SwapTablesColumns(DataTable dtA, DataTable dtB, string cnA, string cnB, string cFK)
        {
            Type columnType = dtB.Columns[cnB].DataType;

            DataTable result = dtA.Copy();

            if (result.Columns.Contains(cnA)) result.Columns.Remove(cnA);

            result.Columns.Add(cnB, columnType);

            var lookupB = dtB.AsEnumerable().Where(row => row[cFK] != DBNull.Value).
                ToDictionary(row => row[cFK].ToString(), row => row[cnB]);

            foreach (DataRow row in result.Rows)
            {
                string key = row[cFK].ToString();

                if (key != null && lookupB.ContainsKey(key)) row[cnB] = lookupB[key];
                else row[cnB] = DBNull.Value;
            }

            return result;
        }

        /// <summary>Extraction row from data base with row id</summary>
        internal DataRow ExtractionRow(string ID)
        {
            foreach (DataRow row in DataTable.Rows)
            {
                if (row["ID"].ToString() == ID)
                {
                    return row;
                }
            }
            return null;
        }

        /// <summary>Extarction row from data base with row column and it value</summary>
        internal DataRow ExtractionRow(string Column, string Value)
        {
            foreach (DataRow row in DataTable.Rows)
            {
                if (row[Column].ToString() == Value)
                {
                    return row;
                }
            }
            return null;
        }
        /// <summary>Extarction row from data base with row column and it value</summary>
        internal DataRow[] ExtractionRows(string Column, string Value)
        {
            List<DataRow> result = new List<DataRow>();

            foreach (DataRow row in DataTable.Rows)
            {
                if (row[Column].ToString() == Value)
                {
                    result.Add(row);
                }
            }
            return result.ToArray();
        }

        /// <summary>Convert TimeSpan to Sql server time</summary>
        public string ConvertTimeSpanToSqlTime(TimeSpan timeSpan)
        {
            return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
        }

        /// <summary>Make random and uniqe code with optional size</summary>
        internal string CreateID(int size)
        {
            string ID = string.Empty;

            do { ID = RCode(); }
            while (Find("ID", ID) != null);

            return ID;

            string RCode()
            {
                if (size > 60) size = 60;
                if (size < 0) size = 0;

                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random random = new Random();
                char[] result = new char[size];

                for (int i = 0; i < size; i++)
                {
                    result[i] = chars[random.Next(chars.Length)];
                }

                return new string(result);
            }
        }

        /// <summary>Make random and uniqe code with optional size</summary>
        internal string CreateID(string columnName,int size)
        {
            string ID = string.Empty;

            do { ID = RCode(); }
            while (Find(columnName, ID) != null);

            return ID;

            string RCode()
            {
                if (size > 60) size = 60;
                if (size < 0) size = 0;

                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random random = new Random();
                char[] result = new char[size];

                for (int i = 0; i < size; i++)
                {
                    result[i] = chars[random.Next(chars.Length)];
                }

                return new string(result);
            }
        }

        /// <summary>Reconnect to database</summary>
        internal void Refresh()
        {
            DataTable.Clear();
            Connect();
        }

        ///////////////////////////////////////////////////////////////////////////////////
    }
}
