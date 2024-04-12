﻿using Microsoft.EntityFrameworkCore;
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
                mailChkBox.ItemsSource = db.Customers.Select(cust => cust.Mail).ToList();// Загрузите всех клиентов заранее    
                var orders = db.Orders.Where(o => o.OrderStatus != "Готов")
                    .Join(db.Customers,
                    o => o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new OrdView
                    {
                        oId = o.Id,
                        oData = o.Data,
                        oCustomerId = o.CustomerId,
                        oCustomersMail = c.Mail,
                        oStartPoint = o.StartPoint,
                        oFinalPoint = o.FinalPoint,
                        oTrackNumber = o.TrackNumber,
                        oOrderStatus = o.OrderStatus,
                        oComment = o.Comment,
                        oMoneyPerOrder = o.MoneyPerOrder
                    }).ToList();
                activeOrdersGrid.ItemsSource = orders;
            }





            //    using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            //    {

            //        //var orders = db.Orders.ToList(); // загрузка всех заказов
            //        //foreach (var order in orders)
            //        //{
            //        //    // Получение информации о клиенте по CustomerId и заполнение свойства oCustomersMail
            //        //    order.oCustomersMail = db.Customers.FirstOrDefault(c => c.CustomerId == order.CustomerId)?.Mail;
            //        //}
            //    //    var ord = from order in db.Orders
            //    //              //where order.Completed == 0
            //    //              where order.OrderStatus != "Готов"
            //    //              select new OrdView
            //    //              {
            //    //                  oId = order.Id,
            //    //                  oData = order.Data,
            //    //                  //oCustomersMail = order.CustomersMail,
            //    //                  oStartPoint = order.StartPoint,
            //    //                  oFinalPoint = order.FinalPoint,
            //    //                  oTrackNumber = order.TrackNumber,
            //    //                  oOrderStatus = order.OrderStatus,
            //    //                  oComment = order.Comment,
            //    //                  oMoneyPerOrder = order.MoneyPerOrder
            //    //              };
            //    //    activeOrdersGrid.ItemsSource = ord.ToList();
            //    //}
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
                            //entry.Mail = ((OrdView)item).oCustomersMail;
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
                                //oCustomersMail = ((Order)item).oCustomersMail,
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
