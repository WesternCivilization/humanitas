using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;

namespace Logging
{
    public class Logger
    {
        private Type type;

        private static Dictionary<string, Stopwatch> _watches = new Dictionary<string, Stopwatch>();
        private static List<LogScope> _scopes = new List<LogScope>();

        public Type Type
        {
            get { return this.type; }
        }

        public Logger(Type type)
        {
            this.type = type;
        }

        public void Info(LogScope scope, string msg)
        {
            LogTracer.Instance.Info?.Invoke(this, scope, msg, null);
        }

        public void Info(LogScope scope, string msg, Stopwatch watch)
        {
            LogTracer.Instance.Info?.Invoke(this, scope, msg, watch);
        }

        public void Warn(LogScope scope, string msg)
        {
            LogTracer.Instance.Warn?.Invoke(this, scope, msg, null);
        }

        public void Warn(LogScope scope, string msg, Stopwatch watch)
        {
            LogTracer.Instance.Warn?.Invoke(this, scope, msg, watch);
        }

        public void Error(LogScope scope, string msg, Exception ex, Stopwatch watch)
        {
            LogTracer.Instance.Error?.Invoke(this, scope, msg, ex, watch);
        }

        public void Error(LogScope scope, string msg, Exception ex)
        {
            LogTracer.Instance.Error?.Invoke(this, scope, msg, ex, null);
        }

        public void Error(LogScope scope, Exception ex)
        {
            LogTracer.Instance.Error?.Invoke(this, scope, "Error in " + scope.Name, ex, null);
        }

        // TODO: headers
        public LogScope Scope(string name, HttpRequestHeaders headers)
        {
            var scope = new LogScope(this.type.Name + ">" + name, Thread.CurrentThread.ManagedThreadId);
            scope.Origin = headers?.Referrer?.ToString() + " [" + string.Join(";", headers?.UserAgent) + "]";
            this.BeginStopwatch(scope, name);
            scope.OnDispose = () =>
            {
                this.EndStopwatch(scope, name);
            };
            return scope;
        }

        public LogScope Scope(string name)
        {
            var scope = new LogScope(this.type.Name + ">" + name, Thread.CurrentThread.ManagedThreadId);
            lock (_scopes)
            {
                var origin = _scopes.FirstOrDefault(f => f.ThreadId == scope.ThreadId && !string.IsNullOrEmpty(f.Origin));
                scope.Level = _scopes.Count(f => f.ThreadId == scope.ThreadId);
                if (origin != null)
                {
                    scope.Origin = origin?.Origin;
                }
            }
            this.BeginStopwatch(scope, name);
            scope.OnDispose = () =>
            {
                this.EndStopwatch(scope, name);
            };
            return scope;
        }

        private void BeginStopwatch(LogScope scope, string process)
        {
            lock (_watches)
            {
                _watches[process + "~" + Thread.CurrentThread.ManagedThreadId] = new System.Diagnostics.Stopwatch();
                _watches[process + "~" + Thread.CurrentThread.ManagedThreadId].Start();
            }
            lock (_scopes)
            {
                _scopes.Add(scope);
            }
            this.Info(scope, $"{process} has started...");
        }

        public void EndStopwatch(LogScope scope, string process)
        {
            var name = process + "~" + Thread.CurrentThread.ManagedThreadId;
            if (_watches.ContainsKey(name))
            {
                var watch = _watches[process + "~" + Thread.CurrentThread.ManagedThreadId];
                watch.Stop();
                this.Info(scope, $"{process} has completed [{watch.Elapsed}].", watch);
                lock (_watches)
                {
                    _watches.Remove(process + "~" + Thread.CurrentThread.ManagedThreadId);
                }
                lock (_scopes)
                {
                    _scopes.RemoveAll(f => f.Id == scope.Id);
                }
            }
        }

    }

    public class LogScope : IDisposable
    {

        public LogScope(string process, int threadId)
        {
            this.Id = Guid.NewGuid();
            this.Name = process;
            this.ThreadId = ThreadId;
        }

        public int ThreadId { get; set; }

        public Guid Id { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public Action OnDispose { get; set; }

        public string Origin { get; internal set; }

        public void Dispose()
        {
            this.OnDispose?.Invoke();
            this.OnDispose = null;
        }

    }
}
