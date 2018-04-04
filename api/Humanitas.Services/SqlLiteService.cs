using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using SqlCache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Humanitas.Services
{
    public class SqlLiteService : ISqlLiteService
    {

        private Logger log = new Logger(typeof(SqlLiteService));
        private AppConfiguration _config = null;

        public SqlLiteService(AppConfiguration config)
        {
            this._config = config;
        }

        string ISqlLiteService.CreatesTables()
        {
            using (var scope = log.Scope("CreatesTables()"))
            {
                try
                {
                    var sb = new StringBuilder();
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        foreach (var table in conn.Tables())
                        {
                            var cols = conn.GetTableSchema(table.Key);
                            sb.AppendLine($"DROP TABLE IF EXISTS {table.Key}");
                            sb.AppendLine("|~|");
                        }
                        foreach (var table in conn.Tables())
                        {
                            var cols = conn.GetTableSchema(table.Key);
                            sb.AppendLine($"CREATE TABLE IF NOT EXISTS {table.Key} ");
                            sb.AppendLine($"(");
                            foreach (var col in cols)
                            {
                                var dataType = string.Empty;
                                switch (col.DataType)
                                {
                                    case ColDefinition.DataTypes.SMALLINT:
                                    case ColDefinition.DataTypes.INT:
                                    case ColDefinition.DataTypes.BIGINT:
                                    case ColDefinition.DataTypes.BIT:
                                        dataType = "INTEGER";
                                        if (col == cols.First())
                                        {
                                            dataType = $"{dataType} PRIMARY KEY ASC";
                                        }
                                        break;
                                    case ColDefinition.DataTypes.VARCHAR:
                                    case ColDefinition.DataTypes.TEXT:
                                    case ColDefinition.DataTypes.UNIQUEIDENTIFIER:
                                        dataType = "TEXT";
                                        if (col == cols.First())
                                        {
                                            dataType = $"{dataType} PRIMARY KEY ASC";
                                        }
                                        break;
                                    case ColDefinition.DataTypes.DATETIME:
                                    case ColDefinition.DataTypes.DATE:
                                    case ColDefinition.DataTypes.DECIMAL:
                                    case ColDefinition.DataTypes.FLOAT:
                                        dataType = "REAL";
                                        break;
                                }
                                //var charLength = col.CharactersLength > 0 ? $"[{col.CharactersLength}]" : string.Empty;
                                sb.AppendLine($"\t{col.ColumnName} {dataType},");
                            }
                            sb.Remove(sb.Length - 3, 3);
                            sb.AppendLine();
                            sb.AppendLine($")");
                            sb.AppendLine("|~|");
                            sb.AppendLine();
                        }
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        string[] ISqlLiteService.Tables()
        {
            using (var scope = log.Scope("Tables()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        return conn.Tables().Select(f => f.Key).ToArray();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        string ISqlLiteService.Inserts(int? top = null)
        {
            using (var scope = log.Scope("Inserts()"))
            {
                try
                {
                    var sb = new StringBuilder();
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        foreach (var table in conn.Tables())
                        {
                            var cols = conn.GetTableSchema(table.Key);
                            var index = 0;
                            foreach (var row in conn.Rows(table.Key))
                            {
                                var fields = new StringBuilder();
                                var values = new StringBuilder();

                                fields.AppendLine($"(");
                                values.AppendLine($"(");
                                foreach (var col in cols)
                                {
                                    fields.AppendLine($"\t{col.ColumnName},");
                                    var value = row[col.ColumnName].ToString();
                                    if (string.IsNullOrEmpty(value))
                                    {
                                        values.AppendLine($"\tNULL,");
                                    }
                                    else
                                    {
                                        switch (col.DataType)
                                        {
                                            case ColDefinition.DataTypes.SMALLINT:
                                            case ColDefinition.DataTypes.INT:
                                            case ColDefinition.DataTypes.BIGINT:
                                            case ColDefinition.DataTypes.BIT:
                                                values.AppendLine($"\t{value},");
                                                break;
                                            case ColDefinition.DataTypes.DATETIME:
                                            case ColDefinition.DataTypes.DATE:
                                                values.AppendLine($"\tjulianday('{DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm:ss")}'),");
                                                break;
                                            case ColDefinition.DataTypes.VARCHAR:
                                            case ColDefinition.DataTypes.TEXT:
                                                values.AppendLine($"\t'{value.Replace("'", "''").Replace("~", "-").Replace("|", "/")}',");
                                                break;
                                            case ColDefinition.DataTypes.UNIQUEIDENTIFIER:
                                                values.AppendLine($"\t'{value.Replace("'", "''").Replace("~", "-").Replace("|", "/").ToUpper()}',");
                                                break;
                                            case ColDefinition.DataTypes.DECIMAL:
                                                values.AppendLine($"\t{value},");
                                                break;
                                        }
                                    }
                                }
                                fields.Remove(fields.Length - 3, 3);
                                values.Remove(values.Length - 3, 3);
                                fields.AppendLine();
                                values.AppendLine();
                                fields.Append($")");
                                values.Append($")");
                                sb.AppendLine($"INSERT INTO {table.Key} {fields} VALUES {values}");
                                sb.AppendLine("|~|");
                                index++;
                                if (top != null && index >= top.Value)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        string ISqlLiteService.LatestVersion(string rootPath)
        {
            using (var scope = log.Scope("LatestVersion()"))
            {
                try
                {
                    var latest = Directory.GetFiles(rootPath, "Leibniz_v*.info")
                                .OrderByDescending(f => f)
                                .FirstOrDefault();
                    var current = string.Empty;
                    if (latest != null)
                    {
                        current = File.ReadAllText(latest);
                    }
                    var sb = new StringBuilder();
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        foreach (var table in conn.Tables())
                        {
                            sb.AppendLine($"{table}:={conn.Count(table.Key)}");
                        }
                    }
                    if (current != sb.ToString())
                    {
                        var nextVersion = 1;
                        if (latest != null)
                        {
                            var version = int.Parse(Path.GetFileNameWithoutExtension(latest).Substring(9));
                            nextVersion = version + 1;
                        }
                        var path = Path.Combine(rootPath, $"Leibniz_v{nextVersion.ToString("000")}.info");
                        File.WriteAllText(path, sb.ToString());
                        path = Path.ChangeExtension(path, ".sql");
                        var tables = ((ISqlLiteService)this).CreatesTables();
                        var indexes = ((ISqlLiteService)this).CreatesIndexes();
                        var inserts = ((ISqlLiteService)this).Inserts();
                        var content = tables + Environment.NewLine +
                                      indexes + Environment.NewLine +
                                      inserts;
                        File.WriteAllText(path, content);
                        return Path.GetFileName(path);
                    }
                    else
                    {
                        return Path.GetFileName(Path.ChangeExtension(latest, ".sql"));
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        string ISqlLiteService.GetVersion(string rootPath, int version)
        {
            throw new NotImplementedException();
        }

        string ISqlLiteService.CreatesIndexes()
        {
            using (var scope = log.Scope("CreatesIndexes()"))
            {
                try
                {
                    var sb = new StringBuilder();
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        foreach (var table in conn.Tables())
                        {
                            var fields = conn.GetTableIndexes(table.Key);
                            foreach (var field in fields)
                            {
                                sb.AppendLine($"CREATE INDEX IX_{table}_{field} ON {table}({field})");
                                sb.AppendLine("|~|");
                            }
                        }
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        void ISqlLiteService.InstallPackage(string scripts)
        {
            using (var scope = log.Scope("InstallPackage()"))
            {
                try
                {
                    var regex = new Regex("\\|~\\|");
                    var cmds = regex.Split(scripts);
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        foreach (var cmd in cmds)
                        {
                            var sql = MigrateFromSqlLiteToSqlCache(cmd).Trim('\n', '\r');
                            conn.Execute(sql);
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

        private string MigrateFromSqlLiteToSqlCache(string cmd)
        {
            return cmd
                .Replace("julianday('now')", $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'")
                .Replace("(SELECT MAX(FragmentLikeId) + 1 FROM FragmentLikes), ", "")
                .Replace("INSERT INTO FragmentLikes (FragmentLikeId, ", "INSERT INTO FragmentLikes (")
                .Replace("IFNULL(ListenCount, 0)", "ListenCount");
        }

        string ISqlLiteService.AutoCompleteDb()
        {
            var sb = new StringBuilder();
            using (var scope = log.Scope("AutoCompleteDb()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        sb.Append("CREATE TABLE AutoComplete (Id text NOT NULL, Category text NOT NULL, Type integer NULL, Name text NOT NULL, PRIMARY KEY (Category, Id))`");
                        var domains = conn.Query("SELECT DomainId AS Id, Name FROM Domains").ToList();
                        var tags = conn.Query("SELECT TagId AS Id, Type, Name FROM Tags").ToList();
                        var tagTypes = conn.Query("SELECT Type AS Id, Description AS Name FROM TagTypes").ToList();
                        var fragmentTypes = conn.Query("SELECT Type AS Id, Description AS Name FROM FragmentTypes").ToList();
                        foreach(var item in domains)
                            sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('{item.Id}', 'DOMAIN', NULL, '{item.Name}')`");
                        foreach (var item in tags)
                            sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('{((string)item.Id.ToString()).ToUpper()}', 'TAG', {item.Type}, '{((string)item.Name).Replace("'", "''")}')`");
                        foreach (var item in tagTypes)
                            sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('{item.Id}', 'TAGTYPE', {item.Id}, '{item.Name}')`");
                        foreach (var item in fragmentTypes)
                            sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('{item.Id}', 'FRAGMENTTYPE', {item.Id}, '{item.Name}')`");

                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('0', 'SORTTYPE', NULL, 'Mais avaliados')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('1', 'SORTTYPE', NULL, 'Melhores avaliados')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('2', 'SORTTYPE', NULL, 'Piores avaliados')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('3', 'SORTTYPE', NULL, 'Não avaliados')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('4', 'SORTTYPE', NULL, 'Mais recentes')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('5', 'SORTTYPE', NULL, 'Mais antigos')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('6', 'SORTTYPE', NULL, 'Mais ouvidos')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('7', 'SORTTYPE', NULL, 'Não ouvidos')`");
                        sb.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('8', 'SORTTYPE', NULL, 'Ouvidos recentemente')`");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
            return sb.ToString();
        }

    }
}
