using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NotSoEpicApp
{
    public partial class Charts : Page
    {
        public Charts()
        {
            InitializeComponent();
            UpdateCharts("Last Month");
        }

        private void Transactions_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Transactions());
        }

        private void Time_Range_Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)Time_Range_Box.SelectedItem;
            string selectedPeriod = selectedItem.Tag.ToString();

            UpdateCharts(selectedPeriod);
        }

        private void UpdateCharts(string period)
        {
            DateTime startDate;
            DateTime endDate = DateTime.Now;

            switch (period)
            {
                case "LastMonth":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "LastQuarter":
                    startDate = DateTime.Now.AddMonths(-3);
                    break;
                case "LastYear":
                    startDate = DateTime.Now.AddYears(-1);
                    break;
                case "Lifetime":
                    startDate = DateTime.MinValue;
                    break;
                default:
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
            }

            var filteredTransactions = Login.transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToList();

            UpdateIncomeVsExpensesChart(filteredTransactions);
            UpdateIncomesByCategoryChart(filteredTransactions);
            UpdateExpensesByCategoryChart(filteredTransactions);
            UpdateBalanceOverTimeChart(filteredTransactions);
            UpdateIncomesVsExpensesOverTimeChart(filteredTransactions);
        }


        private void UpdateIncomeVsExpensesChart(System.Collections.Generic.List<Transaction> transactions)
        {
            var totalIncome = transactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            var totalExpenses = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

            var pieChartModel = new PlotModel { Title = "Income vs Expenses" };
            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                AngleSpan = 360,
                StartAngle = 0,
                TrackerFormatString = "{1}: {2:C}"
            };

            if (totalIncome == 0 && totalExpenses == 0)
            {
                pieSeries.Slices.Add(new PieSlice("No Data Available", 1) { Fill = OxyColors.Gray });
            }
            else
            {
                if (totalIncome > 0)
                    pieSeries.Slices.Add(new PieSlice("Income", Math.Abs((double)totalIncome)) { Fill = OxyColors.Green });
                if (totalExpenses > 0)
                    pieSeries.Slices.Add(new PieSlice("Expenses", Math.Abs((double)totalExpenses)) { Fill = OxyColors.Red });
            }

            pieChartModel.Series.Add(pieSeries);
            IncomeVsExpensesPieChartPlotView.Model = pieChartModel;
        }

        private void UpdateIncomesByCategoryChart(System.Collections.Generic.List<Transaction> transactions)
        {
            var incomesByCategory = transactions
                .Where(t => t.IsIncome)
                .GroupBy(t => t.Type)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .ToList();

            var pieChartModel = new PlotModel { Title = "Incomes by Category" };
            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                AngleSpan = 360,
                StartAngle = 0,
                TrackerFormatString = "{1}: {2:C}"
            };

            if (!incomesByCategory.Any())
            {
                pieSeries.Slices.Add(new PieSlice("No Data Available", 1) { Fill = OxyColors.Gray });
            }
            else
            {
                foreach (var income in incomesByCategory)
                {
                    OxyColor color = GetColorForCategory(income.Category.ToString());
                    pieSeries.Slices.Add(new PieSlice(income.Category.ToString(), Math.Abs((double)income.Total)) { Fill = color });
                }
            }

            pieChartModel.Series.Add(pieSeries);
            IncomesByCategoryPieChartPlotView.Model = pieChartModel;
        }

        private void UpdateExpensesByCategoryChart(System.Collections.Generic.List<Transaction> transactions)
        {
            var expensesByCategory = transactions
                .Where(t => !t.IsIncome)
                .GroupBy(t => t.Type)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .ToList();

            var pieChartModel = new PlotModel { Title = "Expenses by Category" };
            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                AngleSpan = 360,
                StartAngle = 0,
                TrackerFormatString = "{1}: {2:C}"
            };

            if (!expensesByCategory.Any())
            {
                pieSeries.Slices.Add(new PieSlice("No Data Available", 1) { Fill = OxyColors.Gray });
            }
            else
            {
                foreach (var expense in expensesByCategory)
                {
                    OxyColor color = GetColorForCategory(expense.Category.ToString());
                    pieSeries.Slices.Add(new PieSlice(expense.Category.ToString(), Math.Abs((double)expense.Total)) { Fill = color });
                }
            }

            pieChartModel.Series.Add(pieSeries);
            ExpensesByCategoryPieChartPlotView.Model = pieChartModel;
        }


        private OxyColor GetColorForCategory(string category)
        {
            switch (category)
            {
                case "Entertainment":
                    return OxyColors.Purple;
                case "Healthcare":
                    return OxyColors.Teal;
                case "Insurance":
                    return OxyColors.Orange;
                case "Groceries":
                    return OxyColors.Green;
                case "Gift":
                    return OxyColors.Pink;
                case "Tax":
                    return OxyColors.Red;
                case "Rent":
                    return OxyColors.Blue;
                case "Shopping":
                    return OxyColors.Yellow;
                case "Other":
                    return OxyColors.Gray;
                default:
                    return OxyColors.Gray;
            }
        }

        private void UpdateBalanceOverTimeChart(System.Collections.Generic.List<Transaction> transactions)
        {
            var balanceOverTimeModel = new PlotModel { Title = "Balance Over Time" };

            if (!transactions.Any())
            {
                BalanceOverTimeLineChartPlotView.Model = balanceOverTimeModel;
                return;
            }

            DateTime startDate = transactions.Min(t => t.Date);
            DateTime endDate = transactions.Max(t => t.Date);

            if (startDate == endDate)
            {
                endDate = startDate.AddDays(1);
            }

            TimeSpan dateRange = endDate - startDate;

            bool showYear = dateRange.TotalDays > 90;

            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = showYear ? "MM/dd/yyyy" : "MM/dd",
                Title = "Date",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80,
                LabelFormatter = dt => DateTimeAxis.ToDateTime(dt).ToString(showYear ? "MM/dd/yyyy" : "MM/dd"),
                MajorStep = Math.Max(dateRange.TotalDays / 8.0, 1)
            };
            balanceOverTimeModel.Axes.Add(dateTimeAxis);

            var stairStepSeries = new StairStepSeries
            {
                Title = "Balance",
                Color = OxyColors.Blue,
                TrackerFormatString = "Date: {2:MM/dd/yyyy}\nBalance: {4:#,##0.00}$"
            };

            decimal currentBalance = 0;

            stairStepSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startDate), (double)currentBalance));

            foreach (var transaction in transactions.OrderBy(t => t.Date))
            {
                currentBalance += transaction.IsIncome ? transaction.Amount : -transaction.Amount;
                stairStepSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(transaction.Date), (double)currentBalance));
            }

            stairStepSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(endDate), (double)currentBalance));

            balanceOverTimeModel.Series.Add(stairStepSeries);
            BalanceOverTimeLineChartPlotView.Model = balanceOverTimeModel;
        }

        private void UpdateIncomesVsExpensesOverTimeChart(System.Collections.Generic.List<Transaction> transactions)
        {
            var model = new PlotModel { Title = "Incomes vs Expenses Over Time" };

            if (!transactions.Any())
            {
                IncomesVsExpensesOverTimeLineChartPlotView.Model = model;
                return;
            }

            DateTime startDate = transactions.Min(t => t.Date);
            DateTime endDate = transactions.Max(t => t.Date);

            if (startDate == endDate)
            {
                endDate = startDate.AddDays(1);
            }

            TimeSpan dateRange = endDate - startDate;
            bool showYear = dateRange.TotalDays > 90;

            var dateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = showYear ? "MM/dd/yyyy" : "MM/dd",
                Title = "Date",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IntervalLength = 80,
                LabelFormatter = dt => DateTimeAxis.ToDateTime(dt).ToString(showYear ? "MM/dd/yyyy" : "MM/dd"),
                MajorStep = Math.Max(dateRange.TotalDays / 8.0, 1)
            };
            model.Axes.Add(dateTimeAxis);

            var incomeSeries = new StairStepSeries
            {
                Title = "Incomes",
                Color = OxyColors.Green,
                TrackerFormatString = "Date: {2:MM/dd/yyyy}\nIncomes: {4:C}"
            };
            var expenseSeries = new StairStepSeries
            {
                Title = "Expenses",
                Color = OxyColors.Red,
                TrackerFormatString = "Date: {2:MM/dd/yyyy}\nExpenses: {4:C}"
            };

            decimal cumulativeIncome = 0;
            decimal cumulativeExpenses = 0;

            incomeSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startDate), (double)cumulativeIncome));
            expenseSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startDate), (double)cumulativeExpenses));

            foreach (var transaction in transactions.OrderBy(t => t.Date))
            {
                if (transaction.IsIncome)
                {
                    cumulativeIncome += transaction.Amount;
                    incomeSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(transaction.Date), (double)cumulativeIncome));
                }
                else
                {
                    cumulativeExpenses += transaction.Amount;
                    expenseSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(transaction.Date), (double)cumulativeExpenses));
                }
            }
            incomeSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(endDate), (double)cumulativeIncome));
            expenseSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(endDate), (double)cumulativeExpenses));

            model.Series.Add(incomeSeries);
            model.Series.Add(expenseSeries);
            IncomesVsExpensesOverTimeLineChartPlotView.Model = model;
        }


    }
}
