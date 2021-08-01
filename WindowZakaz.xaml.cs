using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для WindowZakaz.xaml
    /// </summary>
    public partial class WindowZakaz : Window
    {
        List<int> ids = new List<int>();
        bool inZakaz = false;
        public double sum;
        public WindowZakaz(List<int> ids, UIElementCollection collection,double sum, bool inZakaz = true)
        {
            InitializeComponent();
            ApplicationInfo.WindowZakaz = this;
            this.ids = ids;
            this.sum = sum;
            foreach (MiniItem item in collection)
            {
                spItems.Children.Add((MiniItem)item.Clone());
            }
            this.inZakaz = inZakaz;
            if (inZakaz)
                for (int i = 1; i < spItems.Children.Count; i++)
                {
                    ((MiniItem)spItems.Children[i]).btnDeleteFromBascet.Visibility = Visibility.Hidden;
                }

            var users = DB.GetUsers();
            foreach (var user in users)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Tag = user.First();
                item.Content = user.Second();
                cboxUsers.Items.Add(item);
            }
            cboxUsers.SelectedIndex = 0;
            spNewUser.Visibility = Visibility.Hidden;
            spSelf.Visibility = Visibility.Hidden;
            cbNewUser.Unchecked += CbNewUser_Unchecked;
            cboxSelf.Checked += CboxSelf_Checked;
            cboxSelf.Unchecked += CboxSelf_Unchecked;
        }

        private void CboxSelf_Unchecked(object sender, RoutedEventArgs e)
        {
            spSelf.Visibility = Visibility.Hidden;
        }

        private void CboxSelf_Checked(object sender, RoutedEventArgs e)
        {
            spSelf.Visibility = Visibility.Visible;
        }

        private void CbNewUser_Unchecked(object sender, RoutedEventArgs e)
        {
            spNewUser.Visibility = Visibility.Hidden;
        }

        private void cbNewUser_Checked(object sender, RoutedEventArgs e)
        {
            spNewUser.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!cbNewUser.IsChecked)//Если не новый
            {
                if (ids.Count > 0)
                {
                    if ((bool)cboxSelf.IsChecked)//С доставкой
                    {
                        if (tbAdress.Text != "")
                        {
                            new NewZakaz(ids, (int)((ComboBoxItem)cboxUsers.SelectedItem).Tag, tbAdress.Text);
                            MessageBox.Show("Заказ добавлен", "Добавление заказа",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Адрес доставки не указан","Отказ",MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            new NewZakaz(ids, (int)((ComboBoxItem)cboxUsers.SelectedItem).Tag);
                            MessageBox.Show("Заказ добавлен", "Добавление заказа",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Невозможно сформировать заказ.", "Отказ",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                    MessageBox.Show("Провести заказ невозможно, так как нет товаров в заказе", "Отказ",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else//Новый
            {
                if (tbFam.Text != "" &&
                    tbName.Text != "" &&
                    tbOt.Text != "" &&
                    tbLogin.Text != "" &&
                    tbPassword.Text != "")
                {
                    if (tbAdress.Text != "")
                    {
                        new NewZakaz(ids, tbAdress.Text, tbFam.Text, tbName.Text, tbOt.Text, tbName.Text, tbPassword.Text);
                        MessageBox.Show("Заказ добавлен", "Добавление заказа",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        new NewZakaz(ids, tbFam.Text, tbName.Text, tbOt.Text, tbName.Text, tbPassword.Text);
                        MessageBox.Show("Заказ добавлен", "Добавление заказа",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно сформировать заказ, так как некоректно заполнен новый клиент.", "Отказ",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class NewZakaz
    {
        public NewZakaz(List<int> ids, int idKlient)
        {
            StageOne(ids, idKlient);
        }

        private static void StageOne(List<int> ids, int idKlient, string adr = "No")
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Zakaz.Add(new Zakaz { Status = "ok", Summa = ApplicationInfo.WindowZakaz.sum });
                db.SaveChanges();
                Zakaz z = db.Zakaz.Where(x => x.IdZakaz == db.Zakaz.Max(x => x.IdZakaz)).First();
                int idz = z.IdZakaz;
                Dostavka d = new Dostavka
                {
                    IdZakazl = z.IdZakaz,
                    IdKlient = idKlient,
                    Adress = adr
                };
                db.Dostavka.Add(d);
                db.SaveChanges();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (db.SostavZakaza.Where(x => x.IdZakaz == idz).Where(x => x.IdTovar == ids[i]).Any())
                    {
                        db.SostavZakaza.Where(x => x.IdZakaz == idz).Where(x => x.IdTovar == ids[i]).First().Kolichestvo++;
                    }
                    else
                    {
                        db.SostavZakaza.Add(new Sostav_zakaza
                        {
                            IdTovar = ids[i],
                            IdZakaz = idz,
                            Kolichestvo = 1
                        });
                    }
                    db.SaveChanges();
                }
            }
        }

        public NewZakaz(List<int> ids, int idKlient, string adress)
        {
            StageOne(ids, idKlient, adress);
        }

        public NewZakaz(List<int> ids, string adress, string f, string n, string o, string l, string p)
        {
            int idk = 0;
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Klient.Add(new Klient
                {
                    Familiya = f,
                    Imya = n,
                    login = l,
                    Otchestvo = o,
                    password = p
                });
                db.SaveChanges();
                idk = db.Klient.Where(x => x.IdKlient == (db.Klient.Max(x => x.IdKlient))).First().IdKlient;
            }
            StageOne(ids, idk, adress);
        }

        public NewZakaz(List<int> ids,  string f, string n, string o, string l, string p)
        {
            int idk = 0;
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Klient.Add(new Klient
                {
                    Familiya = f,
                    Imya = n,
                    login = l,
                    Otchestvo = o,
                    password = p
                });
                db.SaveChanges();
                idk = db.Klient.Where(x => x.IdKlient == (db.Klient.Max(x => x.IdKlient))).First().IdKlient;
            }
            StageOne(ids, idk);
        }
    }
}
