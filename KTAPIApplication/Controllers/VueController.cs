using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace KTAPIApplication.Controllers
{

    [ApiController]
    public class VueController : ControllerBase
    {
        private readonly IDamageAnalysisService _analysisService;
        public IConfiguration Configuration { get; }


        public VueController(IDamageAnalysisService analysisService, IConfiguration configuration)
        {
            _analysisService = analysisService ??
                throw new ArgumentNullException(nameof(analysisService));

            Configuration = configuration;

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

        [HttpGet("jumpUrl")]
        public ActionResult JumpUrl()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = Configuration["ServiceUrls:JumpURL"]
            });
        }

    }
}
