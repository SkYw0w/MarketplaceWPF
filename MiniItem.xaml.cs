using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_MARKET_APP
{
    /// <summary>
    /// Логика взаимодействия для MiniItem.xaml
    /// </summary>
    public partial class MiniItem : UserControl, ICloneable
    {
        public int Id { get; set; }
        public string NameItem { get; set; }
        public double Price { get; set; }
        public MiniItem(int id, string name, double price)
        {
            InitializeComponent();
            Id = id;
            NameItem = name;
            Price = price;
            tblName.Text = name;
            tblPrice.Text = string.Format("Цена {0} руб",price);
        }

        public object Clone()
        {
            return new MiniItem(Id, NameItem, Price);
        }
    }
}
