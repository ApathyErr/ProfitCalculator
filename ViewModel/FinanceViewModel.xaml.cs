using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для FinanceViewModel.xaml
    /// </summary>
    public partial class FinanceViewModel : UserControl
    {
        public FinanceViewModel()
        {
            InitializeComponent();
        }
        private void metodShow(object sender, RoutedEventArgs e)
        {
            using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            {
                var ord = from order in db.Orders
                          select new OrdView
                          {
                              oNum = order.Num,
                              oData = order.Data,
                              oCustomersMail = order.CustomersMail,
                              oOrderStatus = order.OrderStatus,
                              oMoneyPerOrder = order.MoneyPerOrder,
                              oExpenses = order.Expenses,
                              oProfit = order.MoneyPerOrder - order.Expenses
                          };
                financeGrid.ItemsSource = ord.ToList();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
