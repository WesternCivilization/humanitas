using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Caching;
using SqlCache.Framework;
using System.Net;
using System.Web;
using System.Globalization;
using Logging;

namespace SqlCache
{
    public class SqlCacheConnection : System.Data.IDbConnection
    {

        private Logger log = new Logger(typeof(SqlCacheConnection));
        internal CacheServer _server = new CacheServer();
        private Dictionary<string, string> _settings = new Dictionary<string, string>();
        private NativeConnection _native;

        public Action<dynamic, string> SqlExecuted;

        public void RebuildIndexes()
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            this._server.Tables.ForEach(f => f.RebuildIndexes());
        }

        private System.Web.Caching.Cache AspNetCache
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                    return System.Web.HttpContext.Current.Cache;
                else
                    return null;
            }
        }

        public List<string> GetTableIndexes(string table)
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            return this._server[table]
                        .LinkedValueFields
                        .Select(f => f.Key).ToList();
        }

        public List<ColDefinition> GetTableSchema(string table)
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            if (this._native != null)
                return this._native.GetTableSchema(table);

            var defs = new List<ColDefinition>();
            foreach (var col in this._server[table].ColsDef)
            {
                var parts = col.Value.ToString().Split('|');
                var dataType = parts.ElementAtOrDefault(0).ToUpper();
                defs.Add(new ColDefinition
                {
                    ColumnName = col.Key,
                    DataType = (ColDefinition.DataTypes)Enum.Parse(typeof(ColDefinition.DataTypes), dataType),
                    CharactersLength = !string.IsNullOrEmpty(parts.ElementAtOrDefault(1)) ?
                                        long.Parse(parts.ElementAtOrDefault(1)) : -1,
                    Nullable = parts.ElementAtOrDefault(2) != "NO"
                });
            }
            return defs;
        }

        public IList<KeyValuePair<string, long>> Tables()
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            if (this._native != null)
                return this._native.Tables();

            if (!string.IsNullOrEmpty(this.WebUri))
            {
                using (var client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    var url = this.BuildUrl(this.WebUri, "query", this.WebConnectionName, "TABLES");
                    var result = (IEnumerable<dynamic>)JsonConvert.DeserializeObject(client.DownloadString(url));
                    if (result != null)
                    {
                        return result
                            .Select(f => new KeyValuePair<string, long>(f.TableName.ToString(), long.Parse(f.TotalRows.ToString())))
                            .ToList();
                    }
                }
            }
            return this._server.Tables
                    .Select(f => new KeyValuePair<string, long>(f.TableName, this.Count(f.TableName)))
                    .ToList();
        }

        public IEnumerable<Dictionary<string, object>> Rows(string table)
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            foreach (JObject row in this.Query($"SELECT * FROM {table}"))
            {
                var item = new Dictionary<string, object>();
                foreach (var field in row)
                {
                    item[field.Key] = field.Value.ToString();
                }
                yield return item;
            }
        }

        public bool Any(string table, string where)
        {
            if(this._native != null)
            {
                var count = (long)this._native.Execute($"SELECT COUNT(*) FROM {table} WHERE {where}");
                return count > 0;
            }
            else
            {
                var count = this.ExecuteScalar<long>($"SELECT COUNT(*) FROM {table} WHERE {where}");
                return count > 0;
            }
        }

        private System.Runtime.Caching.MemoryCache _consoleCache = null;

        private System.Runtime.Caching.MemoryCache ConsoleCache
        {
            get
            {
                if (_consoleCache == null)
                    _consoleCache = System.Runtime.Caching.MemoryCache.Default;
                return _consoleCache;
            }
        }

        public SqlCacheConnection(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.WebUri = null;
        }

        public string ConnectionString { get; set; }

        public string WebUri { get; set; }

        public IEnumerable<string> DbScripts()
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            foreach (var table in this._server.Tables)
            {
                yield return table.CreateTableScript();
                foreach (var row in table.Rows)
                {
                    if (row.Key.ToString() != DataTable.DEF_ID.ToString())
                    {
                        var script = table.ToInsertScript(row.Value);
                        if (!string.IsNullOrEmpty(script))
                            yield return script;
                    }
                }
            }
        }

        public string WebConnectionName { get; set; }

        public string ImageUrl
        {
            get { return this._server.ImageUrl; }
        }

        public int ConnectionTimeout => 1000;

        public string Database
        {
            get { return "SqlCache"; }
        }

        private System.Data.ConnectionState _state = System.Data.ConnectionState.Closed;

        public System.Data.ConnectionState State => this._state;

        public System.Data.IDbTransaction BeginTransaction()
        {
            throw new NotSupportedException();
        }

        public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            throw new NotSupportedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }

        public void Close()
        {
            if (_server != null)
                _server.Dispose();
            _server = null;
        }

        public TValue ExecuteScalar<TValue>(string sql, TValue defaultValue = default(TValue))
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            if (!string.IsNullOrEmpty(this.WebUri))
            {
                using (var client = new WebClient())
                {
                    var url = this.BuildUrl(this.WebUri, "scalar", this.WebConnectionName, sql);
                    dynamic result = JsonConvert.DeserializeObject(client.DownloadString(url));
                    try
                    {
                        return (TValue)result.Result;
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
            }
            var row = this.Query(sql).FirstOrDefault();
            if (row == null) return defaultValue;
            return ((JObject)row).First.First.Value<TValue>();
        }

        public long Count(string table)
        {
            return this.ExecuteScalar<long>($"SELECT COUNT(*) FROM {table}");
        }

        public T Cast<T>(dynamic row)
        {
            string json = row.ToString();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private string BuildUrl(string uri, string action, string connection, string arg)
        {
            if (action == "newrow" || action == "save")
            {
                return string.Concat(uri.TrimEnd('/'), "/", action, "?connection=", connection, "&table=", HttpUtility.UrlEncode(arg), "&token=", GetPublicToken());
            }
            else
            {
                return string.Concat(uri.TrimEnd('/'), "/", action, "?connection=", connection, "&sql=", HttpUtility.UrlEncode(arg), "&token=", GetPublicToken());
            }
        }

        internal static string GetPublicToken()
        {
            return GZipHelper.Compress("pass" + DateTime.Today.ToString("yyyyMMdd") + "word").Replace("+", "");
        }

        public IEnumerable<T> TypedQuery<T>(string sql)
        {
            var results = this.Query(sql);
            return results.Select(f =>
            {
                if (f != null)
                {
                    return (T)Cast<T>(f);
                }
                else
                {
                    return default(T);
                }
            });
        }

        public IEnumerable<dynamic> Query(string sql)
        {
            using (var scope = log.Scope($"Query({sql})"))
            {
                try
                {
                    if (this.State == System.Data.ConnectionState.Closed)
                    {
                        this._state = System.Data.ConnectionState.Connecting;
                        this.Open();
                        this._state = System.Data.ConnectionState.Open;
                    }
                    if (this._native != null)
                        return this._native.Query(sql);
                    if (!string.IsNullOrEmpty(this.WebUri))
                    {
                        using (var client = new WebClient())
                        {
                            client.Encoding = System.Text.Encoding.UTF8;
                            var url = this.BuildUrl(this.WebUri, "query", this.WebConnectionName, sql);
                            dynamic result = JsonConvert.DeserializeObject(client.DownloadString(url));
                            return (IEnumerable<dynamic>)result;
                        }
                    }
                    if (sql.ToUpper() == "TABLES")
                    {
                        return this.Tables().Select(f => new { TableName = f.Key, TotalRows = f.Value });
                    }
                    var builder = new SqlBuilder.SqlBuilder(sql);
                    if (builder.Method == "SELECT" || builder.Method == "SUMMARIZE")
                    {
                        return this.QueryData(builder);
                    }
                    else if (builder.Method == "HISTORY")
                    {
                        return this.QueryHistory(builder);
                    }
                    else return null;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        public object Save(string table, dynamic data)
        {
            using (var scope = log.Scope("Save()"))
            {
                try
                {
                    if (this.State == System.Data.ConnectionState.Closed)
                    {
                        this._state = System.Data.ConnectionState.Connecting;
                        this.Open();
                        this._state = System.Data.ConnectionState.Open;
                    }
                    if (this._native != null)
                        return this._native.Save(table, data);
                    if (!string.IsNullOrEmpty(this.WebUri))
                    {
                        var url = this.BuildUrl(this.WebUri, "save", this.WebConnectionName, table);
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(JsonConvert.SerializeObject(data));
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            return (long)JsonConvert.DeserializeObject<dynamic>(result).Id.Value;
                        }
                    }
                    return this._server.WriteRow(table, data, true);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        private IEnumerable<dynamic> QueryData(SqlBuilder.SqlBuilder builder)
        {
            var completedOperation = false;
            var table = this._server[builder.FromTables.First().TrimEnd(' ')];
            var loadAllDataToSearch = false;
            foreach (var filter in builder.WhereFilters)
            {
                var field = filter.Field.TrimEnd('1', '2', '3', '4', '5', ' ');
                if (filter.Field.ToLower() != table.RowIdField.ToLower() &&
                    (table.LabelField == null || filter.Field.ToLower() != table.LabelField.ToLower()) &&
                    !table.LinkedValues.ContainsKey(field) &&
                    filter.Field[0] != '@' &&
                    filter.Field[0] != '&' &&
                    !filter.Field.Contains('*'))
                {
                    loadAllDataToSearch = true;
                }
            }

            if (!loadAllDataToSearch)
            {
                foreach (var filter in builder.WhereFilters)
                {
                    if (filter.Field == null) continue;
                    if (filter.Field.ToLower() == table.RowIdField.ToLower())
                    {
                        filter.Field = "Id";
                    }
                    else if (table.LabelField != null && filter.Field.ToLower() == table.LabelField.ToLower())
                    {
                        filter.Field = "Label";
                    }
                }
                for (var i = 0; i < builder.OrderFields.Count; i++)
                {
                    var field = builder.OrderFields[i].Split(' ')[0].ToLower();
                    var sortType = builder.OrderFields[i].Split(' ').Last().ToLower();
                    if (field == table.RowIdField.ToLower())
                    {
                        builder.OrderFields[i] = "Id" + (field != sortType ? " " + sortType : string.Empty);
                    }
                    else if (table.LabelField != null && field == table.LabelField.ToLower())
                    {
                        builder.OrderFields[i] = "Label" + (field != sortType ? " " + sortType : string.Empty);
                    }
                }
            }

            var loadAllDataToSelect = false;
            if (builder.Method == "SELECT")
            {
                foreach (var filter in builder.SelectedFields)
                {
                    var field = filter.Field.TrimEnd('1', '2', '3', '4', '5', ' ');
                    if (filter.Field.ToLower() != table.RowIdField.ToLower() &&
                        (table.LabelField == null || filter.Field.ToLower() != table.LabelField.ToLower()) &&
                        !table.CachedValueFields.Contains(field) &&
                        filter.Field[0] != '@' &&
                        filter.Field[0] != '\'' &&
                        !filter.Field.Contains('*'))
                    {
                        loadAllDataToSelect = true;
                    }
                }

                if (!loadAllDataToSelect)
                {
                    for (var i = 0; i < builder.SelectedFields.Count; i++)
                    {
                        if (builder.SelectedFields[i].Field.ToLower() == table.RowIdField.ToLower())
                        {
                            builder.SelectedFields[i].Field = "Id";
                        }
                        else if (table.LabelField != null && builder.SelectedFields[i].Field.ToLower() == table.LabelField.ToLower())
                        {
                            builder.SelectedFields[i].Field = "Label";
                        }
                    }
                }
            }

            // Applied Linked Values for quick search
            IEnumerable<KeyValuePair<object, DataRow>> results = new List<KeyValuePair<object, DataRow>>();
            var hasIndexes = false;
            List<object> ids = null;
            foreach (var filter in builder.WhereFilters)
            {
                var field = filter.Field.Trim('1', '2', '3', '4', '5', ' ', '&');
                if (field[0] == '@')
                {
                    var reference = field.Substring(1).ToLower();
                    var foreignKeys = table.LinkedValueFields
                                        .Where(f => f.Value.ToLower() == reference)
                                        .Select(f => f.Key.TrimEnd('1', '2', '3', '4', '5', ' '))
                                        .Distinct().ToList();
                    var allIn = new List<object>();
                    if (filter.InValues != null && filter.InValues.Any())
                    {
                        foreach (var valueIn in filter.InValues)
                        {
                            var value = (object)valueIn;
                            foreach (var foreignKey in foreignKeys)
                            {
                                var sublist = this.GetLinkedValue(table.TableName, foreignKey, value);
                                if (sublist != null && sublist.Any())
                                    allIn.AddRange(sublist);
                            }
                            if (value is string)
                            {
                                allIn.Add(value);
                            }
                        }
                    }
                    else
                    {
                        var value = (long)filter.Value;
                        foreach (var foreignKey in foreignKeys)
                        {
                            var sublist = this.GetLinkedValue(table.TableName, foreignKey, value);
                            if (sublist != null && sublist.Any())
                                allIn.AddRange(sublist);
                        }
                    }
                    if (ids == null) ids = allIn.Distinct().ToList();
                    else ids = ids.Intersect(allIn.Distinct()).ToList();
                    filter.Field = null;
                    hasIndexes = true;
                }
                else if (field[0] == '?')
                {
                    var reference = field.Substring(1).ToLower();
                    var foreignKeys = table.LinkedValueFields
                                        .Where(f => f.Value.ToLower() == reference)
                                        .Select(f => f.Key.TrimEnd('1', '2', '3', '4', '5', ' '))
                                        .Distinct().ToList();
                    var parts = ((string)filter.Value).Substring(6).TrimEnd(')').Split(',');
                    var parentBooks = this._server[parts.ElementAt(1)];
                    var filterKeyPair = parts.LastOrDefault().Split('=');
                    if (parentBooks.LinkedValues[filterKeyPair.FirstOrDefault()].ContainsKey(filterKeyPair.LastOrDefault()))
                    {
                        var allIn = parentBooks.LinkedValues[filterKeyPair.FirstOrDefault()][filterKeyPair.LastOrDefault()]
                                        .Select(f => this.GetCachedValue<object>(parts.ElementAt(1), parts.ElementAt(0), f))
                                        .ToList();
                        if (ids != null)
                        {
                            allIn = allIn.Where(f => ids.Contains(f)).ToList();
                        }
                        if (ids == null) ids = allIn.Distinct().ToList();
                        else ids = ids.Intersect(allIn.Distinct()).ToList();
                        filter.Field = null;
                        hasIndexes = true;
                    }
                }
                else
                {
                    if (table.LinkedValues.ContainsKey(field))
                    {
                        if (filter.InValues == null)
                        {
                            if (filter.Value is string && filter.Value == "#NULL#")
                            {
                                var allRowsWithRef = (from parent in table.LinkedValues[field]
                                                      from key in parent.Value
                                                      select key).Distinct().ToList();
                                var allRowsWithNoRef = table.Rows.Keys.Where(f =>
                                    (filter.Op == SqlBuilder.SqlWhereFilter.Operator.EqualTo && !allRowsWithRef.Contains(f)) ||
                                    (filter.Op == SqlBuilder.SqlWhereFilter.Operator.DifferentThan && allRowsWithRef.Contains(f))).ToList();
                                if (ids == null) ids = allRowsWithNoRef;
                                else ids = ids.Intersect(allRowsWithNoRef).ToList();
                            }
                            else
                            {
                                var contains = table.LinkedValues[field].ContainsKey((object)filter.Value);
                                if (ids == null && contains) ids = table.LinkedValues[field][(object)filter.Value];
                                else if (contains) ids = ids.Intersect(table.LinkedValues[field][(object)filter.Value]).ToList();
                                else ids = new List<object>();
                            }
                        }
                        else
                        {
                            var allIn = new List<object>();
                            foreach (var value in filter.InValues)
                            {
                                var contains = table.LinkedValues[field].ContainsKey(value);
                                if (contains)
                                    allIn.AddRange(table.LinkedValues[field][value]);
                            }
                            if (ids == null && allIn.Any()) ids = allIn.Distinct().ToList();
                            else if (allIn.Any()) ids = ids.Intersect(allIn).ToList();
                            else ids = new List<object>();
                        }
                        filter.Field = null;
                        hasIndexes = true;
                    }
                    else
                    {
                        if (table.CachedValues.ContainsKey(field))
                        {
                            if (filter.Value is string && filter.Value == "#NULL#")
                            {
                                var allRowsWithRef = (from parent in table.CachedValues[field]
                                                      select parent.Key).Distinct().ToList();
                                var allRowsWithNoRef = table.Rows.Keys.Where(f =>
                                    (filter.Op == SqlBuilder.SqlWhereFilter.Operator.EqualTo && !allRowsWithRef.Contains(f)) ||
                                    (filter.Op == SqlBuilder.SqlWhereFilter.Operator.DifferentThan && allRowsWithRef.Contains(f))).ToList();
                                if (ids == null) ids = allRowsWithNoRef;
                                else ids = ids.Intersect(allRowsWithNoRef).ToList();
                                filter.Field = null;
                            }
                        }
                    }
                }
            }

            var topIndex = 0;
            if (builder.Method == "SUMMARIZE" && builder.SelectedFields[0].Field[0] == '&')
            {
                var field = builder.SelectedFields[0].Field.Substring(1);
                if (table.LinkedValues.ContainsKey(field))
                {
                    var newIds = new Dictionary<object, int>();
                    foreach (var list in table.LinkedValues[field])
                    {
                        var count = list.Value.Intersect(ids).Count();
                        if (count > 0)
                        {
                            newIds.Add(list.Key, count);
                        }
                    }
                    var refTable = table.LinkedValueFields.First(f => f.Key.TrimEnd('1', '2', '3', '4', '5', ' ') == field).Value;
                    table = this._server[refTable];
                    var otherFields = builder.SelectedFields.Skip(1).ToList();
                    foreach (var row in newIds)
                    {
                        if (!table.Rows.ContainsKey(row.Key) || row.Key.ToString() == DataTable.DEF_ID.ToString()) continue;
                        var obj = new JObject();
                        dynamic data = table.Rows[row.Key];
                        foreach (var otherField in otherFields)
                        {
                            if (string.IsNullOrWhiteSpace(otherField.Field)) continue;
                            var aliasName = otherField.Alias;
                            if (otherField.Field[0] == '\'' && otherField.Field[otherField.Field.Length - 1] == '\'')
                            {
                                obj[aliasName] = otherField.Field.Trim('\'');
                            }
                            else if (otherField.Field.ToLower() == "null")
                            {
                                obj[aliasName] = null;
                            }
                            else if (otherField.Field.ToLower() == "count?")
                            {
                                obj["Count"] = row.Value;
                            }
                            else
                            {
                                if (data is DataRow && otherField.Field == table.RowIdField)
                                {
                                    obj[aliasName] = table.GetFieldValue(data, "Id");
                                }
                                else if (data is DataRow && otherField.Field == table.LabelField)
                                {
                                    obj[aliasName] = table.GetFieldValue(data, "Label");
                                }
                                else if (data is DataRow && table.CachedValues.ContainsKey(otherField.Field))
                                {
                                    obj[aliasName] = this.GetCachedValue<dynamic>(table.TableName, otherField.Field, row.Key);
                                }
                                else
                                {
                                    obj[aliasName] = table.GetFieldValue(data, otherField.Field);
                                }
                            }
                        }
                        yield return obj;
                        topIndex++;
                        if (builder.TopCount > 0 && topIndex >= builder.TopCount)
                        {
                            completedOperation = true;
                            break;
                        }
                    }
                    completedOperation = true;
                }
            }

            var totalRows = 0;
            if (!completedOperation)
            {

                if (builder.SelectedFields.Any(f => f.Field.ToUpper() == "COUNT(*)"))
                {
                    if (ids != null)
                    {
                        totalRows = ids.Count(f => f != DataTable.DEF_ID);
                    }
                    else
                    {
                        totalRows = table.Rows.Count(f => !DataTable.DEF_ID.Equals(f.Key));
                    }
                }

                if (hasIndexes) results = table.Rows.Where(f => ids.Contains(f.Key)).ToList();
                else results = table.Rows;
                if (!loadAllDataToSearch && builder.OrderFields.Any())
                {
                    var firstOrder = true;
                    foreach (var sort in builder.OrderFields)
                    {
                        if (sort.ToLower() == "label" || sort.ToLower() == "label asc")
                        {
                            if (firstOrder)
                                results = results.OrderBy(f => f.Value.Label);
                            else
                                results = ((IOrderedEnumerable<KeyValuePair<object, DataRow>>)results).ThenBy(f => f.Value.Label);
                            firstOrder = false;
                        }
                        else if (sort.ToLower() == "label desc")
                        {
                            if (firstOrder)
                                results = results.OrderByDescending(f => f.Value.Label);
                            else
                                results = ((IOrderedEnumerable<KeyValuePair<object, DataRow>>)results).ThenByDescending(f => f.Value.Label);
                            firstOrder = false;
                        }
                        else if (sort.ToLower() == "id" || sort.ToLower() == "id asc")
                        {
                            if (firstOrder)
                                results = results.OrderBy(f => f.Key);
                            else
                                results = ((IOrderedEnumerable<KeyValuePair<object, DataRow>>)results).ThenBy(f => f.Key);
                            firstOrder = false;
                        }
                        else if (sort.ToLower() == "id desc")
                        {
                            if (firstOrder)
                                results = results.OrderByDescending(f => f.Key);
                            else
                                results = ((IOrderedEnumerable<KeyValuePair<object, DataRow>>)results).ThenByDescending(f => f.Key);
                            firstOrder = false;
                        }
                        else if (sort.ToLower().StartsWith("contains("))
                        {
                            var ci = new CultureInfo("en-US");
                            var parts = sort.Split('=');
                            var field = parts.FirstOrDefault().Split('(').LastOrDefault();
                            var expression = parts.LastOrDefault().Split(')').FirstOrDefault().Trim('\'');
                            if (firstOrder)
                                results = results.OrderBy(f =>
                                            f.Value.Label.StartsWith(expression, true, ci) ? 0
                                                : f.Value.Label.EndsWith(expression, true, ci) ? 2 : 1)
                                                .ThenBy(f => f.Value.Label);
                            else
                                results = ((IOrderedEnumerable<KeyValuePair<object, DataRow>>)results)
                                            .ThenBy(f =>
                                                f.Value.Label.StartsWith(expression, true, ci) ? 0
                                                 : f.Value.Label.EndsWith(expression, true, ci) ? 2 : 1)
                                                 .ThenBy(f => f.Value.Label);
                            firstOrder = false;
                        }
                        else if (sort.ToLower() == "random()")
                        {
                            if (firstOrder)
                                results = results.OrderByDescending(f => Guid.NewGuid());
                            else
                                results = ((IOrderedEnumerable<KeyValuePair<object, DataRow>>)results).ThenByDescending(f => Guid.NewGuid());
                            firstOrder = false;
                        }
                        else if (table.CachedValueFields.Contains(sort.Split(' ')[0]))
                        {
                            var field = sort.Split(' ')[0];
                            var desc = sort.ToLower().EndsWith(" desc");
                            var type = table.ColsDef[field].Value<string>().Split('|').First();
                            if (type == "smallint")
                                results = SortList<short>(table.TableName, field, desc, results);
                            else if (type == "int")
                                results = SortList<int>(table.TableName, field, desc, results);
                            else if (type == "bigint")
                                results = SortList<long>(table.TableName, field, desc, results);
                            else if (type == "datetime")
                                results = SortList<DateTime>(table.TableName, field, desc, results);
                            else
                                results = SortList<string>(table.TableName, field, desc, results);
                            firstOrder = false;
                        }
                    }
                }
                else if (builder.OrderFields.Any())
                {
                    foreach (var sort in builder.OrderFields)
                    {
                        if (table.CachedValueFields.Contains(sort.Split(' ')[0]))
                        {
                            var field = sort.Split(' ')[0];
                            var desc = sort.ToLower().EndsWith(" desc");
                            var type = table.ColsDef[field].Value<string>().Split('|').First();
                            if (type == "smallint")
                                results = SortList<short>(table.TableName, field, desc, results);
                            else if (type == "int")
                                results = SortList<int>(table.TableName, field, desc, results);
                            else if (type == "bigint")
                                results = SortList<long>(table.TableName, field, desc, results);
                            else if (type == "datetime")
                                results = SortList<DateTime>(table.TableName, field, desc, results);
                            else
                                results = SortList<string>(table.TableName, field, desc, results);
                        }
                    }
                }
                foreach (var row in results)
                {
                    if (row.Key.ToString() == DataTable.DEF_ID.ToString()) continue;
                    dynamic data = row.Value;
                    if (loadAllDataToSearch) data = table.ReadRow(row.Key);
                    if (!builder.WhereFilters.Any(f => f.Field != null) ||
                        builder.HasMatch(table, data))
                    {
                        if (builder.SelectAllFields)
                        {
                            yield return loadAllDataToSearch ? data : table.ReadRow(row.Key);
                            topIndex++;
                            if (builder.TopCount > 0 && topIndex >= builder.TopCount)
                            {
                                completedOperation = true;
                                break;
                            }
                        }
                        else
                        {
                            if (loadAllDataToSelect)
                            {
                                data = table.ReadRow(row.Key);
                            }
                            else
                            {
                                data = row.Value;
                            }
                            var obj = new JObject();
                            foreach (var field in builder.SelectedFields)
                            {
                                var aliasName = field.Alias;
                                if (!loadAllDataToSelect && field.Field != field.Alias && aliasName == "Id") aliasName = table.RowIdField;
                                else if (!loadAllDataToSelect && field.Field != field.Alias && aliasName == "Label") aliasName = table.LabelField;
                                if (field.Field[0] == '\'' && field.Field[field.Field.Length - 1] == '\'')
                                {
                                    obj[aliasName] = field.Field.Trim('\'');
                                }
                                else if (field.Field.ToLower() == "null")
                                {
                                    obj[aliasName] = null;
                                }
                                else
                                {
                                    if (field.Field.ToUpper() == "COUNT(*)")
                                    {
                                        if (field.Field == field.Alias)
                                        {
                                            field.Alias = "Count";
                                        }
                                        obj[field.Alias] = totalRows;
                                    }
                                    else
                                    {
                                        if (data is DataRow && field.Field != "Id" && field.Field != "Label")
                                        {
                                            obj[aliasName] = this.GetCachedValue<dynamic>(table.TableName, field.Field, row.Key);
                                        }
                                        else
                                        {
                                            obj[aliasName] = table.GetFieldValue(data, field.Field);
                                        }
                                    }
                                }
                            }
                            yield return obj;
                            topIndex++;
                            if (builder.TopCount > 0 && topIndex >= builder.TopCount)
                            {
                                completedOperation = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public dynamic NewRow(string tableName)
        {
            using (var scope = log.Scope("NewRow()"))
            {
                try
                {
                    if (this.State == System.Data.ConnectionState.Closed)
                    {
                        this._state = System.Data.ConnectionState.Connecting;
                        this.Open();
                        this._state = System.Data.ConnectionState.Open;
                    }
                    if (this._native != null)
                        return this._native.NewRow(tableName);
                    if (!string.IsNullOrEmpty(this.WebUri))
                    {
                        using (var client = new WebClient())
                        {
                            var url = this.BuildUrl(this.WebUri, "newrow", this.WebConnectionName, tableName);
                            return JsonConvert.DeserializeObject(client.DownloadString(url));
                        }
                    }
                    var table = this._server[tableName];
                    return table.NewRow();
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        private IEnumerable<KeyValuePair<object, DataRow>> SortList<TSort>(string tableName, string field, bool isDesc, IEnumerable<KeyValuePair<object, DataRow>> results)
        {
            var table = this._server[tableName];
            var keys = results.Select(f => new KeyValuePair<object, TSort>(f.Key, this.GetCachedValue<TSort>(table.TableName, field, f.Key)));
            if (isDesc) keys = keys.ToList().OrderByDescending(f => f.Value);
            else keys = keys.ToList().OrderBy(f => f.Value);
            return keys.Select(f => new KeyValuePair<object, DataRow>(f.Key, table.Rows[f.Key]));
        }

        public string GetRowLabel(string table, object id)
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            if (this._native != null)
                return this._native.GetRowLabel(table, id);
            if (this._server[table].Rows.ContainsKey(id))
                return this._server[table].Rows[id].Label;
            else
                return null;
        }

        public List<object> GetLinkedValue(string table, string field, object id)
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            if (this._native != null)
                return this._native.GetLinkedValue(table, field, id);
            if (!this._server[table].LinkedValues.ContainsKey(field)) return new List<object>();
            if (!this._server[table].LinkedValues[field].ContainsKey(id)) return new List<object>();
            return this._server[table].LinkedValues[field][id];
        }

        public TReturn GetCachedValue<TReturn>(string table, string field, object id)
        {
            if (this.State == System.Data.ConnectionState.Closed)
            {
                this._state = System.Data.ConnectionState.Connecting;
                this.Open();
                this._state = System.Data.ConnectionState.Open;
            }
            if (this._native != null)
                return this._native.GetCachedValue<TReturn>(table, field, id);
            if (!this._server[table].CachedValues.ContainsKey(field)) return default(TReturn);
            if (!this._server[table].CachedValues[field].ContainsKey(id)) return default(TReturn);
            return (TReturn)this._server[table].CachedValues[field][id];
            throw new InvalidOperationException($"Field '{field}' not found in '{table}'.");
        }

        public object Execute(string sql)
        {
            using (var scope = log.Scope($"Execute({sql})"))
            {
                try
                {
                    if (this.State == System.Data.ConnectionState.Closed)
                    {
                        this._state = System.Data.ConnectionState.Connecting;
                        this.Open();
                        this._state = System.Data.ConnectionState.Open;
                    }
                    if (this._native != null)
                        return this._native.Execute(sql);
                    if (!string.IsNullOrEmpty(this.WebUri))
                    {
                        using (var client = new WebClient())
                        {
                            var url = this.BuildUrl(this.WebUri, "execute", this.WebConnectionName, sql);
                            dynamic result = JsonConvert.DeserializeObject(client.DownloadString(url));
                            return (long)result.Count;
                        }
                    }
                    if (sql.ToUpper() == "REBUILD ALL_INDEXES")
                    {
                        foreach (var table in this._server.Tables)
                        {
                            table.RebuildIndexes();
                        }
                    }
                    var builder = new SqlBuilder.SqlBuilder(sql);
                    if (builder.Method == "UPDATE")
                    {
                        return this.RunUpdate(builder);
                    }
                    else if (builder.Method == "DELETE")
                    {
                        return this.RunDelete(builder);
                    }
                    else if (builder.Method == "INSERT")
                    {
                        return this.RunInsert(builder);
                    }
                    else if (builder.Method == "ROLLBACK")
                    {
                        return this.RunRollback(builder);
                    }
                    else return -1;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        private int RunRollback(SqlBuilder.SqlBuilder builder)
        {
            var count = 0;
            var dt = this._server[builder.FromTables.First()];
            var rows = this.QueryHistory(builder).Reverse().ToList();
            Dictionary<long, dynamic> rollbacks = new Dictionary<long, dynamic>();
            Dictionary<long, int> rollbackVersions = new Dictionary<long, int>();
            foreach (var row in rows)
            {
                var id = (long)row.RowId;
                var version = (int)row.Version;
                if (!rollbackVersions.ContainsKey(id))
                    rollbackVersions.Add(id, version);

                if (row.Action == "DELETED") continue;
                if (!rollbacks.ContainsKey(id))
                    rollbacks.Add(id, row);
            }
            foreach (var id in rollbacks.Keys)
            {
                dt.RollbackRow(id, rollbacks[id], rollbackVersions[id]);
            }
            return count;
        }

        private IEnumerable<dynamic> QueryHistory(SqlBuilder.SqlBuilder builder)
        {
            var table = this._server[builder.FromTables.First()];
            table.EnsureHistoryLoaded();


            var needToLoadAllData = false;
            foreach (var filter in builder.WhereFilters)
            {
                if (filter.Field.ToLower() != table.RowIdField.ToLower())
                {
                    needToLoadAllData = true;
                }
            }
            foreach (var field in builder.SelectedFields)
            {
                if (field.Field.ToLower() != table.RowIdField.ToLower())
                {
                    needToLoadAllData = true;
                }
            }
            if (!needToLoadAllData)
            {
                foreach (var filter in builder.WhereFilters)
                {
                    if (filter.Field.ToLower() == table.RowIdField.ToLower())
                    {
                        filter.Field = "Id";
                    }
                }
                for (var i = 0; i < builder.SelectedFields.Count; i++)
                {
                    if (builder.SelectedFields[i].Field.ToLower() == table.RowIdField.ToLower())
                    {
                        builder.SelectedFields[i].Field = "Id";
                    }
                }
            }

            IEnumerable<KeyValuePair<object, DataRow>> results = new List<KeyValuePair<object, DataRow>>();
            var hasIndexes = false;
            List<object> ids = null;
            foreach (var filter in builder.WhereFilters)
            {
                if (table.LinkedValues.ContainsKey(filter.Field))
                {
                    if (filter.InValues == null)
                    {
                        if (ids == null) ids = table.LinkedValues[filter.Field][(long)filter.Value];
                        else ids = ids.Intersect(table.LinkedValues[filter.Field][(long)filter.Value]).ToList();
                    }
                    else
                    {
                        foreach (var value in filter.InValues)
                        {
                            if (ids == null) ids = table.LinkedValues[filter.Field][(long)value];
                            else ids = ids.Intersect(table.LinkedValues[filter.Field][(long)value]).ToList();
                        }
                    }
                    filter.Field = null;
                    hasIndexes = true;
                }
            }
            if (hasIndexes) results = table.Rows.Where(f => ids.Contains(f.Key)).ToList();
            else results = table.Rows;
            var rowsToRemove = new List<long>();
            foreach (var row in table.History)
            {
                dynamic data = row.Value.Last();
                var removedRecord = false;
                if (data.Action == "DELETED")
                {
                    data = row.Value.ElementAt(row.Value.Count - 2);
                    removedRecord = true;
                }
                if (needToLoadAllData) data = !removedRecord ?
                        table.ReadRow(row.Key) :
                        table.ReadRow((long)data.DataStart, (long)data.DataEnd);
                if (!builder.WhereFilters.Any(f => f.Field != null) ||
                    builder.HasMatch(table, data))
                {
                    var history = table.History[row.Key];
                    foreach (var lead in history)
                    {
                        var json = JsonConvert.SerializeObject(new
                        {
                            RowId = lead.Id,
                            Version = lead.Version,
                            DateTime = lead.EventDate,
                            Action = lead.Action,
                            HeaderStart = lead.HeaderStart,
                            RowData = lead.DataStart != lead.DataEnd ?
                            table.ReadRow(lead.DataStart, lead.DataEnd) : null
                        });
                        yield return JObject.Parse(json);
                    }
                }
            }
        }

        private object RunInsert(SqlBuilder.SqlBuilder builder)
        {
            var table = this._server[builder.FromTables.First()];
            var id = table.GetNextRowId();
            var data = new JObject();
            if (id is long)
            {
                data[table.RowIdField] = new JValue((long)id);
            }
            else
            {
                data[table.RowIdField] = new JValue(id.ToString());
            }
            for (var i = 0; i < builder.InsertFields.Count; i++)
            {
                var field = builder.InsertFields[i];
                var value = builder.InsertValues[i];
                data[field] = JToken.FromObject(value);
            }
            var cols = (JObject)this._server.ReadRow(table.TableName, DataTable.DEF_ID.ToString());
            foreach (var field in cols)
            {
                if (data.Property(field.Key) == null)
                {
                    data[field.Key] = null;
                }
            }
            this._server.WriteRow(table.TableName, data, true);
            return id;
        }

        private int RunDelete(SqlBuilder.SqlBuilder builder)
        {
            var count = 0;
            var table = this._server[builder.FromTables.First()];
            var needToLoadAllData = false;
            foreach (var filter in builder.WhereFilters)
            {
                if (filter.Field.ToLower() != table.RowIdField.ToLower() &&
                    (table.LabelField == null || filter.Field.ToLower() != table.LabelField.ToLower()))
                {
                    needToLoadAllData = true;
                }
            }
            foreach (var field in builder.SelectedFields)
            {
                if (field.Field.ToLower() != table.RowIdField.ToLower() &&
                    (table.LabelField == null || field.Field.ToLower() != table.LabelField.ToLower()))
                {
                    needToLoadAllData = true;
                }
            }
            if (!needToLoadAllData)
            {
                foreach (var filter in builder.WhereFilters)
                {
                    if (filter.Field.ToLower() == table.RowIdField.ToLower())
                    {
                        filter.Field = "Id";
                    }
                    else if (filter.Field.ToLower() == table.LabelField.ToLower())
                    {
                        filter.Field = "Label";
                    }
                }
                for (var i = 0; i < builder.SelectedFields.Count; i++)
                {
                    if (builder.SelectedFields[i].Field.ToLower() == table.RowIdField.ToLower())
                    {
                        builder.SelectedFields[i].Field = "Id";
                    }
                    else if (builder.SelectedFields[i].Field.ToLower() == table.LabelField.ToLower())
                    {
                        builder.SelectedFields[i].Field = "Label";
                    }
                }
            }
            IEnumerable<KeyValuePair<object, DataRow>> results = new List<KeyValuePair<object, DataRow>>();
            var hasIndexes = false;
            List<object> ids = null;
            foreach (var filter in builder.WhereFilters)
            {
                if (table.LinkedValues.ContainsKey(filter.Field))
                {
                    if (filter.InValues == null)
                    {
                        if (ids == null)
                        {
                            if (!table.LinkedValues[filter.Field].ContainsKey((object)filter.Value))
                                table.LinkedValues[filter.Field][(object)filter.Value] = new List<object>();
                            ids = table.LinkedValues[filter.Field][(object)filter.Value];
                        }
                        else ids = ids.Intersect(table.LinkedValues[filter.Field][(object)filter.Value]).ToList();
                    }
                    else
                    {
                        foreach (var value in filter.InValues)
                        {
                            if (ids == null)
                            {
                                if (!table.LinkedValues[filter.Field].ContainsKey((object)value))
                                    table.LinkedValues[filter.Field][(object)value] = new List<object>();
                                ids = table.LinkedValues[filter.Field][(object)value];
                            }
                            else ids = ids.Intersect(table.LinkedValues[filter.Field][(object)value]).ToList();
                        }
                    }
                    filter.Field = null;
                    hasIndexes = true;
                }
            }
            if (hasIndexes) results = table.Rows.Where(f => ids.Contains(f.Key)).ToList();
            else results = table.Rows;
            var rowsToRemove = new List<object>();
            foreach (var row in results)
            {
                if (row.Key.ToString() == DataTable.DEF_ID.ToString()) continue;
                dynamic data = row.Value;
                if (needToLoadAllData) data = table.ReadRow(row.Key);
                if (!builder.WhereFilters.Any(f => f.Field != null) ||
                    builder.HasMatch(table, data))
                {
                    data = table.ReadRow(row.Key);
                    foreach (var prop in ((JObject)data).Properties())
                    {
                        var field = prop.Name.TrimEnd('1', '2', '3', '4', '5', '_');
                        if (table.LinkedValues.ContainsKey(field))
                        {
                            var propValue = table.GetFieldValue(data, prop.Name);
                            if (propValue != null && table.LinkedValues[field].ContainsKey((object)propValue))
                            {
                                table.LinkedValues[field][(object)propValue].Remove(row.Key);
                            }
                        }
                    }
                    rowsToRemove.Add(row.Key);
                    count++;
                }
            }
            foreach (var id in rowsToRemove)
            {
                this._server.DeleteRow(table.TableName, id);
            }
            return count;
        }

        private int RunUpdate(SqlBuilder.SqlBuilder builder)
        {
            var count = 0;
            var table = this._server[builder.FromTables.First()];
            var needToLoadAllData = false;
            foreach (var filter in builder.WhereFilters)
            {
                if (filter.Field.ToLower() != table.RowIdField.ToLower() &&
                    (table.LabelField != null && filter.Field.ToLower() != table.LabelField.ToLower()))
                {
                    needToLoadAllData = true;
                }
            }
            foreach (var field in builder.SelectedFields)
            {
                if (field.Field.ToLower() != table.RowIdField.ToLower() &&
                    field.Field.ToLower() != table.LabelField.ToLower())
                {
                    needToLoadAllData = true;
                }
            }
            if (!needToLoadAllData)
            {
                foreach (var filter in builder.WhereFilters)
                {
                    if (filter.Field.ToLower() == table.RowIdField.ToLower())
                    {
                        filter.Field = "Id";
                    }
                    else if (table.LabelField != null && filter.Field.ToLower() == table.LabelField.ToLower())
                    {
                        filter.Field = "Label";
                    }
                }
                for (var i = 0; i < builder.SelectedFields.Count; i++)
                {
                    if (builder.SelectedFields[i].Field.ToLower() == table.RowIdField.ToLower())
                    {
                        builder.SelectedFields[i].Field = "Id";
                    }
                    else if (builder.SelectedFields[i].Field.ToLower() == table.LabelField.ToLower())
                    {
                        builder.SelectedFields[i].Field = "Label";
                    }
                }
            }
            IEnumerable<KeyValuePair<object, DataRow>> results = new List<KeyValuePair<object, DataRow>>();
            var hasIndexes = false;
            List<object> ids = null;
            foreach (var filter in builder.WhereFilters)
            {
                if (table.LinkedValues.ContainsKey(filter.Field))
                {
                    if (filter.InValues == null)
                    {
                        if (ids == null) ids = table.LinkedValues[filter.Field].ContainsKey(filter.Value) ? table.LinkedValues[filter.Field][filter.Value] : null;
                        else
                        {
                            var list2 = table.LinkedValues[filter.Field][filter.Value];
                            var list1 = ids.Where(f => list2.Contains(f)).ToList();
                            //ids
                        }
                    }
                    else
                    {
                        foreach (var value in filter.InValues)
                        {
                            if (ids == null) ids = table.LinkedValues[filter.Field][(object)value];
                            else ids = ids.Intersect(table.LinkedValues[filter.Field][(object)value]).ToList();
                        }
                    }
                    filter.Field = null;
                    hasIndexes = true;
                }
            }
            if (hasIndexes) results = table.Rows.Where(f => ids.Contains(f.Key)).ToList();
            else results = table.Rows;
            foreach (var row in results.ToList())
            {
                dynamic data = row.Value;
                if (needToLoadAllData) data = table.ReadRow(row.Key);
                if (!builder.WhereFilters.Any(f => f.Field != null) ||
                    builder.HasMatch(table, data))
                {
                    data = table.ReadRow(row.Key);
                    var changedValues = builder.ChangeValues(ref data);
                    if (changedValues)
                    {
                        foreach (var statement in builder.SetStatements)
                        {
                            var field = statement.Field.TrimEnd('1', '2', '3', '4', '5', '_');
                            if (table.LinkedValues.ContainsKey(field))
                            {
                                var previousValue = table.GetFieldValue(data, statement.Field);
                                var newValue = statement.Value;
                                if (previousValue != newValue)
                                {
                                    table.LinkedValues[field][(long)previousValue].Remove(row.Key);
                                    table.LinkedValues[field][(long)newValue].Add(row.Key);
                                }
                            }
                        }
                        this._server.WriteRow(table.TableName, data, true);
                        count++;
                    }
                }
            }
            return count;
        }

        public System.Data.IDbCommand CreateCommand()
        {
            var result = new SqlCacheCommand();
            result.Connection = this;
            return result;
        }

        public void Dispose()
        {
            _server = null;
        }

        public void Open()
        {
            if (this._native != null) return;
            if (this.ConnectionString == null) throw new ArgumentNullException("ConnectionString not defined.");
            var parameters = this.ConnectionString.Split(';');
            foreach (var keyValue in parameters)
            {
                var parts = keyValue.Split('=');
                this._settings[parts.First().ToLower()] = parts.Last();
            }
            if (this._settings.ContainsKey("database type"))
            {
                var connString = $"Server={this._settings["server"]};Database={this._settings["database"]};Uid={this._settings["uid"]};Password={this._settings["password"]};MultipleActiveResultSets=true";
                this._native = new NativeConnection(this._settings["database type"], connString);
                this._native.SqlExecuted = this.SqlExecuted;
            }
            else
            {
                foreach (var key in this._settings.Keys)
                {
                    if (key == "data source")
                    {
                        var filePath = this._settings[key];
                        if (filePath.ToLower().StartsWith("http://"))
                        {
                            this.WebUri = filePath;
                        }
                        else
                        {
                            this._server.CachePath = Path.GetDirectoryName(filePath);
                            this.LoadConfigFile(filePath);
                        }
                    }
                    else if (key.ToLower() == "connection name")
                    {
                        this.WebConnectionName = this._settings[key];
                    }
                }
                var file = "cachedb::" + this._settings["data source"].ToLower();
                if (this._settings.ContainsKey("aspnet cache") && this._settings["aspnet cache"].ToLower() == "true")
                {
                    if (AspNetCache == null) throw new ArgumentException("aspnet cache parameter is supported for web applications only.");
                    var fromCache = AspNetCache.Get(file);
                    if (fromCache != null)
                    {
                        this._server = (CacheServer)fromCache;
                        return;
                    }
                    else
                    {
                        this._server.Start();
                    }
                    AspNetCache.Insert(file, this._server, null,
                        DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);
                }
                else if (string.IsNullOrEmpty(this.WebUri))
                {
                    var fromCache = ConsoleCache.Get(file);
                    if (fromCache != null)
                    {
                        this._server = (CacheServer)fromCache;
                        return;
                    }
                    else
                    {
                        this._server.Start();
                    }
                    var policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTime.Now.AddYears(1);
                    ConsoleCache.Set(file, this._server, policy);
                }
            }
        }

        private void LoadConfigFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            DataTable dt = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("EXTERNAL_CONNECTION_STRING:="))
                {
                    this._server.ConnectionString = line.Substring("EXTERNAL_CONNECTION_STRING:=".Length);
                }
                else if (line.StartsWith("EXTERNAL_IMAGE_PATH:="))
                {
                    this._server.ImagePath = line.Substring("EXTERNAL_IMAGE_PATH:=".Length);
                }
                else if (line.StartsWith("IMAGE_URL:="))
                {
                    this._server.ImageUrl = line.Substring("IMAGE_URL:=".Length);
                }
                else if (line.StartsWith("IMAGE_PREVIEW_MAX_SIZE:="))
                {
                    var parts = line.Substring("IMAGE_PREVIEW_MAX_SIZE:=".Length).Split(';');
                    this._server.ImagePreviewSize = new System.Drawing.Size(int.Parse(parts[0]), int.Parse(parts[1]));
                }
                else if (line.StartsWith("TABLE:="))
                {
                    var parts = line.Substring("TABLE:=".Length).Split(';');
                    dt = this._server.AddTable(parts.ElementAtOrDefault(0), parts.ElementAtOrDefault(1), parts.ElementAtOrDefault(2), parts.ElementAtOrDefault(3));
                }
                else if (line.StartsWith("LINKED_INDEX:="))
                {
                    var parts = line.Substring("LINKED_INDEX:=".Length).Split(';');
                    dt.AddLinkedValueField(parts[0], parts.Length > 1 ? parts[1] : "-");
                }
                else if (line.StartsWith("CACHED_INDEX:="))
                {
                    var parts = line.Substring("CACHED_INDEX:=".Length).Split(';');
                    dt.AddCachedValueField(parts[0]);
                }
            }
        }

    }
}
