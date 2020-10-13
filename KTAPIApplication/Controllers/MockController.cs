using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    [ApiController]
    public class MockController : ControllerBase
    {
        [HttpPost("mock/earthconfig/tab/select")]
        public string Select()
        {
            //return "{\"return_status\":0,\"return_msg\":\"查询成功\",\"return_data\":[{\"name\":\"反击\",\"group\":[{\"name\":\"基地\",\"members\":[\"61基地\",\"62基地\",\"63基地\",\"64基地\",\"65基地\",\"66基地\",\"67基地\",\"68基地\"]},{\"name\":\"发射平台\",\"members\":[\"发射车\",\"发射井\",\"营区\",\"中心库\",\"待机库\",\"发射场\",\"通信站\"]},{\"name\":\"弹型\",\"members\":[\"DF-5C\",\"DF-31\",\"DF-41\"]}]}]}";
            return "{\"return_status\":0,\"return_msg\":\"查询成功\",\"return_data\":[{\"name\":\"反击\",\"group\":[{\"displayName\":\"基地\",\"sourceName\":\"基地\",\"members\":[{\"displayName\":\"61基地\",\"sourceName\":61},{\"displayName\":\"62基地\",\"sourceName\":62}]},{\"displayName\":\"发射平台\",\"sourceName\":\"发射平台\",\"members\":[{\"displayName\":\"发射车\",\"sourceName\":61},{\"displayName\":\"发射井\",\"sourceName\":\"发射井\"}]},{\"displayName\":\"弹型\",\"sourceName\":\"弹型\",\"members\":[{\"displayName\":\"DF-5C\",\"sourceName\":\"DF-5C\"},{\"displayName\":\"DF-5B\",\"sourceName\":\"DF-5B\"}]}]}]}";
        }

    }
}
