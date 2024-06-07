using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProfitCalculator.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для ActiveOrdersViewModel.xaml
    /// </summary>

    public partial class ActiveOrdersViewModel : UserControl
    {
        public ActiveOrdersViewModel()
        {
            InitializeComponent();
        }
        private void metodShow(object sender, RoutedEventArgs e)
        {

            using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            {
                nameChkBox.ItemsSource = db.Customers.Select(cust => cust.CompanyName).ToList(); 
                var orders = db.Orders.Where(o => o.OrderStatus != "Готов")
                    .Join(db.Customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new OrdView
                    {
                        oId = o.Id,
                        oData = o.Data,
                        oCustomerId = o.CustomerId,
                        oCustomersName = c.CompanyName,
                        oStartPoint = o.StartPoint,
                        oFinalPoint = o.FinalPoint,
                        oTrackNumber = o.TrackNumber,
                        oOrderStatus = o.OrderStatus,
                        oComment = o.Comment,
                        oMoneyPerOrder = o.MoneyPerOrder
                    }).ToList();
                activeOrdersGrid.ItemsSource = orders;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var items = activeOrdersGrid.ItemsSource;
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
                            //entry.CustomerId = ((OrdView)item).oCustomersMail;
                            entry.StartPoint = ((OrdView)item).oStartPoint;
                            entry.FinalPoint = ((OrdView)item).oFinalPoint;
                            entry.TrackNumber = ((OrdView)item).oTrackNumber;
                            entry.OrderStatus = ((OrdView)item).oOrderStatus;
                            entry.Comment = ((OrdView)item).oComment;
                            entry.MoneyPerOrder = ((OrdView)item).oMoneyPerOrder;
                            
                            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CompanyName == ((OrdView)item).oCustomersName);
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
                                StartPoint = ((OrdView)item).oStartPoint,
                                FinalPoint = ((OrdView)item).oFinalPoint,
                                TrackNumber = ((OrdView)item).oFinalPoint,
                                OrderStatus = ((OrdView)item).oOrderStatus,
                                Comment = ((OrdView)item).oComment,
                                MoneyPerOrder = ((OrdView)item).oMoneyPerOrder,
                                Expenses = ((OrdView)item).oExpenses
                        };
                            var customer = await db.Customers.FirstOrDefaultAsync(c => c.CompanyName == ((OrdView)item).oCustomersName);
                            if (customer != null)
                            {
                                order.CustomerId = customer.CustomerId; // Устанавливаем идентификатор клиента в заказе
                            }

                            db.Orders.Add(order); // Добавляем новую запись в базу данных
                        }

                        await db.SaveChangesAsync(); // Сохраняем изменения
                    }
                }
            }
            metodShow(sender, e);
        }




        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (activeOrdersGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нет выбранных записей для удаления.", "Уведомление");
                return;
            }

            var selectedIds = activeOrdersGrid.SelectedItems.Cast<OrdView>()
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
