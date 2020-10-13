using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    [ApiController]
    public class TargetListController : ControllerBase
    {
        public IConfiguration Configuration { get; }


        public TargetListController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpPost("earthconfig/tab/select")]
        public ActionResult Select([FromBody] dynamic body)
        {
            //天气接口
            string url = Configuration["ThirdPartyServiceUrls:TabSelect"]; //http://192.168.10.202/earthconfig/tab/select
            string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            try
            {
                Task<string> s = MyCore.Utils.HttpCli.PostAsyncJson(url, postBody);
                s.Wait();
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(s.Result);//或者JObject jo = JObject.Parse(jsonText);
                return new JsonResult(jo);
            }
            catch (Exception)
            {
                Console.WriteLine("检查ThirdPartyServiceUrls配置");
            }
            return new JsonResult(new
            {
                return_status = 1,
                return_msg = "检查ThirdPartyServiceUrls配置",
                return_data = ""
            });
        }
    }
}
