using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using Newtonsoft.Json;
using System.Drawing;
using Logging;

namespace SqlCache.Framework
{
    public class DataTable
    {

        internal DataTable()
        {
            this.LinkedValueFields = new Dictionary<string, string>();
            this.CachedValueFields = new List<string>();
            this.History = new Dictionary<object, List<DataRowHistory>>();
            this.HistoryLoaded = false;
        }

        private Logger log = new Logger(typeof(DataTable));

        public string TableName { get; set; }

        public string RowIdField { get; set; }

        public Dictionary<object, DataRow> Rows { get; set; }

        public Dictionary<object, List<DataRowHistory>> History { get; set; }

        public bool HistoryLoaded { get; set; }

        public string LabelField { get; set; }

        public static object DEF_ID = Guid.Empty.ToString();

        public Dictionary<string, string> LinkedValueFields { get; set; }

        public List<string> CachedValueFields { get; set; }

        public Dictionary<string, Dictionary<object, List<object>>> LinkedValues { get; set; }

        public Dictionary<string, Dictionary<object, object>> CachedValues { get; set; }

        public string DataFilePath { get; set; }

        public string HeaderFilePath { get; set; }

        public string ImageFilePath { get; set; }

        public string IndexesFilePath { get; set; }

        public string FileName { get; set; }

        public string ImagePath { get; set; }

        public Size PreviewSize { get; set; }

        public string HistoryFilePath { get; set; }

        public JObject ColsDef { get; set; }

        public void LoadFiles()
        {
            using (var scope = log.Scope($"[{this.TableName}] LoadFiles()"))
            {
                try
                {
                    this.Rows = new Dictionary<object, DataRow>();
                    this.LinkedValues = new Dictionary<string, Dictionary<object, List<object>>>();
                    this.CachedValues = new Dictionary<string, Dictionary<object, object>>();
                    this.LoadHeaderFile();
                    this.ColsDef = this.ReadRow(DEF_ID);
                    this.LoadIndexesFile(true);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        internal string ToInsertScript(DataRow row)
        {
            var sb = new StringBuilder();
            var fields = new StringBuilder();
            var values = new StringBuilder();
            JObject data = this.ReadRow(row.Id);
            if (data == null) return null;
            foreach (var col in this.ColsDef)
            {
                var colName = col.Key;
                var defs = col.Value.ToString().Split('|');
                var dataType = defs[0];
                var nullable = defs[2].ToLower() == "yes";
                fields.Append($"{colName},");
                values.Append($"{this.GetDbValue(data, colName, dataType, nullable)},");
            }
            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);
            sb.AppendLine($"INSERT INTO {this.TableName} ({fields.ToString()}) VALUES ({values.ToString()});");
            return sb.ToString();
        }

        private string GetDbValue(JObject data, string colName, string dataType, bool nullable)
        {
            var value = data[colName].ToString().Replace("'", "''");
            if (string.IsNullOrEmpty(value) && nullable) return "NULL";
            switch (dataType)
            {
                case "bigint":
                    return value;
                case "bit":
                    return bool.Parse(value) ? "1" : "0";
                case "char":
                    return $"'{value}'";
                case "date":
                    try { return $"'{DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm:ss")}'"; }
                    catch { return "NULL"; }
                case "datetime":
                    try { return $"'{DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm:ss")}'"; }
                    catch { return "NULL"; }
                case "decimal":
                    return value;
                case "float":
                    return value;
                case "int":
                    return value;
                case "nvarchar":
                    return $"'{value}'";
                case "smallint":
                    return value;
                case "varchar":
                case "text":
                    return $"'{value}'";
                case "uniqueidentifier":
                    return $"'{value.ToUpper()}'";
                default:
                    return value;
            }
        }

        internal string CreateTableScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE {this.TableName}");
            sb.AppendLine("(");
            foreach (var col in this.ColsDef)
            {
                var colName = col.Key;
                var defs = col.Value.ToString().Split('|');
                var dataType = defs[0];
                var characters = !string.IsNullOrEmpty(defs[1]) && int.Parse(defs[1]) > 0 ? int.Parse(defs[1]) : -1;
                if (dataType == "varchar" && (defs[1] == "-1" || characters >= 8000))
                {
                    dataType = "text";
                }
                var nullable = defs[2].ToLower() == "yes";
                sb.AppendLine($"\t{colName} {dataType}{(characters > 0 && characters < 1000 ? "(" + characters.ToString() + ")" : string.Empty)} {(!nullable ? " NOT NULL" : " NULL")},");
            }
            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine();
            sb.AppendLine(");");
            return sb.ToString();
        }

        private void LoadHeaderFile()
        {
            var startIndex = 0;
            using (var header = new FileStream(this.HeaderFilePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new StringBuilder();
                int bt;
                bt = header.ReadByte();
                var chr = Convert.ToChar(bt);
                buffer.Append(chr);
                var last3Chars = new List<char>();
                while (bt > 0 || (bt <= 0 && buffer.Length > 0))
                {
                    if (last3Chars.Count == 3) last3Chars.RemoveAt(0);
                    last3Chars.Add(chr);
                    if (buffer.Length > 4000) System.Diagnostics.Debugger.Break();
                    if ((last3Chars.ElementAtOrDefault(0) == '|' &&
                        last3Chars.ElementAtOrDefault(1) == ';' &&
                        last3Chars.ElementAtOrDefault(0) == '|') || bt == -1)
                    {
                        if (buffer.Length > 2)
                        {
                            var dr = this.ParseDataRow(startIndex, buffer.ToString());
                            if (!dr.Id.ToString().Contains("-"))
                            {
                                dr.Id = long.Parse(dr.Id.ToString());
                            }
                            if (dr.DataStart != dr.DataEnd)
                            {
                                this.Rows[dr.Id] = dr;
                            }
                            else
                            {
                                // THIS ROW WAS DELETED
                            }
                        }
                        startIndex += buffer.Length;
                        buffer.Clear();
                    }
                    bt = header.ReadByte();
                    if (bt > 0)
                    {
                        chr = Convert.ToChar(bt);
                        buffer.Append(chr);
                    }
                }
            }
        }

        private DataRow ParseDataRow(long start, string text)
        {
            var parts = text.Split(new string[] { "|~|" }, StringSplitOptions.None);
            return new DataRow
            {
                Id = parts[0].Split('=').Last().ToUpper(),
                Version = int.Parse(parts[1]),
                Label = parts[5].Substring(0, parts[5].Length - 3).Trim(),
                DataStart = long.Parse(parts[2].Split(';').First()),
                DataEnd = long.Parse(parts[2].Split(';').Last()),
                IndexesStart = long.Parse(parts[3].Split(';').First()),
                IndexesEnd = long.Parse(parts[3].Split(';').Last()),
                ImageStart = long.Parse(parts[4].Split(';').First()),
                ImageEnd = long.Parse(parts[4].Split(';').Last()),
                HeaderStart = start,
                HeaderEnd = start + text.Length,
            };
        }

        private bool LoadIndexesFile(bool retryIfError)
        {
            try
            {
                if (!File.Exists(this.IndexesFilePath)) return false;
                using (var indexesHeader = new FileStream(this.IndexesFilePath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new StringBuilder();
                    int bt;
                    bt = indexesHeader.ReadByte();
                    if (bt == -1) return true;
                    var chr = Convert.ToChar(bt);
                    buffer.Append(chr);
                    var last3Chars = new List<char>();
                    object rowId;
                    while (bt != -1 || (bt == -1 && buffer.Length > 0))
                    {
                        if (last3Chars.Count == 3)
                            last3Chars.RemoveAt(0);
                        last3Chars.Add(chr);
                        if ((last3Chars.ElementAtOrDefault(0) == '|' &&
                            last3Chars.ElementAtOrDefault(1) == ';' &&
                            last3Chars.ElementAtOrDefault(0) == '|') || bt == -1)
                        {
                            var parts = buffer.ToString().Split(new string[] { "|~|" }, StringSplitOptions.None);
                            rowId = parts[0].Split('=').Last().ToUpper();
                            if (!((string)rowId).Contains("-"))
                                rowId = long.Parse((string)rowId);
                            if (rowId.ToString() != DEF_ID.ToString())
                            {
                                var tokens = parts[1].Substring(0, parts[1].Length - 3).Trim().Split(';');
                                foreach (var token in tokens)
                                {
                                    if (!string.IsNullOrEmpty(token))
                                    {
                                        var name = token.Split('=').First();
                                        var label = name.TrimEnd('1', '2', '3', '4', '5', '_');
                                        var value = token.Substring(name.Length + 1);
                                        if (label[0] == '&')
                                        {
                                            long vlr;
                                            if (long.TryParse(value, out vlr))
                                            {
                                                this.AddLinkedValue(label.Substring(1), vlr, rowId);
                                            }
                                            else
                                            {
                                                this.AddLinkedValue(label.Substring(1), value, rowId);
                                            }
                                        }
                                        else if (label[0] == '@' && value != null)
                                        {
                                            var field = label.Substring(1);
                                            if (this.ColsDef[field] != null)
                                            {
                                                var correctType = this.ColsDef[field].ToString().Split('|')[0];
                                                this.AddCachedValue(field, rowId, ChangeToActualType(value, correctType));
                                            }
                                            else
                                            {
                                                throw new ArgumentOutOfRangeException($"Invalid field for caching. The field '{field}' does not exists in the table '{this.TableName}'");
                                            }
                                        }
                                    }
                                }
                            }
                            buffer.Clear();
                        }
                        bt = indexesHeader.ReadByte();
                        if (bt > 0)
                        {
                            chr = Convert.ToChar(bt);
                            buffer.Append(chr);
                        }
                    }
                }
            }
            catch (FormatException)
            {
                this.RebuildIndexes();
                this.LoadIndexesFile(false);
                return true;
            }
            return false;
        }

        public object GetNextRowId()
        {
            var query = this.Rows.Where(f => f.Key != DEF_ID);
            if (((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)this.ColsDef.First).Value).Value.ToString().Split('|').FirstOrDefault() == "bigint")
            {
                var max = query.Any() ? query.Where(f => f.Key is long).Select(f => f.Key).Max() : 0;
                if (max == null) return 1;
                else return (long)max + 1;
            }
            else
            {
                return Guid.NewGuid().ToString().ToUpper();
            }
        }

        private object ChangeToActualType(string value, string type)
        {
            if (type == "varchar" || type == "char" || type == "text") return value;
            else if (type == "int") return int.Parse(value);
            else if (type == "bigint") return long.Parse(value);
            else if (type == "smallint") return short.Parse(value);
            else if (type == "datetime") return DateTime.Parse(value);
            else if (type == "decimal") return decimal.Parse(value);
            else if (type == "float") return float.Parse(value);
            else throw new NotSupportedException(type);
        }

        public void RollbackRow(long id, dynamic data, int version)
        {
            var needToRecreate = !this.Rows.ContainsKey(id);
            var entity = data;
            entity = data.RowData;
            var headerStart = (long)data.HeaderStart;
            var label = this.GetLabels(entity).Trim();
            var image = !string.IsNullOrEmpty(this.FileName) ?
                        this.GetFieldValue(entity, this.FileName) : string.Empty;
            var item = !needToRecreate ?
                        this.Rows[id] :
                        ReadHeader(headerStart, headerStart + 315);
            item.Version = version + 1;


            using (var dataWriter = new FileStream(this.DataFilePath, FileMode.Append, FileAccess.Write))
            {
                this.WriteInDataFile(dataWriter, entity, ref item);
            }

            using (var headerWriter = new FileStream(this.HeaderFilePath, FileMode.Open, FileAccess.Write))
            {
                headerWriter.Seek(headerStart, SeekOrigin.Begin);
                this.WriteInHeaderFile(headerWriter, ref item);
            }

            using (var historyWriter = new FileStream(this.HistoryFilePath, FileMode.Append, FileAccess.Write))
            {
                this.WriteInHistoryFile(historyWriter, ref item, "ROLLBACK");
            }

            using (var indexesWriter = new FileStream(this.IndexesFilePath, FileMode.Open, FileAccess.Write))
            {
                indexesWriter.Seek(item.IndexesStart, SeekOrigin.Begin);
                this.WriteInIndexesFile(indexesWriter, item, ref item);
            }
        }

        private void LoadHistoryFile()
        {
            using (var reader = new FileStream(this.HistoryFilePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new StringBuilder();
                int bt;
                bt = reader.ReadByte();
                var chr = Convert.ToChar(bt);
                buffer.Append(chr);
                var last3Chars = new List<char>();
                while (bt != -1 || (bt == -1 && buffer.Length > 0))
                {
                    if (last3Chars.Count == 3)
                        last3Chars.RemoveAt(0);
                    last3Chars.Add(chr);
                    if ((last3Chars.ElementAtOrDefault(0) == '|' &&
                        last3Chars.ElementAtOrDefault(1) == ';' &&
                        last3Chars.ElementAtOrDefault(0) == '|') || bt == -1)
                    {
                        var parts = buffer.ToString().Split(new string[] { "|~|" }, StringSplitOptions.None);
                        var rowId = long.Parse(parts[0].Split('=').Last());
                        var historyLead = new DataRowHistory();
                        historyLead.Id = rowId;
                        historyLead.Version = int.Parse(parts[1]);
                        historyLead.HeaderStart = long.Parse(parts[2]);
                        var dataPosition = parts[3].Split(';');
                        historyLead.DataStart = int.Parse(dataPosition[0]);
                        historyLead.DataEnd = int.Parse(dataPosition[1]);
                        historyLead.EventDate = DateTime.Parse(parts[4]);
                        historyLead.Action = parts[5].Substring(0, parts[5].Length - 3).Trim();
                        if (!this.History.ContainsKey(rowId))
                        {
                            this.History[rowId] = new List<DataRowHistory>();
                        }
                        this.History[rowId].Add(historyLead);
                        buffer.Clear();
                    }
                    bt = reader.ReadByte();
                    if (bt > 0)
                    {
                        chr = Convert.ToChar(bt);
                        buffer.Append(chr);
                    }
                }
            }
        }

        public void EnsureHistoryLoaded()
        {
            if (!this.HistoryLoaded)
                this.LoadHistoryFile();
            this.HistoryLoaded = true;
        }

        public void AddLinkedValueField(string field, string table)
        {
            this.LinkedValueFields.Add(field, table);
        }

        private void AddLinkedValue(string label, object value, object rowId)
        {
            if (value != null && value.ToString().StartsWith("="))
            {
                System.Diagnostics.Debugger.Break();
            }
            if (!this.LinkedValues.ContainsKey(label))
                this.LinkedValues[label] = new Dictionary<object, List<object>>();
            if (!this.LinkedValues[label].ContainsKey(value))
                this.LinkedValues[label][value] = new List<object>();
            if (!this.LinkedValues[label][value].Contains(rowId))
                this.LinkedValues[label][value].Add(rowId);
        }

        public void CreateFiles(string connectionString)
        {
            using (var scope = log.Scope($"[{this.TableName}] CreateFiles()"))
            {
                try
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        var cols = conn.Query<dynamic>($"SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{this.TableName}' ORDER BY ORDINAL_POSITION").ToList();
                        var rows = conn.Query<dynamic>($"SELECT * FROM {this.TableName} ORDER BY {this.RowIdField}");
                        using (var headerWriter = new FileStream(this.HeaderFilePath, FileMode.CreateNew, FileAccess.Write))
                        {
                            using (var dataWriter = new FileStream(this.DataFilePath, FileMode.CreateNew, FileAccess.Write))
                            {
                                using (var imageWriter = new FileStream(this.ImageFilePath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    using (var indexesWriter = new FileStream(this.IndexesFilePath, FileMode.CreateNew, FileAccess.Write))
                                    {
                                        using (var historyWriter = new FileStream(this.HistoryFilePath, FileMode.Append, FileAccess.Write))
                                        {
                                            object id = DEF_ID;
                                            var label = "Table definitions";
                                            var fileName = string.Empty;
                                            var dr = new DataRow(id, label, fileName);
                                            dr.Version = 0;
                                            var defs = GetTableColumns(cols);
                                            this.WriteInDataFile(dataWriter, defs, ref dr);
                                            this.WriteInImageFile(imageWriter, ref dr);
                                            this.WriteInIndexesFile(indexesWriter, defs, ref dr);
                                            this.WriteInHeaderFile(headerWriter, ref dr);
                                            this.WriteInHistoryFile(historyWriter, ref dr, "ADDED");

                                            foreach (var row in rows)
                                            {
                                                id = this.GetFieldValue(row, this.RowIdField);
                                                label = this.GetLabels(row);
                                                fileName = (string)this.GetFieldValue(row, this.FileName);
                                                dr = new DataRow(id, label, fileName);
                                                dr.Version = 0;
                                                this.WriteInDataFile(dataWriter, row, ref dr);
                                                this.WriteInImageFile(imageWriter, ref dr);
                                                this.WriteInIndexesFile(indexesWriter, row, ref dr);
                                                this.WriteInHeaderFile(headerWriter, ref dr);
                                                this.WriteInHistoryFile(historyWriter, ref dr, "ADDED");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        internal void RebuildIndexes()
        {
            using (var scope = log.Scope($"[{this.TableName}] RebuildIndexes()"))
            {
                try
                {
                    using (var headerWriter = new FileStream(this.HeaderFilePath, FileMode.Open, FileAccess.Write))
                    {
                        if (File.Exists(this.IndexesFilePath)) File.Delete(this.IndexesFilePath);
                        using (var indexesWriter = new FileStream(this.IndexesFilePath, FileMode.CreateNew, FileAccess.Write))
                        {
                            object id = DEF_ID;
                            var label = "Table definitions";
                            var fileName = string.Empty;
                            var dr = new DataRow(id, label, fileName);
                            dr.Version = 0;
                            var defs = this.ReadRow(id);

                            this.WriteInIndexesFile(indexesWriter, defs, ref dr);
                            this.WriteInHeaderFile(headerWriter, ref dr);

                            foreach (var rowId in this.Rows.Keys)
                            {
                                dr = this.Rows[rowId];
                                var data = this.ReadRow(rowId);
                                this.WriteInIndexesFile(indexesWriter, data, ref dr);
                                this.WriteInHeaderFile(headerWriter, ref dr);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }


        private dynamic GetTableColumns(List<dynamic> cols)
        {
            var obj = new JObject();
            foreach (var col in cols)
            {
                obj[col.COLUMN_NAME] = $"{col.DATA_TYPE}|{col.CHARACTER_MAXIMUM_LENGTH}|{col.IS_NULLABLE}";
            }
            return obj;
        }

        private long WriteInHistoryFile(FileStream writer, ref DataRow dr, string action)
        {
            var now = DateTime.Now;
            var text = $"@ROWID={FormatId(dr.Id)}|~|{dr.Version.ToString("000")}|~|" +
                   $"{dr.HeaderStart.ToString("000000000000")}|~|" +
                   $"{dr.DataStart.ToString("000000000000")};{dr.DataEnd.ToString("000000000000")}|~|" +
                   $"{now.ToString("HH:mm:ss yyyy-MM-dd")}|~|{string.Format("{0,-8}", action)}|;|";
            foreach (char chr in text)
            {
                var vlr = chr;
                if (chr == 8211) vlr = '-';
                else if (chr > 8200) continue;
                writer.WriteByte(Convert.ToByte(vlr));
            }
            if (this.HistoryLoaded)
            {
                if (!this.History.ContainsKey(dr.Id))
                {
                    this.History[dr.Id] = new List<DataRowHistory>();
                }
                this.History[dr.Id].Add(new DataRowHistory
                {
                    Id = dr.Id,
                    Action = action,
                    Version = dr.Version,
                    DataStart = dr.DataStart,
                    DataEnd = dr.DataEnd,
                    EventDate = now,
                    HeaderStart = dr.HeaderStart
                });
            }
            return writer.Length;
        }

        private string FormatId(object id)
        {
            if (id is long)
            {
                return ((long)id).ToString("000000000000");
            }
            else
            {
                return id.ToString().ToUpper();
            }
        }

        private long WriteInIndexesFile(FileStream writer, dynamic row, ref DataRow dr)
        {
            dr.IndexesStart = writer.Position;
            var text = $"@ROWID={dr.Id}|~|{string.Format("{0,-550}", this.GetLinkedAndCachedValuesString(row))}|;|";
            foreach (char chr in text)
            {
                writer.WriteByte(Convert.ToByte(chr));
            }
            dr.IndexesEnd = writer.Position;
            return writer.Position;
        }

        public void DeleteRow(object rowId)
        {
            var data = this.ReadRow(rowId);
            this.ResetLinkedAndCachedValues(rowId, data, null);

            var row = this.Rows[rowId];
            row.DataStart = 0;
            row.DataEnd = 0;
            row.Version++;


            using (var headerWriter = new FileStream(this.HeaderFilePath, FileMode.Open, FileAccess.Write))
            {
                headerWriter.Seek(row.HeaderStart, SeekOrigin.Begin);
                this.WriteInHeaderFile(headerWriter, ref row);
            }

            using (var historyWriter = new FileStream(this.HistoryFilePath, FileMode.Append, FileAccess.Write))
            {
                this.WriteInHistoryFile(historyWriter, ref row, "DELETED");
            }

            this.Rows.Remove(rowId);
        }

        private long WriteInImageFile(FileStream writer, ref DataRow dr)
        {
            dr.ImageStart = writer.Position;
            if (!string.IsNullOrEmpty(dr.FileName))
            {
                var img = Path.Combine(this.ImagePath, dr.FileName);
                if (img != null)
                {
                    var tempPath = Path.Combine(this.ImagePath, $"{Guid.NewGuid()}.jpg");
                    ImageHelper.ResizeImage(BinaryHelper.FromImageToString(img), this.PreviewSize.Width, this.PreviewSize.Height, tempPath);
                    img = BinaryHelper.FromImageToString(tempPath);
                    var text = $"@ROWID={dr.Id}|~|{img}|;|";
                    foreach (char chr in text)
                    {
                        writer.WriteByte(Convert.ToByte(chr));
                    }
                    File.Delete(tempPath);
                }
            }
            dr.ImageEnd = writer.Position;
            return writer.Position;
        }

        private long WriteInDataFile(FileStream writer, dynamic row, ref DataRow dr)
        {
            dr.DataStart = writer.Position;
            var rowData = JsonConvert.SerializeObject(row);
            rowData = GZipHelper.Compress(rowData);
            var text = $"@ROWID={dr.Id}|~|{rowData}|;|";
            foreach (char chr in text)
            {
                writer.WriteByte(Convert.ToByte(chr));
            }
            dr.DataEnd = writer.Position;
            return writer.Position;
        }

        internal dynamic NewRow()
        {
            var obj = new JObject();
            foreach (var col in this.ColsDef)
            {
                obj[col.Key] = null;
            }
            obj[this.RowIdField] = 0;
            if (this.LabelField != null)
            {
                obj[this.LabelField] = string.Empty;
            }
            return obj;
        }

        private long WriteInHeaderFile(FileStream writer, ref DataRow dr)
        {
            dr.HeaderStart = writer.Position;
            var text = $"@ROWID={FormatId(dr.Id)}|~|{dr.Version.ToString("000")}|~|" +
                   $"{dr.DataStart.ToString("000000000000")};{dr.DataEnd.ToString("000000000000")}|~|" +
                   $"{dr.IndexesStart.ToString("000000000000")};{dr.IndexesEnd.ToString("000000000000")}|~|" +
                   $"{dr.ImageStart.ToString("000000000000")};{dr.ImageEnd.ToString("000000000000")}|~|" +
                   $"{string.Format("{0,-200}", dr.Label)}|;|";
            foreach (char chr in text)
            {
                var vlr = chr;
                if (chr == 8211) vlr = '-';
                else if (chr > 8200) continue;
                writer.WriteByte(Convert.ToByte(vlr));
            }
            dr.HeaderEnd = writer.Position;
            return writer.Position;
        }

        private string GetLabels(object data)
        {
            if (this.LabelField == null) return null;
            var sb = new StringBuilder();
            try
            {
                return this.GetFieldValue(data, this.LabelField).ToString().Trim();
            }
            catch (NullReferenceException)
            {
                var content = string.Empty;
                if (data is JObject)
                {
                    content = ((JObject)data).ToString();
                }
                else if (data is ExpandoObject || data.GetType().Name == "DapperRow")
                {
                    content = Newtonsoft.Json.JsonConvert.SerializeObject(data).ToString();
                }
                throw new ArgumentNullException($"label not found (field: {this.LabelField}) in '{content}'.");
            }
        }

        public bool FileExists()
        {
            return File.Exists(this.HeaderFilePath) &&
                File.Exists(this.DataFilePath);
        }

        public object GetFieldValue(object data, string field)
        {
            if (field == null || data == null) return null;
            if (data is JObject)
            {
                return ((JObject)data)[field];
            }
            else if (data is ExpandoObject || data.GetType().Name == "DapperRow")
            {
                return ((IDictionary<string, object>)data)[field];
            }
            else
            {
                var type = ((object)data).GetType();
                var propInfo = type.GetProperty(field);
                if (propInfo != null)
                {
                    return propInfo.GetValue(data);
                }
            }
            return null;
        }

        public void SetFieldValue(object data, string field, object value)
        {
            if (field == null) return;
            if (data is JObject)
            {
                ((JObject)data)[field] = JToken.FromObject(value);
            }
            else if (data is ExpandoObject || data.GetType().Name == "DapperRow")
            {
                ((IDictionary<string, object>)data)[field] = value;
            }
            else
            {
                var type = ((object)data).GetType();
                var propInfo = type.GetProperty(field);
                if (propInfo != null)
                {
                    propInfo.SetValue(data, value);
                }
            }
        }

        public void AddCachedValueField(string field)
        {
            this.CachedValueFields.Add(field);
        }

        public void AddCachedValue(string field, object id, object value)
        {
            if (value == null || value.ToString().Length == 0) return;
            if (!this.CachedValues.ContainsKey(field))
                this.CachedValues[field] = new Dictionary<object, object>();
            this.CachedValues[field][id] = value;
        }

        public string GetDbValue(object value)
        {
            if (value is DateTime)
            {
                if (((DateTime)value).Year == 1)
                    value = DateTime.Now;
                return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            else if (value is long || value is int || value is short)
            {
                return value.ToString();
            }
            else if (value is decimal || value is double)
            {
                return ((decimal)value).ToString("n2");
            }
            else if (value != null && value.GetType().BaseType == typeof(System.Enum))
            {
                return ((int)value).ToString();
            }
            else if (value != null)
            {
                return $"'{value.ToString().Replace("'", "''")}'";
            }
            else
                return "NULL";
        }

        public string GetLinkedAndCachedValuesString(dynamic data)
        {
            Dictionary<string, object> results = this.GetLinkedAndCachedValuesArray(data);
            var sb = new StringBuilder();
            foreach (var item in results)
            {
                sb.Append($"{item.Key}={CachedValueToString(item.Value)};");
            }
            return sb.ToString();
        }

        private string CachedValueToString(object value)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (value is JValue && ((JValue)value).Value is DateTime)
            {
                return ((DateTime)((JValue)value).Value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (value != null)
            {
                return value.ToString();
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, object> GetLinkedAndCachedValuesArray(dynamic data)
        {
            var results = new Dictionary<string, object>();
            foreach (var foreign in this.LinkedValueFields.Keys)
            {
                var value = this.GetFieldValue(data, foreign);
                if (value != null)
                {
                    results[$"&{foreign}"] = value;
                }
            }
            foreach (var foreign in this.CachedValueFields)
            {
                var value = this.GetFieldValue(data, foreign);
                if (value != null)
                {
                    results[$"@{foreign}"] = value;
                }
            }
            return results;
        }

        public void ResetLinkedAndCachedValues(object rowId, dynamic oldState, dynamic newState)
        {
            if (oldState != null)
            {
                Dictionary<string, object> oldValues = GetLinkedAndCachedValuesArray(oldState);

                // Remove old values
                foreach (var item in oldValues)
                {
                    var field = item.Key.Substring(1).TrimEnd('1', '2', '3', '4', '5', ' ');
                    if (item.Key[0] == '&' && item.Value is JValue)
                    {
                        if (((JValue)item.Value).Type != JTokenType.Object &&
                            ((JValue)item.Value).Type != JTokenType.Null)
                        {
                            if (((JValue)item.Value).ToString().Contains("-"))
                            {
                                var value = ((JValue)item.Value).ToString().ToUpper();
                                if (this.LinkedValues[field].ContainsKey(value))
                                    this.LinkedValues[field][value].Remove(rowId);
                            }
                            else
                            {
                                var value = long.Parse(((JValue)item.Value).ToString());
                                this.LinkedValues[field][value].Remove(rowId);
                            }
                        }
                    }
                    else if (item.Key[0] == '@' && item.Value is JValue)
                    {
                        var value = ((JValue)item.Value).Value;
                        if (this.CachedValues.ContainsKey(field))
                            this.CachedValues[field].Remove(rowId);
                    }
                }
            }

            if (newState != null)
            {
                Dictionary<string, object> newValues = GetLinkedAndCachedValuesArray(newState);
                // Add new values
                foreach (var item in newValues)
                {
                    var field = item.Key.Substring(1).TrimEnd('1', '2', '3', '4', '5', ' ');
                    if (item.Key[0] == '&' && item.Value != null)
                    {
                        try
                        {
                            object value = item.Value.ToString();
                            if (value.ToString().ToUpper() != "NULL")
                            {
                                if (!string.IsNullOrEmpty((string)value) && !((string)value).Contains("-"))
                                {
                                    try
                                    {
                                        value = long.Parse(value.ToString());
                                    }
                                    catch { continue; }

                                }
                                object id = rowId.ToString();
                                if (!string.IsNullOrEmpty((string)id) && !((string)id).Contains("-"))
                                {
                                    id = long.Parse(id.ToString());
                                }
                                if (!this.LinkedValues.ContainsKey(field))
                                    this.LinkedValues[field] = new Dictionary<object, List<object>>();
                                if (!this.LinkedValues[field].ContainsKey(value))
                                    this.LinkedValues[field][value] = new List<object>();
                                this.LinkedValues[field][value].Add(id);
                            }
                        }
                        catch
                        {
                        }
                    }
                    else if (item.Key[0] == '@' && item.Value != null)
                    {
                        var value = item.Value.ToString();
                        var typedValue = this.GetTypedValue(item.Key.Substring(1), value);
                        if (!this.CachedValues.ContainsKey(field))
                        {
                            this.CachedValues[field] = new Dictionary<object, object>();
                        }
                        this.CachedValues[field][rowId] = typedValue;
                    }
                }
            }
        }

        private object GetTypedValue(string field, object value)
        {
            var type = this.ColsDef[field].Value<string>().Split('|').First();
            if (type == "smallint")
                return Convert.ToInt16(value);
            else if (type == "int")
                return Convert.ToInt32(value);
            else if (type == "bigint")
                return Convert.ToInt64(value);
            else if (type == "datetime")
                return Convert.ToDateTime(value);
            else
                return value.ToString();
        }

        public dynamic ReadRow(object rowId)
        {
            try
            {
                using (var reader = new FileStream(this.DataFilePath, FileMode.Open, FileAccess.Read))
                {
                    var start = this.Rows[rowId].DataStart;
                    var end = this.Rows[rowId].DataEnd;
                    reader.Seek(start, SeekOrigin.Begin);
                    var sb = new StringBuilder();
                    for (var i = start; i <= end - 4; i++)
                    {
                        var chr = reader.ReadByte();
                        if (chr > 0)
                        {
                            sb.Append(Convert.ToChar(chr));
                        }
                    }
                    var parts = sb.ToString().Split(new string[] { "|~|" }, StringSplitOptions.None);
                    var id = parts[0].Split('=').Last().ToUpper();
                    if (parts.Length == 2)
                    {
                        var content = parts[1];
                        content = GZipHelper.Decompress(content);
                        if (!string.IsNullOrEmpty(content))
                        {
                            return JsonConvert.DeserializeObject<dynamic>(content);
                        }
                        else
                        {
                            return new { Start = start, End = end, RowId = rowId, ErrorMessage = "Invalid file segmentation." };
                        }
                    }
                    else
                    {
                        if (rowId.ToString() != Guid.Empty.ToString())
                            throw new ArgumentOutOfRangeException($"Unable to find row id '{rowId}'");
                        else
                            return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        public dynamic ReadRow(long start, long end)
        {
            using (var reader = new FileStream(this.DataFilePath, FileMode.Open, FileAccess.Read))
            {
                reader.Seek(start, SeekOrigin.Begin);
                var sb = new StringBuilder();
                for (var i = start; i <= end - 4; i++)
                {
                    sb.Append(Convert.ToChar(reader.ReadByte()));
                }
                var parts = sb.ToString().Split(new string[] { "|~|" }, StringSplitOptions.None);
                var id = long.Parse(parts[0].Split('=').Last());
                var content = parts[1];
                content = GZipHelper.Decompress(content);
                return JsonConvert.DeserializeObject<dynamic>(content);
            }
        }

        public DataRow ReadHeader(long start, long end)
        {
            using (var reader = new FileStream(this.HeaderFilePath, FileMode.Open, FileAccess.Read))
            {
                reader.Seek(start, SeekOrigin.Begin);
                var sb = new StringBuilder();
                for (var i = start; i < end; i++)
                {
                    sb.Append(Convert.ToChar(reader.ReadByte()));
                }
                return ParseDataRow(start, sb.ToString());
            }
        }

        public void WriteRow(object rowId, dynamic data, bool refreshIndexes = false)
        {
            using (var scope = log.Scope($"[{this.TableName}] WriteRow({data})"))
            {
                try
                {
                    DataRow item;
                    var isNewRow = false;
                    if (this.Rows.ContainsKey(rowId))
                    {
                        item = this.Rows[rowId];
                        if (this.LabelField != null)
                            item.Label = this.GetLabels(data).Trim();
                        item.Version++;

                        // Refresh cache of indexes
                        var old = this.ReadRow(rowId);
                        this.ResetLinkedAndCachedValues(rowId, old, data);
                    }
                    else
                    {
                        var id = (object)this.GetFieldValue(data, this.RowIdField);
                        if (!id.ToString().Contains("-"))
                        {
                            id = long.Parse(id.ToString());
                        }
                        else
                        {
                            id = id.ToString();
                        }
                        var label = this.GetLabels(data);
                        var fileName = (string)this.GetFieldValue(data, this.FileName);
                        item = new DataRow(id, label, fileName);
                        item.Version = 1;
                        this.Rows[id] = item;
                        isNewRow = true;

                        // Refresh cache of indexes
                        this.ResetLinkedAndCachedValues(rowId, null, data);
                    }

                    using (var dataWriter = new FileStream(this.DataFilePath, FileMode.Append, FileAccess.Write))
                    {
                        this.WriteInDataFile(dataWriter, data, ref item);
                    }

                    using (var imageWriter = new FileStream(this.ImageFilePath, FileMode.Append, FileAccess.Write))
                    {
                        this.WriteInImageFile(imageWriter, ref item);
                    }

                    var mode = isNewRow ? FileMode.Append : FileMode.Open;
                    if (refreshIndexes || item.Version == 1)
                    {
                        using (var indexesWriter = new FileStream(this.IndexesFilePath, mode, FileAccess.Write))
                        {
                            if (mode == FileMode.Open)
                                indexesWriter.Seek(item.IndexesStart, SeekOrigin.Begin);
                            this.WriteInIndexesFile(indexesWriter, data, ref item);
                        }
                    }

                    using (var headerWriter = new FileStream(this.HeaderFilePath, mode, FileAccess.Write))
                    {
                        if (mode == FileMode.Open)
                            headerWriter.Seek(item.HeaderStart, SeekOrigin.Begin);
                        this.WriteInHeaderFile(headerWriter, ref item);
                    }

                    using (var historyWriter = new FileStream(this.HistoryFilePath, FileMode.Append, FileAccess.Write))
                    {
                        this.WriteInHistoryFile(historyWriter, ref item, item.Version > 1 ? "UPDATED" : "ADDED");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }

        }

    }
}
