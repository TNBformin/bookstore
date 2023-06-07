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

namespace bookstore.Pages
{
    /// <summary>
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        // Определение необходимых переменных
        double FullPriceWithoutDiscount = 0;
        double PriceWithDiscount = 0;
        private static MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        public CartPage()
        {
            InitializeComponent();
            // Получение используемого заказа
            int OrderID = App.current_order;

            // Получение данных для заполнения DataTemplate
            CViewServices.ItemsSource = CartBooksInit(OrderID);
            
        }

        // Функция получения и установки необхдимых данных на странице
        public List<Entities.Book> CartBooksInit(int OrderID)
        {
            // Обнуление переменных
            FullPriceWithoutDiscount = 0;
            PriceWithDiscount = 0;

            // Получение списка книг в заказе
            var OrderBooksList = App.Context.OrderBooks.Where(s => s.OrderId == OrderID).ToList();

            // Определение новой перменной в виде списка книг
            List<Entities.Book> Books = new List<Entities.Book>();

            // Перебор всех книг
            foreach (var OrderBook in OrderBooksList)
            {
                // Получение самой книги
                var book = App.Context.Books.FirstOrDefault(b => b.BookId == OrderBook.BookId);

                // Запись её в недавно созданную переменную
                Books.Add(book);

                // Подсчет сумм без скидки и со скидкой
                FullPriceWithoutDiscount += (double)book.BookPrice * OrderBook.BookQuantityInOrder;
                PriceWithDiscount += ((double)book.BookPrice * (1 - book.BookCurrentDiscount * 0.01)) * OrderBook.BookQuantityInOrder;
            }

            // Проверка количеств в БД
            bool AllMoreThenTwo = Books.All(s => s.BookQuantityInStock > 2);

            // Установка времени доставки
            if (AllMoreThenTwo == true)
            {
                mainWindow.DeliveryDate.Text = "Дата доставки: " + DateTime.Now.AddDays(3).ToString("dd.MM.yyyy");
            }
            else
            {
                mainWindow.DeliveryDate.Text = "Дата доставки: " + DateTime.Now.AddDays(6).ToString("dd.MM.yyyy");
            }

            // Установка полученных значений
            mainWindow.FullPrice.Text = "Цена без скидки: " + FullPriceWithoutDiscount.ToString() + "руб.";
            mainWindow.LastPrice.Text = "Цена со скидкой: " + PriceWithDiscount.ToString() + "руб.";
            mainWindow.DiscountAmount.Text = "Размер скидки: " + (FullPriceWithoutDiscount - PriceWithDiscount).ToString() + "руб.";

            // Включение отображения кнопки "Оформить заказ"
            mainWindow.OrderCreate.Visibility = Visibility.Visible;

            // Возвращение списка книг из заказа
            return Books;
        }

        // Функция выполняющаяся сразу после открытия страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация кол-ва каждой книги в TextBox
            QuanInit();

            // Включение отображения необходимых элементов интерфейся на странице корзины
            mainWindow.BackBtn.Visibility = Visibility.Visible;
            mainWindow.OrderCreate.Visibility = Visibility.Visible;
            mainWindow.FullPrice.Visibility = Visibility.Visible;
            mainWindow.LastPrice.Visibility = Visibility.Visible;
            mainWindow.DiscountAmount.Visibility = Visibility.Visible;
            mainWindow.DeliveryDate.Visibility = Visibility.Visible;
            mainWindow.PickUpPointsPanel.Visibility = Visibility.Visible;

            var PickUpPointsData = App.Context.PickUpPoints.ToList();
            mainWindow.PickUpPoints.ItemsSource = PickUpPointsData.Select(p => p.PickUpPointName);
        }


        // Функция нициализации кол-ва каждой книги в TextBox
        public void QuanInit()
        {
            // Перебор всех DataTemplate с книгами в корзине
            foreach (var item in CViewServices.Items)
            {
                // Определение заказа
                int OrderID = App.current_order;

                // Получение всех книг из заказа
                var OrderBooksList = App.Context.OrderBooks.Where(s => s.OrderId == OrderID).ToList();

                // Получение всех контейнеров с книгами и обнаружение TextBox для каждой отдельной книге
                var listViewItem = CViewServices.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                var contentPresenter = FindVisualChild<ContentPresenter>(listViewItem);
                var dataTemplate = contentPresenter.ContentTemplate;
                var QuanTextBox = dataTemplate.FindName("ProductNeedQuantity", contentPresenter) as TextBox;
                var BookNameTextBlock = dataTemplate.FindName("BookName", contentPresenter) as TextBlock;
                if (QuanTextBox != null)
                {
                    // Получения кол-ва книги в заказе
                    int BookQuan = OrderBooksList.FirstOrDefault(s => s.BookId.ToString() == BookNameTextBlock.Tag.ToString()).BookQuantityInOrder;

                    // Запись кол-ва книги в его TextBox
                    QuanTextBox.Text = BookQuan.ToString();
                }
            }
        }

        // Функция поиска TextBox
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                    if (child is T typedChild)
                    {
                        return typedChild;
                    }

                    T foundChild = FindVisualChild<T>(child);
                    if (foundChild != null)
                    {
                        return foundChild;
                    }
                }
            }

            return null;
        }


        // Определение функций кнопки "Удалить товар"
        public void DellBtn(object sender, EventArgs e)
        {
            // Подтвержение действия пользователем
            MessageBoxResult answer = MessageBox.Show("Вы уверены что хотите удалить этот товар из корзины?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (answer == MessageBoxResult.OK)
            {
                // Получение выбранной книги
                Entities.Book book = CViewServices.SelectedItem as Entities.Book;

                // Получение идентификатора заказа
                var OrderID = App.current_order;

                // Получение книги в заказе
                var OrderBook = App.Context.OrderBooks.Where(s => s.OrderId == OrderID).FirstOrDefault(s => s.BookId == book.BookId);

                // Возвращение кол-ва на склад
                book.BookQuantityInStock += OrderBook.BookQuantityInOrder;

                // Удаление записи из БД
                App.Context.OrderBooks.Remove(OrderBook);

                // Сохранение изменений
                App.Context.SaveChanges();

                // Определение оставшихся книг в заказе
                var OrderBooks = App.Context.OrderBooks.Where(s => s.OrderId == OrderID).ToList();

                // Проверка на их наличие
                if (OrderBooks.Count != 0)
                {
                    // Обновление страницы
                    mainWindow.MainFrame.Navigate(new CartPage());
                }
                else
                {
                    // Закрытие страницы корзины, ведь она пуста
                    mainWindow.Cart.Visibility = Visibility.Hidden;
                    mainWindow.Back(this, null);
                }
            }
        }

        // Функция обработки изменения необходимого кол-ва книг в заказе
        public void ProductNeedQuantityChanged(object sender, TextChangedEventArgs e)
        {
            // Полученние TextBox который вызвал функцию
            var currentTextBox = ((TextBox)sender);

            // Получение используемого заказа
            int OrderID = App.current_order;

            // Получение списка книг из заказа
            var OrderBooksList = App.Context.OrderBooks.Where(s => s.OrderId == OrderID).ToList();

            // Получение тега определенного TextBox`a
            int currentTextBoxTag = int.Parse(((TextBox)sender).Tag.ToString());

            // Получение выбранной книги
            var book = App.Context.Books.FirstOrDefault(s => s.BookId == currentTextBoxTag);

            // Проверка на то что в строке только цифры и поле не пустое
            if ((currentTextBox.Text.ToString().All(char.IsDigit)) & (currentTextBox.Text.ToString() != ""))
            {
                // Получение указанного кол-ва необходимых книг
                int QuanBook = int.Parse(currentTextBox.Text.ToString());

                // Проверка кол-ва
                if (QuanBook == 0)
                {
                    // Удаление товара из корзины
                    // Получение заказа книги
                    var OrderBook = OrderBooksList.FirstOrDefault(s => s.BookId == book.BookId);
                    // Подтвержение действия пользователем
                    MessageBoxResult answer = MessageBox.Show("Вы уверены что хотите удалить этот товар из корзины?", "Подтверждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (answer == MessageBoxResult.OK)
                    {
                        // Возвращение кол-ва на склад
                        book.BookQuantityInStock += OrderBook.BookQuantityInOrder;

                        // Удаление записи из БД
                        App.Context.OrderBooks.Remove(OrderBook);

                        // Сохранение изменений
                        App.Context.SaveChanges();

                        // Определение оставшихся книг в заказе
                        var OrderBooks = App.Context.OrderBooks.Where(s => s.OrderId == OrderID).ToList();

                        // Проверка на их наличие
                        if (OrderBooks.Count != 0)
                        {
                            // Обновление страницы
                            mainWindow.MainFrame.Navigate(new CartPage());
                        }
                        else
                        {
                            // Закрытие страницы корзины, ведь она пуста
                            mainWindow.Cart.Visibility = Visibility.Hidden;
                            mainWindow.Back(this, null);
                        }
                    }
                    else
                    {
                        // Установка числа по умолчанию и сохранение изменений в БД
                        book.BookQuantityInStock += OrderBook.BookQuantityInOrder - 1;
                        currentTextBox.Text = "1";
                        OrderBook.BookQuantityInOrder = 1;
                        App.Context.SaveChanges();
                    }
                }
                else
                {
                    // Восстановление кол-ва книг в БД
                    book.BookQuantityInStock += OrderBooksList.FirstOrDefault(s => s.BookId == currentTextBoxTag).BookQuantityInOrder;

                    // Проверка на хватку книг на складе
                    if (book.BookQuantityInStock < QuanBook)
                    {
                        // Восстановление прошлого прошедшего условие числа
                        currentTextBox.Text = OrderBooksList.FirstOrDefault(s => s.BookId == currentTextBoxTag).BookQuantityInOrder.ToString();

                        // Вычитание кол-ва книг из БД
                        book.BookQuantityInStock -= OrderBooksList.FirstOrDefault(s => s.BookId == currentTextBoxTag).BookQuantityInOrder;

                        // Диалоговое окно с предупрежднением об ошибке
                        MessageBox.Show("Недостаточно товара на складе", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        // Вычитание кол-ва книг из БД
                        book.BookQuantityInStock -= QuanBook;

                        // Запись в кол-ва в заказ
                        OrderBooksList.FirstOrDefault(s => s.BookId == currentTextBoxTag).BookQuantityInOrder = QuanBook;

                    }
                }

                
            }
            // Проверка на то что в поле есть символы
            else if (!currentTextBox.Text.ToString().All(char.IsDigit))
            {
                // Восстановление начального значения
                currentTextBox.Text = OrderBooksList.FirstOrDefault(s => s.BookId == currentTextBoxTag).BookQuantityInOrder.ToString();
            }
            // Сохранение изменений БД
            App.Context.SaveChanges();

            // Повторная инициализаация списка
            CartBooksInit(OrderID);
        }
    }
}
