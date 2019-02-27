using Blazor.Extensions.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBlazor.Services
{
    public class ServiceOfLocalization
    {
        private readonly LocalStorage localStorage;
        private readonly ServiceOfRequest serviceOfRequest;
        public Task<Dictionary<string, string>> LanguageDictionary;
        public List<KeyValuePair<string, string>> Languages { get; set; }
        public ServiceOfLocalization(LocalStorage localStorage, ServiceOfRequest serviceOfRequest)
        {
            this.localStorage = localStorage;
            this.serviceOfRequest = serviceOfRequest;
            Languages = new List<KeyValuePair<string, string>>(new[] {
                new KeyValuePair<string, string>("Русский", "ru-RU"),
                new KeyValuePair<string, string>("English", "en-US")
            });
            LanguageDictionary = ChangeLanguage();
        }
        public async Task SetLanguage(string value)
        {
            await localStorage.SetItem("language", value);
            await JsInteropClasses.ReloadPage.Reload();
        }
        public async Task<Dictionary<string, string>> ChangeLanguage(string language = null)
        {
            language = (language == null) ? await localStorage.GetItem<string>("language") : language;
            string value = null;
            if(language != null)
            {
                value = Languages.FirstOrDefault(a => a.Key == language).Value;
            }
            var dictionary = await serviceOfRequest.GetJsonAsync<Dictionary<string, string>>($"/api/Localization/Get{((language != null) ? $"?culture={value}" : "")}");
            var currentLanguage = Languages.FirstOrDefault(a => a.Key == dictionary["current language"]);
            Languages.Remove(currentLanguage);
            Languages.Insert(0, currentLanguage);
            return dictionary;
        }
    }
}
