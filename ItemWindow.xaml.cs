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
using System.Windows.Shapes;

namespace WPF_MARKET_APP
{
    /// <summary>
    /// Логика взаимодействия для ItemWindow.xaml
    /// </summary>
    public partial class ItemWindow : Window
    {
        int id;
        public TovarInfo t;
        List<string> images;
        public ItemWindow(int id)
        {
            InitializeComponent();
            this.id = id;
            GetItemInfo();
        }

        public void GetItemInfo()
        {
            t = DB.GetTovar(id);
            tbManufacturer.Text = t.manufacturer;
            tbCategory.Text = t.category;
            tbDescription.Text = t.t.Opisanie;
            tbName.Text = t.t.Nazvanie;
            tbPrice.Text = string.Format("Цена {0} руб.",t.t.Cena);
            images = t.images;
            imgItem.Source = new BitmapImage(new Uri(Settings.ImagesPath + images[0]));
            imgItem.MouseMove += ImgItem_MouseMove;
        }

        private void ImgItem_MouseMove(object sender, MouseEventArgs e)
        {
            int positionX = (int)e.GetPosition(imgItem).X;
            int position = 0;
            if (positionX < imgItem.Width * 0.25)
            {
                position = 0;
                SwitchEllipce(0);
            }
            else
                if (positionX < imgItem.Width * 0.5)
            {
                position = 1;
                SwitchEllipce(1);
            }
            else
                if (positionX < imgItem.Width * 0.75)
            {
                position = 2;
                SwitchEllipce(2);
            }
            else
            {
                position = 3;
                SwitchEllipce(3);
            }
            imgItem.Source = new BitmapImage(new Uri(Settings.ImagesPath + images[position]));
        }

        private void SwitchEllipce(int active)
        {
            el0.Fill = active == 0 ? Brushes.Black : Brushes.White;
            el1.Fill = active == 1 ? Brushes.Black : Brushes.White;
            el2.Fill = active == 2 ? Brushes.Black : Brushes.White;
            el3.Fill = active == 3 ? Brushes.Black : Brushes.White;
        }
    }
}
