namespace SqlCache.SqlBuilder
{
    public class SqlSelect
    {

        public SqlSelect(string select)
        {
            if (select.ToLower().Contains(" as "))
            {
                this.Field = select.Substring(0, select.IndexOf(' '));
                this.Alias = select.Substring(select.LastIndexOf(' ') + 1);
            }
            else
            {
                this.Field = select.TrimEnd(' ');
                this.Alias = select.TrimEnd(' ');
            }
        }

        public string Field { get; set; }

        public string Alias { get; set; }

    }
}
