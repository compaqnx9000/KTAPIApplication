using KTAPIApplication.bo;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public RuleController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        [HttpGet("rules")]
        public ActionResult GetRules()
        {
            var result = _mongoService.GetRules();
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpGet("rules/{id}")]
        public ActionResult GetRule([FromRoute] string id)
        {
            var result = _mongoService.GetRule(id);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpPut("rules/{id}")]
        public ActionResult UpdateRule([FromRoute] string id, [FromBody] RuleBo bo)
        {
            bool result = _mongoService.UpdateRule(id, bo);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }

        [HttpDelete("rules/{id}")]
        public ActionResult DeleteRule([FromRoute] string id)
        {
            bool result = _mongoService.DeleteRule(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }

        [HttpPost("rules")]
        public ActionResult AddRule([FromBody] RuleBo bo)
        {
            var result = _mongoService.AddRule(bo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
    }
}
