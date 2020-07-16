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
    public class OverlayController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public OverlayController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        [HttpGet("overlays")]
        public ActionResult GetOverlays()
        {
            var result = _mongoService.GetOverlays();
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpGet("overlays/{id}")]
        public ActionResult GetOverlay([FromRoute] string id)
        {
            var result = _mongoService.GetOverlay(id);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }

        [HttpPut("overlays/{id}")]
        public ActionResult UpdateOverlay([FromRoute] string id, [FromBody] OverlayBO overlay)
        {
            bool result = _mongoService.UpdateOverlay(id, overlay);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "更新成功" : "更新失败",
                return_data = ""
            });
        }

        [HttpDelete("overlays/{id}")]
        public ActionResult DeleteOverlay([FromRoute] string id)
        {
            bool result = _mongoService.DeleteOverlay(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }

        [HttpPost("overlays")]
        public ActionResult PostConfig([FromBody] OverlayBO overlay)
        {
            var result = _mongoService.AddOverlay(overlay);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
    }
}
