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
                              oId = order.Id,
                              oData = order.Data,
                              oCustomerId = order.CustomerId,
                              //oCustomersMail = order.CustomersMail,
                              oOrderStatus = order.OrderStatus,
                              oMoneyPerOrder = order.MoneyPerOrder,
                              oExpenses = order.Expenses,
                              oProfit = order.MoneyPerOrder - order.Expenses
                          };
                financeGrid.ItemsSource = ord.ToList();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var items = financeGrid.ItemsSource;
            if (items != null)
            {
                using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
                {
                    // Go through all the records and add or update them in the database
                    foreach (var item in items)
                    {
                        var entry = await db.Orders.FindAsync(((OrdView)item).oId);
                        if (entry != null)
                        {
                            entry.Data = ((OrdView)item).oData;
                            entry.CustomerId = ((OrdView)item).oCustomerId;
                            //oCustomersMail = ((Order)item).oCustomersMail;
                            entry.OrderStatus = ((OrdView)item).oOrderStatus;
                            entry.MoneyPerOrder = ((OrdView)item).oMoneyPerOrder;
                            entry.Expenses = ((OrdView)item).oExpenses;
                        }
                        else
                        {
                            Order order = new Order
                            {
                                Data = ((OrdView)item).oData,
                                CustomerId = ((OrdView)item).oCustomerId,
                                //oCustomersMail = ((Order)item).oCustomersMail,
                                OrderStatus = ((OrdView)item).oOrderStatus,
                                MoneyPerOrder = ((OrdView)item).oMoneyPerOrder,
                                Expenses = ((OrdView)item).oExpenses
                        
                        };

                            db.Orders.Add(order); // Adding a new entry to the database
                        }

                        await db.SaveChangesAsync(); // Saving changes
                    }
                }
            }
            metodShow(sender, e);
        }




        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (financeGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нет выбранных записей для удаления.", "Уведомление");
                return;
            }

            var selectedIds = financeGrid.SelectedItems.Cast<OrdView>()
                                           .Select(x => x.oId).ToArray();

            if (MessageBox.Show($"Вы действительно хотите удалить записи с ID: {string.Join(", ", selectedIds)}?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
                {
                    foreach (var id in selectedIds)
                    {
                        var orderToDelete = db.Orders.FirstOrDefault(o => o.Id == id);
                        if (orderToDelete != null)
                        {
                            db.Orders.Remove(orderToDelete);
                        }
                    }
                    db.SaveChanges();
                }
            }
            metodShow(sender, e);
        }
    }
}
