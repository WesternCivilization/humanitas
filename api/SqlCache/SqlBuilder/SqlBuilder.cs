using Newtonsoft.Json.Linq;
using SqlCache.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlCache.SqlBuilder
{
    internal class SqlBuilder
    {

        internal SqlBuilder(string sql)
        {
            this.SelectedFields = new List<SqlSelect>();
            this.InsertFields = new List<string>();
            this.InsertValues = new List<object>();
            this.SetStatements = new List<SqlSetStatement>();
            this.FromTables = new List<string>();
            this.WhereFilters = new List<SqlWhereFilter>();
            this.OrderFields = new List<string>();

            var phase = "NONE";
            var buffer = new StringBuilder();
            foreach (var chr in sql)
            {
                var value = chr;
                if (value == '\r' || value == '\t' || value == '\n') value = ' ';
                buffer.Append(value);
                if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "select ")
                {
                    phase = "SELECT";
                    buffer.Clear();
                    this.Method = "SELECT";
                }
                else if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "summarize ")
                {
                    phase = "SUMMARIZE";
                    buffer.Clear();
                    this.Method = "SUMMARIZE";
                }
                else if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "update ")
                {
                    phase = "UPDATE";
                    buffer.Clear();
                    this.Method = "UPDATE";
                }
                else if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "history ")
                {
                    phase = "HISTORY";
                    buffer.Clear();
                    buffer.Append(' ');
                    this.Method = "HISTORY";
                }
                else if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "insert into ")
                {
                    phase = "INSERT";
                    buffer.Clear();
                    this.Method = "INSERT";
                }
                else if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "rollback ")
                {
                    phase = "ROLLBACK";
                    buffer.Clear();
                    buffer.Append(' ');
                    this.Method = "ROLLBACK";
                }
                else if (phase == "NONE" &&
                    buffer.ToString().ToLower() == "delete ")
                {
                    phase = "DELETE";
                    buffer.Clear();
                    buffer.Append(' ');
                    this.Method = "DELETE";
                }
                else if ((phase == "SELECT" || phase == "DELETE" ||
                    phase == "HISTORY" || phase == "ROLLBACK") &&
                    buffer.ToString().ToLower().EndsWith(" from "))
                {
                    if (phase == "SELECT")
                    {
                        var field = buffer.ToString().Substring(0, buffer.Length - 6).Trim(',', ' ');
                        if (field.ToUpper().StartsWith("TOP "))
                        {
                            var topValue = int.Parse(field.Split(' ')[1]);
                            this.TopCount = topValue;
                            field = string.Join(" ", field.Split(' ').Skip(2)).TrimEnd(',');
                        }
                        this.SelectedFields.Add(new SqlSelect(field.Trim(' ', ',')));
                    }
                    phase = "FROM";
                    buffer.Clear();
                }
                else if ((phase == "FROM" || phase == "SET") &&
                    buffer.ToString().ToLower().EndsWith(" where "))
                {
                    if (phase == "FROM")
                        this.FromTables.Add(buffer.ToString().Substring(0, buffer.Length - 7).Trim());
                    if (phase == "SET")
                        this.SetStatements.Add(new SqlSetStatement(buffer.ToString().Substring(0, buffer.Length - 7).Trim(',', ' ')));
                    phase = "WHERE";
                    buffer.Clear();
                }
                else if (phase == "INSERT_VALUES_PENDING" &&
                    buffer.ToString().ToLower().Contains("values") &&
                    buffer.ToString().Contains("("))
                {
                    phase = "INSERT_VALUES";
                    buffer.Clear();
                }
                else if (phase == "UPDATE" &&
                    buffer.ToString().ToLower().EndsWith(" set "))
                {
                    if (phase == "UPDATE")
                        this.FromTables.Add(buffer.ToString().Substring(0, buffer.Length - 5).Trim());
                    phase = "SET";
                    buffer.Clear();
                }
                else if (phase == "SUMMARIZE" &&
                    buffer.ToString().ToLower().EndsWith("("))
                {
                    this.SelectedFields.Add(new SqlSelect(buffer.ToString().Substring(0, buffer.Length - 1).Trim(',', ' ')));
                    buffer.Clear();
                    phase = "SUMMARIZE_FIELDS";
                }
                else if (phase == "INSERT" &&
                    buffer.ToString().ToLower().EndsWith("("))
                {
                    if (phase == "INSERT")
                        this.FromTables.Add(buffer.ToString().Trim(' ', '('));
                    phase = "INSERT_FIELDS";
                    buffer.Clear();
                }
                else if ((phase == "FROM" || phase == "WHERE") &&
                    buffer.ToString().ToLower().EndsWith(" order by "))
                {
                    if (phase == "FROM")
                        this.FromTables.Add(buffer.ToString().Substring(0, buffer.Length - 10).Trim());
                    else if (phase == "WHERE")
                        this.WhereFilters.Add(new SqlWhereFilter(buffer.ToString().Substring(0, buffer.Length - 10)));
                    phase = "ORDER BY";
                    buffer.Clear();
                }
                else if (chr == ',' && phase == "SELECT")
                {
                    var field = buffer.ToString();
                    if (field.ToUpper().StartsWith("TOP "))
                    {
                        var topValue = int.Parse(field.Split(' ')[1]);
                        this.TopCount = topValue;
                        field = string.Join(" ", field.Split(' ').Skip(2)).TrimEnd(',');
                    }
                    this.SelectedFields.Add(new SqlSelect(field.Trim(' ', ',')));
                    buffer.Clear();
                }
                else if ((chr == ',' || chr == ')') && phase == "SUMMARIZE_FIELDS")
                {
                    var field = buffer.ToString();
                    this.SelectedFields.Add(new SqlSelect(buffer.ToString().Trim(',', ' ', ')')));
                    if (chr == ')') phase = "SELECT";
                    buffer.Clear();
                }
                //
                else if ((chr == ',' || chr == ')') && phase == "INSERT_FIELDS")
                {
                    var field = buffer.ToString();
                    this.InsertFields.Add(buffer.ToString().Trim(',', ' ', ')'));
                    if (chr == ')') phase = "INSERT_VALUES_PENDING";
                    buffer.Clear();
                }
                else if ((!buffer.ToString().Trim().StartsWith("'") &&
                    (chr == ',' || chr == ')') ||
                    buffer.ToString().EndsWith("',") ||
                    buffer.ToString().EndsWith("')")) &&
                    phase == "INSERT_VALUES")
                {
                    var vlr = buffer.ToString().Trim(' ', ',', ')');

                    if (vlr.StartsWith("'") && vlr.EndsWith("'"))
                    {
                        if (vlr.Length == 19 &&
                            vlr.Contains(':') &&
                            vlr.Contains('-') &&
                            vlr.Contains(' '))
                        {
                            this.InsertValues.Add(DateTime.Parse(vlr.Trim(' ', '\'')));
                        }
                        else
                        {
                            this.InsertValues.Add(vlr.Trim(' ', '\''));
                        }
                    }
                    else
                    {
                        if (vlr.Contains('.'))
                        {
                            this.InsertValues.Add(decimal.Parse(vlr));
                        }
                        else
                        {
                            if (vlr.StartsWith("(") && vlr.EndsWith(")"))
                            {
                                //LIST OF values
                            }
                            else
                            {
                                if (vlr.ToUpper() != "NULL")
                                {
                                    this.InsertValues.Add(long.Parse(vlr));
                                }
                                else
                                {
                                    this.InsertValues.Add("NULL");
                                }
                            }
                        }
                    }
                    if (chr == ')') phase = "NONE";
                    buffer.Clear();
                }
                else if (chr == ',' && phase == "UPDATE")
                {
                    var field = buffer.ToString();
                    this.FromTables.Add(buffer.ToString().Trim(',', ' '));
                    buffer.Clear();
                }
                else if (chr == ',' && phase == "FROM")
                {
                    var field = buffer.ToString();
                    this.FromTables.Add(buffer.ToString().Trim());
                    buffer.Clear();
                }
                else if (chr == ',' && phase == "ORDER BY")
                {
                    var field = buffer.ToString();
                    this.OrderFields.Add(buffer.ToString().Trim());
                    buffer.Clear();
                }
                else if (phase == "WHERE" &&
                    (buffer.ToString().EndsWith(" AND ") || buffer.ToString().EndsWith(" OR ")))
                {
                    var filter = buffer.ToString().Trim();
                    this.WhereFilters.Add(new SqlWhereFilter(filter));
                    buffer.Clear();
                }
                else if (phase == "SET" &&
                    ((buffer.ToString().Split('=').Last().TrimStart().StartsWith("'") &&
                    buffer.ToString().EndsWith("', ")) ||
                    (!buffer.ToString().Split('=').Last().TrimStart().StartsWith("'") &&
                    buffer.ToString().EndsWith(", "))))
                {
                    var statement = buffer.ToString().Trim();
                    this.SetStatements.Add(new SqlSetStatement(statement.Trim(',', ' ')));
                    buffer.Clear();
                }
            }
            if (phase == "SELECT")
                this.SelectedFields.Add(new SqlSelect(buffer.ToString().Trim(',', ' ')));
            if (phase == "FROM")
                this.FromTables.Add(buffer.ToString());
            else if (phase == "WHERE")
                this.WhereFilters.Add(new SqlWhereFilter(buffer.ToString().Trim()));
            else if (phase == "ORDER BY")
                this.OrderFields.Add(buffer.ToString().Trim());
            buffer.Clear();
            if (this.SelectedFields.Count == 1 && this.SelectedFields[0].Field == "*")
            {
                this.SelectAllFields = true;
            }
        }

        public string ToNativeSql(string databaseType)
        {
            var sb = new StringBuilder();
            foreach (var filter in this.WhereFilters)
            {
                if (filter.Field.StartsWith("?"))
                {
                    if (databaseType.ToLower() == "mysql" || databaseType.ToLower() == "mssql")
                    {
                        filter.Field = filter.Field.Substring(1);
                        string expression = filter.Value;
                        if (expression.StartsWith("@FROM("))
                        {
                            expression = expression.Substring(6).TrimEnd(')');
                            var parts = expression.Split(',');
                            var whereParts = parts.Last().Split('=');
                            filter.Value = $"SELECT {parts.ElementAtOrDefault(0)} FROM {parts.ElementAtOrDefault(1)} WHERE {whereParts.ElementAtOrDefault(0)} = '{whereParts.ElementAtOrDefault(1)}'";
                        }
                        else
                            throw new ArgumentException(expression);
                    }
                }
                else if (filter.Field.StartsWith("@"))
                {
                    if (databaseType.ToLower() == "mysql" || databaseType.ToLower() == "mssql")
                    {
                        filter.Field = filter.Field.Substring(1);
                        if (filter.Field == "Tags")
                        {
                            filter.Field = "ParentId,AreaId,BookId,InstitutionId1,InstitutionId2,LawId,PeriodId,PersonId1,PersonId2,PersonId3,PersonId4,PersonId5,SkillId1,SkillId2,SkillId3,SkillId4,SkillId5,StateId1,StateId2,TopicId1,TopicId2,TopicId3,TopicId4,TopicId5";
                        }
                    }
                }
            }
            var containsSortNum = string.Empty;
            foreach(var sort in this.OrderFields)
            {
                if(sort.Contains("Contains("))
                {
                    var sortNum = sort.Substring(9).TrimEnd(')');
                    var parts = sortNum.Split('=');
                    containsSortNum = $@"CASE 
		WHEN Name LIKE '{parts.Last().Trim('\'')}%' THEN 1
        WHEN Name LIKE '%{parts.Last().Trim('\'')}%' THEN 2
        ELSE 3
	END, {parts.First()}";
                }
            }
            if(!string.IsNullOrEmpty(containsSortNum))
            {
                this.OrderFields.Clear();
                this.OrderFields.Add(containsSortNum);
            }
            sb.AppendLine("SELECT ");
            if (this.SelectAllFields)
            {
                sb.AppendLine($"    *");
            }
            else
            {
                foreach (var field in this.SelectedFields)
                {
                    if (field.Field != field.Alias)
                    {
                        sb.AppendLine($"    {field.Field} AS {field.Alias}, ");
                    }
                    else
                    {
                        sb.AppendLine($"    {field.Field}, ");
                    }
                }
                sb = sb.Remove(sb.Length - 4, 4);
                sb.AppendLine();
            }
            sb.AppendLine($"FROM ");
            foreach (var from in this.FromTables)
            {
                sb.AppendLine($"    {from} ");
            }
            sb = sb.Remove(sb.Length - 2, 2);
            sb.AppendLine();

            if (this.WhereFilters.Any())
            {
                sb.AppendLine($"WHERE ");
                foreach (var filter in this.WhereFilters)
                {
                    var fields = filter.Field.Split(',');
                    if (fields.Length > 1)
                    {
                        sb.AppendLine("     (");
                        foreach (var field in fields)
                        {
                            sb.AppendLine($"    {field} {GetOp(filter)} {GetValue(filter)} OR ");
                        }
                        sb = sb.Remove(sb.Length - 5, 5);
                        sb.AppendLine("     ) AND ");
                    }
                    else
                    {
                        sb.AppendLine($"    {fields[0]} {GetOp(filter)} {GetValue(filter)} AND ");
                    }

                }
                sb = sb.Remove(sb.Length - 6, 6);
                sb.AppendLine();
            }

            if (this.OrderFields.Any())
            {
                sb.AppendLine($"ORDER BY ");
                foreach (var sort in this.OrderFields)
                {
                    sb.AppendLine($"    {sort}, ");
                }
                sb = sb.Remove(sb.Length - 4, 4);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GetValue(SqlWhereFilter filter)
        {
            if (filter.Op == SqlWhereFilter.Operator.In)
            {
                if (filter.InValues != null && filter.InValues.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var item in filter.InValues)
                    {
                        sb.Append($"'{item}',");
                    }
                    return "(" + sb.ToString().TrimEnd(',') + ")";
                }
                else
                {
                    return $"({filter.Value})";
                }
            }
            else
            {
                if(filter.Value is string)
                {
                    if(filter.Value.ToString() != "#NULL#")
                    {
                        return $"'{filter.Value}'";
                    }
                    else
                    {
                        return $"NULL";
                    }
                }
                else
                {
                    return $"{filter.Value}";
                }
            }
        }

        private string GetOp(SqlWhereFilter filter)
        {
            if (filter.Value is string && filter.Value == "#NULL#")
            {
                if(filter.Op == SqlWhereFilter.Operator.EqualTo) return "IS";
                else if (filter.Op == SqlWhereFilter.Operator.DifferentThan) return "IS NOT";
            }
            if (filter.Op == SqlWhereFilter.Operator.EqualTo) return "=";
            else if (filter.Op == SqlWhereFilter.Operator.DifferentThan) return "!=";
            else if (filter.Op == SqlWhereFilter.Operator.GreaterOrEqualTo) return ">=";
            else if (filter.Op == SqlWhereFilter.Operator.GreaterThan) return ">";
            else if (filter.Op == SqlWhereFilter.Operator.LessThanOrEqualTo) return "<=";
            else if (filter.Op == SqlWhereFilter.Operator.LessThan) return "<";
            else if (filter.Op == SqlWhereFilter.Operator.In) return "IN";
            else if (filter.Op == SqlWhereFilter.Operator.Contains) return "LIKE";
            else throw new ArgumentOutOfRangeException(filter.Op.ToString());
        }

        public bool HasMatch(DataTable dt, object data)
        {
            if (data is DataRow)
            {
                if (((DataRow)data).Id.ToString() == DataTable.DEF_ID.ToString())
                {
                    return false;
                }
            }
            var allMatch = true;
            foreach (var filter in this.WhereFilters)
            {
                if (filter.Field != null)
                {
                    allMatch = allMatch && filter.HasMatch(dt, data);
                }
            }
            return allMatch;
        }

        public bool SelectAllFields { get; set; }

        public IList<SqlSelect> SelectedFields { get; set; }

        public IList<string> InsertFields { get; set; }

        public IList<object> InsertValues { get; set; }

        public IList<string> FromTables { get; set; }

        public IList<SqlWhereFilter> WhereFilters { get; set; }

        public IList<SqlSetStatement> SetStatements { get; set; }

        public IList<string> OrderFields { get; set; }

        public string Method { get; set; }

        public int TopCount { get; set; }

        internal bool ChangeValues(ref object data)
        {
            var noChanges = true;
            foreach (var statement in this.SetStatements)
            {
                if (data is JObject)
                {
                    JToken obj;
                    if (!string.IsNullOrEmpty(statement.ValueExpression))
                        statement.Value = EvalExpression((JObject)data, statement.ValueExpression);

                    if (((JObject)data).TryGetValue(statement.Field, out obj))
                    {
                        noChanges = noChanges && (obj.ToString() == "" && statement.Value == null ||
                            obj.ToString() == (statement.Value != null ? statement.Value.ToString() : null));
                        if (!noChanges)
                            ((JObject)data)[statement.Field] = statement.Value;
                    }
                    else
                        throw new InvalidCastException($"Invalid property {statement.Field}.");
                }
                else
                {
                    var info = data.GetType().GetProperty(statement.Field);
                    if (info == null)
                        throw new InvalidCastException($"Invalid property {statement.Field}.");
                    var previousValue = info.GetValue(data, null);
                    noChanges = noChanges && (previousValue.ToString() == statement.Value.ToString());
                    if (!noChanges)
                        info.SetValue(data, statement.Value);
                }
            }
            return !noChanges;
        }

        private dynamic EvalExpression(JObject data, string exp)
        {
            if (exp.EndsWith(" + 1"))
            {
                foreach (var field in data)
                {
                    if (exp.Contains(field.Key))
                    {
                        if (data[field.Key].ToString() == "") return 1;
                        else return long.Parse(data[field.Key].ToString()) + 1;
                    }
                }
                throw new ArgumentOutOfRangeException("It could not parse the expression: " + exp);
            }
            else if (exp.EndsWith(" - 1"))
            {
                foreach (var field in data)
                {
                    if (exp.Contains(field.Key))
                    {
                        if (data[field.Key].ToString() == "") return -1;
                        else return long.Parse(data[field.Key].ToString()) - 1;
                    }
                }
                throw new ArgumentOutOfRangeException("It could not parse the expression: " + exp);
            }
            else
                throw new ArgumentOutOfRangeException("It could not parse the expression: " + exp);
        }

    }
}
