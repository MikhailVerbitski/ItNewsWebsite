using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebBlazor.Services
{
    public class ServiceOfLocalization
    {
        public Task<Dictionary<string, string>> LanguageDictionary;
        public ServiceOfLocalization(ServiceOfRequest serviceOfRequest)
        {
            LanguageDictionary = serviceOfRequest.GetJsonAsync<Dictionary<string, string>>("/api/Localization/Get");
        }
    }
}
