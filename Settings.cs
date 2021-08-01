using System;
using System.Collections.Generic;
using System.Text;

namespace WPF_MARKET_APP
{
    public static class Settings
    {
        public static string ImagesPath = "C:\\Users\\kulba\\Desktop\\unik\\Технологии программирования(c#)\\RGR WPF\\WPF marketplace mebel\\ItemImages\\";
        public static UserRole role = UserRole.Administrator; 
    }

    public enum UserRole
    {
        Administrator,
        Manager,
        User
    }
}
