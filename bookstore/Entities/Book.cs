//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace bookstore.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            this.OrderBooks = new HashSet<OrderBook>();
        }

        // Получение цены книги с валютой
        public string StringPrice { get
            {
                if (BookCurrentDiscount > 0)
                {
                    return BookPrice.ToString() + " руб.";
                }
                else
                {
                    return string.Empty;
                }
            } 
        }

        // Получение цены с расчетом скидки и валюты
        public string Price { get
            {
                if (BookCurrentDiscount > 0)
                {
                    return ((double)BookPrice * (1 - BookCurrentDiscount * 0.01)).ToString() + " руб.";
                }
                else
                {
                    return BookPrice.ToString() + " руб.";
                }
            }
        }

        // Получение данных об остатке книги в шт.
        public string Left { get
            {
                return "Осталось: "+ BookQuantityInStock.ToString() + " шт.";
            }
        }


        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        public string BookAuthor { get; set; }
        public decimal BookPrice { get; set; }
        public int BookQuantityInStock { get; set; }
        public int BookCurrentDiscount { get; set; }
        public byte[] BookImage { get; set; }
        public int IdManufacture { get; set; }
    
        public virtual Manufacture Manufacture { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderBook> OrderBooks { get; set; }
    }
}