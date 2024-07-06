using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace JobHunter.Database
{
    public class MySQL : DbContext
    {
        public string DbString = Environment.GetEnvironmentVariable("DB_STRING");
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                DbString,
                ServerVersion.AutoDetect(DbString),
                options =>
                {
                    options.EnableStringComparisonTranslations();
                }
            );
        }

        public DbSet<VacancyDb> Vacancies { get; set; }
    }
}
