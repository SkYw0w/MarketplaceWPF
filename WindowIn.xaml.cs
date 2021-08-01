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
    /// Логика взаимодействия для WindowIn.xaml
    /// </summary>
    public partial class WindowIn : Window
    {
        public WindowIn()
        {
            InitializeComponent();
        }

        private void btnIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbLogin!.Text != "")
                if (tbPassword.Text != "")
                    Authorization();
        }

        private void Authorization()
        {
            int role = DB.Autorization(tbLogin.Text, tbPassword.Text);
            if (role == -1)
            {
                MessageBox.Show("Логин или пароль не верны!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                if (role == 3)
                Settings.role = UserRole.Administrator;
            else if (role == 2)
                Settings.role = UserRole.Manager;
            else
                Settings.role = UserRole.User;
            if (role != -1)
            {
                MessageBox.Show("Добро пожаловать, " + Settings.role.ToString() + " " + tbLogin.Text,
                    "Вход", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Visibility = Visibility.Hidden;
            }
        }
    }
}
