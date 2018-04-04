using Humanitas.Interfaces;
using Humanitas.Services.Interfaces;
using Logging;
using SqlCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humanitas.Services
{
    public class UserAccessService : IUserAccessService
    {

        private Logger log = new Logger(typeof(UserAccessService));
        private AppConfiguration _config = null;

        public UserAccessService(AppConfiguration config)
        {
            this._config = config;
        }

        User IUserAccessService.Detail(string userId, string token)
        {
            if (userId == "undefined") userId = null;
            using (var scope = log.Scope("Detail()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        return conn.TypedQuery<User>($"SELECT * FROM Users " +
                                    $"WHERE UserId = '{userId.ToUpper()}'").FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        PagedList<User> IUserAccessService.List(int start, int count, string token)
        {
            using (var scope = log.Scope("List()"))
            {
                try
                {
                    using (var conn = new SqlCacheConnection(this._config.ConnectionString))
                    {
                        var sql = string.Empty;
                        var sqlCount = string.Empty;
                        sql = $"SELECT * FROM Users ORDER BY Name";
                        sqlCount = $"SELECT COUNT(*) FROM Users";
                        var query = conn.TypedQuery<User>(sql)
                            .Skip(start).Take(count).ToList();
                        var queryCount = conn.ExecuteScalar<long>(sqlCount);
                        return new PagedList<User>(query) { TotalOfRecords = queryCount };
                    }
                }
                catch (Exception ex)
                {
                    log.Error(scope, ex);
                    throw;
                }
            }
        }

        string IUserAccessService.Login(User user)
        {
            using (var conn = new SqlCacheConnection(this._config.ConnectionString))
            {
                if (conn.Any("Users", $"Provider = '{user.Provider}' AND ExternalId = '{user.ExternalId}'"))
                {
                    var sql = $"SELECT * FROM Users WHERE Provider = '{user.Provider}' AND ExternalId = '{user.ExternalId}'";
                    var item = conn.TypedQuery<User>(sql).FirstOrDefault();
                    item.Token = user.Token;
                    item.Email = user.Email;
                    item.Name = user.Name;
                    item.PhotoUrl = user.PhotoUrl;
                    return conn.Save("Users", item).ToString() + ";" + item.UserTypeId;
                }
                else
                {
                    var item = conn.NewRow("Users");
                    item.UserId = Guid.NewGuid().ToString().ToUpper();
                    item.Provider = user.Provider;
                    item.ExternalId = user.ExternalId;
                    item.Token = user.Token;
                    item.Email = user.Email;
                    item.Name = user.Name;
                    item.PhotoUrl = user.PhotoUrl;
                    item.UserTypeId = 2;
                    item.CreatedAt = DateTime.Now;
                    return conn.Save("Users", item).ToString() + ";" + item.UserTypeId;
                }
            }
        }

        string IUserAccessService.Logout(User user)
        {
            using (var conn = new SqlCacheConnection(this._config.ConnectionString))
            {
                if (conn.Any("Users", $"Provider = '{user.Provider}' AND ExternalId = '{user.ExternalId}'"))
                {
                    var sql = $"SELECT * FROM Users WHERE Provider = '{user.Provider}' AND ExternalId = '{user.ExternalId}'";
                    var item = conn.TypedQuery<User>(sql).FirstOrDefault();
                    item.Token = null;
                    conn.Save("Users", item);
                    return "OK!";
                }
                else
                {
                    return "USER NOT FOUND!";
                }
            }
        }

        dynamic IUserAccessService.Metrics(string userId, string token)
        {
            using (var conn = new SqlCacheConnection(this._config.ConnectionString))
            {
                return new
                {
                    quotes = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Fragments WHERE Type = 0 AND CreatedBy = '{userId}'"),
                    totalQuotes = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Fragments WHERE Type = 0"),
                    notes = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Fragments WHERE Type = 1 AND CreatedBy = '{userId}'"),
                    totalNotes = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Fragments WHERE Type = 1"),
                    books = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Type = 4 AND CreatedBy = '{userId}'"),
                    totalBooks = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Type = 4"),
                    authors = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Type = 2 AND CreatedBy = '{userId}'"),
                    totalAuthors = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Type = 2"),
                    topics = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Type = 7 AND CreatedBy = '{userId}'"),
                    totalTopics = conn.ExecuteScalar<long>($"SELECT COUNT(*) FROM Tags WHERE Type = 7")
                };

            }
        }

        string IUserAccessService.TokenToUserId(string token)
        {
            using (var conn = new SqlCacheConnection(this._config.ConnectionString))
            {
                return conn.ExecuteScalar<object>($"SELECT UserId FROM Users WHERE Token = '{token}'").ToString().ToUpper();
            }
        }

    }
}
