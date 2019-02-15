using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Localization/[action]")]
    public class LocalizationController : Controller
    {
        private readonly IStringLocalizer<LocalizationController> localizer;

        public LocalizationController(IStringLocalizer<LocalizationController> localizer)
        {
            this.localizer = localizer;
        }

        [HttpGet]
        public JsonResult Get()
        {
            var dictionary = new Dictionary<string, string>(localizer.GetAllStrings().Select(a => KeyValuePair.Create<string, string>(a.Name, a.Value)));
            return Json(dictionary);
        }
    }
}
