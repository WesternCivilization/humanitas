using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Humanitas.Services
{
    public class TerminalService : ITerminalService
    {

        private Logger log = new Logger(typeof(TerminalService));
        private AppConfiguration _config = null;
        private IUserAccessService _userService;

        public TerminalService(AppConfiguration config, IUserAccessService userService)
        {
            this._config = config;
            this._userService = userService;
        }

        void ITerminalService.Run(string cmds, string token, out string html, out string sql)
        {
            var htmlOutput = new StringBuilder();
            var sqlOutput = new StringBuilder();
            var lines = Regex.Split(cmds, "\n");
            var keyValues = new Dictionary<string, string>();
            bool doubleQuoteOpen = false;
            var quoteBuffer = new StringBuilder();
            var author = string.Empty;
            var quoteNum = 1;
            var userId = this._userService.TokenToUserId(token);
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentOutOfRangeException($"Invalid user token '{token}'.");
            using (var conn = new SqlCache.SqlCacheConnection(this._config.ConnectionString))
            {
                conn.SqlExecuted = (data, cmd) =>
                {
                    if (cmd.Contains("INSERT INTO Tags"))
                    {
                        sqlOutput.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('{((string)data.TagId.ToString()).ToUpper()}', 'TAG', {data.Type}, '{((string)data.Name).Replace("'", "''")}')`");
                    }
                    else if (cmd.Contains("UPDATE Tags"))
                    {
                        sqlOutput.Append($"UPDATE AutoComplete SET Type = {data.Type}, Name = '{((string)data.Name).Replace("'", "''")}' WHERE Id = '{((string)data.TagId.ToString()).ToUpper()}'`");
                    }
                };
                foreach (var line in lines)
                {
                    if (!doubleQuoteOpen && line.StartsWith("Tópico: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(8).TrimEnd('\r');
                        tag.Type = 7;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("Habilidade: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(12).TrimEnd('\r');
                        tag.Type = 10;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("Livro: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(7).TrimEnd('\r');
                        tag.Type = 4;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("Autor: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(7).TrimEnd('\r');
                        tag.Type = 2;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("Período: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(9).TrimEnd('\r');
                        tag.Type = 1;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("Área: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(6).TrimEnd('\r');
                        tag.Type = 0;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("Instituição: "))
                    {
                        var tag = conn.NewRow("Tags");
                        tag.TagId = Guid.NewGuid().ToString().ToUpper();
                        tag.Name = line.Substring(13).TrimEnd('\r');
                        tag.Type = 3;
                        tag.DomainId = 41;
                        tag.CreatedAt = DateTime.Now;
                        tag.CreatedBy = userId;
                        tag.UpdatedAt = DateTime.Now;
                        tag.UpdatedBy = userId;
                        var id = conn.Save("Tags", tag);

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.TagId = id;
                        act.DomainId = tag.DomainId;
                        act.CreatedAt = tag.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />");
                    }
                    else if (!doubleQuoteOpen && line.StartsWith("\""))
                    {
                        doubleQuoteOpen = !line.Substring(1).TrimEnd('\r').EndsWith("\"");
                        quoteBuffer.AppendLine(line.Substring(1).TrimEnd('\r', '"'));
                    }
                    else if (doubleQuoteOpen)
                    {
                        quoteBuffer.AppendLine(line.Trim('"'));
                        if (line.Trim().EndsWith("\""))
                        {
                            doubleQuoteOpen = false;
                        }
                    }
                    else if (!doubleQuoteOpen && quoteBuffer.Length > 0 && !string.IsNullOrEmpty(line.Trim()))
                    {
                        author = line.Trim().TrimEnd('\r');
                    }
                    else if(!string.IsNullOrEmpty(line.Trim().TrimEnd('\r')))
                    {
                        throw new ArgumentOutOfRangeException($"Invalid line '{line.TrimEnd('\r')}'");
                    }

                    if (quoteBuffer.Length > 0 &&
                        !string.IsNullOrEmpty(author))
                    {
                        var quote = conn.NewRow("Fragments");
                        quote.FragmentId = Guid.NewGuid().ToString().ToUpper();
                        quote.Type = 0;
                        quote.Title = $"Fragment {quoteNum}";
                        quote.Content = quoteBuffer.ToString();
                        quote.Author = author;
                        quote.DomainId = 41;
                        quote.CreatedAt = DateTime.Now;
                        quote.CreatedBy = userId;
                        quote.UpdatedAt = DateTime.Now;
                        quote.UpdatedBy = userId;
                        var id = conn.Save("Fragments", quote);
                        quoteNum++;
                        quoteBuffer = new StringBuilder();
                        author = string.Empty;

                        var act = conn.NewRow("Activities");
                        act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                        act.UserId = userId;
                        act.FragmentId = id;
                        act.DomainId = quote.DomainId;
                        act.CreatedAt = quote.CreatedAt;
                        conn.Save("Activities", act);

                        htmlOutput.AppendLine($"Fragmento <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-fragment/{quote.FragmentId}' target='_blank'>{quote.Title} ({quote.Author})</a> adicionado.<br />");

                    }
                }
            }
            html = htmlOutput.ToString();
            sql = sqlOutput.ToString();
        }

        string ITerminalService.SaveObject(Dictionary<string, string> keyValues, string token)
        {
            var sb = new StringBuilder();
            using (var scope = log.Scope("SaveObject()"))
            {
                try
                {
                    using (var conn = new SqlCache.SqlCacheConnection(this._config.ConnectionString))
                    {
                        if (keyValues.ContainsKey("Nome"))
                        {
                            var tag = conn.NewRow("Tags");
                            var act = conn.NewRow("Activities");
                            tag.DomainId = 41;
                            tag.CreatedAt = DateTime.Now;
                            tag.CreatedBy = this._userService.TokenToUserId(token);
                            tag.UpdatedAt = DateTime.Now;
                            tag.UpdatedBy = this._userService.TokenToUserId(token);

                            tag.TagId = Guid.NewGuid().ToString().ToUpper();
                            act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                            act.TagId = tag.TagId;
                            if (keyValues.ContainsKey("Nome")) tag.Name = keyValues["Nome"];
                            if (keyValues.ContainsKey("TipoTag")) tag.Type = conn.ExecuteScalar<int>($"SELECT Type FROM TagTypes WHERE Description = '{keyValues["TipoTag"]}'");
                            else tag.Type = conn.ExecuteScalar<int>($"SELECT Type FROM TagTypes WHERE Description = 'Tópico'");
                            if (keyValues.ContainsKey("Texto")) tag.Text = keyValues["Texto"];
                            if (keyValues.ContainsKey("Nascimento"))
                            {

                            }
                            if (keyValues.ContainsKey("Morte"))
                            {

                            }

                            if (keyValues.ContainsKey("Livro"))
                            {
                                tag.BookId = conn.ExecuteScalar<string>($"SELECT TagId FROM Tags WHERE Type = 4 AND Name LIKE '%{keyValues["Livro"]}%' ORDER BY Contains(Name='{keyValues["Livro"]}')");
                                act.BookId = tag.BookId;
                            }
                            if (keyValues.ContainsKey("Domínio"))
                            {
                                tag.DomainId = conn.ExecuteScalar<int>($"SELECT DomainId FROM Domains WHERE Name LIKE '%{keyValues["Domínio"]}%' ORDER BY Contains(Name='{keyValues["Domínio"]}')");
                                act.DomainId = tag.DomainId;
                            }
                            if (keyValues.ContainsKey("Dentro de"))
                            {
                                tag.ParentId = conn.ExecuteScalar<string>($"SELECT TagId FROM Tags WHERE Name LIKE '%{keyValues["Dentro de"]}%' ORDER BY Contains(Name='{keyValues["Dentro de"]}')");
                                act.ParentId = tag.ParentId;
                            }
                            if (keyValues.ContainsKey("Pessoas"))
                            {
                                var persons = keyValues["Pessoas"].Split(';');
                                var index = 1;
                                foreach (var person in persons)
                                {
                                    if (!string.IsNullOrEmpty(person))
                                    {
                                        var sql = $"SELECT TagId FROM Tags WHERE Type = 2 AND Name LIKE '%{person}%' ORDER BY Contains(Name='{person}')";
                                        var personId = conn.ExecuteScalar<string>(sql);
                                        if (string.IsNullOrEmpty(personId))
                                        {
                                            var tagValues = new Dictionary<string, string>();
                                            tagValues["Nome"] = person;
                                            tagValues["TipoTag"] = "Autor";
                                            sb.AppendLine(((ITerminalService)this).SaveObject(tagValues, token));
                                            personId = conn.ExecuteScalar<string>(sql);
                                        }
                                        if (index == 1)
                                        {
                                            tag.PersonId1 = personId;
                                            act.PersonId1 = tag.PersonId1;
                                        }
                                        else if (index == 2)
                                        {
                                            tag.PersonId2 = personId;
                                            act.PersonId2 = tag.PersonId2;
                                        }
                                        else if (index == 3)
                                        {
                                            tag.PersonId3 = personId;
                                            act.PersonId3 = tag.PersonId3;
                                        }
                                        else if (index == 4)
                                        {
                                            tag.PersonId4 = personId;
                                            act.PersonId4 = tag.PersonId4;
                                        }
                                        else if (index == 5)
                                        {
                                            tag.PersonId5 = personId;
                                            act.PersonId5 = tag.PersonId5;
                                        }
                                        index++;
                                    }
                                }
                            }
                            if (keyValues.ContainsKey("Tópicos"))
                            {
                                var topics = keyValues["Tópicos"].Split(';');
                                var index = 1;
                                foreach (var topic in topics)
                                {
                                    if (!string.IsNullOrEmpty(topic))
                                    {
                                        var sql = $"SELECT TagId FROM Tags WHERE Type = 7 AND Name LIKE '%{topic}%' ORDER BY Contains(Name='{topic}')";
                                        var topicId = conn.ExecuteScalar<string>(sql);
                                        if (string.IsNullOrEmpty(topicId))
                                        {
                                            var tagValues = new Dictionary<string, string>();
                                            tagValues["Nome"] = topic;
                                            tagValues["TipoTag"] = "Tópico";
                                            sb.AppendLine(((ITerminalService)this).SaveObject(tagValues, token));
                                            topicId = conn.ExecuteScalar<string>(sql);
                                        }
                                        if (index == 1)
                                        {
                                            tag.TopicId1 = topicId;
                                            act.TopicId1 = tag.TopicId1;
                                        }
                                        else if (index == 2)
                                        {
                                            tag.TopicId2 = topicId;
                                            act.TopicId2 = tag.TopicId2;
                                        }
                                        else if (index == 3)
                                        {
                                            tag.TopicId3 = topicId;
                                            act.TopicId3 = tag.TopicId3;
                                        }
                                        else if (index == 4)
                                        {
                                            tag.TopicId4 = topicId;
                                            act.TopicId4 = tag.TopicId4;
                                        }
                                        else if (index == 5)
                                        {
                                            tag.TopicId5 = topicId;
                                            act.TopicId5 = tag.TopicId5;
                                        }
                                        index++;
                                    }
                                }
                            }

                            act.DomainId = tag.DomainId;
                            act.ParentId = tag.ParentId;
                            act.CreatedAt = tag.CreatedAt;
                            act.CreatedBy = tag.CreatedBy;
                            act.UpdatedAt = tag.UpdatedAt;
                            act.UpdatedBy = tag.UpdatedBy;

                            conn.Save("Tags", tag);
                            conn.Save("Activities", act);

                            return sb.ToString() + $"Tag <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-tag/{tag.TagId}' target='_blank'>{tag.Name}</a> adicionada.<br />";

                        }
                        if (keyValues.ContainsKey("Título"))
                        {
                            var fragment = conn.NewRow("Fragments");
                            var act = conn.NewRow("Activities");
                            fragment.DomainId = 41;
                            fragment.CreatedAt = DateTime.Now;
                            fragment.CreatedBy = this._userService.TokenToUserId(token);
                            fragment.UpdatedAt = DateTime.Now;
                            fragment.UpdatedBy = this._userService.TokenToUserId(token);

                            fragment.FragmentId = Guid.NewGuid().ToString().ToUpper();
                            act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                            act.FragmentId = fragment.FragmentId;
                            if (keyValues.ContainsKey("Título")) fragment.Title = keyValues["Título"];
                            if (keyValues.ContainsKey("TipoFragmento")) fragment.Type = conn.ExecuteScalar<int>($"SELECT Type FROM FragmentTypes WHERE Description = '{keyValues["TipoFragmento"]}'");
                            else fragment.Type = conn.ExecuteScalar<int>($"SELECT Type FROM FragmentTypes WHERE Description = 'Citação'");
                            if (keyValues.ContainsKey("Texto")) fragment.Content = keyValues["Texto"];
                            if (keyValues.ContainsKey("Autor")) fragment.Author = keyValues["Autor"];
                            if (keyValues.ContainsKey("Livro"))
                            {
                                fragment.BookId = conn.ExecuteScalar<string>($"SELECT TagId FROM Tags WHERE Type = 4 AND Name LIKE '%{keyValues["Livro"]}%' ORDER BY Contains(Name='{keyValues["Livro"]}')");
                                act.BookId = fragment.BookId;
                            }
                            if (keyValues.ContainsKey("Domínio"))
                            {
                                fragment.DomainId = conn.ExecuteScalar<int>($"SELECT DomainId FROM Domains WHERE Name LIKE '%{keyValues["Domínio"]}%' ORDER BY Contains(Name='{keyValues["Domínio"]}')");
                                act.DomainId = fragment.DomainId;
                            }
                            if (keyValues.ContainsKey("Dentro de"))
                            {
                                fragment.ParentId = conn.ExecuteScalar<string>($"SELECT TagId FROM Tags WHERE Name LIKE '%{keyValues["Dentro de"]}%' ORDER BY Contains(Name='{keyValues["Dentro de"]}')");
                                act.ParentId = fragment.ParentId;
                            }
                            if (keyValues.ContainsKey("Pessoas"))
                            {
                                var persons = keyValues["Pessoas"].Split(';');
                                var index = 1;
                                foreach (var person in persons)
                                {
                                    if (!string.IsNullOrEmpty(person))
                                    {
                                        var sql = $"SELECT TagId FROM Tags WHERE Type = 2 AND Name LIKE '%{person}%' ORDER BY Contains(Name='{person}')";
                                        var personId = conn.ExecuteScalar<string>(sql);
                                        if (string.IsNullOrEmpty(personId))
                                        {
                                            var tagValues = new Dictionary<string, string>();
                                            tagValues["Nome"] = person;
                                            tagValues["TipoTag"] = "Autor";
                                            sb.AppendLine(((ITerminalService)this).SaveObject(tagValues, token));
                                            personId = conn.ExecuteScalar<string>(sql);
                                        }
                                        if (index == 1)
                                        {
                                            fragment.PersonId1 = personId;
                                            act.PersonId1 = fragment.PersonId1;
                                        }
                                        else if (index == 2)
                                        {
                                            fragment.PersonId2 = personId;
                                            act.PersonId2 = fragment.PersonId2;
                                        }
                                        else if (index == 3)
                                        {
                                            fragment.PersonId3 = personId;
                                            act.PersonId3 = fragment.PersonId3;
                                        }
                                        else if (index == 4)
                                        {
                                            fragment.PersonId4 = personId;
                                            act.PersonId4 = fragment.PersonId4;
                                        }
                                        else if (index == 5)
                                        {
                                            fragment.PersonId5 = personId;
                                            act.PersonId5 = fragment.PersonId5;
                                        }
                                        index++;
                                    }
                                }
                            }
                            if (keyValues.ContainsKey("Tópicos"))
                            {
                                var topics = keyValues["Tópicos"].Split(';');
                                var index = 1;
                                foreach (var topic in topics)
                                {
                                    if (!string.IsNullOrEmpty(topic))
                                    {
                                        var sql = $"SELECT TagId FROM Tags WHERE Type = 7 AND Name LIKE '%{topic}%' ORDER BY Contains(Name='{topic}')";
                                        var topicId = conn.ExecuteScalar<string>(sql);
                                        if (string.IsNullOrEmpty(topicId))
                                        {
                                            var tagValues = new Dictionary<string, string>();
                                            tagValues["Nome"] = topic;
                                            tagValues["TipoTag"] = "Tópico";
                                            sb.AppendLine(((ITerminalService)this).SaveObject(tagValues, token));
                                            topicId = conn.ExecuteScalar<string>(sql);
                                        }
                                        if (index == 1)
                                        {
                                            fragment.TopicId1 = topicId;
                                            act.TopicId1 = fragment.TopicId1;
                                        }
                                        else if (index == 2)
                                        {
                                            fragment.TopicId2 = topicId;
                                            act.TopicId2 = fragment.TopicId2;
                                        }
                                        else if (index == 3)
                                        {
                                            fragment.TopicId3 = topicId;
                                            act.TopicId3 = fragment.TopicId3;
                                        }
                                        else if (index == 4)
                                        {
                                            fragment.TopicId4 = topicId;
                                            act.TopicId4 = fragment.TopicId4;
                                        }
                                        else if (index == 5)
                                        {
                                            fragment.TopicId5 = topicId;
                                            act.TopicId5 = fragment.TopicId5;
                                        }
                                        index++;
                                    }
                                }
                            }
                            if (keyValues.ContainsKey("Página")) fragment.Page = keyValues["Página"];
                            if (keyValues.ContainsKey("Referência")) fragment.Reference = keyValues["Referência"];

                            act.DomainId = fragment.DomainId;
                            act.ParentId = fragment.ParentId;
                            act.CreatedAt = fragment.CreatedAt;
                            act.UserId = fragment.CreatedBy;

                            conn.Save("Fragments", fragment);
                            conn.Save("Activities", act);

                            return sb.ToString() + $"Fragmento <a href='http://rafaelmelo.web1612.kinghost.net/humanitas/#/edit-fragment/{fragment.FragmentId}' target='_blank'>{fragment.Title} ({fragment.Author})</a> adicionado.<br />";

                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

    }
}
