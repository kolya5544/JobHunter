using JobHunter.Database;
using JobHunter.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace JobHunter
{
    public class RAMUser
    {
        public UserStateEnum state;
        public List<VacancyDb> associatedJson = null;
        public int? FromFilter = null;
        public int? ToFilter = null;
        public string? CityFilter = null;
        public string? CurrencyFilter = null;
        public string? ExperienceFilter = null;
    }
    public class UserState
    {
        private Dictionary<long, RAMUser> db = new Dictionary<long, RAMUser>();

        public RAMUser GetUser(long id)
        {
            if (db.ContainsKey(id)) return db[id];
            db.Add(id, new RAMUser() { state = UserStateEnum.INITIAL });
            return db[id];
        }

        public UserStateEnum GetUserState(User user)
        {
            var u = GetUser(user.Id);
            return u.state;
        }

        public void SetUserState(User user, UserStateEnum value)
        {
            var u = GetUser(user.Id);
            u.state = value;
        }
    }

    public enum UserStateEnum
    {
        INITIAL, // send general info, then wait for job name
        WAITING_FOR_NAME, // send initial jobs, then wait for additional filters
        WAITING_FOR_FILTERS, // send jobs with filters preferred, then ask for more filters.
        WAITING_FOR_FILTER_FROM, // wait for lower bound of salary
        WAITING_FOR_FILTER_TO, // wait for upper bound of salary
        WAITING_FOR_FILTER_CITY, // wait for city
        WAITING_FOR_FILTER_CURRENCY, // wait for currency
        WAITING_FOR_FILTER_EXPERIENCE, // wait for experience
        AWAITING_UPDATE, // by the end of codeflow, send user a new version of vacancies...
    }
}
