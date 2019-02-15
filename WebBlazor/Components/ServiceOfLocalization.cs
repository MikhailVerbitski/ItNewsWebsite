using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebBlazor.Components
{
    public class ServiceOfLocalization
    {
        public Task<Dictionary<string, string>> LanguageDictionary;
        public ServiceOfLocalization(ServiceOfAuthorize serviceOfAuthorize)
        {
            LanguageDictionary = serviceOfAuthorize.GetJsonAsync<Dictionary<string, string>>("/api/Localization/Get");
        }
    }
}
