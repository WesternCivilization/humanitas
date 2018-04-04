using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCache
{
    public class NativeConnection
    {

        public NativeConnection(string databaseType, string connString)
        {
            this.DatabaseType = databaseType;
            this.ConnectionString = connString;
            if (databaseType.ToLower() == "mysql")
            {
                this._conn = new MySqlConnection(connString);
            }
            else if (databaseType.ToLower() == "mssql")
            {
                this._conn = new SqlConnection(connString);
            }
        }

        public string DatabaseType { get; set; }

        public string ConnectionString { get; set; }


        private System.Data.IDbConnection _conn;

        public IEnumerable<dynamic> Query(string sql)
        {
            var builder = new SqlBuilder.SqlBuilder(sql);
            var nativeSql = builder.ToNativeSql(this.DatabaseType);
            using (var cmd = this._conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = nativeSql;
                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new JObject();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            var value = reader.GetValue(i);
                            row[name] = JToken.FromObject(value);
                        }
                        yield return row;
                    }
                }
            }
        }

        public object Save(string table, dynamic data)
        {
            JObject json;
            if (data is JObject)
                json = data;
            else
                json = JObject.FromObject(data);
            string keyField = table.TrimEnd('s');
            if (keyField.EndsWith("ie")) keyField = keyField.Substring(0, keyField.Length - 2) + "y";
            keyField = keyField + "Id";
            string keyValue = json[keyField].ToString();

            var sb = new StringBuilder();
            using (var cmd = this._conn.CreateCommand())
            {
                if (string.IsNullOrEmpty(keyValue)) keyValue = Guid.Empty.ToString();

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"SELECT COUNT(*) FROM {table} WHERE {keyField} = '{keyValue}'";
                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                var count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    sb.AppendLine($"UPDATE {table} ");
                    sb.AppendLine($"SET ");
                    foreach (var field in json)
                    {
                        if (keyField != field.Key)
                        {
                            sb.AppendLine($"\t{field.Key} = {JsonToDb((JValue)json[field.Key])}, ");
                        }
                    }
                    sb = sb.Remove(sb.Length - 4, 4);
                    sb.AppendLine(" ");
                    sb.AppendLine($"WHERE {keyField} = '{keyValue}'");
                }
                else
                {
                    var fields = new StringBuilder();
                    var values = new StringBuilder();
                    sb.AppendLine($"INSERT INTO {table} ");
                    foreach (var field in json)
                    {
                        fields.AppendLine($"{field.Key}, ");
                        values.AppendLine($"{JsonToDb((JValue)json[field.Key])}, ");
                    }

                    fields = fields.Remove(fields.Length - 4, 4);
                    values = values.Remove(values.Length - 4, 4);
                    sb.AppendLine($"({fields}) VALUES ({values})");
                }
            }

            using (var cmd2 = this._conn.CreateCommand())
            {
                cmd2.CommandType = System.Data.CommandType.Text;
                cmd2.CommandText = sb.ToString();
                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                cmd2.ExecuteNonQuery();
            }

            return keyValue;
        }

        internal IList<KeyValuePair<string, long>> Tables()
        {
            var result = new List<KeyValuePair<string, long>>();
            using (var cmd = this._conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"SELECT TABLE_NAME, TABLE_ROWS FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new KeyValuePair<string, long>(reader.GetString(0), reader.GetInt64(1)));
                }
            }
            return result;
        }

        internal List<ColDefinition> GetTableSchema(string table)
        {
            var result = new List<ColDefinition>();
            using (var cmd = this._conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' ORDER BY ORDINAL_POSITION";

                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    long count = !reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) ?
                                    reader.GetInt64(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH")) : -1;
                    result.Add(new ColDefinition
                    {
                        ColumnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME")),
                        DataType = (ColDefinition.DataTypes)Enum.Parse(typeof(ColDefinition.DataTypes), reader.GetString(reader.GetOrdinal("DATA_TYPE")).ToUpper()),
                        CharactersLength = count,
                        Nullable = reader.GetString(reader.GetOrdinal("IS_NULLABLE")) == "YES"
                    });
                }
            }
            return result;
        }

        private string JsonToDb(JValue token)
        {
            var value = token.Value;
            if (value is DateTime)
                return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            else if (value is string)
                return "'" + ((string)value).Replace("'", "''") + "'";
            else if (value != null)
                return value.ToString();
            else
                return "NULL";
        }

        public dynamic NewRow(string tableName)
        {
            var obj = new JObject();
            using (var cmd = this._conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' ORDER BY ORDINAL_POSITION";

                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    obj[reader.GetString(0)] = null;
                }
            }
            return obj;
        }

        public string GetRowLabel(string table, object id)
        {
            using (var cmd = this._conn.CreateCommand())
            {
                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"SELECT Name FROM {table} WHERE {table.TrimEnd('s')}Id = '{id}'";
                return (string)cmd.ExecuteScalar();
            }
        }

        public List<object> GetLinkedValue(string table, string field, object id)
        {
            throw new NotImplementedException();
        }

        internal TReturn GetCachedValue<TReturn>(string table, string field, object id)
        {
            using (var cmd = this._conn.CreateCommand())
            {
                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                cmd.CommandType = System.Data.CommandType.Text;
                if (this._conn is SqlConnection)
                {
                    cmd.CommandText = $"SELECT ISNULL({field}, 0) FROM {table} WHERE {table.TrimEnd('s')}Id = '{id}'";
                }
                else
                {
                    cmd.CommandText = $"SELECT IFNULL({field}, 0) FROM {table} WHERE {table.TrimEnd('s')}Id = '{id}'";
                }
                return (TReturn)cmd.ExecuteScalar();
            }
        }

        public object Execute(string sql)
        {
            using (var cmd = this._conn.CreateCommand())
            {
                if (this._conn.State != System.Data.ConnectionState.Open)
                    this._conn.Open();

                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery();
            }
        }

    }
}
