using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace services.golf1052.com.Controllers
{
    [Route("api/[controller]")]
    public class ThemeController : Controller
    {
        [HttpGet]
        public async Task<JObject> GetTheme([FromQuery] string latitude = null, [FromQuery] string longitude = null)
        {
            JObject returnObject = new JObject();
            // Weather Underground killed their free API in Feb 2019 so for now just return a random theme until I find
            // a new API source.
            Random random = new Random();
            if (random.NextDouble() < 0.5)
            {
                returnObject["theme"] = "light";
            }
            else
            {
                returnObject["theme"] = "dark";
            }
            return returnObject;
        }

        private DateTimeOffset DateTimeHelper(int hour, int minute)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, TimeSpan.FromHours(0));
        }
    }
}
