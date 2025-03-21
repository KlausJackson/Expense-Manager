using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace quan_ly_chi_tieu
{
    internal class Helper
    {
        private DataAccess DataAccess;
        private DataGridView dataCat;
        private string connectionString;
        private readonly Action reloadYear;
        private readonly int userID;

        public Helper(string connectionString, DataGridView dataCat, Action reloadYear, int userID)
        {
            DataAccess = new DataAccess(connectionString);
            this.connectionString = connectionString;
            this.dataCat = dataCat;
            this.reloadYear = reloadYear;
            this.userID = userID;
        }

        public void checkOpenForm(Type form)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.GetType() == form)
                {
                    openForm.Close();
                    return;
                }
            }
        } // vu ha vy

        public void handleClick(Action reload, string s1, string s2, int id)
        {
            checkOpenForm(typeof(addForm));
            List<object> list = DataAccess.getCat(dataCat);
            addForm newForm = new addForm(connectionString, reload, s1, s2, id, list, reloadYear);
            newForm.Show();
        } // pham van viet

        public void handleSearch(DataGridView dtgv, Action<DataTable, DataGridView> updateSearch)
        {
            checkOpenForm(typeof(search));
            List<object> list = DataAccess.getCat(dataCat);
            string type = dtgv.Name;
            search newForm = new search(updateSearch, dtgv, list, type);
            newForm.Show();
        } // ha van phong


        public void enableButtons(DataGridView dtgv, System.Windows.Forms.Button edit, System.Windows.Forms.Button del)
        {
            if (dtgv.SelectedCells.Count == 0)
            {
                edit.Enabled = false;
                del.Enabled = false;
            }
            edit.Enabled = true;
            del.Enabled = true;
        } // vu ha vy


        // pham van viet
        public void downloadFile(string file, System.Windows.Forms.ComboBox cb, DataGridView dtgv)
        {
            string filetype = cb.SelectedItem.ToString();
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = file;

            if (filetype == "Excel")
            {
                try
                {
                    saveFileDialog.DefaultExt = ".xlsx";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                        {
                            using (IWorkbook workbook = new XSSFWorkbook())
                            {
                                ISheet sheet = workbook.CreateSheet("Expenses");
                                IRow headerRow = sheet.CreateRow(0);
                                for (int i = 0; i < dtgv.Columns.Count; i++)
                                {
                                    headerRow.CreateCell(i).SetCellValue(dtgv.Columns[i].HeaderText);
                                }

                                for (int i = 0; i < dtgv.Rows.Count - 1; i++)
                                {
                                    IRow dataRow = sheet.CreateRow(i + 1);
                                    for (int j = 0; j < dtgv.Columns.Count; j++)
                                    {
                                        dataRow.CreateCell(j).SetCellValue(dtgv[j, i].Value?.ToString() ?? "");
                                    }
                                }
                                workbook.Write(fs);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (filetype == "CSV")
            {
                var csv = new StringBuilder();
                // use the headers of the datagridview
                var headers = dtgv.Columns.Cast<DataGridViewColumn>();
                csv.AppendLine(string.Join(",", headers.Select(column => "\"" + column.HeaderText + "\"").ToArray()));
                foreach (DataGridViewRow row in dtgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    var cells = row.Cells.Cast<DataGridViewCell>();
                    csv.AppendLine(string.Join(",", cells.Select(cell => "\"" + cell.Value + "\"").ToArray()));
                }

                saveFileDialog.DefaultExt = ".csv";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, csv.ToString());
                }
            }

            if (filetype == "JSON")
            {
                saveFileDialog.DefaultExt = ".json";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var json = JsonConvert.SerializeObject(dtgv.Rows.Cast<DataGridViewRow>().Where(row => !row.IsNewRow).Select(row => row.Cells.Cast<DataGridViewCell>().ToDictionary(cell => dtgv.Columns[cell.ColumnIndex].Name, cell => cell.Value)));
                    System.IO.File.WriteAllText(saveFileDialog.FileName, json);
                }
            }
            // MessageBox.Show("File downloaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void uploadFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.csv;*.json;*.xlsx;*.xls)|*.csv;*.xlsx;*.xls;*.json|CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                        string filepath = openFileDialog.FileName;
                        DataTable dt = null;
                        if (extension == ".csv") dt = readCSV(filepath);
                        else if (extension == ".json") dt = readJSON(filepath);
                        else if (extension == ".xlsx" || extension == ".xls") dt = readExcel(filepath);

                        if (dt != null)
                        {
                            insertData(updateDT(dt));
                            MessageBox.Show("Data uploaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Clipboard.SetText(ex.Message);
                    }
                }
            }
        }

        private void insertData(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data to insert.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = string.Empty;
                if (dt.Columns.Contains("name"))
                {
                    query = "insert into expenses (amount, description, date, catID, userID) values (@amount, @description, @date, @catID, @userID)";
                }
                else
                {
                    query = "insert into income (amount, description, date, userID) values (@amount, @description, @date, @userID)";
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Create the parameters *once* outside the loop
                    //SqlParameter catIDParam = new SqlParameter("@catID", SqlDbType.Int);
                    //SqlParameter amountParam = new SqlParameter("@amount", SqlDbType.Decimal);
                    //SqlParameter descriptionParam = new SqlParameter("@description", SqlDbType.NVarChar, 255);
                    //SqlParameter dateParam = new SqlParameter("@date", SqlDbType.DateTime);
                    //SqlParameter userIDParam = new SqlParameter("@userID", SqlDbType.Int);

                    //command.Parameters.Add(amountParam);
                    //command.Parameters.Add(descriptionParam);
                    //command.Parameters.Add(dateParam);
                    //command.Parameters.Add(userIDParam).Value = userID;

                    //if (query.Contains("@catID"))
                    //{
                    //    command.Parameters.Add(catIDParam);
                    //}

                    foreach (DataRow row in dt.Rows)
                    {
                        //if (query.Contains("@catID"))
                        //{
                        //    if (row[4].ToString() == "0")
                        //        catIDParam.Value = DBNull.Value;
                        //    else
                        //        catIDParam.Value = int.Parse(row[4].ToString());
                        //}

                        //amountParam.Value = decimal.Parse(row[1].ToString());
                        //descriptionParam.Value = row[2].ToString();
                        //dateParam.Value = row[3].ToString();

                        command.Parameters.Clear();
                        if (query.Contains("@catID"))
                        {
                            if (row[4].ToString() == "0") command.Parameters.AddWithValue("@catID", DBNull.Value);
                            else command.Parameters.AddWithValue("@catID", int.Parse(row[4].ToString()));
                        }
                        command.Parameters.AddWithValue("@amount", decimal.Parse(row[1].ToString())); // remove the double quotes
                        command.Parameters.AddWithValue("@description", row[2].ToString());
                        command.Parameters.AddWithValue("@date", row[3].ToString());
                        command.Parameters.AddWithValue("@userID", userID);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private DataTable readCSV(string filepath)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var reader = new StreamReader(filepath))
                {
                    string[] headers = reader.ReadLine().Split(',');
                    foreach (var header in headers) dt.Columns.Add(header.Trim().Replace("\"", ""));

                    while (!reader.EndOfStream)
                    {
                        string[] data = reader.ReadLine().Split(',');
                        dt.Rows.Add(data.Select(d => d.Trim().Replace("\"", "")).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading CSV file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        private DataTable updateDT(DataTable dt)
        {
            if (dt == null) return null;
            if (!dt.Columns.Contains("name")) return dt;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataRow row in dt.Rows)
                {
                    if (row[4].ToString() == "0") continue;
                    string query = "select catID from categories where name = @name and userID = @userID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", row[4].ToString());
                        command.Parameters.AddWithValue("@userID", userID);
                        var catID = command.ExecuteScalar();
                        if (catID == null)
                        {
                            query = "insert into categories (name, userID) values (@name, @userID)";
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                            query = "select catID from categories where name = @name";
                            command.CommandText = query;
                            catID = command.ExecuteScalar();
                        }
                        row[4] = catID;
                    }
                }
            }
            return dt;
        }


        private DataTable readJSON(string filepath)
        {
            try
            {
                string content = File.ReadAllText(filepath);
                //if (string.IsNullOrEmpty(content))
                //{
                //    MessageBox.Show("JSON file is empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return null;
                //}
                return JsonConvert.DeserializeObject<DataTable>(content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading JSON file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private DataTable readExcel(string filepath)
        {
            DataTable dt = new DataTable();
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook;
                    if (Path.GetExtension(filepath).ToLower() == ".xlsx") workbook = new XSSFWorkbook(fs);
                    else workbook = new HSSFWorkbook(fs);

                    ISheet sheet = workbook.GetSheetAt(0); // Get the first sheet
                    IRow headerRow = sheet.GetRow(0);

                    for (int i = sheet.FirstRowNum; i < headerRow.LastCellNum; i++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(i);
                        if (cell != null) dt.Columns.Add(cell.ToString());
                    } // add columns

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        DataRow dataRow = dt.NewRow();

                        for (int j = sheet.FirstRowNum; j < headerRow.LastCellNum; j++)
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading Excel file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }
    }
}
