using ProfitCalculator.ViewModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProfitCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }
        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var tablesControl = new ViewModel.ActiveOrdersViewModel();
            contentControl.Content = tablesControl;
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            var tablesControl = new ViewModel.CompletedOrdersViewModel();
            contentControl.Content = tablesControl;
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            var tablesControl = new ViewModel.Customers();
            contentControl.Content = tablesControl;
        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            var tablesControl = new ViewModel.FinanceViewModel();
            contentControl.Content = tablesControl;
        }
        //private void Button5_Click(object sender, RoutedEventArgs e)
        //{
        //    var tablesControl = new ViewModel.ReportsViewModel();
        //    contentControl.Content = tablesControl;
        //}
    }
}