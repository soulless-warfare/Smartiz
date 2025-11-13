using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace Smartiz
{
    public class SqliteDataTable
    {
        public DataTable DataTable;
        private string SqliteTableName;
        private string SqliteConnectionString;

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>Make a new Sqlite data base</summary>
        public SqliteDataTable(string sqliteConnectionString, string sqliteTableName)
        {
            SqliteConnectionString = sqliteConnectionString;
            SqliteTableName = sqliteTableName;
            DataTable = null;
        }

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>Connect to sqlite database with central connection string</summary>
        internal bool Connect()
        {
            if (string.IsNullOrEmpty(SqliteConnectionString) || string.IsNullOrEmpty(SqliteTableName))
                return false;         

            try
            {
                using var connection = new SqliteConnection(SqliteConnectionString);
                connection.Open();

                using var cmd = new SqliteCommand($"SELECT * FROM {SqliteTableName}", connection);
                using var reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                DataTable = dt;

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>Insert row to database</summary>
        public Exception Add(DataRow Row)
        {
            if (SqliteConnectionString == null || SqliteTableName == null)
                return new Exception("Connection string or table name is null");

            try
            {
                using var connection = new SqliteConnection(SqliteConnectionString);
                connection.Open();

                string columns = string.Join(", ", Row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                string values = string.Join(", ", Row.Table.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));

                using var cmd = new SqliteCommand($"INSERT INTO {SqliteTableName} ({columns}) VALUES ({values})", connection);

                foreach (DataColumn column in Row.Table.Columns)
                {
                    cmd.Parameters.AddWithValue($"@{column.ColumnName}", Row[column] ?? DBNull.Value);
                }

                int result = cmd.ExecuteNonQuery();
                if (result == 1)
                {
                    DataRow newRow = DataTable.NewRow();
                    foreach (DataColumn column in Row.Table.Columns)
                    {
                        newRow[column.ColumnName] = Row[column.ColumnName];
                    }
                    DataTable.Rows.Add(newRow);

                    return new Exception("Successfuly");
                }
                else
                {
                    return new Exception("Cannot insert into database");
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>Remove row from database by key column</summary>
        internal Exception Remove(string KeyColumnsName, string keyValue)
        {
            if (SqliteConnectionString == null || SqliteTableName == null)
                return new Exception("((Sqlite Conncetion String)) or ((SqliteTableName)) is null");

            using var connection = new SqliteConnection(SqliteConnectionString);
            try
            {
                connection.Open();
                using var cmd = new SqliteCommand($"DELETE FROM {SqliteTableName} WHERE {KeyColumnsName} = @Value", connection);
                cmd.Parameters.AddWithValue("@Value", keyValue);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable()
                        .FirstOrDefault(row => row.Field<string>(KeyColumnsName) == keyValue);
                    if (targetRow != null) DataTable.Rows.Remove(targetRow);
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex) { return ex; }
        }

        /// <summary>Update any column by key</summary>
        internal Exception Update(string KeyColumnsName, string keyValue, string ColumnName, object Value)
        {
            if (SqliteConnectionString == null || SqliteTableName == null)
                return new Exception("((Sqlite Conncetion String)) or ((SqliteTableName)) is null");

            using var connection = new SqliteConnection(SqliteConnectionString);
            try
            {
                connection.Open();
                using var cmd = new SqliteCommand($"UPDATE {SqliteTableName} SET {ColumnName} = @Value WHERE {KeyColumnsName} = @KeyValue", connection);
                cmd.Parameters.AddWithValue("@Value", Value ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@KeyValue", keyValue);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable().FirstOrDefault(row => row.Field<string>(KeyColumnsName) == keyValue);
                    if (targetRow != null) targetRow[ColumnName] = Value;
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex) { return ex; }
        }

        /// <summary>Update image column (BLOB) by ID</summary>
        internal Exception UpdateImage(string KeyColumnsName, string keyValue, string ColumnName, byte[] Value)
        {
            if (SqliteConnectionString == null || SqliteTableName == null)
                return new Exception("((Sqlite Conncetion String)) or ((SqliteTableName)) is null");

            using var connection = new SqliteConnection(SqliteConnectionString);
            try
            {
                connection.Open();
                using var cmd = new SqliteCommand($"UPDATE {SqliteTableName} SET {ColumnName} = @Value WHERE {KeyColumnsName} = @ID", connection);
                cmd.Parameters.Add("@ID", SqliteType.Text).Value = keyValue;
                cmd.Parameters.Add("@Value", SqliteType.Blob).Value = Value ?? Array.Empty<byte>();

                if (cmd.ExecuteNonQuery() == 1)
                {
                    DataRow targetRow = DataTable.AsEnumerable().FirstOrDefault(row => row.Field<string>(KeyColumnsName) == keyValue);
                    if (targetRow != null) targetRow[ColumnName] = Value;
                    return new Exception("Successfuly");
                }
                else return new Exception("Cant Rewrite in Table");
            }
            catch (Exception ex) { return ex; }
        }

        /// <summary>Find a row with column value</summary>
        internal DataRow Find(string ColumnName, object Value)
        {
            if (SqliteConnectionString == null || SqliteTableName == null) return null;

            using var connection = new SqliteConnection(SqliteConnectionString);
            try
            {
                connection.Open();
                using var cmd = new SqliteCommand($"SELECT * FROM {SqliteTableName} WHERE {ColumnName} = @Value LIMIT 1", connection);
                cmd.Parameters.AddWithValue("@Value", Value ?? DBNull.Value);

                using var reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>Create Uniq ID for connected Database</summary>
        internal string CreateID(string columnName, int size)
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
                    result[i] = chars[random.Next(chars.Length)];
                return new string(result);
            }
        }

        /// <summary>Extract row from DataTable not DataBase</summary>
        internal DataRow ExtractionRow(string Column, string Value) =>
            DataTable.AsEnumerable().FirstOrDefault(r => r[Column].ToString() == Value);

        /// <summary>Extract rows from DataTable not DataBase</summary>
        internal DataRow[] ExtractionRows(string Column, string Value) =>
            DataTable.AsEnumerable().Where(r => r[Column].ToString() == Value).ToArray();

        /// <summary>Refresh Class with all properties</summary>
        internal void Refresh()
        {
            DataTable?.Clear();
            Connect();
        }
    }
}
