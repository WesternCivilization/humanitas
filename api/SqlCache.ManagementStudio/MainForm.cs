using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SqlCache.ManagementStudio
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private SqlCacheConnection conn;

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.gbExplorer.Enabled = false;
            this.gbQuery.Enabled = false;
            this.gbResults.Enabled = false;
            this.btnConnect.Focus();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            conn = new SqlCacheConnection(this.txtConnectionString.Text);
            var tables = conn.Tables();
            this.tvExplorer.Nodes.Clear();
            foreach (var table in tables)
            {
                this.tvExplorer.Nodes.Add($"{table.Key} ({table.Value})");
            }
            this.gbExplorer.Enabled = true;
            this.gbQuery.Enabled = true;
            this.gbResults.Enabled = true;
            this.txtQuery.Focus();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.lvResults.Columns.Clear();
            this.lvResults.Items.Clear();
            var results = conn.Query(this.txtQuery.Text);
            var header = results.FirstOrDefault();
            if (header != null)
            {
                foreach (JProperty col in header)
                {
                    var colItem = this.lvResults.Columns.Add(col.Name);
                    colItem.Width = 120;
                }
            }
            foreach (var row in results)
            {
                ListViewItem item = null;
                foreach (JProperty col in header)
                {
                    if (item == null)
                    {
                        item = this.lvResults.Items.Add(GetFieldValue(row, col.Name));
                    }
                    else
                    {
                        item.SubItems.Add(GetFieldValue(row, col.Name));
                    }
                }
            }
        }

        private string GetFieldValue(JObject row, string name)
        {
            return row[name].ToString();
        }

        private void tvExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node != null)
            {
                var name = e.Node.Text.Split('(').FirstOrDefault().Trim();
                this.txtQuery.Text = "SELECT TOP 1 * FROM " + name;
                this.btnRun_Click(sender, e);
            }
        }
    }
}
