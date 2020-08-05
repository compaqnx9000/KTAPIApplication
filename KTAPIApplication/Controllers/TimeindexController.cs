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
    public class TimeindexController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public TimeindexController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        [HttpGet("timeindexs")]
        public ActionResult GetTimeindexs()
        {
            var result = _mongoService.GetTimeindexs();
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpGet("timeindexs/{id}")]
        public ActionResult GetOverlay([FromRoute] string id)
        {
            var result = _mongoService.GetTimeindex(id);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpPut("timeindexs/{id}")]
        public ActionResult UpdateTimeindex([FromRoute] string id, [FromBody] TimeindexBO bo)
        {
            bool result = _mongoService.UpdateTimeindex(id, bo);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }

        [HttpDelete("timeindexs/{id}")]
        public ActionResult DeleteTimeindex([FromRoute] string id)
        {
            bool result = _mongoService.DeleteTimeindex(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }

        [HttpPost("timeindexs")]
        public ActionResult AddTimeindex([FromBody] TimeindexBO bo)
        {
            var result = _mongoService.AddTimeindex(bo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpGet("timeindexs/query")]
        public ActionResult Query(string platform, string missileNo)
        {
            var result = _mongoService.QueryTimeindex(platform, missileNo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
    }
}
