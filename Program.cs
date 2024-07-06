using JobHunter.Database;
using JobHunter.JSON;
using System.Text;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobHunter
{
    internal class Program
    {
        public static TelegramBotClient bot;
        public static string TG_TOKEN = Environment.GetEnvironmentVariable("API_TOKEN");
        public static UserState us = new UserState();
        public static API api = new API();

        static async Task Main(string[] args)
        {
            bot = new TelegramBotClient(TG_TOKEN);
            bot.StartReceiving(HandleNewMessage, async (bot, ex, ct) => Console.WriteLine(ex), new Telegram.Bot.Polling.ReceiverOptions() { ThrowPendingUpdates = true });

            var me = await bot.GetMeAsync();
            Console.WriteLine($"@{me.Username} is running...");

            while (true) Thread.Sleep(1000); // waste CPU cycles
        }

        private static async Task HandleNewMessage(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message is null) return;
            if (update.Message.Text is null) return;

            var msg = update.Message;
            var state = us.GetUserState(msg.From);
            Console.WriteLine($"Received message '{msg.Text}' in {msg.Chat}, state: {state}");

            var disableFilter = new KeyboardButton("❌ Отключить фильтр ❌");

            var experienceKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton>()
                    {
                        new KeyboardButton("Нет опыта"),
                        new KeyboardButton("От 1 года до 3 лет"),
                        new KeyboardButton("От 3 до 6 лет"),
                        new KeyboardButton("Более 6 лет"),
                        disableFilter
                    });

            var disableKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton>() { disableFilter });

            if (msg.Text.Equals("/clear", StringComparison.OrdinalIgnoreCase))
            {
                // очищаем дазу банных и ресетаем юзверя :D
                using (var db = new MySQL())
                {
                    db.Vacancies.RemoveRange(db.Vacancies); // 0_0 drops all the rows!
                    await db.SaveChangesAsync();
                }
                await bot.SendTextMessageAsync(msg.Chat, $"😀 База данных успешна очищена! Все данные будут подгружены с нуля.");
                us.SetUserState(msg.From, UserStateEnum.INITIAL);
            }

            if (state == UserStateEnum.INITIAL)
            {
                await bot.SendTextMessageAsync(msg.Chat, $"👷‍♂️👷‍♀️⚒ Приветствую, <i>{msg.From.FirstName}</i>! Давай познакомимся.\r\nЯ - <b>JobHunter</b>, твой помощник в поиске работы на сайте HH.ru прямо не выходя из Telegram.\r\nТы можешь использовать этого бота для поиска вакансий. <b>Давай попробуем - введи название интересующей тебя вакансии.</b>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                us.SetUserState(msg.From, state + 1);
                return;
            }
            if (state == UserStateEnum.WAITING_FOR_NAME)
            {
                string name = msg.Text.Trim();
                if (name.Length <= 1 || name.Length >= 127 || name.StartsWith("/"))
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"❌ <i>{msg.From.FirstName}</i>, <b>введи название должности длиной от 2 до 126 символов</b>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    return;
                }

                await bot.SendTextMessageAsync(msg.Chat, $"⌛ <i>{msg.From.FirstName}</i>, запрашиваем вакансии, содержащие <b>\"{name}\"</b> в названии или описании. Ожидай!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);

                var results = await ProcessVacancyDb(name);
                var user = us.GetUser(msg.From.Id);
                user.associatedJson = results;
                user.request = name;

                var text = CreateVacanciesText(results, msg.From.FirstName);

                await bot.SendTextMessageAsync(msg.Chat, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, disableWebPagePreview: true);

                us.SetUserState(msg.From, state + 1);
                return;
            }
            if (state == UserStateEnum.WAITING_FOR_FILTERS)
            {
                #region Команды /set_XXX
                // тут мы ожидаем только одну из предложенных команд
                string cmd = msg.Text.Trim();
                
                if (cmd == "/set_from")
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"⌛ <i>{msg.From.FirstName}</i>, введи минимальную зарплату, которую ты хотел бы получать на работе.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                                                                                                                                                     replyMarkup: disableKeyboard);

                    us.SetUserState(msg.From, UserStateEnum.WAITING_FOR_FILTER_FROM);
                    return;
                }
                if (cmd == "/set_to")
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"⌛ <i>{msg.From.FirstName}</i>, введи максимальную зарплату, которую ты хотел бы получать на работе.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                                                                                                                                                      replyMarkup: disableKeyboard);

                    us.SetUserState(msg.From, UserStateEnum.WAITING_FOR_FILTER_TO);
                    return;
                }
                if (cmd == "/set_city")
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"⌛ <i>{msg.From.FirstName}</i>, введи город, в котором ты хотел бы работать.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                                                                                                                              replyMarkup: disableKeyboard);

                    us.SetUserState(msg.From, UserStateEnum.WAITING_FOR_FILTER_CITY);
                    return;
                }
                if (cmd == "/set_currency")
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"⌛ <i>{msg.From.FirstName}</i>, введи валюту, в которой хотел бы получать ЗП. Популярные значения: RUR, BYR, UAH", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                                                                                                                                                                  replyMarkup: disableKeyboard);

                    us.SetUserState(msg.From, UserStateEnum.WAITING_FOR_FILTER_CURRENCY);
                    return;
                }
                if (cmd == "/set_experience")
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"⌛ <i>{msg.From.FirstName}</i>, выбери опыт, по которому нужно выполнять поиск.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                                                                                                                                 replyMarkup: experienceKeyboard);

                    us.SetUserState(msg.From, UserStateEnum.WAITING_FOR_FILTER_EXPERIENCE);
                    return;
                }
                if (cmd == "/reset")
                {
                    us.SetUserState(msg.From, UserStateEnum.INITIAL);
                    var u = us.GetUser(msg.From.Id);
                    u.associatedJson = null;
                    u.request = null;
                    u.CurrencyFilter = null;
                    u.ExperienceFilter = null;
                    u.CityFilter = null;
                    u.ToFilter = null;
                    u.FromFilter = null;
                    await HandleNewMessage(client, update, token); // cry about it
                    return;
                }
                #endregion
            }
            #region Filters processed
            if (state == UserStateEnum.WAITING_FOR_FILTER_EXPERIENCE)
            {
                // применяем фильтр по опыту...
                string[] allowed = ["Нет опыта", "От 1 года до 3 лет", "От 3 до 6 лет", "Более 6 лет", "❌ Отключить фильтр ❌"];
                if (!allowed.Contains(msg.Text))
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"❌ <i>{msg.From.FirstName}</i>, не понял! Пользуйся кнопками в Telegram для выбора.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                                                                                                                                     replyMarkup: experienceKeyboard);
                    return;
                }

                // применяем фильтр
                var fltr = msg.Text;
                if (msg.Text == "❌ Отключить фильтр ❌") fltr = null;
                us.GetUser(msg.From.Id).ExperienceFilter = fltr;

                // мда теперь наверное стоит временно включить стейт получения обновления и прислать всё таки дурочку обновления
                us.SetUserState(msg.From, UserStateEnum.AWAITING_UPDATE);
            }
            if (state == UserStateEnum.WAITING_FOR_FILTER_CITY)
            {
                // применяем фильтр
                var fltr = msg.Text;
                if (msg.Text == "❌ Отключить фильтр ❌") fltr = null;
                us.GetUser(msg.From.Id).CityFilter = fltr;

                us.SetUserState(msg.From, UserStateEnum.AWAITING_UPDATE);
            }
            if (state == UserStateEnum.WAITING_FOR_FILTER_CURRENCY)
            {
                // применяем фильтр
                var fltr = msg.Text;
                if (msg.Text == "❌ Отключить фильтр ❌") fltr = null;
                us.GetUser(msg.From.Id).CurrencyFilter = fltr;

                us.SetUserState(msg.From, UserStateEnum.AWAITING_UPDATE);
            }
            if (state == UserStateEnum.WAITING_FOR_FILTER_FROM)
            {
                // применяем фильтр
                var fltr = msg.Text;
                if (msg.Text == "❌ Отключить фильтр ❌") fltr = null;
                us.GetUser(msg.From.Id).FromFilter = fltr is null ? null : int.Parse(fltr);

                us.SetUserState(msg.From, UserStateEnum.AWAITING_UPDATE);
            }
            if (state == UserStateEnum.WAITING_FOR_FILTER_TO)
            {
                // применяем фильтр
                var fltr = msg.Text;
                if (msg.Text == "❌ Отключить фильтр ❌") fltr = null;
                us.GetUser(msg.From.Id).ToFilter = fltr is null ? null : int.Parse(fltr);

                us.SetUserState(msg.From, UserStateEnum.AWAITING_UPDATE);
            }
            #endregion

            if (us.GetUserState(msg.From) == UserStateEnum.AWAITING_UPDATE)
            {
                // прислать с учётом фильтров...

                var user = us.GetUser(msg.From.Id);
                var results = user.associatedJson;

                var filtered = results.Where((z) => ApplyFilters(user, z)).ToList();

                // если с фильтрами в БД вакансий нет - ПЕРЕПРОВЕРИТЬ на всякий случай 0_0, спросив у АПИшки
                if (filtered.Count < 10)
                {
                    var newVacancies = await ProcessVacancyDb(user.request, user.CityFilter, user.CurrencyFilter, user.ExperienceFilter, true);
                    filtered = newVacancies.Where((z) => ApplyFilters(user, z)).ToList();
                }

                var text = CreateVacanciesText(filtered, msg.From.FirstName);
                text += $"\r\nТекущие фильтры: <b>ГОРОД: {(user.CityFilter is null ? "❌" : user.CityFilter)}</b>, <b>ЗП ОТ: {(user.FromFilter is null ? "❌" : user.FromFilter)}</b>, <b>ЗП ДО: {(user.ToFilter is null ? "❌" : user.ToFilter)}</b>, <b>ОПЫТ: {(user.ExperienceFilter is null ? "❌" : user.ExperienceFilter)}</b>, <b>ВАЛЮТА: {(user.CurrencyFilter is null ? "❌" : user.CurrencyFilter)}</b>";

                await bot.SendTextMessageAsync(msg.Chat, text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, disableWebPagePreview: true);
                us.SetUserState(msg.From, UserStateEnum.WAITING_FOR_FILTERS);
                return;
            }
        }

        public static bool ApplyFilters(RAMUser user, VacancyDb z)
        {
            var cityMatch = user.CityFilter is null ? true : z.CityName is not null && z.CityName.Equals(user.CityFilter, StringComparison.OrdinalIgnoreCase);
            var fromMatch = user.FromFilter is null ? true : (z.SalaryFrom.HasValue ? z.SalaryFrom >= user.FromFilter.Value : false);
            var toMatch = user.ToFilter is null ? true : (z.SalaryTo.HasValue ? z.SalaryTo <= user.ToFilter.Value : false);
            var experienceMatch = user.ExperienceFilter is null ? true : z.Experience is not null && z.Experience.Equals(user.ExperienceFilter, StringComparison.OrdinalIgnoreCase);
            var currencyMatch = user.CurrencyFilter is null ? true : z.SalaryCurrency is not null && z.SalaryCurrency.Equals(user.CurrencyFilter, StringComparison.OrdinalIgnoreCase);

            return cityMatch && fromMatch && toMatch && experienceMatch && currencyMatch;
        }

        public static string CreateVacanciesText(List<VacancyDb> results, string firstName)
        {
            bool isOverflow = results.Count >= 2000;
            StringBuilder sb = new StringBuilder($"✅ <i>{firstName}</i>, было найдено <b>{(isOverflow ? "более 2000" : results.Count)} подходящих тебе вакансий</b>. Вот некоторые из них:\r\n");
            for (int i = 0; i < Math.Min(10, results.Count); i++)
            {
                var vacancy = results[i];

                string? cityIfExists = (vacancy.AddressRaw is null) ? null : $" в районе <a href=\"https://yandex.ru/maps/?mode=search&text={vacancy.AddressRaw}\">{vacancy.AddressRaw}</a>";
                string? salaryIfSpecified = (vacancy.SalaryCurrency is null) ? "Зарплата не указана." : $"Зарплата{(vacancy.SalaryFrom is null ? null : $" от {vacancy.SalaryFrom}")}{(vacancy.SalaryTo is null ? null : $" до {vacancy.SalaryTo}")} {vacancy.SalaryCurrency}.";

                sb.AppendLine($"[#{i + 1}] \"<b>{vacancy.Name}</b>\"{cityIfExists} @ <i>\"<a href=\"{vacancy.EmployerUrl}\">{vacancy.EmployerName}</a>\"</i>. {salaryIfSpecified} Опыт: <i>{vacancy.Experience}</i>");
            }

            sb.AppendLine($"\r\n💡 Вы можете <i>отфильтровать</i> результаты поиска по <b>зарплате от /set_from</b>, <b>зарплате до /set_to</b>, <b>городу /set_city</b>, <b>валюте ЗП /set_currency</b> и <b>опыту /set_experience</b>. Используйте <b>/reset</b> чтобы выполнить новый поиск, или <b>/clear</b> для очистки кеша и поиска с нуля.");
            //sb.AppendLine($"🎯 Альтернативно, вы можете выбрать вакансию из представленных, используя <b>/select</b>"); 
            // :)
            return sb.ToString();
        }

        public static async Task<List<VacancyDb>> ProcessVacancyDb(string name, string? area = null, string? currency = null, string? experience = null, bool ignoreDb = false)
        {
            using (var db = new MySQL())
            {
                if (!ignoreDb)
                {
                    var applicable = db.Vacancies.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase) || a.Responsibility.Contains(name, StringComparison.OrdinalIgnoreCase) || a.Requirements.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

                    // удаление через 24 часа
                    var toRemove = applicable.Where((z) => (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - z.Timestamp) >= 24 * 3600).ToList();
                    db.Vacancies.RemoveRange(toRemove);
                    foreach (var it in toRemove) applicable.Remove(it);
                    await db.SaveChangesAsync();

                    if (applicable is not null && applicable.Count > 0)
                    {
                        return applicable; // нашли в кеше - вернули
                    }
                }

                // иначе идём по апи
                var results = await api.GetVacancies(text: name, area: area, currency: currency, experience: experience);
                var newResults = new List<VacancyDb>();
                // ...и добавляем результаты в БД
                foreach (var item in results.Items)
                {
                    var vdb = new VacancyDb()
                    {
                        Id = item.Id,

                        Name = item.Name,
                        AddressRaw = (item.Address is null) ? null : item.Address.Raw,
                        CityName = (item.Address is null) ? null : item.Address.City,
                        EmployerName = item.Employer.Name,
                        EmployerUrl = item.Employer.AlternateUrl,
                        EmployerLogo = (item.Employer.LogoUrls is null) ? null : item.Employer.LogoUrls.Original,
                        Requirements = item.Snippet.Requirement,
                        Responsibility = item.Snippet.Responsibility,
                        SalaryCurrency = (item.Salary is null) ? null : item.Salary.Currency,
                        SalaryFrom = (item.Salary is null || item.Salary.From is null) ? null : item.Salary.From,
                        SalaryTo = (item.Salary is null || item.Salary.To is null) ? null : item.Salary.To,
                        Experience = item.Experience.Name,
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    };
                    if (!newResults.Any((z) => z.Id == item.Id))
                    {
                        newResults.Add(vdb);
                    }
                    if (db.Vacancies.Find(vdb.Id) is null) db.Add(vdb);
                }
                await db.SaveChangesAsync();
                return newResults;
            }
        }
    }
}
