using JobHunter.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public async Task<VacancyJson> GetVacancies(int page = 0, int per_page = 100, string text = null, bool getAllPages = true, string? area = null, string? currency = null, string? experience = null)
        {
            // справочник для experience
            Dictionary<string, string> expKVP = new Dictionary<string, string>()
            {
                ["Нет опыта"] = "noExperience",
                ["От 1 года до 3 лет"] = "between1And3",
                ["От 3 до 6 лет"] = "between3And6",
                ["Более 6 лет"] = "moreThan6"
            };

            // справочник для area
            Dictionary<string, string> areaKVP = new Dictionary<string, string>();
            if (area is not null)
            {
                var areaResp = await http.GetAsync($"{BaseURL}/areas");
                var areasJson = await areaResp.Content.ReadAsStringAsync();

                // парсим справочник
                var aJson = JsonConvert.DeserializeObject<AreaJson>(areasJson);
                areaKVP.Add(aJson.Name, aJson.Id);

                foreach (var i in aJson.Areas)
                {
                    areaKVP.Add(i.Name, i.Id);

                    i.Areas.ForEach(x =>
                    {
                        areaKVP.Add(x.Name, x.Id);
                    });
                }
            }

            // строим список параметров
            var dict = new Dictionary<string, object>()
            {
                ["page"] = page,
                ["per_page"] = per_page,
                ["text"] = text
            };

            if (area is not null)
            {
                dict.Add("area", areaKVP[area]);
            }

            if (currency is not null)
            {
                dict.Add("currency", currency);
            }

            if (experience is not null)
            {
                dict.Add("experience", expKVP[experience]);
            }

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
