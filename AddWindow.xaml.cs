using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        string [] paths;
        bool addMode;
        int id;
        public AddWindow()
        {
            InitializeComponent();

            List<Twince<int, string>> cats = DB.GetCategories();
            for (int i = 0; i < cats.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = cats[i].First();
                item.Content = cats[i].Second();
                cbCategory.Items.Add(item);
            }
            cbCategory.SelectedIndex = 0;
            List<Twince<int, string>> manufacturers = DB.GetManufacturers();
            for (int i = 0; i < manufacturers.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = manufacturers[i].First();
                item.Content = manufacturers[i].Second();
                cbManufacturer.Items.Add(item);
            }
            cbManufacturer.SelectedIndex = 0;
            addMode = true;
        }

        public AddWindow(int id)
        {
            InitializeComponent();
            Tovar t = DB.GetSimpleTovar(id);

            List<Twince<int, string>> cats = DB.GetCategories();
            for (int i = 0; i < cats.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = cats[i].First();
                item.Content = cats[i].Second();
                cbCategory.Items.Add(item);
            }

            for (int i = 0; i < cbCategory.Items.Count; i++)
            {
                if ((int)((ComboBoxItem)cbCategory.Items[i]).Tag == t.IdKategoriya_Tovarov)
                {
                    cbCategory.SelectedIndex = i;
                    break;
                }    
            }

            List<Twince<int, string>> manufacturers = DB.GetManufacturers();
            for (int i = 0; i < manufacturers.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = manufacturers[i].First();
                item.Content = manufacturers[i].Second();
                cbManufacturer.Items.Add(item);
            }

            for (int i = 0; i < cbManufacturer.Items.Count; i++)
            {
                if ((int)((ComboBoxItem)cbManufacturer.Items[i]).Tag == t.IdManufacturer)
                {
                    cbManufacturer.SelectedIndex = i;
                    break;
                }
            }

            tbName.Text = t.Nazvanie;
            tbDescription.Text = t.Opisanie;
            tbPrice.Text = t.Cena.ToString();
            cboxActual.IsChecked = t.Actual == 1 ? true : false;

            string[] imgs = t.Izobrazhenie.Split(';');
            lPath1.Content = imgs[0];
            lPath2.Content = imgs[1];
            lPath3.Content = imgs[2];
            lPath4.Content = imgs[3];
            paths = new string[4];
            paths[0] = Settings.ImagesPath + imgs[0];
            paths[1] = Settings.ImagesPath + imgs[1];
            paths[2] = Settings.ImagesPath + imgs[2];
            paths[3] = Settings.ImagesPath + imgs[3];
            this.id = id;
            Title = "Обновить";
            btnAdd.Content = "Обновить";
            if (Settings.role == UserRole.Administrator || Settings.role == UserRole.Manager)
            {
                Button btn = new Button();
                btn.Content = "Удалить доп. товары";
                btn.Click += RemoveAdditionals;
                btn.Height = 30;
                btn.Width = 120;
                btn.Margin = new Thickness(5, 5, 5, 5);
                spMain.Children.Add(btn);
            }
        }

        private void RemoveAdditionals(object sender, RoutedEventArgs e)
        {
            if (DB.RemoveAdditionals(id))
            {
                MessageBox.Show("Доп. товары удалены", "Удаление доп. товаров", MessageBoxButton.OK, MessageBoxImage.Information);
                ApplicationInfo.MainWindow.Sorting();
            }
            else
                MessageBox.Show("Доп. товары не были удалены", "Удаление доп. товаров", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы рисунков (*.bmp, *.jpg)|*.bmp;*.jpg";
            ofd.Title = "Выбрать изображения";
            ofd.Multiselect = true;
            ofd.ShowDialog();
            paths = ofd.FileNames;
            if (paths.Length > 0)
            {
                lPath1.Content = paths[0].Substring(paths[0].LastIndexOf('\\')+1); ;
                if (paths.Length > 1)
                    lPath2.Content = paths[1].Substring(paths[1].LastIndexOf('\\')+1);
                if (paths.Length > 2)
                    lPath3.Content = paths[2].Substring(paths[2].LastIndexOf('\\')+1); ;
                if (paths.Length > 3)
                    lPath4.Content = paths[3].Substring(paths[3].LastIndexOf('\\')+1); ;
            }
        }

        private void tbPrice_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!tbPrice.Text.Contains(".")
               && tbPrice.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            double price;
            if (tbName.Text != "" && tbName.Text.Length <= 45)
            {
                if (tbDescription.Text != "" && tbDescription.Text.Length <= 200)
                {
                    if (tbPrice.Text != "" && double.TryParse(tbPrice.Text, out price))
                    {
                        if (paths.Length > 0)
                        {
                            //throw new NotImplementedException("Добавление товара");
                            string imgs = "";
                            if (paths.Length != 4)
                            {
                                string[] buf = paths;
                                paths = new string[4];
                                int p = 0;
                                for (; p < buf.Length; p++)
                                {
                                    paths[p] = buf[p];
                                }
                                for (; p < 4; p++)
                                    paths[p] = Settings.ImagesPath + "camera.jpg";
                            }
                            for (int i = 0; i < paths.Length; i++)
                            {
                                imgs += paths[i].Substring(paths[i].LastIndexOf('\\') + 1) + ';';
                                if (!File.Exists(Settings.ImagesPath + paths[i].Substring(paths[i].LastIndexOf('\\') + 1)))
                                    File.Copy(paths[i], Settings.ImagesPath + paths[i].Substring(paths[i].LastIndexOf('\\') + 1));
                            }
                            if (addMode)
                            {
                                Tovar t = new Tovar()
                                {
                                    Actual = (bool)cboxActual.IsChecked ? 1 : 0,
                                    Nazvanie = tbName.Text,
                                    Opisanie = tbDescription.Text,
                                    Cena = double.Parse(tbPrice.Text),
                                    Izobrazhenie = imgs,
                                    IdKategoriya_Tovarov = int.Parse(((ComboBoxItem)cbCategory.SelectedItem).Tag.ToString()),
                                    IdManufacturer = int.Parse(((ComboBoxItem)cbManufacturer.SelectedItem).Tag.ToString())
                                };
                                if (DB.AddTovar(t))
                                    MessageBox.Show("Товар добавлен", "Добавление товара", MessageBoxButton.OK, MessageBoxImage.Information);
                                else
                                    MessageBox.Show("Товар не был добавлен из-за внезапной ошибки.", "Добавление товара",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                Tovar t = new Tovar()
                                {
                                    IdTovar = id,
                                    Actual = (bool)cboxActual.IsChecked ? 1 : 0,
                                    Nazvanie = tbName.Text,
                                    Opisanie = tbDescription.Text,
                                    Cena = double.Parse(tbPrice.Text),
                                    Izobrazhenie = imgs,
                                    IdKategoriya_Tovarov = int.Parse(((ComboBoxItem)cbCategory.SelectedItem).Tag.ToString()),
                                    IdManufacturer = int.Parse(((ComboBoxItem)cbManufacturer.SelectedItem).Tag.ToString())
                                };
                                if (DB.UpdateTovar(t))
                                    MessageBox.Show("Товар обновлён", "Обновление товара", MessageBoxButton.OK, MessageBoxImage.Information);
                                else
                                    MessageBox.Show("Товар не был обновлён из-за внезапной ошибки.", "Обновление товара",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Изображение товара осутствует!", "Ошибка изображения товара", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Цена товара пуста или не является числом!", "Ошибка цены товара", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Описание товара пусто или более 200 символов!", "Ошибка описания товара", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Название товара пусто или более 45 символов!", "Ошибка названия товара", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
