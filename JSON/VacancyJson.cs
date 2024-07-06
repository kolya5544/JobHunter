using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobHunter.JSON
{
    public partial class VacancyJson
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("found")]
        public long Found { get; set; }

        [JsonProperty("pages")]
        public long Pages { get; set; }

        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("per_page")]
        public long PerPage { get; set; }

        [JsonProperty("clusters")]
        public object Clusters { get; set; }

        [JsonProperty("arguments")]
        public object Arguments { get; set; }

        [JsonProperty("fixes")]
        public object Fixes { get; set; }

        [JsonProperty("suggests")]
        public object Suggests { get; set; }

        [JsonProperty("alternate_url")]
        public Uri AlternateUrl { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("department")]
        public object Department { get; set; }

        [JsonProperty("has_test")]
        public bool HasTest { get; set; }

        [JsonProperty("response_letter_required")]
        public bool ResponseLetterRequired { get; set; }

        [JsonProperty("area")]
        public Area Area { get; set; }

        [JsonProperty("salary")]
        public Salary Salary { get; set; }

        [JsonProperty("type")]
        public Employment Type { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("response_url")]
        public object ResponseUrl { get; set; }

        [JsonProperty("sort_point_distance")]
        public object SortPointDistance { get; set; }

        [JsonProperty("published_at")]
        public string PublishedAt { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("archived")]
        public bool Archived { get; set; }

        [JsonProperty("apply_alternate_url")]
        public Uri ApplyAlternateUrl { get; set; }

        [JsonProperty("insider_interview")]
        public object InsiderInterview { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("alternate_url")]
        public Uri AlternateUrl { get; set; }

        [JsonProperty("relations")]
        public List<object> Relations { get; set; }

        [JsonProperty("employer")]
        public Employer Employer { get; set; }

        [JsonProperty("snippet")]
        public Snippet Snippet { get; set; }

        [JsonProperty("contacts")]
        public object Contacts { get; set; }

        [JsonProperty("schedule")]
        public Employment Schedule { get; set; }

        [JsonProperty("working_days")]
        public List<object> WorkingDays { get; set; }

        [JsonProperty("working_time_intervals")]
        public List<Employment> WorkingTimeIntervals { get; set; }

        [JsonProperty("working_time_modes")]
        public List<Employment> WorkingTimeModes { get; set; }

        [JsonProperty("accept_temporary")]
        public bool AcceptTemporary { get; set; }

        [JsonProperty("professional_roles")]
        public List<Employment> ProfessionalRoles { get; set; }

        [JsonProperty("accept_incomplete_resumes")]
        public bool AcceptIncompleteResumes { get; set; }

        [JsonProperty("experience")]
        public Employment Experience { get; set; }

        [JsonProperty("employment")]
        public Employment Employment { get; set; }

        [JsonProperty("adv_response_url")]
        public object AdvResponseUrl { get; set; }

        [JsonProperty("is_adv_vacancy")]
        public bool IsAdvVacancy { get; set; }

        [JsonProperty("adv_context")]
        public object AdvContext { get; set; }

        [JsonProperty("show_logo_in_search")]
        public object ShowLogoInSearch { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("building")]
        public string Building { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("raw")]
        public string Raw { get; set; }

        [JsonProperty("metro")]
        public object Metro { get; set; }

        [JsonProperty("metro_stations")]
        public List<object> MetroStations { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public partial class Area
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Employer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("alternate_url")]
        public string AlternateUrl { get; set; }

        [JsonProperty("logo_urls")]
        public LogoUrls LogoUrls { get; set; }

        [JsonProperty("vacancies_url")]
        public Uri VacanciesUrl { get; set; }

        [JsonProperty("accredited_it_employer")]
        public bool AccreditedItEmployer { get; set; }

        [JsonProperty("trusted")]
        public bool Trusted { get; set; }
    }

    public partial class LogoUrls
    {
        [JsonProperty("90")]
        public Uri The90 { get; set; }

        [JsonProperty("240")]
        public Uri The240 { get; set; }

        [JsonProperty("original")]
        public string Original { get; set; }
    }

    public partial class Employment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Salary
    {
        [JsonProperty("from")]
        public long? From { get; set; }

        [JsonProperty("to")]
        public long? To { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("gross")]
        public bool Gross { get; set; }
    }

    public partial class Snippet
    {
        [JsonProperty("requirement")]
        public string Requirement { get; set; }

        [JsonProperty("responsibility")]
        public string Responsibility { get; set; }
    }
}
