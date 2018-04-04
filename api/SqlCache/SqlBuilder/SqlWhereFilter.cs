using Newtonsoft.Json.Linq;
using SqlCache.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlCache.Extensions;
using System.Globalization;
using System.Threading;

namespace SqlCache.SqlBuilder
{
    internal class SqlWhereFilter
    {

        public enum Operator
        {
            EqualTo,
            GreaterThan,
            GreaterOrEqualTo,
            LessThan,
            LessThanOrEqualTo,
            DifferentThan,
            Contains,
            In
        }

        internal SqlWhereFilter(string filter)
        {
            var phase = "FIELD";
            if (filter.ToLower().EndsWith(" and")) filter = filter.Substring(0, filter.Length - 4);
            var buffer = new StringBuilder();
            foreach (var chr in filter)
            {
                buffer.Append(chr);
                if (buffer.ToString().EndsWith(" = ") ||
                    buffer.ToString().EndsWith(" >= ") ||
                    buffer.ToString().EndsWith(" <= ") ||
                    buffer.ToString().EndsWith(" > ") ||
                    buffer.ToString().EndsWith(" < ") ||
                    buffer.ToString().EndsWith(" <> ") ||
                    buffer.ToString().EndsWith(" != ") ||
                    buffer.ToString().EndsWith(" LIKE ") ||
                    buffer.ToString().EndsWith(" IN "))
                {
                    if (buffer.ToString().EndsWith(" = "))
                    {
                        this.Op = Operator.EqualTo;
                        buffer.Remove(buffer.Length - 3, 3);
                    }
                    else if (buffer.ToString().EndsWith(" > "))
                    {
                        this.Op = Operator.GreaterThan;
                        buffer.Remove(buffer.Length - 3, 3);
                    }
                    else if (buffer.ToString().EndsWith(" < "))
                    {
                        this.Op = Operator.LessThan;
                        buffer.Remove(buffer.Length - 3, 3);
                    }
                    else if (buffer.ToString().EndsWith(" >= "))
                    {
                        this.Op = Operator.GreaterOrEqualTo;
                        buffer.Remove(buffer.Length - 4, 4);
                    }
                    else if (buffer.ToString().EndsWith(" <= "))
                    {
                        this.Op = Operator.LessThanOrEqualTo;
                        buffer.Remove(buffer.Length - 4, 4);
                    }
                    else if (buffer.ToString().EndsWith(" != "))
                    {
                        this.Op = Operator.DifferentThan;
                        buffer.Remove(buffer.Length - 4, 4);
                    }
                    else if (buffer.ToString().EndsWith(" <> "))
                    {
                        this.Op = Operator.DifferentThan;
                        buffer.Remove(buffer.Length - 4, 4);
                    }
                    else if (buffer.ToString().EndsWith(" IN "))
                    {
                        this.Op = Operator.In;
                        buffer.Remove(buffer.Length - 4, 4);
                    }
                    else if (buffer.ToString().EndsWith(" LIKE "))
                    {
                        this.Op = Operator.Contains;
                        buffer.Remove(buffer.Length - 6, 6);
                    }
                    this.Field = buffer.ToString().Trim();
                    phase = "VALUE";
                    buffer.Clear();
                }
            }
            if (phase == "VALUE")
            {
                if (buffer.ToString().StartsWith("'") && buffer.ToString().EndsWith("'"))
                {
                    if (buffer.Length == 19 &&
                        buffer.ToString().Contains(':') &&
                        buffer.ToString().Contains('-') &&
                        buffer.ToString().Contains(' '))
                    {
                        this.Value = DateTime.Parse(buffer.ToString().Trim(' ', '\''));
                    }
                    else
                    {
                        this.Value = buffer.ToString().Trim(' ', '\'');
                    }
                }
                else
                {
                    if (buffer.ToString().Contains('.'))
                    {
                        this.Value = decimal.Parse(buffer.ToString());
                    }
                    else
                    {
                        var value = buffer.ToString().Trim(' ');
                        if (value.StartsWith("(") && value.EndsWith(")"))
                        {
                            this.InValues = new List<dynamic>();
                            var values = value.Substring(1, value.Length - 2).Split(',');
                            foreach (var vlr in values)
                            {
                                var str = vlr.Trim();
                                if (str.StartsWith("'") && str.EndsWith("'"))
                                {
                                    this.InValues.Add(str.Substring(1, str.Length - 2));
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(str))
                                    {
                                        if(str.Contains('-'))
                                        {
                                            this.InValues.Add(str);
                                        }
                                        else
                                        {
                                            this.InValues.Add(long.Parse(str));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (buffer.ToString() == "NULL")
                                this.Value = "#NULL#";
                            else
                                this.Value = long.Parse(buffer.ToString());
                        }
                    }
                }
            }
        }

        internal bool HasMatch(DataTable dt, object data)
        {
            object sourceValue = null;
            if (data.GetType().Name == "DataRow")
            {
                var info = data.GetType().GetProperty(this.Field);
                sourceValue = info.GetValue(data, null);
            }
            else if (data is JObject)
            {
                var obj = data as JObject;
                if (this.Value != null)
                {
                    if (this.Value is string)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<string>() : string.Empty;
                    else if (this.Value is long)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<long>() : default(long);
                    else if (this.Value is decimal)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<decimal>() : default(decimal);
                    else if (this.Value is DateTime)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<DateTime>() : default(DateTime);
                }
                else
                {
                    if (this.InValues.FirstOrDefault() is string)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<string>() : string.Empty;
                    else if (this.InValues.FirstOrDefault() is long)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<long>() : default(long);
                    else if (this.InValues.FirstOrDefault() is decimal)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<decimal>() : default(decimal);
                    else if (this.InValues.FirstOrDefault() is DateTime)
                        sourceValue = !obj[this.Field].IsNullOrEmpty() ? obj[this.Field].Value<DateTime>() : default(DateTime);
                }
            }
            else
            {
                var info = data.GetType().GetProperty(this.Field);
                sourceValue = info.GetValue(data, null);
            }
            if (this.Op == Operator.EqualTo)
            {
                if (sourceValue != null && this.Value != null)
                {
                    return sourceValue.ToString() == this.Value.ToString();
                }
                else
                {
                    return sourceValue == this.Value;
                }
            }
            else if (this.Op == Operator.DifferentThan)
            {
                return sourceValue != this.Value;
            }
            else if (this.Op == Operator.Contains)
            {
                if (sourceValue == null) return false;
                var value = (string)this.Value;
                if (value.First() == '%' && value.Last() == '%')
                    return Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(sourceValue.ToString(), value.Substring(1, value.Length - 2), CompareOptions.IgnoreCase) >= 0;
                else if (value.First() == '%' && value.Last() != '%')
                    return sourceValue.ToString().EndsWith(value.Substring(1, value.Length - 1));
                else if (value.First() != '%' && value.Last() == '%')
                    return Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(sourceValue.ToString(), value.Substring(0, value.Length - 1), CompareOptions.IgnoreCase) == 0;
            }
            else if (this.Op == Operator.In && this.InValues != null)
            {
                return this.InValues.Contains(sourceValue);
            }
            else if (this.Op == Operator.GreaterThan)
            {
                return sourceValue > this.Value;
            }
            else if (this.Op == Operator.GreaterOrEqualTo)
            {
                return sourceValue >= this.Value;
            }
            else if (this.Op == Operator.LessThan)
            {
                return sourceValue < this.Value;
            }
            else if (this.Op == Operator.LessThanOrEqualTo)
            {
                return sourceValue <= this.Value;
            }
            return true;
        }

        internal string Field { get; set; }

        internal Operator Op { get; set; }

        internal dynamic Value { get; set; }

        internal List<dynamic> InValues { get; set; }

    }
}
