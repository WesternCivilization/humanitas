using Humanitas.Common.Extensions;
using Humanitas.Common.Helpers;
using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using Newtonsoft.Json.Linq;
using RepositoryWatcher;
using SqlCache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Humanitas.Services
{
    public class TopicaService : ITopicaService
    {

        private Logger log = new Logger(typeof(TopicaService));
        private ImageRepositoryWatcher imageProcessor = null;

        private AppConfiguration _config = null;

        public TopicaService(AppConfiguration config)
        {
            this._config = config;
            this.imageProcessor = new ImageRepositoryWatcher($@"{config.RepositoryPath}\tags\raw\*.jpg");
            this.imageProcessor.AddResizeRule($@"{config.RepositoryPath}\tags\width340", 340, 340);
            this.imageProcessor.AddResizeRule($@"{config.RepositoryPath}\tags\width160", 160, 160);
            this.imageProcessor.AddResizeRule($@"{config.RepositoryPath}\tags\width50", 50, 50);
        }

        IList<Option> ITopicaService.AutoComplete(string type, string expression, string token)
        {
            using (var scope = log.Scope("AutoComplete()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        string[] typeIds = new string[] { };
                        if (type == "all" || type == "tag")
                        {
                            typeIds = new string[] { "0", "1", "2", "3", "4", "7", "8", "9", "10", "12" };
                        }
                        else if (type == "area")
                        {
                            typeIds = new string[] { "0" };
                        }
                        else if (type == "period")
                        {
                            typeIds = new string[] { "1" };
                        }
                        else if (type == "author")
                        {
                            typeIds = new string[] { "2" };
                        }
                        else if (type == "institution")
                        {
                            typeIds = new string[] { "3" };
                        }
                        else if (type == "book")
                        {
                            typeIds = new string[] { "4" };
                        }
                        else if (type == "topic")
                        {
                            typeIds = new string[] { "7" };
                        }
                        else if (type == "law")
                        {
                            typeIds = new string[] { "8" };
                        }
                        else if (type == "state")
                        {
                            typeIds = new string[] { "9" };
                        }
                        else if (type == "skill")
                        {
                            typeIds = new string[] { "10" };
                        }
                        else if (type == "library")
                        {
                            typeIds = new string[] { "12" };
                        }
                        if (type == "domain")
                        {
                            var query = conn.Query($"SELECT TOP 10 DomainId, Name FROM Domains " +
                                                    $"WHERE Name LIKE '%{expression}%' " +
                                                    $"ORDER BY Contains(Name='{expression}')");
                            return query.Select(f => new Option { value = f.DomainId, label = f.Name }).ToList();
                        }
                        else if (type == "tagtype")
                        {
                            var query = conn.Query($"SELECT TOP 10 Type, Description FROM TagTypes " +
                                                    $"WHERE Description LIKE '%{expression}%' " +
                                                    $"ORDER BY Contains(Description='{expression}')");
                            return query.Select(f => new Option { value = f.Type, label = f.Description }).ToList();
                        }
                        else if (type == "fragmenttype")
                        {
                            var query = conn.Query($"SELECT TOP 10 Type, Description FROM FragmentTypes " +
                                                    $"WHERE Description LIKE '%{expression}%' " +
                                                    $"ORDER BY Contains(Description='{expression}')");
                            return query.Select(f => new Option { value = f.Type, label = f.Description }).ToList();
                        }
                        else if (type == "sorttype")
                        {
                            var list = new List<Option>();
                            list.Add(new Option { value = "0", label = "Mais avaliados" });
                            list.Add(new Option { value = "1", label = "Melhores avaliados" });
                            list.Add(new Option { value = "2", label = "Piores avaliados" });
                            list.Add(new Option { value = "3", label = "Não avaliados" });
                            list.Add(new Option { value = "4", label = "Mais recentes" });
                            list.Add(new Option { value = "5", label = "Mais antigos" });
                            list.Add(new Option { value = "6", label = "Mais ouvidos" });
                            list.Add(new Option { value = "7", label = "Não ouvidos" });
                            list.Add(new Option { value = "8", label = "Ouvidos recentemente" });
                            return list.Where(f => string.IsNullOrEmpty(expression) || f.label.ToLower().Contains(expression.ToLower())).ToList();
                        }
                        else
                        {
                            var query = conn.Query($"SELECT TOP 10 TagId, Name, Type FROM Tags " +
                                                    $"WHERE Name LIKE '%{expression}%' AND " +
                                                    $"Type IN ({string.Join(",", typeIds)}) " +
                                                    $"ORDER BY Contains(Name='{expression}')");
                            return query.Select(f => new Option { value = f.TagId, label = f.Name, type = f.Type }).ToList();
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

        Option ITopicaService.Select(string type, string id, string token)
        {
            using (var scope = log.Scope("Select()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        string[] typeIds = new string[] { };
                        if (type == "all" || type == "tag")
                        {
                            typeIds = new string[] { "0", "1", "2", "3", "4", "7", "8", "9", "10", "12" };
                        }
                        else if (type == "area")
                        {
                            typeIds = new string[] { "0" };
                        }
                        else if (type == "period")
                        {
                            typeIds = new string[] { "1" };
                        }
                        else if (type == "author")
                        {
                            typeIds = new string[] { "2" };
                        }
                        else if (type == "institution")
                        {
                            typeIds = new string[] { "3" };
                        }
                        else if (type == "book")
                        {
                            typeIds = new string[] { "4" };
                        }
                        else if (type == "topic")
                        {
                            typeIds = new string[] { "7" };
                        }
                        else if (type == "law")
                        {
                            typeIds = new string[] { "8" };
                        }
                        else if (type == "state")
                        {
                            typeIds = new string[] { "9" };
                        }
                        else if (type == "skill")
                        {
                            typeIds = new string[] { "10" };
                        }
                        else if (type == "library")
                        {
                            typeIds = new string[] { "12" };
                        }

                        if (type == "domain")
                        {
                            var query = conn.Query($"SELECT DomainId, Name FROM Domains " +
                                                    $"WHERE DomainId = {id}");
                            return query.Select(f => new Option { value = f.DomainId, label = f.Name }).FirstOrDefault();
                        }
                        else if (type == "tagtype")
                        {
                            var query = conn.Query($"SELECT Type, Description FROM TagTypes " +
                                                    $"WHERE Type = {id}");
                            return query.Select(f => new Option { value = f.Type, label = f.Description }).FirstOrDefault();
                        }
                        else if (type == "fragmenttype")
                        {
                            var query = conn.Query($"SELECT Type, Description FROM FragmentTypes " +
                                                    $"WHERE Type = {id}");
                            return query.Select(f => new Option { value = f.Type, label = f.Description }).FirstOrDefault();
                        }
                        else if (type == "sorttype")
                        {
                            var list = new List<Option>();
                            list.Add(new Option { value = "1", label = "Melhores classificados" });
                            list.Add(new Option { value = "2", label = "Piores classificados" });
                            list.Add(new Option { value = "3", label = "Não classificados" });
                            list.Add(new Option { value = "4", label = "Mais recentes" });
                            list.Add(new Option { value = "5", label = "Mais antigos" });
                            list.Add(new Option { value = "6", label = "Mais ouvidos" });
                            list.Add(new Option { value = "7", label = "Não ouvidos" });
                            list.Add(new Option { value = "8", label = "Ouvidos recentemente" });
                            return list.First(f => f.value == id);
                        }
                        else
                        {
                            var query = conn.Query($"SELECT TagId, Name FROM Tags " +
                                                    $"WHERE TagId = '{id.ToUpper()}' AND " +
                                                    $"Type IN ({string.Join(",", typeIds)})");
                            return query.Select(f => new Option { value = f.TagId, label = f.Name }).FirstOrDefault();
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

        PagedList<Tag> ITopicaService.Books(string libraryId, string[] tags, int start, int count, string token)
        {
            using (var scope = log.Scope("Books()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var sql = string.Empty;
                        var sqlCount = string.Empty;
                        if (libraryId != null)
                        {
                            if (tags != null && tags.Length > 0)
                            {
                                sql = $"SELECT DomainId, TagId, Name, FileName FROM Tags WHERE Type = 4 AND @Tags IN ('{string.Join("') AND @Tags IN ('", tags)}') AND ?TagId IN '@FROM(BookId,LibraryBooks,LibraryId={libraryId.ToUpper()})'";
                                sqlCount = $"SELECT COUNT(*) FROM Tags WHERE Type = 4 AND @Tags IN ('{string.Join("') AND @Tags IN ('", tags)}') AND ?TagId IN '@FROM(BookId,LibraryBooks,LibraryId={libraryId.ToUpper()})'";
                            }
                            else
                            {
                                sql = $"SELECT DomainId, TagId, Name, FileName FROM Tags WHERE Type = 4 AND ?TagId IN '@FROM(BookId,LibraryBooks,LibraryId={libraryId.ToUpper()})'";
                                sqlCount = $"SELECT COUNT(*) FROM Tags WHERE Type = 4 AND ?TagId IN '@FROM(BookId,LibraryBooks,LibraryId={libraryId.ToUpper()})'";
                            }
                        }
                        else
                        {
                            if (tags != null && tags.Length > 0)
                            {
                                sql = $"SELECT DomainId, TagId, Name, FileName FROM Tags WHERE Type = 4 AND @Tags IN ('{string.Join("') AND @Tags IN ('", tags)}')";
                                sqlCount = $"SELECT COUNT(*) FROM Tags WHERE Type = 4 AND @Tags IN ('{string.Join("') AND @Tags IN ('", tags)}')";
                            }
                            else
                            {
                                sql = $"SELECT DomainId, TagId, Name, FileName FROM Tags WHERE Type = -10";
                                sqlCount = $"SELECT COUNT(*) FROM Tags WHERE Type = -10";
                            }
                        }
                        var query = conn.TypedQuery<Tag>(sql)
                            .Skip(start).Take(count).ToList();
                        var queryCount = conn.ExecuteScalar<long>(sqlCount);
                        foreach (var book in query)
                        {
                            sqlCount = $"SELECT COUNT(*) FROM Fragments WHERE @Tags IN ('{book.TagId.ToString().ToUpper()}')";
                            book.TotalOfFragments = conn.ExecuteScalar<long>(sqlCount);
                        }
                        return new PagedList<Tag>(query) { TotalOfRecords = queryCount };
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        Tag ITopicaService.Detail(string tagId, string token)
        {
            if (tagId == "undefined") tagId = null;
            using (var scope = log.Scope("Detail()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var tag = conn.TypedQuery<Tag>($"SELECT * FROM Tags " +
                                    $"WHERE TagId = '{tagId.ToUpper()}'").FirstOrDefault();
                        if (tag == null)
                        {
                            tag = new Tag();
                            tag.TagId = Guid.NewGuid().ToString().ToUpper();
                            tag.Name = "Nova Tag";
                            return tag;
                        }
                        if (tag.ParentId != null)
                        {
                            var id = tag.ParentId.ToString().ToUpper();
                            var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.AreaId != null)
                        {
                            var id = tag.AreaId.ToString().ToUpper();
                            var type = "area";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.PeriodId != null)
                        {
                            var id = tag.PeriodId.ToString().ToUpper();
                            var type = "period";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.BookId != null)
                        {
                            var id = tag.BookId.ToString().ToUpper();
                            var type = "book";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.InstitutionId1 != null)
                        {
                            var id = tag.InstitutionId1.ToString().ToUpper();
                            var type = "institution";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.InstitutionId2 != null)
                        {
                            var id = tag.InstitutionId2.ToString().ToUpper();
                            var type = "institution";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.LawId != null)
                        {
                            var id = tag.LawId.ToString().ToUpper();
                            var type = "law";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.PersonId1 != null)
                        {
                            var id = tag.PersonId1.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.PersonId2 != null)
                        {
                            var id = tag.PersonId2.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.PersonId3 != null)
                        {
                            var id = tag.PersonId3.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.PersonId4 != null)
                        {
                            var id = tag.PersonId4.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.PersonId5 != null)
                        {
                            var id = tag.PersonId5.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.SkillId1 != null)
                        {
                            var id = tag.SkillId1.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.SkillId2 != null)
                        {
                            var id = tag.SkillId2.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.SkillId3 != null)
                        {
                            var id = tag.SkillId3.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.SkillId4 != null)
                        {
                            var id = tag.SkillId4.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.SkillId5 != null)
                        {
                            var id = tag.SkillId5.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.StateId1 != null)
                        {
                            var id = tag.StateId1.ToString().ToUpper();
                            var type = "state";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (tag.StateId2 != null)
                        {
                            var id = tag.StateId2.ToString().ToUpper();
                            var type = "state";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            tag.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        tag.Links = conn.TypedQuery<TagLink>($"SELECT Label, Url FROM TagLinks WHERE TagId = '{tagId}'").ToList();
                        tag.Events = conn.TypedQuery<TagEvent>($"SELECT Century, Year, Date, Name, Url FROM TagEvents WHERE TagId = '{tagId}'").ToList();
                        return tag;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        IList<Tag> ITopicaService.Libraries(string token)
        {
            using (var scope = log.Scope("Libraries()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        return conn.TypedQuery<Tag>($"SELECT * FROM Tags " +
                                    $"WHERE Type = 12").ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        IList<Option> ITopicaService.Folders(int domainId, string parent, string token)
        {
            if (parent == "undefined") parent = null;
            using (var scope = log.Scope("Nodes()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var query = conn.Query($"SELECT Type, TagId, Name FROM Tags " +
                                    $"WHERE DomainId = {domainId} AND ParentId = {(parent != null ? "'" + parent + "'" : "NULL")} " +
                                    $"ORDER BY Name");
                        return query.Select(f => new Option
                        {
                            type = EntityHelper.GetEntityType("tag", (int)f.Type),
                            value = f.TagId,
                            label = f.Name
                        }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        PagedList<TimelineActivity> ITopicaService.References(string tagId, string token)
        {
            using (var scope = log.Scope("References()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        PagedList<TimelineActivity> results = new PagedList<TimelineActivity>();
                        var sql = string.Empty;
                        var sqlCount = string.Empty;
                        if (!string.IsNullOrEmpty(tagId))
                        {
                            sql = $"SELECT * FROM Tags WHERE @Tags IN ('{tagId}') ORDER BY CreatedAt DESC";
                            sqlCount = $"SELECT COUNT(*) FROM Tags WHERE @Tags IN ('{tagId}') ORDER BY CreatedAt DESC";
                        }
                        var query = conn.TypedQuery<Tag>(sql).ToList();
                        var queryCount = conn.ExecuteScalar<long>(sqlCount);
                        results.TotalOfRecords = queryCount;
                        foreach (var item in query)
                        {
                            TimelineActivity act = null;
                            act = new TimelineActivity();
                            act.Id = item.TagId.ToString().ToUpper();
                            act.Type = EntityHelper.GetEntityType("tag", item.Type);
                            act.Title = item.Name;
                            act.Content = item.Text;
                            act.FileName = item.FileName;
                            act.CreatedTimeElapsed = (DateTime.Now - item.CreatedAt).TimeElapsed();
                            results.Add(act);
                            if (item.ParentId != null)
                            {
                                var id = item.ParentId.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.AreaId != null)
                            {
                                var id = item.AreaId.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PeriodId != null)
                            {
                                var id = item.PeriodId.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.BookId != null)
                            {
                                var id = item.BookId.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.InstitutionId1 != null)
                            {
                                var id = item.InstitutionId1.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.InstitutionId2 != null)
                            {
                                var id = item.InstitutionId2.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.LawId != null)
                            {
                                var id = item.LawId.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId1 != null)
                            {
                                var id = item.PersonId1.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId2 != null)
                            {
                                var id = item.PersonId2.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId3 != null)
                            {
                                var id = item.PersonId3.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId4 != null)
                            {
                                var id = item.PersonId4.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId5 != null)
                            {
                                var id = item.PersonId5.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId1 != null)
                            {
                                var id = item.SkillId1.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId2 != null)
                            {
                                var id = item.SkillId2.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId3 != null)
                            {
                                var id = item.SkillId3.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId4 != null)
                            {
                                var id = item.SkillId4.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId5 != null)
                            {
                                var id = item.SkillId5.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.StateId1 != null)
                            {
                                var id = item.StateId1.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.StateId2 != null)
                            {
                                var id = item.StateId2.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId1 != null)
                            {
                                var id = item.TopicId1.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId2 != null)
                            {
                                var id = item.TopicId2.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId3 != null)
                            {
                                var id = item.TopicId3.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId4 != null)
                            {
                                var id = item.TopicId4.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId5 != null)
                            {
                                var id = item.TopicId5.ToString().ToUpper();
                                var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                        }
                        return results;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        bool ITopicaService.SaveImage(string token, HttpPostedFile file)
        {
            using (var scope = log.Scope("SaveImage()"))
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var name = Path.GetFileName(file.FileName);
                        var path = Path.Combine(this._config.RepositoryPath, @"tags\raw", name);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        file.SaveAs(path);
                        this.imageProcessor.ProcessImage(path);
                        return true;
                    }
                    else return false;
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        string ITopicaService.SaveTag(JObject obj, string token, out string sqlExecuted)
        {
            using (var scope = log.Scope("SaveTag()"))
            {
                try
                {
                    var sql = new StringBuilder();
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        conn.SqlExecuted = (data, cmd) =>
                        {
                            if (cmd.Contains("INSERT INTO Tags"))
                            {
                                sql.Append($"INSERT INTO AutoComplete (Id, Category, Type, Name) VALUES ('{((string)data.TagId.ToString()).ToUpper()}', 'TAG', {data.Type}, '{((string)data.Name).Replace("'", "''")}')`");
                            }
                            else if (cmd.Contains("UPDATE Tags"))
                            {
                                sql.Append($"UPDATE AutoComplete SET Type = {data.Type}, Name = '{((string)data.Name).Replace("'", "''")}' WHERE Id = '{((string)data.TagId.ToString()).ToUpper()}'`");
                            }
                        };
                        var id = (string)conn.Save("Tags", obj);
                        var act = conn.TypedQuery<Activity>($"SELECT * FROM Activities WHERE TagId = '{id}'").FirstOrDefault();
                        if (act == null)
                        {
                            act = new Activity();
                            act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                            act.TagId = obj["TagId"].ToString();
                        }
                        else
                        {
                            act.ActivityId = act.ActivityId.ToUpper();
                            act.TagId = act.TagId.ToUpper();
                        }
                        act.CreatedAt = DateTime.Parse(obj["CreatedAt"].ToString());
                        act.UserId = Nvl(obj["CreatedBy"].ToString());
                        act.ParentId = Nvl(obj["ParentId"].ToString());
                        act.PeriodId = Nvl(obj["PeriodId"].ToString());
                        act.AreaId = Nvl(obj["AreaId"].ToString());
                        act.PeriodId = Nvl(obj["PeriodId"].ToString());
                        act.BookId = Nvl(obj["BookId"].ToString());
                        act.DomainId = string.IsNullOrEmpty(obj["DomainId"].ToString()) ? 41 : int.Parse(obj["DomainId"].ToString());
                        act.InstitutionId1 = Nvl(obj["InstitutionId1"].ToString());
                        act.InstitutionId2 = Nvl(obj["InstitutionId2"].ToString());
                        act.LawId = Nvl(obj["LawId"].ToString());
                        act.PersonId1 = Nvl(obj["PersonId1"].ToString());
                        act.PersonId2 = Nvl(obj["PersonId2"].ToString());
                        act.PersonId3 = Nvl(obj["PersonId3"].ToString());
                        act.PersonId4 = Nvl(obj["PersonId4"].ToString());
                        act.PersonId5 = Nvl(obj["PersonId5"].ToString());
                        act.SkillId1 = Nvl(obj["SkillId1"].ToString());
                        act.SkillId2 = Nvl(obj["SkillId2"].ToString());
                        act.SkillId3 = Nvl(obj["SkillId3"].ToString());
                        act.SkillId4 = Nvl(obj["SkillId4"].ToString());
                        act.SkillId5 = Nvl(obj["SkillId5"].ToString());
                        act.StateId1 = Nvl(obj["StateId1"].ToString());
                        act.StateId2 = Nvl(obj["StateId2"].ToString());
                        act.TopicId1 = Nvl(obj["TopicId1"].ToString());
                        act.TopicId2 = Nvl(obj["TopicId2"].ToString());
                        act.TopicId3 = Nvl(obj["TopicId3"].ToString());
                        act.TopicId4 = Nvl(obj["TopicId4"].ToString());
                        act.TopicId5 = Nvl(obj["TopicId5"].ToString());
                        var actId = (string)conn.Save("Activities", JObject.FromObject(act));
                        sqlExecuted = sql.ToString();
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        private string Nvl(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                return v;
            }
            return null;
        }

        long ITopicaService.SaveLibraryBook(JObject book, string token)
        {
            using (var scope = log.Scope("SaveLibraryBook()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        return long.Parse(conn.Save("LibraryBooks", book).ToString());
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }



        LibraryBook ITopicaService.LibraryBook(string bookId, string libraryId, string token)
        {
            if (bookId == "undefined") bookId = null;
            if (libraryId == "undefined") libraryId = null;
            using (var scope = log.Scope("LibraryBook()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var book = conn.TypedQuery<LibraryBook>($"SELECT * FROM LibraryBooks " +
                                    $@"WHERE BookId = '{bookId.ToUpper()}'{(libraryId != null ?
                                    "AND LibraryId = '" + libraryId.ToUpper() + "'" : string.Empty)}").FirstOrDefault();
                        if (book == null && !string.IsNullOrEmpty(libraryId))
                        {
                            book = new LibraryBook();
                            book.BookId = bookId;
                            book.LibraryId = libraryId;
                        }
                        return book;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        void ITopicaService.SaveLinks(string tagId, JToken links, string token)
        {
            using (var scope = log.Scope("SaveLinks()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        conn.Execute($"DELETE FROM TagLinks WHERE TagId = '{tagId}'");
                        foreach (var item in links)
                        {
                            var link = conn.NewRow("TagLinks");
                            link.TagLinkId = Guid.NewGuid().ToString().ToUpper();
                            link.TagId = tagId;
                            link.Label = item["Label"].ToString();
                            link.Url = item["Url"].ToString();
                            conn.Save("TagLinks", link);
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

        void ITopicaService.SaveEvents(string tagId, JToken events, string token)
        {
            using (var scope = log.Scope("SaveEvents()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        conn.Execute($"DELETE FROM TagEvents WHERE TagId = '{tagId}'");
                        foreach (var item in events)
                        {
                            var row = conn.NewRow("TagEvents");
                            row.TagId = tagId;
                            row.Century = item["Century"].ToString();
                            row.Year = item["Year"].ToString();
                            row.Date = item["Date"].ToString();
                            row.Name = item["Name"].ToString();
                            row.Url = item["Url"].ToString();
                            conn.Save("TagEvents", row);
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
