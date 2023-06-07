using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace bookstore
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Создание сущности БД
        public static Entities.bookstoreEntities Context { get; } = new Entities.bookstoreEntities();

        // Объявление номера заказа
        public static int current_order = -1;
    }
}
