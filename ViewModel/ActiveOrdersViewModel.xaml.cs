using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                var ord = from order in db.Orders
                          where order.Completed == 0
                          select new OrdView
                          {
                              oId = order.Id,
                              oData = order.Data,
                              oCustomersMail = order.CustomersMail,
                              oStartPoint = order.StartPoint,
                              oFinalPoint = order.FinalPoint,
                              oTrackNumber = order.TrackNumber,
                              oOrderStatus = order.OrderStatus,
                              oComment = order.Comment,
                              oMoneyPerOrder = order.MoneyPerOrder,
                              oCompleted = order.Completed
                          };
                activeOrdersGrid.ItemsSource = ord.ToList();
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
