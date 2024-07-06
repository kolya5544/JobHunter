using JobHunter.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace JobHunter
{
    public class API
    {
        public string BaseURL = "https://api.hh.ru";
        public string HH_TOKEN = Environment.GetEnvironmentVariable("HH_TOKEN");
        public HttpClient http = new HttpClient();
        public JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public API()
        {
            http.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("JobHunter", "1.0.0"));
        }

        public async Task<VacancyJson> GetVacancies(int page = 0, int per_page = 100, string text = null, bool getAllPages = true)
        {
            // строим список параметров
            var dict = new Dictionary<string, object>()
            {
                ["page"] = page,
                ["per_page"] = per_page,
                ["text"] = text
            };

            // получаем список вакансий
            var resp = await http.GetAsync($"{BaseURL}/vacancies?{QueryString(dict)}");
            var json = await resp.Content.ReadAsStringAsync();

            // парсим
            var vj = JsonConvert.DeserializeObject<VacancyJson>(json, settings);

            // если запрошены ВСЕ страницы, то запрашиваем все страницы на основе первого результата
            if (getAllPages)
            {
                for (int i = page + 1; i < vj.Pages; i++)
                {
                    // получаем список вакансий с учётом новой страницы
                    dict["page"] = i;

                    resp = await http.GetAsync($"{BaseURL}/vacancies?{QueryString(dict)}");
                    json = await resp.Content.ReadAsStringAsync();

                    var nextVj = JsonConvert.DeserializeObject<VacancyJson>(json, settings);
                    // сливаем новые результаты с предыдущими
                    vj.Items.AddRange(nextVj.Items);
                }
            }

            return vj;
        }

        private static string QueryString(IDictionary<string, object> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return string.Join("&", list);
        }
    }
}
