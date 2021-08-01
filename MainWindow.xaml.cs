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

namespace WPF_MARKET_APP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ImageSliderControl> controlls;
        public int columnsCount = 4;
        int sort_Abc = 0;
        int sort_Price = 0;
        string sortName = "";
        bool isSelectCard = false;
        public int selectCard;
        public List<int> bascetIds = new List<int>();
        public MainWindow()
        {
            InitializeComponent();
            ApplicationInfo.MainWindow = this;
            controlls = DB.GetCards();
            GridCardGenerating(controlls);
            foreach (ImageSliderControl control in controlls)
            {
                gItems.Children.Add(control);
            }
            List<string> manufacturers = DB.GetManufacturer();
            var cats = DB.GetCategories();

            cbManufacturer.Items.Clear();
            cbManufacturer.Items.Add("Все");
            cbManufacturer.SelectedIndex = 0;
            foreach (string man in manufacturers)
            {
                cbManufacturer.Items.Add(man);
            }
            cbCategory.Items.Clear();
            cbCategory.Items.Add("Все");
            cbCategory.SelectedIndex = 0;
            for(int i = 0; i < cats.Count; i++)
            {
                cbCategory.Items.Add(cats[i].Second());
            }

            if (Settings.role == UserRole.Administrator)
                Adminis();

        }

        private void Adminis()
        {
            MenuItem item = new MenuItem();
            item.Header = "Администрирование";
            MenuItem subItem = new MenuItem();
            subItem.Header = "Добавить";
            subItem.Click += AddItem;
            item.Items.Add(subItem);
            subItem = new MenuItem();
            subItem.Header = "Просмотреть таблицу";
            subItem.Click += ViewTables; ;
            item.Items.Add(subItem);
            mMenu.Items.Add(item);
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            ApplicationInfo.StartWIndow.Close();
            Application.Current.Shutdown(0);
            Environment.Exit(0);
        }

        private void ViewTables(object sender, RoutedEventArgs e)
        {
            WindowViewTables windowViewTables = new WindowViewTables();
            windowViewTables.ShowDialog();
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow();
            addWindow.ShowDialog();
        }

        private void GridCardGenerating(List<ImageSliderControl> controlls)
        {
            if (controlls != null)
            {
                for (int i = 0; i < controlls.Count; i++)
                    gItems.RowDefinitions.Add(new RowDefinition());
                for (int i = 0; i < columnsCount; i++)
                    gItems.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < gItems.ColumnDefinitions.Count; i++)
                    gItems.ColumnDefinitions[i].Width = new GridLength(300, GridUnitType.Auto);
            }
        }

        private void RBNameChecked(object sender, RoutedEventArgs e)
        {
            
            if (((RadioButton)sender).Content.ToString() == "По возрастанию")
            {
                sort_Abc = 1;
            }
            if (((RadioButton)sender).Content.ToString() == "По убыванию")
            {
                sort_Abc = 2;
            }
            if (((RadioButton)sender).Content.ToString() == "Нет сортировки")
            {
                sort_Abc = 0;
            }
            Sorting();
            
        }

        public void Sorting()
        {
            gItems.Children.Clear();
            List<ImageSliderControl> sortedCard = DB.GetCards();
            foreach (var card in sortedCard)
            {
                card.MouseDown += Card_MouseDown;
            }
            switch (sort_Abc)
            {
                case 0:
                    switch (sort_Price)
                    {
                        case 0:
                            break;
                        case 1:
                            sortedCard = sortedCard.OrderBy(x => x.price).ToList();
                            break;
                        case 2:
                            sortedCard = sortedCard.OrderByDescending(x => x.price).ToList();
                            break;
                    }
                    
                    break;
                case 1:
                    switch (sort_Price)
                    {
                        case 0:
                            sortedCard = sortedCard.OrderBy(x => x.name).ToList();
                            break;
                        case 1:
                            sortedCard = sortedCard.OrderBy(x => x.name).ThenBy(x => x.price).ToList();
                            //sortedCard = (from control in controlls orderby control.name ascending, control.price ascending select control).ToList();
                            break;
                        case 2:
                            sortedCard = sortedCard.OrderBy(x => x.name).ThenByDescending(x => x.price).ToList();
                            //sortedCard = (from control in controlls orderby control.name ascending, control.price descending select control).ToList();
                            break;
                    }
                    break;
                case 2:
                    switch (sort_Price)
                    {
                        case 0:
                            sortedCard = sortedCard.OrderByDescending(x => x.name).ToList();
                            break;
                        case 1:
                            sortedCard = sortedCard.OrderByDescending(x => x.name).ThenBy(x => x.price).ToList();
                            //sortedCard = (from control in controlls orderby control.name descending, control.price ascending select control).ToList();
                            break;
                        case 2:
                            sortedCard = sortedCard.OrderByDescending(x => x.name).ThenByDescending(x => x.price).ToList();
                            //sortedCard = (from control in controlls orderby control.name descending, control.price descending select control).ToList();
                            break;
                    }
                    break;
            }
            if (tbSearch.Text != "")
                sortedCard = sortedCard.Where(x => x.name.ToUpper().Contains(tbSearch.Text.ToUpper())).ToList();
            if (cbManufacturer.SelectedItem != null)
                if (cbManufacturer.SelectedItem.ToString() != "Все")
                {
                    sortedCard = sortedCard.Where(x => x.manufacturer == cbManufacturer.SelectedItem.ToString()).ToList();
                }
            if (cbCategory.SelectedItem != null)
                if (cbCategory.SelectedItem.ToString() != "Все")
                {
                    sortedCard = sortedCard.Where(x => x.category == cbCategory.SelectedItem.ToString()).ToList();
                }
            if (sortedCard != null)
            {
                sortedCard = NewLocationsCard(sortedCard);
                GridCardGenerating(sortedCard);
                foreach (ImageSliderControl control in sortedCard)
                {
                    gItems.Children.Add(control);
                }
            }
            try
            {
                if (lStatus != null)
                    lStatus.Text = string.Format("Выведено {0} из {1} товаров", sortedCard != null ? sortedCard.Count : 0, controlls != null ? controlls.Count : 0);
            }
            catch
            {

            }
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!isSelectCard)
                {
                    ((ImageSliderControl)sender).BorderBrush = Brushes.Blue;
                    isSelectCard = true;
                    selectCard = ((ImageSliderControl)sender).id;
                }
                else
                {
                    selectCard = 0;
                    isSelectCard = false;
                    for (int i = 0; i < gItems.Children.Count; i++)
                        if (((ImageSliderControl)gItems.Children[i]).BorderBrush == Brushes.Blue)
                        {
                            ((ImageSliderControl)gItems.Children[i]).BorderBrush = Brushes.Black;
                            break;
                        }
                }
            }
        }

        private List<ImageSliderControl> NewLocationsCard(List<ImageSliderControl> controlls)
        {
            for (int i = 0; i < controlls.Count; i++)
            {
                controlls[i].ChangeLocation(i % 4, i / 4);
            }
            return controlls;
        }

        private void btnBTN_Click(object sender, RoutedEventArgs e)
        {
            Sorting();
        }

        private void RBPriceChecked(object sender, RoutedEventArgs e)
        {
            if (((RadioButton)sender).Content.ToString() == "По возрастанию")
            {
                sort_Price = 1;
            }
            if (((RadioButton)sender).Content.ToString() == "По убыванию")
            {
                sort_Price = 2;
            }
            if (((RadioButton)sender).Content.ToString() == "Нет сортировки")
            {
                sort_Price = 0;
            }
            Sorting();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sorting();
        }

        private void cbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorting();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorting();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Environment.Exit(0);
        }

        private void btnZakaz_Click(object sender, RoutedEventArgs e)
        {
            WindowZakaz windowZakaz = new WindowZakaz(bascetIds, spBascet.Children, (double)(tbBasketResult.Tag));
            windowZakaz.Show();
            Visibility = Visibility.Hidden;
        }
    }
}
