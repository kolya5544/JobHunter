using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobHunter.Database
{
    [Table("vacancies")]
    public class VacancyDb
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("city")]
        public string? CityName { get; set; }

        [Column("address_raw")]
        public string? AddressRaw { get; set; }

        [Column("employer_name")]
        public string? EmployerName { get; set; }

        [Column("employer_url")]
        public string? EmployerUrl { get; set; }

        [Column("employer_logo")]
        public string? EmployerLogo { get; set; }

        [Column("salary_currency")]
        public string? SalaryCurrency { get; set; }

        [Column("salary_from")]
        public long? SalaryFrom { get; set; }

        [Column("salary_to")]
        public long? SalaryTo { get; set; }

        [Column("snippet_requirement")]
        public string? Requirements { get; set; }

        [Column("snippet_responsibility")]
        public string? Responsibility { get; set; }

        [Column("experience")]
        public string? Experience { get; set; }

        [Column("timestamp")]
        public long Timestamp { get; set; }
    }
}
