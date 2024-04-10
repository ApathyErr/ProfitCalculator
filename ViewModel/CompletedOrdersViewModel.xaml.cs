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
    /// Логика взаимодействия для CompletedOrdersViewModel.xaml
    /// </summary>
    public partial class CompletedOrdersViewModel : UserControl
    {
        public CompletedOrdersViewModel()
        {
            InitializeComponent();
        }
        private void metodShow(object sender, RoutedEventArgs e)
        {
            using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            {
                var customers = db.Customers.ToList(); // Загрузите всех клиентов заранее
                var orders = db.Orders.Where(o => o.OrderStatus == "Готов")
                                     .Select(o => new OrdView
                                     {
                                         oId = o.Id,
                                         oData = o.Data,
                                         oCustomerId = o.CustomerId,
                                         //oCustomersMail = customers.FirstOrDefault(c => c.CustomerId == (int)o.CustomerId)?.Mail,
                                         //oCustomersMail = customers.FirstOrDefault(c => c.CustomerId == null ? null: (int)o.CustomerId).Mail,
                                         oStartPoint = o.StartPoint,
                                         oFinalPoint = o.FinalPoint,
                                         oTrackNumber = o.TrackNumber,
                                         oOrderStatus = o.OrderStatus,
                                         oComment = o.Comment,
                                         oMoneyPerOrder = o.MoneyPerOrder
                                     })
                                     .ToList();

                completedOrdersGrid.ItemsSource = orders;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var items = completedOrdersGrid.ItemsSource;
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
                            //entry.oCustomersMail = ((OrdView)item).oCustomersMail;
                            entry.StartPoint = ((OrdView)item).oStartPoint;
                            entry.FinalPoint = ((OrdView)item).oFinalPoint;
                            entry.TrackNumber = ((OrdView)item).oFinalPoint;
                            entry.OrderStatus = ((OrdView)item).oOrderStatus;
                            entry.Comment = ((OrdView)item).oComment;
                            entry.MoneyPerOrder = ((OrdView)item).oMoneyPerOrder;
                        }
                        else
                        {
                            Order order = new Order
                            {
                                Data = ((OrdView)item).oData,
                                CustomerId = ((OrdView)item).oCustomerId,
                                //oCustomersMail = ((OrdView)item).oCustomersMail,
                                StartPoint = ((OrdView)item).oStartPoint,
                                FinalPoint = ((OrdView)item).oFinalPoint,
                                TrackNumber = ((OrdView)item).oFinalPoint,
                                OrderStatus = ((OrdView)item).oOrderStatus,
                                Comment = ((OrdView)item).oComment,
                                MoneyPerOrder = ((OrdView)item).oMoneyPerOrder
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
            if (completedOrdersGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нет выбранных записей для удаления.", "Уведомление");
                return;
            }

            var selectedIds = completedOrdersGrid.SelectedItems.Cast<OrdView>()
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

