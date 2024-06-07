using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProfitCalculator.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для ReportsViewModel.xaml
    /// </summary>
    public partial class ReportsViewModel : UserControl
    {
        public ReportsViewModel()
        {
            InitializeComponent();
        }
        private void metodShow(object sender, RoutedEventArgs e)
        {

            //using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            //{
            //    var customers = db.Customers.ToList(); // Загрузите всех клиентов заранее
            //    var orders = db.Orders.Where(o => o.Id == null)
            //                         .Select(o => new Order
            //                         {
            //                             startDate = o.Data,
            //                             endDate = o.Data
            //                         })
            //                         .ToList();

            //    reportGrid.ItemsSource = orders;
            //}
        }

        private void btnAverageMoneyPerOrder_Click(object sender, RoutedEventArgs e)
        {
            //using (var context = new ProfitCalculatorDataBaseContext())
            //{

            //    // Поиск записей в таблице Order
            //    var orders = context.Orders
            //        .Where(o => o.Data >= o.startDate && o.Data <= o.endDate)
            //        .ToList();

            //    if (orders.Count > 0)
            //    {
            //        decimal totalMoneyPerOrder = orders.Sum(o => o.MoneyPerOrder);
            //        decimal averageMoneyPerOrder = totalMoneyPerOrder / orders.Count;
            //        MessageBox.Show($"Среднее значение MoneyPerOrder: {averageMoneyPerOrder}");
            //    }
            //    else
            //    {
            //        MessageBox.Show($"Нет записей для расчета среднего значения");


            //    }
            //}

        }

        
    }
}
