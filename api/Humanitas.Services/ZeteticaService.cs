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
    public class ZeteticaService : IZeteticaService
    {

        private Logger log = new Logger(typeof(ZeteticaService));
        private ImageRepositoryWatcher imageProcessor = null;
        private AppConfiguration _config = null;
        private IUserAccessService _userService = null;

        public ZeteticaService(AppConfiguration config, IUserAccessService userService)
        {
            this._config = config;
            this._userService = userService;
            this.imageProcessor = new ImageRepositoryWatcher($@"{config.RepositoryPath}\fragments\raw\*.jpg");
            this.imageProcessor.AddResizeRule($@"{config.RepositoryPath}\fragments\width340", 340, 340);
            this.imageProcessor.AddResizeRule($@"{config.RepositoryPath}\fragments\width160", 160, 160);
            this.imageProcessor.AddResizeRule($@"{config.RepositoryPath}\fragments\width50", 50, 50);
        }

        List<Fragment> IZeteticaService.All(string token)
        {
            using (var scope = log.Scope("All()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        List<Fragment> IZeteticaService.AllAudios(string token)
        {
            using (var scope = log.Scope("AllAudios()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        List<Fragment> IZeteticaService.AllNotes(string token)
        {
            using (var scope = log.Scope("AllNotes()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        List<Fragment> IZeteticaService.AllQuotes(string token)
        {
            using (var scope = log.Scope("AllQuotes()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        List<Fragment> IZeteticaService.AllVideos(string token)
        {
            using (var scope = log.Scope("AllVideos()"))
            {
                try
                {

                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
                return null;
            }
        }

        IList<Option> IZeteticaService.AutoComplete(string type, string expression, string token)
        {
            using (var scope = log.Scope("AutoComplete()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        string[] typeIds = new string[] { };
                        if (type == "all" || type == "fragment")
                        {
                            typeIds = new string[] { "0", "1", "2", "3", "4", "5" };
                        }
                        else if (type == "quote")
                        {
                            typeIds = new string[] { "0" };
                        }
                        else if (type == "note")
                        {
                            typeIds = new string[] { "1" };
                        }
                        else if (type == "question")
                        {
                            typeIds = new string[] { "2" };
                        }
                        else if (type == "video")
                        {
                            typeIds = new string[] { "3" };
                        }
                        else if (type == "article")
                        {
                            typeIds = new string[] { "4" };
                        }
                        else if (type == "audio")
                        {
                            typeIds = new string[] { "5" };
                        }
                        var query = conn.Query($"SELECT FragmentId, Title FROM Fragments " +
                                                $"WHERE Title LIKE '%{expression}%' AND " +
                                                $"Type IN ({string.Join(",", typeIds)})");
                        return query.Select(f => new Option { value = f.TagId, label = f.Name }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        PagedList<TimelineActivity> IZeteticaService.Activities(string tag, string sort, int start, int count, string token)
        {
            using (var scope = log.Scope("Activities()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        PagedList<TimelineActivity> results = new PagedList<TimelineActivity>();
                        var orderBy = new StringBuilder();
                        var where = new StringBuilder();

                        if (tag != null)
                        {
                            where.Append($"@Tags IN ('{tag}')");
                        }

                        // Mais avaliados
                        if (sort == "0")
                        {
                            orderBy = new StringBuilder("TotalVotes DESC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND TotalVotes != NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"TotalVotes != NULL AND FragmentId != NULL");
                            }
                        }
                        // Melhores avaliados
                        else if (sort == "1")
                        {
                            orderBy = new StringBuilder("TotalScore DESC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND TotalScore != NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"TotalScore != NULL AND FragmentId != NULL");
                            }
                        }
                        // Piores avaliados
                        else if (sort == "2")
                        {
                            orderBy = new StringBuilder("TotalScore ASC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND TotalScore != NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"TotalScore != NULL AND FragmentId != NULL");
                            }
                        }
                        // Não avaliados
                        else if (sort == "3")
                        {
                            orderBy = new StringBuilder("TotalScore ASC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND TotalScore = NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"TotalScore = NULL AND FragmentId != NULL");
                            }
                        }

                        // Mais recentes
                        else if (sort == "4")
                        {
                            orderBy = new StringBuilder("CreatedAt DESC");
                        }

                        // Mais antigos
                        else if (sort == "5")
                        {
                            orderBy = new StringBuilder("CreatedAt ASC");
                        }

                        // Mais ouvidos
                        else if (sort == "6")
                        {
                            orderBy = new StringBuilder("TotalListen DESC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND TotalListen != NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"TotalListen != NULL AND FragmentId != NULL");
                            }
                        }

                        // Não ouvidos
                        else if (sort == "7")
                        {
                            orderBy = new StringBuilder("CreatedAt DESC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND TotalListen = NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"TotalListen = NULL AND FragmentId != NULL");
                            }
                        }

                        // Ouvidos recentemente
                        else if (sort == "8")
                        {
                            orderBy = new StringBuilder("LastTimeListen DESC");
                            if (where.Length > 0)
                            {
                                where.Append($" AND LastTimeListen != NULL AND FragmentId != NULL");
                            }
                            else
                            {
                                where.Append($"LastTimeListen != NULL AND FragmentId != NULL");
                            }
                        }

                        if(orderBy.Length == 0)
                        {
                            orderBy = new StringBuilder("CreatedAt DESC");
                        }

                        var sql = $"SELECT * FROM Activities {(where.Length > 0 ? $"WHERE {where}" : string.Empty)} ORDER BY {orderBy}";
                        var query = conn.TypedQuery<Activity>(sql)
                            .Skip(start).Take(count).ToList();
                        var queryCount = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Activities {(where.Length > 0 ? $"WHERE {where}" : string.Empty)}");
                        results.TotalOfRecords = queryCount;
                        foreach (var item in query)
                        {
                            TimelineActivity act = null;
                            if (item.TagId != null)
                            {
                                var tagItem = conn.TypedQuery<Tag>($"SELECT Type, Name, Text, FileName, CreatedAt, CreatedBy FROM Tags WHERE TagId = '{item.TagId.ToString().ToUpper()}'").FirstOrDefault();
                                act = new TimelineActivity();
                                act.UserId = tagItem.CreatedBy.ToUpper();
                                act.DomainId = item.DomainId;
                                act.Id = item.TagId.ToString().ToUpper();
                                act.Type = EntityHelper.GetEntityType("tag", tagItem.Type);
                                act.Title = tagItem.Name;
                                act.Content = tagItem.Text;
                                act.FileName = tagItem.FileName;
                                act.CreatedTimeElapsed = (DateTime.Now - tagItem.CreatedAt).TimeElapsed();
                                results.Add(act);
                            }
                            else if (item.FragmentId != null)
                            {
                                var fragment = conn.TypedQuery<Fragment>($"SELECT Type, Title, Content, Author, ImageFileName, Page, CreatedAt, CreatedBy FROM Fragments WHERE FragmentId = '{item.FragmentId.ToString().ToUpper()}'").FirstOrDefault();
                                var likes = conn.TypedQuery<FragmentLike>($"SELECT * FROM FragmentLikes WHERE FragmentId = '{item.FragmentId.ToString().ToUpper()}'").ToList();
                                act = new TimelineActivity();
                                act.UserId = fragment.CreatedBy != null ? fragment.CreatedBy.ToUpper() : "";
                                act.DomainId = item.DomainId;
                                act.Id = item.FragmentId.ToString().ToUpper();
                                act.Type = EntityHelper.GetEntityType("fragment", fragment.Type);
                                act.Title = fragment.Title;
                                act.Content = fragment.Content;
                                act.Author = fragment.Author;
                                act.FileName = fragment.ImageFileName;
                                act.CreatedTimeElapsed = (DateTime.Now - fragment.CreatedAt).TimeElapsed();
                                act.TotalVotes = item.TotalVotes;
                                act.TotalScore = item.TotalScore;
                                act.TotalListen = item.TotalListen;
                                if (item.LastTimeListen != null)
                                {
                                    act.ListenTimeElapsed = (DateTime.Now - item.LastTimeListen.Value).TimeElapsed();
                                }
                                act.Page = fragment.Page;
                                if (act.Page != null && act.Page.Value > 0 && item.BookId != null)
                                {
                                    act.Reference = conn.GetRowLabel("Tags", item.BookId.ToString().ToUpper()) + " - p. " + act.Page.Value;
                                }
                                results.Add(act);
                            }

                            act.UserPhotoUrl = conn.GetCachedValue<string>("Users", "PhotoUrl", act.UserId);
                            act.UserName = conn.GetRowLabel("Users", act.UserId);

                            if (item.ParentId != null)
                            {
                                var id = item.ParentId.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.AreaId != null)
                            {
                                var id = item.AreaId.ToString().ToUpper();
                                var type = "area";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PeriodId != null)
                            {
                                var id = item.PeriodId.ToString().ToUpper();
                                var type = "period";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.BookId != null)
                            {
                                var id = item.BookId.ToString().ToUpper();
                                var type = "book";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.InstitutionId1 != null)
                            {
                                var id = item.InstitutionId1.ToString().ToUpper();
                                var type = "institution";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.InstitutionId2 != null)
                            {
                                var id = item.InstitutionId2.ToString().ToUpper();
                                var type = "institution";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.LawId != null)
                            {
                                var id = item.LawId.ToString().ToUpper();
                                var type = "law";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId1 != null)
                            {
                                var id = item.PersonId1.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId2 != null)
                            {
                                var id = item.PersonId2.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId3 != null)
                            {
                                var id = item.PersonId3.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId4 != null)
                            {
                                var id = item.PersonId4.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId5 != null)
                            {
                                var id = item.PersonId5.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId1 != null)
                            {
                                var id = item.SkillId1.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId2 != null)
                            {
                                var id = item.SkillId2.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId3 != null)
                            {
                                var id = item.SkillId3.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId4 != null)
                            {
                                var id = item.SkillId4.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId5 != null)
                            {
                                var id = item.SkillId5.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.StateId1 != null)
                            {
                                var id = item.StateId1.ToString().ToUpper();
                                var type = "state";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.StateId2 != null)
                            {
                                var id = item.StateId2.ToString().ToUpper();
                                var type = "state";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId1 != null)
                            {
                                var id = item.TopicId1.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId2 != null)
                            {
                                var id = item.TopicId2.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId3 != null)
                            {
                                var id = item.TopicId3.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId4 != null)
                            {
                                var id = item.TopicId4.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId5 != null)
                            {
                                var id = item.TopicId5.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
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

        PagedList<TimelineActivity> IZeteticaService.Fragments(long domainId, string tagId, int start, int count, string token)
        {
            using (var scope = log.Scope("Fragments()"))
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
                            sql = $"SELECT * FROM Fragments WHERE @Tags IN ('{tagId.ToUpper()}') ORDER BY CreatedAt DESC";
                            sqlCount = $"SELECT COUNT(*) FROM Fragments WHERE @Tags IN ('{tagId.ToUpper()}')";
                        }
                        else
                        {
                            sql = $"SELECT * FROM Fragments WHERE domainId = {domainId} AND ParentId = NULL ORDER BY CreatedAt DESC";
                            sqlCount = $"SELECT COUNT(*) FROM Fragments WHERE domainId = {domainId} AND ParentId = NULL";
                        }
                        var query = conn.TypedQuery<Fragment>(sql)
                            .Skip(start).Take(count).ToList();
                        var queryCount = conn.ExecuteScalar<long>(sqlCount);
                        results.TotalOfRecords = queryCount;
                        foreach (var item in query)
                        {
                            TimelineActivity act = null;
                            act = new TimelineActivity();
                            act.Id = item.FragmentId.ToString().ToUpper();
                            var likes = conn.TypedQuery<FragmentLike>($"SELECT * FROM FragmentLikes WHERE FragmentId = '{act.Id}'").ToList();
                            //if (likes.Any(f => f.FragmentId != act.Id)) throw new InvalidOperationException($"The query is not working for {act.Id}.");
                            act.Type = EntityHelper.GetEntityType("fragment", item.Type);
                            act.Title = item.Title;
                            act.Content = item.Content;
                            act.Author = item.Author;
                            act.FileName = item.ImageFileName;
                            act.CreatedTimeElapsed = (DateTime.Now - item.CreatedAt).TimeElapsed();

                            act.UserId = item.CreatedBy.ToUpper();
                            act.UserPhotoUrl = conn.GetCachedValue<string>("Users", "PhotoUrl", act.UserId);
                            act.UserName = conn.GetRowLabel("Users", act.UserId);

                            if (likes.Any())
                            {
                                act.TotalListen = likes.Where(f => f.ListenCount != null)
                                                       .Sum(f => f.ListenCount.Value);
                                var scores = likes.Where(f => f.Score != null);
                                if (scores.Count() > 0)
                                {
                                    act.TotalScore = scores.Sum(f => f.Score.Value);
                                    act.TotalVotes = (long)scores.Count();
                                }
                            }
                            act.Page = item.Page;
                            if (act.Page != null && act.Page.Value > 0 && item.BookId != null)
                            {
                                act.Reference = conn.GetRowLabel("Tags", item.BookId.ToString().ToUpper()) + " - p. " + act.Page.Value;
                            }
                            results.Add(act);
                            if (item.ParentId != null)
                            {
                                var id = item.ParentId.ToString().ToUpper();
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.AreaId != null)
                            {
                                var id = item.AreaId.ToString().ToUpper();
                                var type = "area";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PeriodId != null)
                            {
                                var id = item.PeriodId.ToString().ToUpper();
                                var type = "period";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.BookId != null)
                            {
                                var id = item.BookId.ToString().ToUpper();
                                var type = "book";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.InstitutionId1 != null)
                            {
                                var id = item.InstitutionId1.ToString().ToUpper();
                                var type = "institution";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.InstitutionId2 != null)
                            {
                                var id = item.InstitutionId2.ToString().ToUpper();
                                var type = "institution";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.LawId != null)
                            {
                                var id = item.LawId.ToString().ToUpper();
                                var type = "law";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId1 != null)
                            {
                                var id = item.PersonId1.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId2 != null)
                            {
                                var id = item.PersonId2.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId3 != null)
                            {
                                var id = item.PersonId3.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId4 != null)
                            {
                                var id = item.PersonId4.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.PersonId5 != null)
                            {
                                var id = item.PersonId5.ToString().ToUpper();
                                var type = "author";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId1 != null)
                            {
                                var id = item.SkillId1.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId2 != null)
                            {
                                var id = item.SkillId2.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId3 != null)
                            {
                                var id = item.SkillId3.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId4 != null)
                            {
                                var id = item.SkillId4.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.SkillId5 != null)
                            {
                                var id = item.SkillId5.ToString().ToUpper();
                                var type = "skill";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.StateId1 != null)
                            {
                                var id = item.StateId1.ToString().ToUpper();
                                var type = "state";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.StateId2 != null)
                            {
                                var id = item.StateId2.ToString().ToUpper();
                                var type = "state";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId1 != null)
                            {
                                var id = item.TopicId1.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId2 != null)
                            {
                                var id = item.TopicId2.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId3 != null)
                            {
                                var id = item.TopicId3.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId4 != null)
                            {
                                var id = item.TopicId4.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                                });
                            }
                            if (item.TopicId5 != null)
                            {
                                var id = item.TopicId5.ToString().ToUpper();
                                var type = "topic";
                                var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                                var label = conn.GetRowLabel("Tags", id);
                                act.Tags.Add(new Option
                                {
                                    value = id,
                                    label = label,
                                    type = type,
                                    link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
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

        Fragment IZeteticaService.Detail(string fragmentId, string token)
        {
            if (fragmentId == "undefined") fragmentId = null;
            using (var scope = log.Scope("Detail()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var fragment = conn.TypedQuery<Fragment>($"SELECT * FROM Fragments " +
                                    $"WHERE FragmentId = '{fragmentId.ToUpper()}'").FirstOrDefault();
                        if (fragment == null)
                        {
                            fragment = new Fragment();
                            fragment.FragmentId = Guid.NewGuid().ToString().ToUpper();
                            fragment.Title = "Novo Fragmento";
                            return fragment;
                        }
                        if (fragment.ParentId != null)
                        {
                            var id = fragment.ParentId.ToString().ToUpper();
                            var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.AreaId != null)
                        {
                            var id = fragment.AreaId.ToString().ToUpper();
                            var type = "area";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.PeriodId != null)
                        {
                            var id = fragment.PeriodId.ToString().ToUpper();
                            var type = "period";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.BookId != null)
                        {
                            var id = fragment.BookId.ToString().ToUpper();
                            var type = "book";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.InstitutionId1 != null)
                        {
                            var id = fragment.InstitutionId1.ToString().ToUpper();
                            var type = "institution";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.InstitutionId2 != null)
                        {
                            var id = fragment.InstitutionId2.ToString().ToUpper();
                            var type = "institution";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.LawId != null)
                        {
                            var id = fragment.LawId.ToString().ToUpper();
                            var type = "law";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.PersonId1 != null)
                        {
                            var id = fragment.PersonId1.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.PersonId2 != null)
                        {
                            var id = fragment.PersonId2.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.PersonId3 != null)
                        {
                            var id = fragment.PersonId3.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.PersonId4 != null)
                        {
                            var id = fragment.PersonId4.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.PersonId5 != null)
                        {
                            var id = fragment.PersonId5.ToString().ToUpper();
                            var type = "author";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.SkillId1 != null)
                        {
                            var id = fragment.SkillId1.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.SkillId2 != null)
                        {
                            var id = fragment.SkillId2.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.SkillId3 != null)
                        {
                            var id = fragment.SkillId3.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.SkillId4 != null)
                        {
                            var id = fragment.SkillId4.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.SkillId5 != null)
                        {
                            var id = fragment.SkillId5.ToString().ToUpper();
                            var type = "skill";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.StateId1 != null)
                        {
                            var id = fragment.StateId1.ToString().ToUpper();
                            var type = "state";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (fragment.StateId2 != null)
                        {
                            var id = fragment.StateId2.ToString().ToUpper();
                            var type = "state";
                            var domainId = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            fragment.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        return fragment;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        bool IZeteticaService.SaveAudio(string token, HttpPostedFile file)
        {
            using (var scope = log.Scope("SaveAudio()"))
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var name = Path.GetFileName(file.FileName);
                        var path = Path.Combine(this._config.RepositoryPath, @"..\leibnizinstitute-audio\", name);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        file.SaveAs(path);
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

        bool IZeteticaService.SaveImage(string token, HttpPostedFile file)
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

        string IZeteticaService.SaveFragment(JObject obj, string token, out string sqlExecuted)
        {
            using (var scope = log.Scope("SaveFragment()"))
            {
                try
                {
                    var sql = new StringBuilder();
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        conn.SqlExecuted = (data, cmd) => 
                        {
                        };
                        var id = (string)conn.Save("Fragments", obj);
                        var act = conn.TypedQuery<Activity>($"SELECT * FROM Activities WHERE FragmentId = '{id}'").FirstOrDefault();
                        if (act == null)
                        {
                            act = new Activity();
                            act.ActivityId = Guid.NewGuid().ToString().ToUpper();
                            act.FragmentId = obj["FragmentId"].ToString();
                        }
                        else
                        {
                            act.ActivityId = act.ActivityId.ToUpper();
                            act.FragmentId = act.FragmentId.ToUpper();
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

        bool IZeteticaService.Score(string fragmentId, short score, string token)
        {
            using (var scope = log.Scope("Score()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var act = conn.TypedQuery<Activity>($"SELECT * FROM Activities WHERE FragmentId = '{fragmentId.ToUpper()}'").FirstOrDefault();
                        act.TotalScore = act.TotalScore != null ? act.TotalScore.Value + score : score;
                        act.TotalVotes = act.TotalVotes != null ? act.TotalVotes.Value + 1 : 1;
                        conn.Save("Activities", act);

                        var like = conn.TypedQuery<FragmentLike>($"SELECT * FROM FragmentLikes WHERE FragmentId = '{fragmentId.ToUpper()}' AND UserId = '{token.ToUpper()}'").FirstOrDefault();
                        if (like != null)
                        {
                            like.Score = score;
                            conn.Save("FragmentLikes", JObject.FromObject(like));
                            return true;
                        }
                        else
                        {
                            like = new FragmentLike();
                            like.FragmentLikeId = Guid.NewGuid().ToString().ToUpper();
                            like.FragmentId = fragmentId;
                            like.UserId = this._userService.TokenToUserId(token);
                            like.Score = score;
                            conn.Save("FragmentLikes", JObject.FromObject(like));
                            return true;
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

        TimelineActivity IZeteticaService.Fragment(string fragmentId, string token)
        {
            using (var scope = log.Scope("Fragments()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        PagedList<TimelineActivity> results = new PagedList<TimelineActivity>();
                        var sql = string.Empty;
                        sql = $"SELECT * FROM Fragments WHERE FragmentId = '{fragmentId.ToUpper()}'";
                        var item = conn.TypedQuery<Fragment>(sql).FirstOrDefault();
                        TimelineActivity act = null;
                        act = new TimelineActivity();
                        act.Id = item.FragmentId.ToString().ToUpper();
                        var likes = conn.TypedQuery<FragmentLike>($"SELECT * FROM FragmentLikes WHERE FragmentId = '{act.Id}'").ToList();
                        act.Type = EntityHelper.GetEntityType("fragment", item.Type);
                        act.Title = item.Title;
                        act.Content = item.Content;
                        act.Author = item.Author;
                        act.FileName = item.ImageFileName;
                        act.CreatedTimeElapsed = (DateTime.Now - item.CreatedAt).TimeElapsed();
                        if (likes.Any())
                        {
                            act.TotalListen = likes.Where(f => f.ListenCount != null)
                                                   .Sum(f => f.ListenCount.Value);
                            var scores = likes.Where(f => f.Score != null);
                            if (scores.Count() > 0)
                            {
                                act.TotalScore = scores.Sum(f => f.Score.Value);
                                act.TotalVotes = (long)scores.Count();
                            }
                        }
                        act.Page = item.Page;
                        if (act.Page != null && act.Page.Value > 0 && item.BookId != null)
                        {
                            act.Reference = conn.GetRowLabel("Tags", item.BookId.ToString().ToUpper()) + " - p. " + act.Page.Value;
                        }
                        results.Add(act);
                        if (item.ParentId != null)
                        {
                            var id = item.ParentId.ToString().ToUpper();
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var type = conn.GetCachedValue<int>("Tags", "Type", id).ToString();
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.AreaId != null)
                        {
                            var id = item.AreaId.ToString().ToUpper();
                            var type = "area";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.PeriodId != null)
                        {
                            var id = item.PeriodId.ToString().ToUpper();
                            var type = "period";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.BookId != null)
                        {
                            var id = item.BookId.ToString().ToUpper();
                            var type = "book";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.InstitutionId1 != null)
                        {
                            var id = item.InstitutionId1.ToString().ToUpper();
                            var type = "institution";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.InstitutionId2 != null)
                        {
                            var id = item.InstitutionId2.ToString().ToUpper();
                            var type = "institution";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.LawId != null)
                        {
                            var id = item.LawId.ToString().ToUpper();
                            var type = "law";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.PersonId1 != null)
                        {
                            var id = item.PersonId1.ToString().ToUpper();
                            var type = "author";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.PersonId2 != null)
                        {
                            var id = item.PersonId2.ToString().ToUpper();
                            var type = "author";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.PersonId3 != null)
                        {
                            var id = item.PersonId3.ToString().ToUpper();
                            var type = "author";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.PersonId4 != null)
                        {
                            var id = item.PersonId4.ToString().ToUpper();
                            var type = "author";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.PersonId5 != null)
                        {
                            var id = item.PersonId5.ToString().ToUpper();
                            var type = "author";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.SkillId1 != null)
                        {
                            var id = item.SkillId1.ToString().ToUpper();
                            var type = "skill";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.SkillId2 != null)
                        {
                            var id = item.SkillId2.ToString().ToUpper();
                            var type = "skill";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.SkillId3 != null)
                        {
                            var id = item.SkillId3.ToString().ToUpper();
                            var type = "skill";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.SkillId4 != null)
                        {
                            var id = item.SkillId4.ToString().ToUpper();
                            var type = "skill";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.SkillId5 != null)
                        {
                            var id = item.SkillId5.ToString().ToUpper();
                            var type = "skill";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.StateId1 != null)
                        {
                            var id = item.StateId1.ToString().ToUpper();
                            var type = "state";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.StateId2 != null)
                        {
                            var id = item.StateId2.ToString().ToUpper();
                            var type = "state";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.TopicId1 != null)
                        {
                            var id = item.TopicId1.ToString().ToUpper();
                            var type = "topic";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.TopicId2 != null)
                        {
                            var id = item.TopicId2.ToString().ToUpper();
                            var type = "topic";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.TopicId3 != null)
                        {
                            var id = item.TopicId3.ToString().ToUpper();
                            var type = "topic";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.TopicId4 != null)
                        {
                            var id = item.TopicId4.ToString().ToUpper();
                            var type = "topic";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        if (item.TopicId5 != null)
                        {
                            var id = item.TopicId5.ToString().ToUpper();
                            var type = "topic";
                            var domainId2 = conn.GetCachedValue<long>("Tags", "DomainId", id);
                            var label = conn.GetRowLabel("Tags", id);
                            act.Tags.Add(new Option
                            {
                                value = id,
                                label = label,
                                type = type,
                                link = "/explorer;domainId=" + domainId2 + "&parent=" + id + "&parentName=" + label + "&parentType=" + type
                            });
                        }
                        return act;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        bool IZeteticaService.Listen(string fragmentId, string token)
        {
            using (var scope = log.Scope("Listen()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var act = conn.TypedQuery<Activity>($"SELECT * FROM Activities WHERE FragmentId = '{fragmentId.ToUpper()}'").FirstOrDefault();
                        act.TotalListen = act.TotalListen != null ? act.TotalListen.Value + 1 : 1;
                        act.LastTimeListen = DateTime.Now;
                        conn.Save("Activities", act);

                        var like = conn.TypedQuery<FragmentLike>($"SELECT * FROM FragmentLikes WHERE FragmentId = '{fragmentId.ToUpper()}' AND UserId = '{token.ToUpper()}'").FirstOrDefault();
                        if (like != null)
                        {
                            like.ListenAt = DateTime.Now;
                            like.ListenCount++;
                            conn.Save("FragmentLikes", JObject.FromObject(like));
                            return true;
                        }
                        else
                        {
                            like = new FragmentLike();
                            like.FragmentLikeId = Guid.NewGuid().ToString().ToUpper();
                            like.FragmentId = fragmentId;
                            like.UserId = this._userService.TokenToUserId(token);
                            like.ListenAt = DateTime.Now;
                            like.ListenCount = 1;
                            conn.Save("FragmentLikes", JObject.FromObject(like));
                            return true;
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

        Activity IZeteticaService.ActivityByFragmentId(string id)
        {
            using (var scope = log.Scope("ActivityByFragmentId()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        return conn.TypedQuery<Activity>($"SELECT * FROM Activities WHERE FragmentId = '{id}'").FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        List<string> IZeteticaService.BatchData(string type, string tagName, string token)
        {
            var results = new List<string>();
            using (var scope = log.Scope("BatchData()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var tagId = conn.ExecuteScalar<string>($"SELECT TagId FROM Tags WHERE Name = '{tagName}'");
                        var fragments = conn.TypedQuery<Fragment>($"SELECT * FROM Fragments WHERE Type = {(type == "note" ? "1" : "0")} AND @Tags IN ('{tagId}')");
                        var sb = new StringBuilder();
                        var index = 1;
                        foreach (var item in fragments)
                        {
                            results.Add($"// [{index++}]");
                            results.Add($"Título:={item.Title}");
                            results.Add($"FragmentoID:={item.FragmentId.ToUpper()}");
                            results.Add($"Texto:={item.Content}");
                            results.Add($"Autor:={item.Author}");
                            results.Add($"Domínio:={conn.GetRowLabel("Domains", item.DomainId)}");
                            results.Add($"Dentro de:={(item.ParentId != null ? conn.GetRowLabel("Tags", item.ParentId) : string.Empty)}");
                            results.Add($"Pessoas:={GetAuthors(conn, item)}");
                            results.Add($"Tópicos:={GetTopics(conn, item)}");
                            if (item.BookId != null)
                            {
                                results.Add($"Livro:={conn.GetRowLabel("Tags", item.BookId)}");
                                results.Add($"Página:={item.Page}");
                            }
                            results.Add($"Referência:={item.Reference}");
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

        private string GetTopics(SqlCacheConnection conn, Fragment item)
        {
            var sb = new StringBuilder();
            if (item.TopicId1 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.TopicId1)};");
            }
            if (item.TopicId2 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.TopicId2)};");
            }
            if (item.TopicId3 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.TopicId3)};");
            }
            if (item.TopicId4 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.TopicId4)};");
            }
            if (item.TopicId5 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.TopicId5)};");
            }
            return sb.ToString().TrimEnd(';');
        }

        private string GetAuthors(SqlCacheConnection conn, Fragment item)
        {
            var sb = new StringBuilder();
            if (item.PersonId1 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.PersonId1)};");
            }
            if (item.PersonId2 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.PersonId2)};");
            }
            if (item.PersonId3 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.PersonId3)};");
            }
            if (item.PersonId4 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.PersonId4)};");
            }
            if (item.PersonId5 != null)
            {
                sb.Append($"{conn.GetRowLabel("Tags", item.PersonId5)};");
            }
            return sb.ToString().TrimEnd(';');
        }

    }
}
