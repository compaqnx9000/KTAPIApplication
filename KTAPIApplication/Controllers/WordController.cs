using System;
using System.Collections;
using System.IO;
using KTAPIApplication.bo;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;


namespace KTAPIApplication.Controllers
{
    [EnableCors("AllowSameDomain")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly IMongoService _mongoService;
       
        public WordController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }

        [HttpPost("word")]
        public IActionResult Word([FromBody]BrigadeBO bo)
        {

            // post 旅名
            // return info：经度、纬度，
            //mock：时间，经、纬、高、当量

            var result = _mongoService.QueryByBrigade(bo.brigade);

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = result
            });

        }
        
    }


}
