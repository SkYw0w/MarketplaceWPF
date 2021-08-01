using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace WPF_MARKET_APP
{
    public class Klient
    {
        [Key]
        public int IdKlient { get; set; }
        public string Familiya { get; set; }
        public string Imya { get; set; }
        public string Otchestvo { get; set; }
        public string login { get; set; }
        public string password { get; set; }
    }

    public class Dostavka
    {
        [Key]
        public int IdDostavka { get; set; }
        public int IdKlient { get; set; }
        public string Adress { get; set; }
        public UInt64 Data { get; set; }
        public int IdZakazl { get; set; }
    }

    public class Zakaz
    {
        [Key]
        public int IdZakaz { get; set; }
        public double Summa { get; set; }
        public string Status { get; set; }
    }

    public class Secret
    {
        [Key]
        public int IdSecrets { get; set; }
        public int IdSotrudnika { get; set; }
        public string Password { get; set; }
    }

    public class Sotrudnik
    {
        [Key]
        public int IdSotrudnik { get; set; }
        public string Login { get; set; }
        public string Familiya { get; set; }
        public string Imya { get; set; }
        public string Otchestvo { get; set; }
        public int IdDolzhnost { get; set; }
        UInt64 Stazh { get; set; }
    }

    public class Dolzhnost
    {
        [Key]
        public int IdDolzhnost { get; set; }
        public string Nazvanie { get; set; }
    }

    public class Kategoriya_tovarov
    {
        [Key]
        public int IdKategoriya_Tovarov { get; set; }
        public string Nazvanie { get; set; }
        public string Opisanie { get; set; }
    }

    public class Tovar
    {
        [Key]
        public int IdTovar { get; set; }
        public string Nazvanie { get; set; }
        public string Opisanie { get; set; }
        public double Cena { get; set; }
        public string Izobrazhenie { get; set; }

        public int IdKategoriya_Tovarov { get; set; }

        [ForeignKey("IdKategoriya_Tovarov")]
        public ICollection<Kategoriya_tovarov> Kategoriyas { get; set; }
        public int IdManufacturer { get; set; }

        [ForeignKey("IdManufacturer")]
        public ICollection<Manufacturer> Manufacturers { get; set; }

        public int Actual { get; set; }

    }

    public class Sostav_zakaza
    {
        [Key]
        public int IdSostavZakaza { get; set; }
        public int IdZakaz { get; set; }
        public int IdTovar { get; set; }
        public int Kolichestvo { get; set; }
    }

    public class Manufacturer
    {
        [Key]
        public int IdManufacturer { get; set; }
        public string name { get; set; }
    }


    public class Additional
    {
        [Key]
        public int IdAdditional { get; set; }
        public int IdTovar1 { get; set; }
        public int IdTovar2 { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Klient> Klient { get; set; }
        public DbSet<Dostavka> Dostavka { get; set; }
        public DbSet<Zakaz> Zakaz { get; set; }
        public DbSet<Secret> Secret { get; set; }
        public DbSet<Sotrudnik> Sotrudnik { get; set; }
        public DbSet<Dolzhnost> Dolzhnost { get; set; }
        public DbSet<Kategoriya_tovarov> Kategoriya_Tovarov { get; set; }
        public DbSet<Tovar> Tovar { get; set; }
        public DbSet<Sostav_zakaza> SostavZakaza { get; set; }

        public DbSet<Manufacturer> Manufacturer { get; set; }

        public DbSet<Additional> Additional { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=root;database=kulba_rgr;",
                new MySqlServerVersion(new Version(8, 0, 19))
            );
        }

    }
}
