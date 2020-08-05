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
    public class DamageLevelController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public DamageLevelController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        /// <summary>
        /// 查询所有。
        /// </summary>
        /// <returns></returns>
        [HttpGet("damageLevels")]
        public ActionResult GetDamageLevels()
        {
            var result = _mongoService.GetDamageLevels();
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
        [HttpGet("damageLevels/{id}")]
        public ActionResult GetDamageLevel([FromRoute] string id)
        {
            var result = _mongoService.GetDamageLevel(id);

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
        [HttpPost("damageLevels")]
        public ActionResult AddDamageLevel([FromBody] DamageLevelBO bo)
        {
            var result = _mongoService.AddDamageLevel(bo);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        [HttpPut("damageLevels/{id}")]
        public ActionResult UpdateDamageLevel([FromRoute] string id, [FromBody] DamageLevelBO bo)
        {
            bool result = _mongoService.UpdateDamageLevel(id, bo);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }
        [HttpDelete("damageLevels/{id}")]
        public ActionResult DeleteDamageLevel([FromRoute] string id)
        {
            bool result = _mongoService.DeleteDamageLevel(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }
    }
}
