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
    /// Логика взаимодействия для Customers.xaml
    /// </summary>
    public partial class Customers : UserControl
    {
        public Customers()
        {
            InitializeComponent();
        }
        private void metodShow(object sender, RoutedEventArgs e)
        {
            using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            {
                var cut = from customer in db.Customers
                          select new Customer
                          {
                              CustomerId = customer.CustomerId,
                              Mail = customer.Mail,
                              Telephone = customer.Telephone,
                              Cities = customer.Cities,
                              CompanyName = customer.CompanyName,
                              Familiya = customer.Familiya,
                              Imya = customer.Imya,
                              Otchestvo = customer.Otchestvo
                          };
                customerGrid.ItemsSource = cut.ToList();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var items = customerGrid.ItemsSource;
            if (items != null)
            {
                using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
                {
                    // Go through all the records and add or update them in the database
                    foreach (var item in items)
                    {
                        var entry = await db.Customers.FindAsync(((Customer)item).CustomerId);
                        if (entry != null)
                        {
                            entry.Mail = ((Customer)item).Mail;
                            entry.Telephone = ((Customer)item).Telephone;
                            entry.Cities = ((Customer)item).Cities;
                            entry.CompanyName = ((Customer)item).CompanyName;
                            entry.Familiya = ((Customer)item).Familiya;
                            entry.Imya = ((Customer)item).Imya;
                            entry.Otchestvo = ((Customer)item).Otchestvo;
                        }
                        else
                        {
                            // Если запись не найдена, добавляем новую запись
                            db.Customers.Add((Customer)item);
                        }
                    }

                    await db.SaveChangesAsync(); // Saving changes
                }
            }

            metodShow(sender, e);
        }




        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (customerGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Нет выбранных записей для удаления.", "Уведомление");
                return;
            }

            var selectedIds = customerGrid.SelectedItems.Cast<Customer>()
                                           .Select(x => x.CustomerId).ToArray();

            if (MessageBox.Show($"Вы действительно хотите удалить записи с ID: {string.Join(", ", selectedIds)}?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
                {
                    foreach (var id in selectedIds)
                    {
                        var customerToDelete = db.Customers.FirstOrDefault(o => o.CustomerId == id);
                        if (customerToDelete != null)
                        {
                            db.Customers.Remove(customerToDelete);
                        }
                    }
                    db.SaveChanges();
                }
            }
            metodShow(sender, e);
        }
    }

}
