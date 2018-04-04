using System;

namespace SqlCache.Framework
{
    public class DataRowHistory
    {

        internal DataRowHistory()
        { }

        public object Id { get; set; }

        public int Version { get; set; }

        public long DataStart { get; set; }

        public long DataEnd { get; set; }

        public DateTime EventDate { get; set; }

        public string Action { get; set; }

        public long HeaderStart { get; set; }

    }
}
