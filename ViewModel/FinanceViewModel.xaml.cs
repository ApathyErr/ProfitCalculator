using Microsoft.EntityFrameworkCore;
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
                mailChkBox.ItemsSource = db.Customers.Select(cust => cust.Mail).ToList(); 
                var orders = db.Orders.Where(o => o.OrderStatus == "Готов")
                    .Join(db.Customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new OrdView
                    {
                        oId = o.Id,
                        oData = o.Data,
                        oCustomerId = o.CustomerId,
                        oCustomersMail = c.Mail,
                        oOrderStatus = o.OrderStatus,
                        oMoneyPerOrder = o.MoneyPerOrder,
                              oExpenses = o.Expenses,
                              oProfit = o.MoneyPerOrder - o.Expenses
                    }).ToList();

                financeGrid.ItemsSource = orders;
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
                            var customer = await db.Customers.FirstOrDefaultAsync(c => c.Mail == ((OrdView)item).oCustomersMail);
                            if (customer != null)
                            {
                                entry.CustomerId = customer.CustomerId; // Устанавливаем идентификатор клиента в заказе
                            }
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
                            var customer = await db.Customers.FirstOrDefaultAsync(c => c.Mail == ((OrdView)item).oCustomersMail);
                            if (customer != null)
                            {
                                order.CustomerId = customer.CustomerId; // Устанавливаем идентификатор клиента в заказе
                            }

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
