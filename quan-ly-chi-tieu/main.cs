using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

// import data 
// messagebox if user expense of that month is getting higher than monthly spend

namespace quan_ly_chi_tieu
{
    public partial class main : Form
    {
        public int userID;
        private DataAccess DataAccess;
        private ChartControl ChartControl;
        private Helper Helper;

        public main(int userID, string connectionString)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.userID = userID;
            DataAccess = new DataAccess(connectionString);
            Helper = new Helper(connectionString, dataCat, reloadYear, userID);
            ChartControl = new ChartControl(year, chart1);
            exportExpense.SelectedIndex = 0;
            exportIncome.SelectedIndex = 0;
            chartChoice.SelectedIndex = 0;
        }

        private void loader()
        {
            reloadExpenses();
            reloadIncome();
            reloadCategory();
            reloadYear();
        } // bui duc hiep

        private void main_Load(object sender, EventArgs e)
        {
            loader();
        }



        private void updateSearch(DataTable results, DataGridView dtgv)
        {
            if (results.Rows.Count == 0)
            {
                if (dtgv == dataCat) reloadCategory(); 
                if (dtgv == dataIncome) reloadIncome();
                if (dtgv == dataExpense) reloadExpenses();
                return;
            }
            dtgv.DataSource = results;
        } // ha van phong


        // RELOAD DATA - bui duc hiep
        public void reloadExpenses()
        {
            string query = "SELECT e.expID, e.amount, e.description, e.date, c.name FROM expenses e left JOIN categories c ON e.catID = c.catID WHERE e.userID = @userID";
            DataAccess.reloadData(query, dataExpense, userID, year);
        }
        private void reloadCategory()
        {
            string query = "SELECT catID, name, monthlySpend FROM categories WHERE userID = @userID";
            DataAccess.reloadData(query, dataCat, userID, year);
        }
        public void reloadIncome()
        {
            string query = "SELECT i.incID, i.amount, i.description, i.date FROM income i WHERE i.userID = @userID";
            DataAccess.reloadData(query, dataIncome, userID, year);
        }
        private void reloadYear()
        {
            string query = "SELECT DISTINCT YEAR(date) FROM expenses WHERE userID = @userID UNION SELECT DISTINCT YEAR(date) FROM income WHERE userID = @userID";
            DataAccess.reloadData(query, null, userID, year);
        }


        // BUTTON CLICKS
        // ADD - pham van viet
        private void addExpense_Click(object sender, EventArgs e)
        {
            Helper.handleClick(reloadExpenses, "Ngày chi tiêu", "Thêm chi tiêu", userID);
        }
        private void addIncome_Click(object sender, EventArgs e)
        {
            Helper.handleClick(reloadIncome, "Ngày thu nhập", "Thêm thu nhập", userID);
        }
        private void addCat_Click(object sender, EventArgs e)
        {
            Helper.handleClick(reloadCategory, "Ngày chi tiêu", "Thêm loại chi tiêu", userID);
        }


        // EDIT - vu ha vy
        private void editIncome_Click(object sender, EventArgs e)
        {
            int incID = Convert.ToInt32(dataIncome.SelectedCells[0].OwningRow.Cells[0].Value);
            Helper.handleClick(reloadIncome, "Ngày thu nhập", "Sửa thu nhập", incID);
        }
        private void editExpense_Click(object sender, EventArgs e)
        {
            int expID = Convert.ToInt32(dataExpense.SelectedCells[0].OwningRow.Cells[0].Value);
            Helper.handleClick(reloadExpenses, "Ngày chi tiêu", "Sửa chi tiêu", expID);
        }
        private void editCat_Click(object sender, EventArgs e)
        {
            int catID = Convert.ToInt32(dataCat.SelectedCells[0].OwningRow.Cells[0].Value);
            Helper.handleClick(reloadCategory, "Ngày chi tiêu", "Sửa loại chi tiêu", catID);
        }


        // DELETE - bui duc hiep
        private void delExpense_Click(object sender, EventArgs e)
        {
            DataAccess.delClick(dataExpense, "expenses", reloadExpenses);
        }
        private void delIncome_Click(object sender, EventArgs e)
        {
            DataAccess.delClick(dataIncome, "income", reloadIncome);
        }
        private void delCat_Click(object sender, EventArgs e)
        {
            DialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa loại chi tiêu này? Bạn có muốn mọi chi tiêu liên quan sẽ bị xóa?", "Cảnh báo", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (DialogResult == DialogResult.Cancel) return;
            if (DialogResult == DialogResult.Yes)
            {
                DataAccess.delAllRelatedCategories(Convert.ToInt32(dataCat.SelectedCells[0].OwningRow.Cells[0].Value));
            }
            if (DialogResult == DialogResult.No)
            {
                DataAccess.updateAllRelatedCategories(Convert.ToInt32(dataCat.SelectedCells[0].OwningRow.Cells[0].Value));
            }
            DataAccess.delClick(dataCat, "categories", reloadCategory);
            reloadExpenses();
        }


        // DATA CELL CLICK - vu ha vy
        private void dataExpense_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Helper.enableButtons(dataExpense, editExpense, delExpense);
        }
        private void dataIncome_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Helper.enableButtons(dataIncome, editIncome, delIncome);
        }
        private void dataCat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Helper.enableButtons(dataCat, editCat, delCat);
        }


        // DOWNLOAD - pham van viet
        private void downloadExpense_Click(object sender, EventArgs e)
        {
            Helper.downloadFile("ExpenseReport", exportExpense, dataExpense);
        }
        private void downloadIncome_Click(object sender, EventArgs e)
        {
            Helper.downloadFile("IncomeReport", exportIncome, dataIncome);
        }

        private void account_Click(object sender, EventArgs e)
        {
            login loginForm = new login();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                this.userID = loginForm.userID;
                loader();
                chartChoice_SelectedIndexChanged(sender, e);
            }
        } // vu ha vy


        // SEARCH - ha van phong
        private void findIncome_Click(object sender, EventArgs e)
        {
            Helper.handleSearch(dataIncome, updateSearch);
        }
        private void findExpense_Click(object sender, EventArgs e)
        {
            Helper.handleSearch(dataExpense, updateSearch);
        }

        private void chartChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chartChoice.SelectedItem == null) return;
            string selectedItem = chartChoice.SelectedItem.ToString();
            if (selectedItem == "Expense Distribution by Category")
            {
                ChartControl.LoadExpensePieChart(dataExpense);
            }
            else if (selectedItem == "Category Expenses vs. Monthly Spend")
            {
                ChartControl.LoadCategoryComparisonChart(dataExpense, dataCat);
            }
            else if (selectedItem == "Monthly Income Trend")
            {
                ChartControl.LoadMonthlyIncomeChart(dataIncome);
            }
            else if (selectedItem == "Monthly Expense Trend")
            {
                ChartControl.LoadMonthlyExpenseChart(dataExpense);
            }
            else if (selectedItem == "Monthly Income vs. Expenses")
            {
                ChartControl.LoadMonthlyIncomeExpenseChart(dataExpense, dataIncome);
            }
        } // ha van phong

        private void upload_Click(object sender, EventArgs e)
        {
            Helper.uploadFile();
            loader();
        } // pham van viet
    }
}
