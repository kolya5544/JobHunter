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
        private readonly string _dbString;

        public MySQL()
        {
            _dbString = Environment.GetEnvironmentVariable("DB_STRING")
                ?? throw new InvalidOperationException("Database connection string is not set.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                _dbString,
                ServerVersion.AutoDetect(_dbString),
                options => options.EnableStringComparisonTranslations()
            );
        }

        public DbSet<VacancyDb> Vacancies { get; set; }
    }
}
