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
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress;
                response = await httpClient.GetAsync($"http://api.wunderground.com/api/{Secrets.WundergroundApiKey}/astronomy/q/autoip.json?geo_ip={ip.ToString()}");
            }
            else
            {
                response = await httpClient.GetAsync($"http://api.wunderground.com/api/{Secrets.WundergroundApiKey}/astronomy/q/{latitude},{longitude}.json");
            }
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (responseObject["response"]["error"] != null)
            {
                Random random = new Random();
                if (random.NextDouble() < 5)
                {
                    returnObject["theme"] = "light";
                }
                else
                {
                    returnObject["theme"] = "dark";
                }
                return returnObject;
            }
            DateTimeOffset sunrise = DateTimeHelper(int.Parse((string)responseObject["moon_phase"]["sunrise"]["hour"]), int.Parse((string)responseObject["moon_phase"]["sunrise"]["minute"]));
            DateTimeOffset sunset = DateTimeHelper(int.Parse((string)responseObject["moon_phase"]["sunset"]["hour"]), int.Parse((string)responseObject["moon_phase"]["sunset"]["minute"]));
            DateTimeOffset userTime = DateTimeHelper(int.Parse((string)responseObject["moon_phase"]["current_time"]["hour"]), int.Parse((string)responseObject["moon_phase"]["current_time"]["minute"]));
            if (sunrise < userTime && userTime < sunset)
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
