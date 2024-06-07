using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Previewer;
using System.Windows;
using System.Windows.Controls;

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
                nameChkBox.ItemsSource = db.Customers.Select(cust => cust.CompanyName).ToList();
                var orders = db.Orders.Where(o => o.OrderStatus == "Готов")
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
                        oMoneyPerOrder = o.MoneyPerOrder,
                        oExpenses = o.Expenses,
                        oProfit = o.MoneyPerOrder - o.Expenses
                    }).ToList();
                completedOrdersGrid.ItemsSource = orders;
            }
        }

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        private async void btnSave_Click(object sender, RoutedEventArgs e)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
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
                            //entry.CustomerId = ((OrdView)item).oCustomersMail;
                            entry.StartPoint = ((OrdView)item).oStartPoint;
                            entry.FinalPoint = ((OrdView)item).oFinalPoint;
                            entry.TrackNumber = ((OrdView)item).oTrackNumber;
                            entry.OrderStatus = ((OrdView)item).oOrderStatus;
                            entry.Comment = ((OrdView)item).oComment;
                            entry.MoneyPerOrder = ((OrdView)item).oMoneyPerOrder;
                            entry.Expenses = ((OrdView)item).oExpenses;
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

                            // Заменим entry на db, так как мы добавляем новую запись, а не обновляем существующую
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

        private async void btnAddAct_Click(object sender, EventArgs e)
        {
            var selectedOrder = completedOrdersGrid.SelectedItem as OrdView;
            if (selectedOrder == null)
            {
                MessageBox.Show("Пожалуйста, выберите строку в таблице.", "Уведомление");
                return;
            }

            double moneyInWordsDouble = (double)selectedOrder.oMoneyPerOrder;

            // Установка лицензии для QuestPDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            using (ProfitCalculatorDataBaseContext db = new ProfitCalculatorDataBaseContext())
            {
                // Выполнение соединения таблиц Orders и Customers
                var orderWithCustomer = db.Orders.Where(o => o.Id == selectedOrder.oId)
                                                 .Join(db.Customers,
                                                       o => o.CustomerId,
                                                       c => c.CustomerId,
                                                       (o, c) => new { Order = o, Customer = c })
                                                 .FirstOrDefault();

                // Создание PDF-документа в отдельном потоке
                await Task.Run(() =>
                {
                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);

                            page.Content().Column(column =>
                            {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                                column.Item().Text($"АКТ №{orderWithCustomer.Order.Id} от {DateTime.Now.ToString("dd.MM.yyyy")} г.")
                                             .Bold()
                                             .FontSize(14)
                                             .FontFamily("Courier New")
                                             .Underline();
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

                                column.Item().Text($"Заказчик: {orderWithCustomer.Customer.CompanyName} ИНН {orderWithCustomer.Customer.Inn}")
                                             .FontSize(10)
                                             .FontFamily("Trebuchet MS");

                                column.Item().Text($"Юр. адрес:")
                                             .FontSize(10)
                                             .FontFamily("Trebuchet MS");

                                column.Item().Text($"Исполнитель: ИП Сухин Владимир Анатольевич; ИНН: 575200444890\r\nЮр. адрес: 302531, обл. Орловская, р-н Орловский, д. Кондырева, ул Мира, д. 36.\r\nОснование: ДОГОВОР - ЗАЯВКА № 21522777 на осуществление перевозки груза от 12.01.2024\r\nВодитель- Сухин В.А.\r\n")
                                             .FontSize(10)
                                             .FontFamily("Trebuchet MS");
                            });
                        });
                    }).ShowInPreviewer();
                });
            }
        }


        public static string ConvertToWords(double input)
        {
            string[] edin = { "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            string[] teen = { "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
            string[] des = { "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
            string[] sto = { "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };
            string[] thousand = { "одна тысяча", "две тысячи", "три тысячи", "четыре тысячи", "пять тысяч", "шесть тысяч", "семь тысяч", "восемь тысяч", "девять тысяч" };

            double numb;
            int natur, dr;
            int a, b, c, d, e, f;
            int x, y, z;
            string odin, dva, tri, chetir, pyat, shest;
            string otr, one, two, three;

            numb = input;
            otr = "";
            one = "";
            two = "";
            three = "";
            odin = "";
            dva = "";
            tri = "";
            chetir = "";
            pyat = "";
            shest = "";

            if (numb < 0)
            {
                otr = "минус";
                numb *= -1;
            }

            dr = (int)(numb * 100) % 100;
            natur = (int)numb;

            a = natur % 10;
            b = (natur % 100) / 10;
            c = (natur % 1000) / 100;
            f = natur / 100000;
            e = (natur % 100000) / 10000;
            d = (natur % 10000) / 1000;

            if (a != 0 && b != 1)
                odin = edin[a - 1];
            else if (a == 0 && b == 1)
                odin = "десять";
            else if (a == 0 && b == 0)
                odin = "";

            if (b == 0)
                dva = "";
            else if (b == 1 && a != 0)
                dva = teen[a - 1];
            else if (b != 1)
                dva = des[b - 2];

            if (c != 0)
                tri = sto[c - 1];

            if (a == 0 && b == 0 && c == 0 && d == 0 && e == 0 && f == 0)
                odin = "ноль";

            if (d != 0)
                chetir = thousand[d - 1];
            else if (d == 0 && e == 1)
                chetir = "десять тысяч";
            else if (d != 0 && e == 1)
                pyat = teen[d - 1] + " тысяч";

            if (e != 0 && e != 1 && d == 0)
                pyat = des[e - 2] + " тысяч";
            else if (e != 0 && e != 1 && d != 0)
                pyat = des[e - 2];

            if (f != 0 && d == 0 && e == 0)
                shest = sto[f - 1] + " тысяч";
            else if (f != 0)
                shest = sto[f - 1];
            x = dr / 100;
            y = (dr % 100) / 10;
            z = dr % 10;

            if (y == 0 && z == 0) //десятые
            {
                if (x == 0)
                    one = "";
                else if (x == 1)
                    one = edin[x - 1] + " десятая";
                else
                    one = edin[x - 1] + " десятых";
            }

            if (z == 0 && y != 0) //сотые
            {
                if (x == 0)
                {
                    if (y == 1)
                        two = edin[y - 1] + " сотая";
                    else
                        two = edin[y - 1] + " сотых";
                }
                else if (x == 1)
                    two = teen[y - 1] + " сотых";
                else if (x != 0)
                {
                    two = des[x - 2];
                    if (y == 1)
                        one = edin[y - 1] + " сотая";
                    else
                        one = edin[y - 1] + " сотых";
                }
            }

            if (z != 0) //тысячные
            {
                if (x == 0 && y == 0)
                {
                    if (z == 1)
                        one = edin[z - 1] + " тысячная";
                    else
                        one = edin[z - 1] + " тысячных";
                }
                else if (x == 0 && y == 1)
                    one = teen[z - 1] + " тысячных";
                else if (x == 0 && y > 1)
                {
                    two = des[y - 2];
                    if (z == 1)
                        one = edin[z - 1] + " тысячная";
                    else
                        one = edin[z - 1] + " тысячных";
                }
                else if (x != 0)
                {
                    three = sto[x - 1];
                    if (y == 0)
                        two = "";
                    else if (y == 1)
                        two = teen[z - 1] + " тысячных";
                    else
                        two = des[y - 2];
                    if (z == 1 && y != 1)
                        one = edin[z - 1] + " тысячная";
                    else if (y != 1)
                        one = edin[z - 1] + " тысячных";
                }
            }

            string[] data = { numb.ToString(), "(", otr, shest, pyat, chetir, tri, dva, odin, " рублей ", dr.ToString(), " копеек )" };

            string result = string.Join(" ", data.Where(s => !string.IsNullOrWhiteSpace(s)));

            result = string.Join(" ", result.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return result;
        }
    }
}

