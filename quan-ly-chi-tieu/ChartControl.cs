using System;
using System.Data;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Drawing;


namespace quan_ly_chi_tieu
{
    internal class ChartControl // ha van phong
    {
        private ComboBox year;
        private Chart chart1;
        public ChartControl(ComboBox year, Chart chart)
        {
            this.year = year;
            this.chart1 = chart;
        }
        private void loadChart(string chartName, Series series)
        {
            chart1.Series.Clear();
            chart1.Series.Add(series);
            chart1.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea();
            chart1.ChartAreas.Add(chartArea);
            chart1.Titles.Clear();
            chart1.Titles.Add(chartName);
        }

        private int getYear()
        {
            if (year.SelectedItem == null)
                return 0;
            return int.Parse(year.SelectedItem.ToString());
        }

        public void LoadExpensePieChart(DataGridView dataExpense)
        {
            int selected = getYear();
            // use data in 2 tables: dataExpense and dataCat
            var expensesByCategory = dataExpense.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells[3].Value is DateTime dateTimeValue && dateTimeValue.Year == selected)
                // because datagridview always have an extra empty row at the end
                .GroupBy(row => row.Cells[4].Value.ToString() == "0" ? "Uncategorized" : row.Cells[4].Value.ToString()) // Group by category name
                .Select(g => new { Category = g.Key, TotalExpenses = g.Sum(row => Convert.ToDecimal(row.Cells[1].Value)) }) // Sum up expenses
                .ToList();

            Series series = new Series("Expenses");
            series.ChartType = SeriesChartType.Pie;
            series.XValueType = ChartValueType.String;
            series.YValueType = ChartValueType.Double;
            series.IsValueShownAsLabel = true; // Show percentages on the pie chart
            series.LabelFormat = "{P2}"; // format the percentage to 2 decimal places
            series.Font = new Font("Arial", 14, FontStyle.Bold);

            // chartArea.Area3DStyle.Enable3D = true;
            series.Points.Clear();
            foreach (var item in expensesByCategory)
            {
                series.Points.AddXY(item.Category, item.TotalExpenses / expensesByCategory.Sum(x => x.TotalExpenses));
            }
            loadChart("Expense Distribution by Category", series);
        }


        public void LoadCategoryComparisonChart(DataGridView dataExpense, DataGridView dataCat)
        {
            int selected = getYear();
            // use data in 2 tables: dataExpense and dataCat
            var categoryData = dataCat.Rows.Cast<DataGridViewRow>()
                .Where(row => Convert.ToDecimal(row.Cells[2].Value) > 0) // Filter out categories with 0 monthly spend
                .Select(row => new
                {
                    name = row.Cells[1].Value, // Category name
                    monthlySpend = Convert.ToDecimal(row.Cells[2].Value), // Monthly spend
                    TotalExpenses = dataExpense.Rows.Cast<DataGridViewRow>() // Total expenses
                        .Where(exp => exp.Cells[4]?.Value?.ToString() == row.Cells[1]?.Value?.ToString()
                                    && exp.Cells[3].Value is DateTime dateTimeValue
                                    && dateTimeValue.Year == selected) // Filter by category and year
                        .Sum(exp => Convert.ToDecimal(exp.Cells[1].Value)) // Sum up expenses
                })
                .ToList();

            chart1.Series.Clear();
            Series monthlySpend = new Series("Monthly Spend");
            monthlySpend.ChartType = SeriesChartType.Column;
            monthlySpend.Color = Color.Green;
            monthlySpend.XValueType = ChartValueType.String;
            monthlySpend.YValueType = ChartValueType.Double;

            monthlySpend.Points.Clear();
            foreach (var item in categoryData)
            {
                monthlySpend.Points.AddXY(item.name, item.monthlySpend);
            }


            chart1.Series.Add(monthlySpend);

            Series totalExpenses = new Series("Total Expenses");
            totalExpenses.ChartType = SeriesChartType.Column;
            totalExpenses.Color = Color.Red;
            totalExpenses.XValueType = ChartValueType.String;
            totalExpenses.YValueType = ChartValueType.Double;

            totalExpenses.Points.Clear();
            foreach (var item in categoryData)
            {
                totalExpenses.Points.AddXY(item.name, item.TotalExpenses);
            }

            chart1.Series.Add(totalExpenses);

            chart1.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea();
            chart1.ChartAreas.Add(chartArea);

            chart1.Titles.Clear();
            chart1.Titles.Add("Category Expenses vs. Monthly Spend");
        }


        public void LoadMonthlyIncomeChart(DataGridView dataIncome)
        {
            int selected = getYear();
            // use data in dataIncome
            var monthlyIncome = dataIncome.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells[3].Value is DateTime dateTimeValue && dateTimeValue.Year == selected)
                .GroupBy(row => Convert.ToDateTime(row.Cells[3].Value).Month) // Group by month
                .Select(g => new
                {
                    Month = g.Key,
                    TotalIncome = g.Sum(row => Convert.ToDecimal(row.Cells[1].Value))
                }) // Sum up income
                .OrderBy(g => g.Month)
                .ToList();

            Series series = new Series("Monthly Income");
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Int32;
            series.YValueType = ChartValueType.Double;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.BorderWidth = 2;

            series.Points.Clear();

            foreach (var item in monthlyIncome)
            {
                series.Points.AddXY(item.Month, item.TotalIncome);
            }

            loadChart("Monthly Income Trend", series);
        }

        public void LoadMonthlyExpenseChart(DataGridView dataExpense)
        {
            int selected = getYear();
            var monthlyExpense = dataExpense.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells[3].Value is DateTime dateTimeValue && dateTimeValue.Year == selected)
                .GroupBy(row => Convert.ToDateTime(row.Cells[3].Value).Month) // Group by month
                .Select(g => new
                {
                    Month = g.Key,
                    TotalExpense = g.Sum(row => Convert.ToDecimal(row.Cells[1].Value))
                }) // Sum up expenses
                .OrderBy(g => g.Month)
                .ToList();

            chart1.Series.Clear();
            Series series = new Series("Monthly Expense");
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Int32;
            series.YValueType = ChartValueType.Double;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.BorderWidth = 2;

            series.Points.Clear();
            foreach (var item in monthlyExpense)
            {
                series.Points.AddXY(item.Month, item.TotalExpense);
            }
            loadChart("Monthly Expense Trend", series);

        }

        public void LoadMonthlyIncomeExpenseChart(DataGridView dataExpense, DataGridView dataIncome)
        {
            int selected = getYear();

            var monthlyExpenses = dataExpense.Rows.Cast<DataGridViewRow>()
                .Where(exp => exp.Cells[3].Value is DateTime dateTimeValue && dateTimeValue.Year == selected)
                .GroupBy(exp => ((DateTime)exp.Cells[3].Value).Month) // Group by month
                .Select(group => new
                {
                    Month = group.Key,
                    TotalExpenses = group.Sum(exp => Convert.ToDecimal(exp.Cells[1].Value))
                })
                .OrderBy(item => item.Month)
                .ToList();

            var monthlyIncome = dataIncome.Rows.Cast<DataGridViewRow>()
                .Where(inc => inc.Cells[3].Value is DateTime dateTimeValue && dateTimeValue.Year == selected)
                .GroupBy(inc => ((DateTime)inc.Cells[3].Value).Month) // Group by month
                .Select(group => new
                {
                    Month = group.Key,
                    TotalIncome = group.Sum(inc => Convert.ToDecimal(inc.Cells[1].Value))
                })
                .OrderBy(item => item.Month)
                .ToList();

            var allMonths = Enumerable.Range(1, 12).ToList();

            var monthlyData = allMonths.GroupJoin(
                    monthlyExpenses,
                    month => month,
                    expense => expense.Month,
                    (month, expenses) => new
                    {
                        Month = month,
                        TotalExpenses = expenses.Sum(e => (decimal?)e.TotalExpenses) ?? 0,
                    }
                ).GroupJoin(
                    monthlyIncome,
                    monthly => monthly.Month,
                    income => income.Month,
                    (monthly, incomes) => new
                    {
                        Month = monthly.Month,
                        TotalExpenses = monthly.TotalExpenses,
                        TotalIncome = incomes.Sum(i => (decimal?)i.TotalIncome) ?? 0
                    }
                ).OrderBy(m => m.Month)
                .ToList();

            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Titles.Clear();

            ChartArea chartArea = new ChartArea();
            chart1.ChartAreas.Add(chartArea);

            chart1.Titles.Add($"Monthly Income vs. Expenses - {selected}");

            Series expensesSeries = new Series("Expenses");
            expensesSeries.ChartType = SeriesChartType.Column;
            expensesSeries.Color = Color.Red;
            expensesSeries.XValueType = ChartValueType.String;
            expensesSeries.YValueType = ChartValueType.Double;
            expensesSeries.IsValueShownAsLabel = true;
            chart1.Series.Add(expensesSeries);

            Series incomeSeries = new Series("Income");
            incomeSeries.ChartType = SeriesChartType.Column;
            incomeSeries.Color = Color.Green;
            incomeSeries.XValueType = ChartValueType.String;
            incomeSeries.YValueType = ChartValueType.Double;
            incomeSeries.IsValueShownAsLabel = true;
            chart1.Series.Add(incomeSeries);

            foreach (var item in monthlyData)
            {
                expensesSeries.Points.AddXY(MonthName(item.Month), item.TotalExpenses);
                incomeSeries.Points.AddXY(MonthName(item.Month), item.TotalIncome);
            }
        }
        private string MonthName(int month)
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }


    }
}
