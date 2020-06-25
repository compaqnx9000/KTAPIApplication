using KTAPIApplication.bo;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    [EnableCors("AllowSameDomain")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public ConfigController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        [HttpGet("configs")]
        public ActionResult GetConfigs()
        {
            var result = _mongoService.GetConfigs();
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpGet("configs/{id}")]
        public ActionResult GetConfig([FromRoute] string id)
        {
            var result = _mongoService.GetConfig(id);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpPut("configs/{id}")]
        public ActionResult PutConfig([FromRoute] string id, [FromBody] ConfigBO config)
        {
            bool result = _mongoService.UpdateConfig(id, config);

            return new JsonResult(new
            {
                return_status = result?0:1,
                return_msg = result ? "更新成功": "更新失败",
                return_data = ""
            });
        }

        [HttpDelete("configs/{id}")]
        public ActionResult DeleteConfig([FromRoute] string id)
        {
            bool result = _mongoService.DeleteConfig(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }

        [HttpPost("configs")]
        public ActionResult PostConfig([FromBody] ConfigBO config)
        {
            var result = _mongoService.AddConfig(config);

            return new JsonResult(new
            {
                return_status =   0,
                return_msg = "",
                return_data = result
            });
        }
    }
}
