using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Localization/[action]")]
    public class LocalizationController : Controller
    {
        private IStringLocalizer localizer;

        public LocalizationController(IStringLocalizerFactory localizerFactory)
        {
            this.localizer = localizerFactory.Create(typeof(LocalizationController));
        }

        [HttpGet]
        public JsonResult Get(string culture = null)
        {
            if(culture != null)
            {
                localizer = localizer.WithCulture(new CultureInfo(culture));
            }
            var dictionary = localizer.GetAllStrings().ToDictionary(x => x.Name, x => x.Value);
            return Json(dictionary);
        }
    }
}
