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
    /// Логика взаимодействия для ImageSliderControl.xaml
    /// </summary>
    public partial class ImageSliderControl : UserControl
    {
        List<string> images;
        List<Tovar> additionals;
        public string name;
        public double price;
        public int id;
        public string manufacturer;
        public int actual;
        public string category;
        /// <summary>
        /// Создать экземпляр класса-карточки
        /// </summary>
        /// <param name="images">Список путей к файлам</param>
        /// <param name="name">Название товара</param>
        /// <param name="price">Цена товара</param>
        /// <param name="id">ИД товара</param>
        /// <param name="column">Столбец в сетке</param>
        /// <param name="row">Строка в сетке</param>
        public ImageSliderControl(List<string> images, string name, List<Tovar> additionals, double price, int id, int column, int row, string manufacturer, int actual, string category)
        {
            InitializeComponent();
            this.images = images;
            imgItem.Source = new BitmapImage(new Uri(Settings.ImagesPath + images[0]));

            this.name = name;
            this.price = price;
            this.id = id;
            this.additionals = additionals;
            this.category = category;

            lName.Content = name;
            lPrice.Content = "Цена "+price+" руб.";

            this.actual = actual;
            BorderBrush = Brushes.Black;
            this.manufacturer = manufacturer;
            ChangeLocation(column, row);
            ParseAdditionals();

            if (actual == 1)
            {
                Background = Brushes.White;
            }
            else
            {
                Background = Brushes.Gray;
            }
        }

        public void ChangeLocation(int column, int row)
        {
            Grid.SetColumn(this, column);
            Grid.SetRow(this, row);
        }

        private void ParseAdditionals()
        {
            AName.Text = "Доп. товары:" + (additionals.Count != 0 ? "" : " Нет");
            AName.Width = 300;
            foreach (Tovar t in additionals)
            {
                if (t.Actual == 1)
                {
                    TextBlock name = new TextBlock();
                    name.Text = t.Nazvanie + "; ";
                    name.Tag = t.IdTovar;
                    name.Margin = new Thickness(0, 0, 2, 0);
                    name.MouseMove += Name_MouseMove;
                    name.MouseLeave += Name_MouseLeave;
                    name.MouseDown += Name_MouseDown;
                    SPAdditionals.Children.Add(name);
                }
            }
        }

        private void Name_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int id = (int)((TextBlock)sender).Tag;
            ItemWindow itemWindow = new ItemWindow(id);
            itemWindow.ShowDialog();
        }

        private void Name_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = Brushes.Black;
        }

        private void Name_MouseMove(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Foreground = Brushes.Blue;
        }

        private void imgItem_MouseMove(object sender, MouseEventArgs e)
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
