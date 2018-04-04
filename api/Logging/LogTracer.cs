using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Logging
{
    public class LogTracer
    {

        static private LogTracer _instance = null;

        static public LogTracer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LogTracer();
                return _instance;
            }
        }

        public object Sync { get; set; }

        public string LogFolder { get; set; }

        private LogTracer()
        {
            this.Sync = new object();
            this.LogFolder = ConfigurationManager.AppSettings["LogFolder"];
        }

        public void Start()
        {
            this.Info = (logger, scope, msg, elapsed) =>
            {
                this.Log(logger, LogType.Info, msg, scope.Name, null, elapsed, scope.Origin, scope.Level);
            };
            this.Warn = (logger, scope, msg, elapsed) =>
            {
                this.Log(logger, LogType.Warn, msg, scope.Name, null, elapsed, scope.Origin, scope.Level);
            };
            this.Error = (logger, scope, msg, ex, elapsed) =>
            {
                this.Log(logger, LogType.Error, msg, scope.Name, ex, elapsed, scope.Origin, scope.Level);
            };
        }

        public void Stop()
        {
            this.Info = null;
            this.Warn = null;
            this.Error = null;
        }

        private void Log(Logger logger, LogType type, string msg, string process = null, Exception ex = null, Stopwatch elapsed = null, string origin = null, int? level = null)
        {
            if (elapsed != null)
            {
                var path = Path.Combine(this.LogFolder, $"{DateTime.Now.ToString("yyyyMMdd_HH")}.log");
                var line = new LogEvent
                {
                    EventDate = DateTime.Now,
                    Namespace = logger.Type.Namespace,
                    ClassName = logger.Type.Name,
                    Process = process,
                    Type = type,
                    Message = msg,
                    Exception = ex,
                    Elapsed = elapsed.Elapsed,
                    Origin = origin,
                    Level = level
                };
                lock (this.Sync)
                {
                    File.AppendAllText(path, line.ToString() + "|;|");
                }
            }
        }

        internal Action<Logger, LogScope, string, Stopwatch> Info { get; set; }

        internal Action<Logger, LogScope, string, Stopwatch> Warn { get; set; }

        internal Action<Logger, LogScope, string, Exception, Stopwatch> Error { get; set; }

    }

    public class LogEvent
    {

        public DateTime EventDate { get; internal set; }

        public LogType Type { get; internal set; }

        public string Namespace { get; internal set; }

        public string ClassName { get; internal set; }

        public string Process { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public string Message { get; internal set; }

        public string Origin { get; internal set; }

        public Exception Exception { get; internal set; }

        public int? Level { get; internal set; }

        public override string ToString()
        {
            return $"{this.EventDate.ToString("yyyy-MM-dd HH:mm:ss")}|~|{this.Type}|~|{this.Namespace}|~|{this.ClassName}|~|{this.Process}|~|{(this.Elapsed != null ? this.Elapsed.TotalMilliseconds : 0)}|~|{this.Message}|~|{(this.Exception != null ? this.Exception.ToString() : string.Empty)}|~|{this.Origin}|~|{this.Level}";
        }

    }

    public enum LogType
    {
        Info,
        Warn,
        Error
    }

}
