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
    [EnableCors("AllowSameDomain")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public InfoController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }
        /// <summary>
        /// 查询所有。
        /// </summary>
        /// <returns></returns>
        [HttpGet("infos")]
        public ActionResult GetInfos()
        {
            var result = _mongoService.GetInfos();
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        /// <summary>
        /// 查询一条记录。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("infos/{id}")]
        public ActionResult GetInfo([FromRoute] string id)
        {
            var result = _mongoService.GetInfo(id);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        /// <summary>
        /// 添加一条记录。
        /// </summary>
        /// <param name="bo"></param>
        /// <returns></returns>
        [HttpPost("infos")]
        public ActionResult AddInfo([FromBody] InfoBO bo)
        {
            var result = _mongoService.AddInfo(bo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        [HttpPut("infos/{id}")]
        public ActionResult UpdateInfo([FromRoute] string id, [FromBody] InfoBO bo)
        {
            bool result = _mongoService.UpdateInfo(id, bo);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }
        [HttpDelete("infos/{id}")]
        public ActionResult DeleteInfo([FromRoute] string id)
        {
            bool result = _mongoService.DeleteInfo(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }
    }
}
