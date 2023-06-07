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

namespace bookstore
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Загрузка страницы с книгами в главный фрейм
            MainFrame.Navigate(new Pages.ProductList());
        }
        
        // Функция кнопки открытия корзины
        private void CartPage(object sender, RoutedEventArgs e)
        {
            // Загрузка страницы корзины в главный фрейм
            MainFrame.Navigate(new Pages.CartPage());
        }

        // Функция возвращения с корзины обратно на страницу с книгами
        public void Back(object sender, RoutedEventArgs e)
        {
            // Загрузка страницы с книгами в главный фрейм
            MainFrame.Navigate(new Pages.ProductList());

            // Скрытие элементов итерфейса корзины
            BackBtn.Visibility = Visibility.Hidden;
            OrderCreate.Visibility = Visibility.Hidden;
            FullPrice.Visibility = Visibility.Hidden;
            LastPrice.Visibility = Visibility.Hidden;
            DiscountAmount.Visibility = Visibility.Hidden;
            DeliveryDate.Visibility = Visibility.Hidden;
            PickUpPointsPanel.Visibility = Visibility.Hidden;
        }

    }

}
