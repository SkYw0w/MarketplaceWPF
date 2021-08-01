using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_MARKET_APP
{
    public static class DB
    {
        public static List<ImageSliderControl> GetCards()
        {
            List<ImageSliderControl> list = new List<ImageSliderControl>();
            using (ApplicationContext db = new ApplicationContext())
            {
                var items = db.Tovar.ToList();
                var kat = db.Kategoriya_Tovarov.ToList();
                var manufacturers = db.Manufacturer.ToList();
                var addi = db.Additional.ToList();
                var kats = db.Kategoriya_Tovarov.ToList();
                int i = 0;

                foreach (Tovar item in items)
                {
                    List<Tovar> additionals = FindAdditional(item);
                    List<string> paths = item.Izobrazhenie.Split(';').ToList();
                    string manufacturer = manufacturers.Where(x => x.IdManufacturer == item.IdManufacturer).First().name.ToString();
                    string category = kats.Where(x => x.IdKategoriya_Tovarov == item.IdKategoriya_Tovarov).First().Nazvanie.ToString();
                    list.Add(new ImageSliderControl(paths, item.Nazvanie, additionals, item.Cena, item.IdTovar, i % 4, i / 4, manufacturer, item.Actual, category));
                    i++;
                }
            }

            ContextMenu context = new ContextMenu();
            MenuItem citem = new MenuItem();
            citem.Header = "Открыть...";
            citem.Click += cItemClickOpen;
            context.Items.Add(citem);
            if (Settings.role == UserRole.Administrator)
            {
                citem = new MenuItem();
                citem.Click += CItemClickDelete;
                citem.Header = "Удалить";
                context.Items.Add(citem);
                citem = new MenuItem();
                citem.Click += Citem_ClickUpdate;
                citem.Header = "Изменить";
                context.Items.Add(citem);
                citem = new MenuItem();
                citem.Click += AddToSelect; ;
                citem.Header = "Добавить к выделенному";
                context.Items.Add(citem);

            }
            if (Settings.role == UserRole.Manager || Settings.role == UserRole.Administrator)
            {
                citem = new MenuItem();
                citem.Click += AddToBasket;
                citem.Header = "Добавить в корзину";
                context.Items.Add(citem);
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].MouseDoubleClick += DB_MouseDoubleClick;
                list[i].Cursor = Cursors.Hand;
                list[i].Margin = new Thickness(5, 5, 5, 5);
                list[i].ContextMenu = context;
            }
            return list;
        }

        private static void AddToBasket(object sender, RoutedEventArgs e)
        {
            int id = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).id;
            string n = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).name;
            double p = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).price;
            MiniItem item = new MiniItem(id, n, p);
            item.Width = 150;
            item.Height = 50;
            item.btnDeleteFromBascet.Click += BtnDeleteFromBascet_Click;
            ApplicationInfo.MainWindow.spBascet.Children.Add(item);
            ApplicationInfo.MainWindow.bascetIds.Add(id);
            double sum = 0;
            for (int i = 0; i < ApplicationInfo.MainWindow.spBascet.Children.Count; i++)
            {
                if (ApplicationInfo.MainWindow.spBascet.Children[i].GetType() == typeof(MiniItem))
                    sum += ((MiniItem)ApplicationInfo.MainWindow.spBascet.Children[i]).Price;
            }
            ApplicationInfo.MainWindow.tbBasketResult.Text = string.Format("Сумма заказа {0} руб.", sum);
            ApplicationInfo.MainWindow.tbBasketResult.Tag = sum;
        }

        private static void BtnDeleteFromBascet_Click(object sender, RoutedEventArgs e)
        {
            var el = (Grid)((StackPanel)((StackPanel)((Button)sender).Parent).Parent).Parent;
            int id = ((MiniItem)el.Parent).Id;
            ApplicationInfo.MainWindow.bascetIds.Remove(ApplicationInfo.MainWindow.bascetIds.Where(x => x == id).First());
            
            for(int i = 0; i < ApplicationInfo.MainWindow.spBascet.Children.Count; i++)
            {
                if (ApplicationInfo.MainWindow.spBascet.Children[i].GetType() == typeof(MiniItem))
                if (((MiniItem)ApplicationInfo.MainWindow.spBascet.Children[i]).Id == id)
                    ApplicationInfo.MainWindow.spBascet.Children.Remove(ApplicationInfo.MainWindow.spBascet.Children[i]);
            }
            double sum = 0;
            for (int i = 0; i < ApplicationInfo.MainWindow.spBascet.Children.Count; i++)
            {
                if (ApplicationInfo.MainWindow.spBascet.Children[i].GetType() == typeof(MiniItem))
                    sum += ((MiniItem)ApplicationInfo.MainWindow.spBascet.Children[i]).Price;
            }
            ApplicationInfo.MainWindow.tbBasketResult.Text = string.Format("Сумма заказа {0} руб.", sum);
            ApplicationInfo.MainWindow.tbBasketResult.Tag = sum;
        }

        /// <summary>
        /// Return role user
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>1 - user; 2 - manager; 3 - admin</returns>
        internal static int Autorization(string login, string password)
        {
            bool userCheck = false;
            int workerCheckId = 0;
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    userCheck = db.Klient.Where(x => x.login == login).Where(x => x.password == password).Any();
                    if (!userCheck)
                    {
                        var buf = db.Sotrudnik.Where(x => x.Login == login).First().IdSotrudnik;
                        var buf1 = db.Secret.Where(x => x.IdSotrudnika == buf);
                        workerCheckId = buf1.Where(x => x.Password == password).First().IdSotrudnika;
                        workerCheckId = db.Sotrudnik.Where(x => x.IdSotrudnik == workerCheckId).First().IdDolzhnost;
                        if (workerCheckId == 1 || workerCheckId == 2 || workerCheckId == 3)
                        {
                            return 2;
                        }
                        else
                        {
                            if (workerCheckId == 4)
                            {
                                return 3;
                            }
                        }
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        private static void AddToSelect(object sender, RoutedEventArgs e)
        {
            int id = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).id;
            if (AddAdditionals(id))
            {
                MessageBox.Show("Доп. товар добавлен!", "Добавление доп. товара", MessageBoxButton.OK, MessageBoxImage.Information);
                ApplicationInfo.MainWindow.Sorting();
            }
            else
                MessageBox.Show("Доп. товар не был добавлен!", "Ошибка Добавления доп. товара", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static bool AddAdditionals(int id) 
        {
            bool result = false;
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var Tovars = db.Tovar.ToList();
                    int idt = Tovars.Where(x => x.IdTovar == ApplicationInfo.MainWindow.selectCard).First().IdTovar;
                    db.Additional.Add(new Additional()
                    {
                        IdTovar1 = Tovars.Where(x => x.IdTovar == ApplicationInfo.MainWindow.selectCard).First().IdTovar,
                        IdTovar2 = id
                    }) ;
                    db.SaveChanges();
                }
                result = true;
            }
            catch (Exception e)
            { }
            return result;
        }

        public static bool RemoveAdditionals(int id)
        {
            bool result = false;
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var list = db.Additional.Where(x => x.IdTovar1 == id);
                    db.Additional.RemoveRange(list);
                    db.SaveChanges();
                }
                result = true;
            }
            catch { }
            return result;
        }

        private static void Citem_ClickUpdate(object sender, RoutedEventArgs e)
        {
            int id = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).id;
            AddWindow addWindow = new AddWindow(id);
            addWindow.ShowDialog();
        }

        private static void CItemClickDelete(object sender, RoutedEventArgs e)
        {
            int id = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).id;
            DeleteTovar(id);
        }

        private static void cItemClickOpen(object sender, RoutedEventArgs e)
        {
            int id = ((ImageSliderControl)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).id;
            ItemWindow itemWindow = new ItemWindow(id);
            itemWindow.ShowDialog();
        }

        private static void DB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ImageSliderControl control = (ImageSliderControl)sender;
            ItemWindow itemWindow = new ItemWindow(control.id);
            itemWindow.ShowDialog();
        }

        public static List<Tovar> FindAdditional(Tovar item)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var Additionals = db.Additional.ToList();
                var buf = Additionals.FindAll(i => i.IdTovar1 == item.IdTovar);
                List<Tovar> items = new List<Tovar>();
                foreach (Additional a in buf)
                {
                    items.Add(db.Tovar.Where(t => t.IdTovar == a.IdTovar2).First());
                }
                return items;
            }
        }

        public static List<string> GetManufacturer()
        {
            List<string> list = null;
            using (ApplicationContext db = new ApplicationContext())
            {
                list = db.Manufacturer.Select(x => x.name).ToList();
            }
            return list;
        }

        public static TovarInfo GetTovar(int id)
        {
            TovarInfo ti;
            using (ApplicationContext db = new ApplicationContext())
            {
                Tovar t = db.Tovar.Where(x => id == x.IdTovar).First();
                Manufacturer m = db.Manufacturer.Where(x => x.IdManufacturer == t.IdManufacturer).First();
                Kategoriya_tovarov kt = db.Kategoriya_Tovarov.Where(x => x.IdKategoriya_Tovarov == t.IdKategoriya_Tovarov).First();
                ti = new TovarInfo();
                ti.t = t;
                ti.category = kt.Nazvanie;
                ti.manufacturer = m.name;
                ti.images = t.Izobrazhenie.Split(';').ToList();
            }
            return ti;
        }

        public static List<Twince<int, string>> GetUsers()
        {
            List<Twince<int, string>> result = new List<Twince<int, string>>();
            using (ApplicationContext db = new ApplicationContext())
            {
                var klients = db.Klient.ToList();
                foreach (Klient k in klients)
                {
                    result.Add(new Twince<int, string>(k.IdKlient, k.Familiya + " " + k.Imya + " " + k.Otchestvo));
                }
            }
            return result;
        }

        public static bool DeleteTovar(int id)
        {
            bool result = false;
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var exists = db.SostavZakaza.Where(x => x.IdTovar == id).Any();
                    //Если существует
                    if (exists)
                        MessageBox.Show("Невозможно удалить товар, так как он находится в составе одного или более заказов.", "Удаление невозможно",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                    {
                        if (MessageBox.Show("После удаления восстановить товар будет невозможно. Вы уверены что хотите его удалить?", "Удаление товара",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                db.Tovar.Remove(db.Tovar.Where(x => x.IdTovar == id).First());
                                db.SaveChanges();
                                MessageBox.Show("Товар удалён", "Товар удалён", MessageBoxButton.OK, MessageBoxImage.Information);
                                ApplicationInfo.MainWindow.Sorting();
                            }
                            catch
                            {
                                MessageBox.Show("Возникла ошибка удаления", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Отмена удаления", "Отмена удаления", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        public static bool RemoveAdditional(int id)
        {
            bool result = false;
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var adds = db.Additional.Where(x => x.IdTovar1 == id);
                    db.Additional.RemoveRange(adds);
                    db.SaveChanges();
                    result = true;
                }
            }
            catch { }
            return result;
        }

        public static bool UpdateTovar(Tovar t)
        {
            bool result = false;
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Tovar.Update(t);
                    db.SaveChanges();
                    result = true;
                }
            }
            catch { }
            return result;
        }

        public static List<Twince<int, string>> GetCategories()
        {
            List<Twince<int, string>> result = new List<Twince<int, string>>();
            List<Kategoriya_tovarov> buf;
            using (ApplicationContext db = new ApplicationContext())
            {
                buf = db.Kategoriya_Tovarov.ToList();
            }
            foreach (Kategoriya_tovarov c in buf)
            {
                result.Add(new Twince<int, string>(c.IdKategoriya_Tovarov, c.Nazvanie));
            }
            return result;
        }

        public static List<Twince<int, string>> GetManufacturers()
        {
            List<Twince<int, string>> result = new List<Twince<int, string>>();
            List<Manufacturer> buf;
            using (ApplicationContext db = new ApplicationContext())
            {
                buf = db.Manufacturer.ToList();
            }
            foreach (Manufacturer m in buf)
            {
                result.Add(new Twince<int, string>(m.IdManufacturer, m.name));
            }
            return result;
        }

        public static bool AddTovar(Tovar t)
        {
            bool result = false;
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Tovar.Add(t);
                db.SaveChanges();
                result = true;
            }
            return result;
        }

        public static Tovar GetSimpleTovar(int id)
        {
            Tovar t = null;
            using (ApplicationContext db = new ApplicationContext())
            {
                t = db.Tovar.Where(x => x.IdTovar == id).First();
            }
            return t;
        }
    }
        public struct TovarInfo
        {
            public Tovar t;
            public string manufacturer;
            public string category;
            public List<string> images;
        }
    
    public class Twince<Tfirst, Tsecond>
    {
        Tfirst first;
        Tsecond second;


        public Tfirst First()
        {
            return first;
        }
        public Tsecond Second()
        {
            return second;
        }

        public Twince(Tfirst first, Tsecond second)
        {
            this.first = first;
            this.second = second;
        }
    }
}
