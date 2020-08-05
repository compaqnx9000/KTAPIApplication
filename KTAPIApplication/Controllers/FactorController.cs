using KTAPIApplication.bo;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    [ApiController]
    public class FactorController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public FactorController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        /// <summary>
        /// 查询所有。
        /// </summary>
        /// <returns></returns>
        [HttpGet("factors")]
        public ActionResult GetFactors()
        {
            var result = _mongoService.GetFactors();
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        /// <summary>
        /// 根据id查询。
        /// </summary>
        /// <param name="id">mongo自动生成的那个_id。</param>
        /// <returns></returns>
        [HttpGet("factors/{id}")]
        public ActionResult GetFactor([FromRoute] string id)
        {
            var result = _mongoService.GetFactor(id);

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
        [HttpPost("factors")]
        public ActionResult AddFactor([FromBody] FactorBO bo)
        {
            var result = _mongoService.AddFactor(bo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        [HttpPut("factors/{id}")]
        public ActionResult UpdateFactor([FromRoute] string id, [FromBody] FactorBO bo)
        {
            bool result = _mongoService.UpdateFactor(id, bo);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }
        [HttpDelete("factors/{id}")]
        public ActionResult DeleteFactor([FromRoute] string id)
        {
            bool result = _mongoService.DeleteFactor(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }
    }
}
