using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Logging;

namespace SqlCache.Framework
{
    internal class CacheServer : IDisposable
    {

        internal CacheServer()
        {
            this.Tables = new List<DataTable>();
            this.IsStarted = false;
        }

        private Logger log = new Logger(typeof(CacheServer));

        internal bool IsStarted { get; set; }

        internal string ConnectionString { get; set; }

        internal string CachePath { get; set; }

        internal string ImagePath { get; set; }

        internal List<DataTable> Tables { get; set; }

        internal Size ImagePreviewSize { get; set; }

        public string ImageUrl { get; set; }

        internal DataTable this[string name]
        {
            get
            {
                return this.Tables
                    .First(f => f.TableName.ToLower() == name.ToLower());
            }
        }

        internal DataTable AddTable(string name, string rowIdField,
            string labelField, string fileName = null)
        {
            var data = new DataTable
            {
                TableName = name,
                RowIdField = rowIdField,
                LabelField = labelField,
                FileName = fileName,
                PreviewSize = this.ImagePreviewSize
            };
            this.Tables.Add(data);
            return data;
        }

        internal void Start()
        {
            using (var scope = log.Scope("Start()"))
            {
                try
                {
                    if (this.IsStarted) return;
                    var path = Path.Combine(this.CachePath, "bin");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    foreach (var table in this.Tables)
                    {
                        var name = table.TableName.ToLower();
                        table.HeaderFilePath = Path.Combine(path, $"{name}.header");
                        table.DataFilePath = Path.Combine(path, $"{name}.data");
                        table.IndexesFilePath = Path.Combine(path, $"{name}.indexes");
                        table.HistoryFilePath = Path.Combine(path, $"{name}.history");
                        table.ImageFilePath = Path.Combine(path, $"{name}.images");
                        table.ImagePath = this.ImagePath;
                        if (!table.FileExists())
                        {
                            try
                            {
                                table.CreateFiles(this.ConnectionString);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Error trying to load '{table.HeaderFilePath}' from db.", ex);
                            }
                        }
                    }
                    this.Tables.AsParallel().ForAll(table => table.LoadFiles());
                    this.IsStarted = true;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        internal List<dynamic> ReadRowsByForeignKey(string table,
            string foreignKeyField, long foreignKeyValue)
        {
            var ids = this[table].LinkedValues[foreignKeyField][foreignKeyValue];
            return this.ReadRows(table, ids.ToArray());
        }

        internal dynamic SearchRow(string table, string expression)
        {
            return this.SearchRows(table, expression).FirstOrDefault();
        }

        internal List<dynamic> SearchRows(string table, string expression)
        {
            var ids = this[table].Rows
                        .Where(f => f.Value.Label.ToLower().Contains(expression.ToLower()))
                        .OrderBy(f =>
                            f.Value.Label.ToLower().StartsWith(expression.ToLower()) ? 1 :
                            f.Value.Label.ToLower().EndsWith(expression.ToLower()) ? 3 : 2)
                        .ThenBy(f => f.Value.Label).Select(f => f.Key).ToArray();
            return this.ReadRows(table, ids);
        }

        internal List<dynamic> ReadRows(string table, object[] ids)
        {
            using (var scope = log.Scope("ReadRows()"))
            {
                try
                {
                    var rows = new List<dynamic>();
                    foreach (var id in ids)
                    {
                        rows.Add(this.ReadRow(table, id));
                    }
                    return rows;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        internal void DeleteRow(string table, object id)
        {
            using (var scope = log.Scope("DeleteRow()"))
            {
                try
                {
                    var dt = this[table];
                    dt.DeleteRow(id);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        internal void Stop()
        {
            using (var scope = log.Scope("Stop()"))
            {
                try
                {
                    this.IsStarted = false;
                    foreach (var table in this.Tables)
                    {
                        table.Rows = null;
                        table.LinkedValues = null;
                        table.History = null;
                    }
                    this.Tables = null;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        internal dynamic ReadRow(string table, object rowId)
        {
            using (var scope = log.Scope("ReadRow()"))
            {
                try
                {
                    var tb = this[table];
                    return tb.ReadRow(rowId);
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        public void Dispose()
        {
            if (this.IsStarted)
                this.Stop();
        }

        internal object WriteRow(string table, dynamic row, bool refreshIndexes = false)
        {
            using (var scope = log.Scope("WriteRow()"))
            {
                try
                {
                    var tb = this[table];
                    var id = (object)tb.GetFieldValue(row, tb.RowIdField);
                    if(id.ToString() == "0")
                    {
                        id = (long)0;
                    }
                    if (id is long)
                    {
                        if ((long)id == 0)
                        {
                            id = tb.GetNextRowId();
                            tb.SetFieldValue(row, tb.RowIdField, id);
                        }
                        tb.WriteRow(id, row, refreshIndexes);
                        return id;
                    }
                    else
                    {
                        if (id == null || id.ToString() == Guid.Empty.ToString())
                        {
                            id = tb.GetNextRowId();
                            tb.SetFieldValue(row, tb.RowIdField, id);
                        }
                        tb.WriteRow(id.ToString().ToUpper(), row, refreshIndexes);
                        if(!id.ToString().Contains("-"))
                        {
                            return long.Parse(id.ToString().ToUpper());
                        }
                        else
                        {
                            return id.ToString().ToUpper();
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

    }
}
