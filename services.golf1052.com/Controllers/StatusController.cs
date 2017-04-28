using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace services.golf1052.com.Controllers
{
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        [HttpGet]
        public async Task<JObject> GetAll()
        {
            string file = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.File.Open("status-settings.json", FileMode.Open)))
            {
                file = reader.ReadToEnd();
            }
            JObject settings = JObject.Parse(file);
            JObject returnO = new JObject();
            HttpClient httpClient = new HttpClient();
            foreach (var o in settings)
            {
                JObject body = (JObject)o.Value;
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.GetAsync((string)body["url"]);
                    if (response.IsSuccessStatusCode)
                    {
                        JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                        returnO[o.Key] = responseObject["status"];
                    }
                    else
                    {
                        returnO[o.Key] = "down";
                    }
                }
                catch (Exception ex)
                {
                    returnO[o.Key] = "down";
                }
            }
            return returnO;
        }
    }
}
