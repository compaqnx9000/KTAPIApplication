using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KTAPIApplication.Controllers
{

    [ApiController]
    public class VueController : ControllerBase
    {
        private readonly IDamageAnalysisService _analysisService;

        public VueController(IDamageAnalysisService analysisService)
        {
            _analysisService = analysisService ??
                throw new ArgumentNullException(nameof(analysisService));
        }

        [HttpGet("single")]
        public ActionResult Single(string data)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _analysisService.Single(data)
            });
        }

        [HttpGet("query")]
        [HttpPost("query")]
        public ActionResult Query()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _analysisService.Query()
            });
        }

    }
}
