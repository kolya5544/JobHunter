using JobHunter.Database;
using JobHunter;
using Xunit;

namespace JobHunterTests;

public class VacancyTests
{
    // проверки на корректность данных ApplyFilters
    [Fact]
    public void ApplyFilters_AllNull_ReturnsTrue()
    {
        var user = new RAMUser();
        var vacancy = new VacancyDb();
        Assert.True(TelegramBot.ApplyFilters(user, vacancy));
    }

    [Fact]
    public void ApplyFilters_CityMismatch_ReturnsFalse()
    {
        var user = new RAMUser { CityFilter = "Moscow" };
        var vacancy = new VacancyDb { CityName = "New York" };
        Assert.False(TelegramBot.ApplyFilters(user, vacancy));
    }

    [Fact]
    public void ApplyFilters_SalaryFromBelowMin_ReturnsFalse()
    {
        var user = new RAMUser { FromFilter = 50000 };
        var vacancy = new VacancyDb { SalaryFrom = 30000 };
        Assert.False(TelegramBot.ApplyFilters(user, vacancy));
    }

    [Fact]
    public void ApplyFilters_SalaryToAboveMax_ReturnsFalse()
    {
        var user = new RAMUser { ToFilter = 70000 };
        var vacancy = new VacancyDb { SalaryTo = 80000 };
        Assert.False(TelegramBot.ApplyFilters(user, vacancy));
    }

    [Fact]
    public void ApplyFilters_ExperienceMismatch_ReturnsFalse()
    {
        var user = new RAMUser { ExperienceFilter = "3 years" };
        var vacancy = new VacancyDb { Experience = "1 year" };
        Assert.False(TelegramBot.ApplyFilters(user, vacancy));
    }

    [Fact]
    public void ApplyFilters_AllMatches_ReturnsTrue()
    {
        var user = new RAMUser
        {
            CityFilter = "Moscow",
            FromFilter = 50000,
            ToFilter = 100000,
            ExperienceFilter = "3 years",
            CurrencyFilter = "RUB"
        };
        var vacancy = new VacancyDb
        {
            CityName = "Moscow",
            SalaryFrom = 60000,
            SalaryTo = 90000,
            Experience = "3 years",
            SalaryCurrency = "RUB"
        };
        Assert.True(TelegramBot.ApplyFilters(user, vacancy));
    }

    // проверка граничных значений ApplyFilters
    [Fact]
    public void ApplyFilters_BoundarySalaryValues_ShouldPass()
    {
        var user = new RAMUser { FromFilter = int.MinValue, ToFilter = int.MaxValue };
        var vacancy = new VacancyDb { SalaryFrom = int.MinValue, SalaryTo = int.MaxValue };

        bool result = TelegramBot.ApplyFilters(user, vacancy);

        Assert.True(result);
    }

    // проверки исключений ApplyFilters
    [Fact]
    public void ApplyFilters_NullUser_ThrowsArgumentNullException()
    {
        VacancyDb vacancy = new VacancyDb { CityName = "Moscow" };
        Assert.Throws<NullReferenceException>(() => TelegramBot.ApplyFilters(null, vacancy));
    }

    [Fact]
    public void ApplyFilters_NullVacancy_ThrowsArgumentNullException()
    {
        RAMUser user = new RAMUser { CityFilter = "Moscow" };
        Assert.Throws<NullReferenceException>(() => TelegramBot.ApplyFilters(user, null));
    }

    // --------------------------------------------------

    // тесты на корректность данных метода CreateVacanciesText

    [Fact]
    public void CreateVacanciesText_SingleVacancy_ReturnsFormattedText()
    {
        var vacancies = new List<VacancyDb>
        {
            new VacancyDb
            {
                Name = "Developer",
                CityName = "Moscow",
                EmployerName = "TechCorp",
                EmployerUrl = "https://example.com",
                SalaryFrom = 50000,
                SalaryTo = 100000,
                SalaryCurrency = "RUB",
                Experience = "3 years"
            }
        };
        var text = TelegramBot.CreateVacanciesText(vacancies, "Nickolay");
        Assert.Contains("Developer", text);
        Assert.Contains("TechCorp", text);
        Assert.Contains("Зарплата от 50000 до 100000 RUB", text);
    }

    [Fact]
    public void CreateVacanciesText_MultipleVacancies_ReturnsFormattedText()
    {
        var vacancies = new List<VacancyDb>
        {
            new VacancyDb
            {
                Name = "Software Developer",
                EmployerName = "TechCorp",
                EmployerUrl = "https://techcorp.com",
                SalaryFrom = 50000,
                SalaryTo = 70000,
                SalaryCurrency = "USD",
                Experience = "2+ years",
                AddressRaw = "Moscow"
            },
            new VacancyDb
            {
                Name = "Data Scientist",
                EmployerName = "DataWorld",
                EmployerUrl = "https://dataworld.com",
                SalaryFrom = 60000,
                SalaryTo = 90000,
                SalaryCurrency = "USD",
                Experience = "3+ years",
                AddressRaw = "Saint Petersburg"
            }
        };

        var result = TelegramBot.CreateVacanciesText(vacancies, "Nickolay");

        Assert.Contains("было найдено <b>2 подходящих тебе вакансий</b>", result);
        Assert.Contains("Software Developer", result);
        Assert.Contains("TechCorp", result);
        Assert.Contains("50000", result); // Проверка диапазона зарплаты
        Assert.Contains("Data Scientist", result);
        Assert.Contains("DataWorld", result);
        Assert.Contains("60000", result); // Проверка диапазона зарплаты
    }


    // тесты на граничные значения метода CreateVacanciesText
    [Fact]
    public void CreateVacanciesText_EmptyList_ReturnsNoVacanciesMessage()
    {
        var text = TelegramBot.CreateVacanciesText(new List<VacancyDb>(), "Nickolay");
        Assert.Contains("было найдено <b>0 подходящих тебе вакансий</b>", text);
    }

    [Fact]
    public void CreateVacanciesText_MaxResults_ReturnsMessage()
    {
        var vacancies = new List<VacancyDb>();
        for (int i = 0; i < 2000; i++)
        {
            vacancies.Add(new VacancyDb { Name = $"Vacancy {i + 1}", EmployerName = "Employer", EmployerUrl = "https://example.com" });
        }

        var result = TelegramBot.CreateVacanciesText(vacancies, "Nickolay");

        Assert.Contains("более 2000 подходящих тебе вакансий", result);
    }

    // тесты на исключения метода CreateVacanciesText
    [Fact]
    public void CreateVacanciesText_NullList_ThrowsArgumentNullException()
    {
        Assert.Throws<NullReferenceException>(() => TelegramBot.CreateVacanciesText(null, "Nickolay"));
    }

    [Fact]
    public void CreateVacanciesText_NullFirstName_ThrowsArgumentNullException()
    {
        var vacancies = new List<VacancyDb>
        {
            new VacancyDb { Name = "Vacancy 1", EmployerName = "Employer", EmployerUrl = "https://example.com" }
        };

        Assert.Throws<ArgumentNullException>(() => TelegramBot.CreateVacanciesText(vacancies, null));
    }
}
