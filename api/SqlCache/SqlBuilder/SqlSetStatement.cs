using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlCache.SqlBuilder
{
    internal class SqlSetStatement
    {

        internal SqlSetStatement(string statement)
        {
            var phase = "FIELD";
            var buffer = new StringBuilder();
            foreach (var chr in statement)
            {
                buffer.Append(chr);
                if (buffer.ToString().EndsWith(" = "))
                {
                    if (buffer.ToString().EndsWith(" = "))
                    {
                        buffer.Remove(buffer.Length - 3, 3);
                    }
                    this.Field = buffer.ToString().Trim();
                    phase = "VALUE";
                    buffer.Clear();
                }
            }
            if (phase == "VALUE")
            {
                // 22:23:23 2015-12-23
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
                        var value = buffer.ToString();
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
                                    this.InValues.Add(long.Parse(str));
                                }
                            }
                        }
                        else
                        {
                            if(buffer.ToString().Contains("+") || buffer.ToString().Contains("-"))
                            {
                                this.ValueExpression = buffer.ToString();
                            }
                            else
                            {
                                this.Value = long.Parse(buffer.ToString());
                            }
                        }
                    }
                }
            }
        }

        public string ValueExpression { get; private set; }

        internal string Field { get; private set; }

        internal dynamic Value { get; set; }

        internal List<dynamic> InValues { get; private set; }

    }
}
