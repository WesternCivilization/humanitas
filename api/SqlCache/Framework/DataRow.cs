namespace SqlCache.Framework
{

    public class DataRow
    {
        internal DataRow()
        {
        }

        internal DataRow(object id, string label, string fileName)
        {
            this.Id = id;
            this.Label = label;
            this.FileName = fileName;
        }

        public object Id { get; set; }

        public int Version { get; set; }

        public long DataStart { get; set; }

        public long DataEnd { get; set; }

        public string Label { get; set; }

        public long HeaderStart { get; set; }

        public long HeaderEnd { get; set; }

        public long IndexesStart { get; set; }

        public long IndexesEnd { get; set; }

        public long ImageStart { get; set; }

        public long ImageEnd { get; set; }

        public string FileName { get; set; }

        public override string ToString()
        {
            return $"[{this.Id}] {this.Label} ({this.DataStart},{this.DataEnd})";
        }

    }
}
