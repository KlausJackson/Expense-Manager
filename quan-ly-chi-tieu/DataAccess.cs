using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace quan_ly_chi_tieu
{
    internal class DataAccess
    {
        private string connectionString;
        public DataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<object> getCat(DataGridView dataCat)
        {
            List<object> results = new List<object>();
            foreach (DataGridViewRow row in dataCat.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    results.Add(new
                    {
                        catID = row.Cells[0].Value,
                        name = row.Cells[1].Value
                    });
                }
            }
            return results;
        } // vu ha vy

        public void reloadData(string query, DataGridView dtgv, int userID, ComboBox year)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);
                        if (dtgv is null)
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                List<int> years = new List<int>();
                                while (reader.Read()) years.Add(reader.GetInt32(0));
                                year.DataSource = years;
                            }
                            return;
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int i = 0; i < dt.Columns.Count; i++)
                                {
                                    if (row[i] == DBNull.Value) row[i] = 0; // replace null with 0
                                }
                            }
                            dtgv.DataSource = dt;
                            dtgv.Columns[0].Visible = false;
                            //if (query.Contains("expenses"))
                            //{
                            //    dtgv.Columns[5].Visible = false;
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // bui duc hiep

        public void delClick(DataGridView dtgv, string table, Action reload)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No) return;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string tableID = table.Substring(0, 3) + "ID";
                    string query = $"DELETE FROM {table} WHERE {tableID} = @id";
                    int idToDelete = Convert.ToInt32(dtgv.SelectedCells[0].OwningRow.Cells[0].Value);
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", idToDelete);
                        command.ExecuteNonQuery();
                    }
                }
                reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // bui duc hiep

        private void workWithCat(string query, int catID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@catID", catID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } // bui duc hiep

        // bui duc hiep
        public void delAllRelatedCategories(int catID)
        {
            workWithCat("DELETE FROM categories WHERE catID = @catID", catID);
        }

        public void updateAllRelatedCategories(int catID)
        {
            workWithCat("UPDATE expenses SET catID = NULL WHERE catID = @catID", catID);
        }
    }
}
