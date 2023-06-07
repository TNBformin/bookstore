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
    /// Логика взаимодействия для ProductList.xaml
    /// </summary>
    public partial class ProductList : Page
    {
        // Создание ссылки на главное окно
        private static MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        public ProductList()
        {
            InitializeComponent();
        }

        // Объявление переменной рандом
        Random rnd = new Random();

        // Создание ссылки на кнопку корзинки по имени
        Button CartBtn = mainWindow.FindName("Cart") as Button;

        // Функция выполняемая при открытии страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Загрузка данных книг в шаблон
            LViewServices.ItemsSource = App.Context.Books.ToList();
        }

        // Функция обработки при нажатии кнопки "Добавить в корзину"
        private void AddBtn(object sender, RoutedEventArgs e)
        {
            // Получение выбранной книги
            Entities.Book book = LViewServices.SelectedItem as Entities.Book;

            // Проверка на наличие на складе
            if (book.BookQuantityInStock > 0)
            {
                // Получение максимального идентификатора заказов
                int maxId = App.Context.Orders.Max(b => b.OrderId);

                // Проверка на отсутсвие уже созданного заказа
                if (App.current_order == -1)
                {
                    // Создание нового заказа
                    Entities.Order new_order = new Entities.Order
                    {
                        // Присвоение необходимых параметров
                        OrderId = maxId + 1,
                        OrderStatus = 0,
                        PickUpCode = rnd.Next(1000),
                        DeliveryTime = new DateTime(2008, 6, 1, 7, 47, 0),
                        PickUpPointID = 1
                    };

                    // Запись и сохранение нового заказа в БД
                    App.current_order = new_order.OrderId;
                    App.Context.Orders.Add(new_order);
                }

                // Получение всех книг из заказа
                var OrderBooks = App.Context.OrderBooks.Where(b => b.OrderId == App.current_order);

                // Проверка книги на наличие в заказе
                if(OrderBooks.Any(b => b.BookId == book.BookId))
                {
                    // Получение ссылки на книгу в заказе
                    var OrderBook = OrderBooks.FirstOrDefault(b => b.BookId == book.BookId);

                    // Увелечение на одну единицу его кол-ва
                    OrderBook.BookQuantityInOrder++;
                }
                else
                {
                    // Получение максимального идентификатора книг из заказов
                    int maxIdOrderBooks = App.Context.OrderBooks.Max(b => b.OrderBooksId);

                    // Запись книги в заказ
                    Entities.OrderBook new_orderbook = new Entities.OrderBook
                    {
                        // Заполнение необходимых параметров
                        OrderBooksId = maxIdOrderBooks + 1,
                        OrderId = App.current_order,
                        BookId = book.BookId,
                        BookQuantityInOrder = 1,
                    };
                    
                    // Запись новых данных в БД
                    App.Context.OrderBooks.Add(new_orderbook);
                }

                // Уменьшение на одну единицу кол-ва на складе в БД
                book.BookQuantityInStock--;

                // Сохранение изменений в БД
                App.Context.SaveChanges();

                // Обновление данных на странице
                LViewServices.ItemsSource = App.Context.Books.ToList();

                // Включение отображения кнопки корзины
                CartBtn.Visibility = Visibility.Visible;
            }
            else
            {
                // Предупреждение об исключении
                MessageBox.Show("Нет на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
