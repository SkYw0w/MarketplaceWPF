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
    /// Логика взаимодействия для WindowViewTables.xaml
    /// </summary>
    public partial class WindowViewTables : Window
    {
        int loadEndIndex;
        int table;
        public WindowViewTables()
        {
            InitializeComponent();
            dgMain.ColumnWidth = DataGridLength.SizeToCells;
            dgMain.RowEditEnding += DgMain_RowEditEnding;
        }

        private void DgMain_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if(table==0)
                ((Dolzhnost)e.Row.Item).IdDolzhnost= ((Dolzhnost)dgMain.Items[((DataGrid)sender).SelectedIndex - 1]).IdDolzhnost+1;
            
            
        }

        private void Doljnost_Click(object sender, RoutedEventArgs e)
        {
            //dgMain.DataContext = new ApplicationContext();
            table = 0;
            List<Dolzhnost> list;
            using (ApplicationContext db = new ApplicationContext())
            {
                list = db.Dolzhnost.ToList();
                loadEndIndex = list[list.Count - 1].IdDolzhnost;
            }
            dgMain.ItemsSource = list;
            dgMain.Columns[0].IsReadOnly = true;
            Button btnSave = new Button();
            btnSave.Content = "Сохранить изменения";
            btnSave.Height = 30;
            btnSave.Width = 150;
            btnSave.Margin = new Thickness(10, 10, 10, 10);
            btnSave.Click += BtnSaveDoljnost_Click;
            spContainer.Children.Add(btnSave);
        }

        private void BtnSaveDoljnost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var add = ((List<Dolzhnost>)dgMain.Items.SourceCollection).Where(x => x.IdDolzhnost > loadEndIndex).ToList();
                var upd = ((List<Dolzhnost>)dgMain.Items.SourceCollection).Where(x => x.IdDolzhnost <= loadEndIndex).ToList();
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Dolzhnost.AddRange(add);
                    db.Dolzhnost.UpdateRange(upd);
                    db.SaveChanges();
                    Doljnost_Click(null, null);
                }
            }
            catch
            {
                MessageBox.Show("Обновление таблицы не выполнено! Ошибка!","Обновление таблицы",MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Обновление таблицы выполнено! ", "Обновление таблицы", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
