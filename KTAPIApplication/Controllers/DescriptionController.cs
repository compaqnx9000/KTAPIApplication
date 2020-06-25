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
    public class DescriptionController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public DescriptionController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        /// <summary>
        /// 查询所有。
        /// </summary>
        /// <returns></returns>
        [HttpGet("descriptions")]
        public ActionResult GetDescriptions()
        {
            var result = _mongoService.GetDescriptions();
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
        [HttpGet("descriptions/{id}")]
        public ActionResult GetDescription([FromRoute] string id)
        {
            var result = _mongoService.GetDescription(id);

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
        [HttpPost("descriptions")]
        public ActionResult AddDescription([FromBody] DescriptionBO bo)
        {
            var result = _mongoService.AddDescription(bo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        [HttpPut("descriptions/{id}")]
        public ActionResult UpdateDescription([FromRoute] string id, [FromBody] DescriptionBO bo)
        {
            bool result = _mongoService.UpdateDescription(id, bo);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }
        [HttpDelete("descriptions/{id}")]
        public ActionResult DeleteDescription([FromRoute] string id)
        {
            bool result = _mongoService.DeleteDescription(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }
    }
}
