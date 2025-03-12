using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace quan_ly_chi_tieu
{
    public partial class search : Form // ha van phong
    {
        private readonly Action<DataTable, DataGridView> reload;
        private readonly DataGridView dtgv;
        private readonly DataTable dt;
        private readonly List<object> cat;
        private readonly string type;
        public search(Action<DataTable, DataGridView> reload, DataGridView dtgv, List<object> cat, string type)
        {
            InitializeComponent();
            this.reload = reload;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.dtgv = dtgv;
            this.cat = cat;
            this.type = type;
            this.dt = dataCopy();
        }

        private DataTable dataCopy()
        {
            return (DataTable)dtgv.DataSource;
        }

        private DataTable makeTable(DataGridView dtgv)
        {
            DataTable table = new DataTable();
            foreach (DataGridViewColumn column in dtgv.Columns)
            {
                table.Columns.Add(column.HeaderText, column.ValueType);
            }
            return table;
        }
 

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txSoTien1.Text))
            {
                if (!decimal.TryParse(txSoTien1.Text, out decimal a))
                {
                    MessageBox.Show("Please enter a valid amount");
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(txSoTien2.Text))
            {
                if (!decimal.TryParse(txSoTien2.Text, out decimal a))
                {
                    MessageBox.Show("Please enter a valid amount");
                    return;
                }
            }
            
            if (dtgv.DataSource == null)
            {
                MessageBox.Show("Data source is null.");
                return;
            }

            DataTable results = makeTable(dtgv);
            foreach (DataRow row in dt.Rows)
            {
                if (row[0] is null) continue; // skip empty rows
                bool shouldAdd = true;

                if (!string.IsNullOrWhiteSpace(txSoTien1.Text) && Convert.ToDecimal(row[1]) < decimal.Parse(txSoTien1.Text))
                {
                        shouldAdd = false;
                 }
                    if (!string.IsNullOrWhiteSpace(txSoTien2.Text) && Convert.ToDecimal(row[1]) > decimal.Parse(txSoTien2.Text))
                    {
                        shouldAdd = false;
                    }
                    if (checkBox1.Checked && Convert.ToDateTime(row[3]) < dateTimePicker1.Value)
                    {
                        shouldAdd = false;
                    }
                    if (checkBox2.Checked && Convert.ToDateTime(row[3]) > dateTimePicker2.Value)
                    {
                        shouldAdd = false;
                    }

                    if (tags.CheckedItems.Count > 0 && !tags.CheckedItems.Contains(row[4].ToString()) && type == "dataExpense")
                    {
                        shouldAdd = false;
                    }

                    if (!string.IsNullOrWhiteSpace(txGhiChu.Text) && !row[2].ToString().Contains(txGhiChu.Text))
                    {
                        shouldAdd = false;
                    }
                if (shouldAdd)
                {
                    DataRow newRow = results.NewRow();
                    for (int i = 0; i < dtgv.Columns.Count; i++) newRow[i] = row[i];
                    results.Rows.Add(newRow);
                }
            }
            reload(results, dtgv);
            if (results.Rows.Count == 0) MessageBox.Show("No results found");
        }

        private void search_Load(object sender, EventArgs e)
        {
            if (type != "dataExpense") tags.Enabled = false;
            List<string> names = new List<string>();
            foreach (var a in cat)
            {
                names.Add(a.GetType().GetProperty("name").GetValue(a, null).ToString());
            }
            tags.DataSource = names;
        }
    }
}
