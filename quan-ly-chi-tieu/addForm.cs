//using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace quan_ly_chi_tieu
{
    public partial class addForm : Form
    {
        private readonly Action reload;
        private readonly Action reloadYear;
        private readonly int ID; // user ID or expense ID / income ID / category ID
        private readonly string connectionString;

        public addForm(string connectionString, Action reload, string label, string button, int ID, List<object> cat, Action reloadYear)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.connectionString = connectionString;
            this.reloadYear = reloadYear;
            this.reload = reload;
            label2.Text = label;
            button1.Text = button;
            Text = button;
            ngay.Value = DateTime.Now;
            this.ID = ID;
            tag.DisplayMember = "name";
            tag.ValueMember = "catID";
            tag.DataSource = cat;
        }


        private void addRecord(decimal amount, string description, DateTime date, int catID, string type)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        if (catID == -1)
                        {
                            command.CommandText = $"INSERT INTO {type} (userID, date, description, amount) VALUES (@userID, @date, @description, @amount)";
                        }
                        else
                        {
                            command.CommandText = $"INSERT INTO expenses (userID, date, description, amount, catID) VALUES (@userID, @date, @description, @amount, @catID)";
                            command.Parameters.AddWithValue("@catID", catID);
                        }
                        command.Parameters.AddWithValue("@userID", this.ID);
                        command.Parameters.AddWithValue("@date", date);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@amount", amount);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void addCat(decimal amount, string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    if (amount == 0)
                    {
                        command.CommandText = $"INSERT INTO categories (name, userID) VALUES (@name, @userID)";
                    }
                    else
                    {
                        command.CommandText = $"INSERT INTO categories (name, userID, monthlySpend) VALUES (@name, @userID, @monthlySpend)";
                        command.Parameters.AddWithValue("@monthlySpend", amount);
                    }
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@userID", this.ID);
                    command.ExecuteNonQuery();
                }
            }
        }



        private void editRecord(decimal amount, string description, DateTime date, int catID, string type)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    string typeID = type == "expenses" ? "expID" : "incID";
                    if (catID == -1)
                    {
                        command.CommandText = $"UPDATE {type} SET date = @date, description = @description, amount = @amount WHERE {typeID} = @ID";
                    }
                    else
                    {
                        command.CommandText = $"UPDATE expenses SET date = @date, description = @description, amount = @amount, catID = @catID WHERE expID = @ID";
                        command.Parameters.AddWithValue("@catID", catID);
                    }
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@ID", this.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        // return data (amount, description, date, catID) from database
        private void getData(string type)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    string typeID = type == "expenses" ? "expID" : "incID";
                    if (type == "expenses")
                    {
                        command.CommandText = $"SELECT amount, description, date, catID FROM {type} WHERE {typeID} = @ID";
                    }
                    else
                    {
                        command.CommandText = $"SELECT amount, description, date FROM {type} WHERE {typeID} = @ID";
                    }
                    command.Parameters.AddWithValue("@ID", this.ID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            txSoTien.Text = reader.GetDecimal(0).ToString();
                            txGhiChu.Text = reader.GetString(1);
                            ngay.Value = reader.GetDateTime(2);
                            if (type == "expenses")
                            {
                                if (!reader.IsDBNull(3)) tag.SelectedValue = reader.GetInt32(3);
                                else tag.SelectedIndex = -1;
                            }
                        }
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Text == "Sửa loại chi tiêu" || this.Text == "Thêm loại chi tiêu")
            {
                if (!string.IsNullOrWhiteSpace(txSoTien.Text) && !decimal.TryParse(txSoTien.Text, out decimal a))
                {
                    MessageBox.Show("Please enter a valid amount");
                    return;
                }
            }
            if (this.Text != "Thêm loại chi tiêu" && this.Text != "Sửa loại chi tiêu")
            {
                if (string.IsNullOrWhiteSpace(txSoTien.Text) || !decimal.TryParse(txSoTien.Text, out decimal a))
                {
                    MessageBox.Show("Please enter a valid amount");
                    return;
                }
            }
            decimal amount = string.IsNullOrWhiteSpace(txSoTien.Text) ? 0 : decimal.Parse(txSoTien.Text);
            try
            {
                int catID;
                if (!tag.Enabled || tag.SelectedValue == null) catID = -1;
                else catID = (int)tag.SelectedValue;
                if (this.Text == "Thêm chi tiêu")
                {
                    addRecord(amount, txGhiChu.Text, ngay.Value, catID, "expenses");
                }
                if (this.Text == "Thêm thu nhập")
                {
                    addRecord(amount, txGhiChu.Text, ngay.Value, catID, "income");
                }
                if (this.Text == "Thêm loại chi tiêu")
                {
                    addCat(amount, tag.Text);
                }
                if (this.Text == "Sửa chi tiêu")
                {
                    editRecord(amount, txGhiChu.Text, ngay.Value, catID, "expenses");
                }
                if (this.Text == "Sửa thu nhập")
                {
                    editRecord(amount, txGhiChu.Text, ngay.Value, catID, "income");
                }
                if (this.Text == "Sửa loại chi tiêu")
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = $"UPDATE categories SET name = @name, monthlySpend = @monthlySpend WHERE catID = @catID";
                            command.Parameters.AddWithValue("@monthlySpend", amount);
                            command.Parameters.AddWithValue("@name", tag.Text);
                            command.Parameters.AddWithValue("@catID", this.ID);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                reload();
                reloadYear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void addForm_Load(object sender, EventArgs e)
        {
            if (this.Text == "Thêm loại chi tiêu")
            {
                label3.Text = "Định mức chi hàng tháng";
                label1.Visible = label2.Visible = txGhiChu.Visible = ngay.Visible = false;
                tag.DropDownStyle = ComboBoxStyle.DropDown;
            }
            if (this.Text == "Sửa chi tiêu") getData("expenses");

            if (this.Text == "Sửa thu nhập" || this.Text == "Thêm thu nhập")
            {
                if (this.Text == "Sửa thu nhập") getData("income");
                tag.Enabled = false;
            }
            if (this.Text == "Sửa loại chi tiêu")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT name, monthlySpend FROM categories WHERE catID = @ID";
                        command.Parameters.AddWithValue("@ID", this.ID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(1)) txSoTien.Text = reader.GetDecimal(1).ToString();
                                tag.Text = reader.GetString(0);
                            }
                        }
                    }
                }
                label3.Text = "Định mức chi hàng tháng";
                label1.Visible = label2.Visible = txGhiChu.Visible = ngay.Visible = false;
            }
        }

        //public void ReappearWindow()
        //{
        //    this.WindowState = FormWindowState.Minimized;
        //    this.Show();
        //    this.WindowState = FormWindowState.Normal;
        //}

    }
}
