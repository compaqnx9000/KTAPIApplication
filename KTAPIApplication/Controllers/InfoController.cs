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
    public class InfoController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        public InfoController(IMongoService mongoService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));
        }
 
        [HttpGet("infos")]
        public IActionResult GetInfos()
        {
            var resultFromRepo = _mongoService.GetInfos();
            if(resultFromRepo ==null || resultFromRepo.Count() <= 0)
            {
                return NotFound(new
                {
                    return_status = 1,
                    return_msg = "",
                    return_data = resultFromRepo
                });
            }
            return Ok(new
            {
                return_status = 0,
                return_msg = "",
                return_data = resultFromRepo
            });
        }

        [HttpGet("infos/{id}",Name = "GetInfo")]
        public IActionResult GetInfo([FromRoute] string id)
        {
            var resultFromRepo = _mongoService.GetInfo(id);

            if (resultFromRepo == null)
            {
                return NotFound(new
                {
                    return_status = 1,
                    return_msg = "",
                    return_data = resultFromRepo
                });
            }

            return Ok(new
            {
                return_status = 0,
                return_msg = "",
                return_data = resultFromRepo
            });
        }

        [HttpPost("infos")]
        public IActionResult CreateInfo([FromBody] InfoBO bo)
        {
            var result = _mongoService.AddInfo(bo);

            return Ok(new
            {
                id = result,
                return_status = 0,
                return_msg = "",
                return_data = result
            });
        }
        [HttpPut("infos/{id}")]
        public IActionResult UpdateInfo([FromRoute] string id, [FromBody] InfoBO bo)
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
        public IActionResult DeleteInfo([FromRoute] string id)
        {
            bool result = _mongoService.DeleteInfo(id);

            return new JsonResult(new
            {
                return_status = result ? 0 : 1,
                return_msg = result ? "删除成功" : "删除失败",
                return_data = ""
            });
        }


        // add 0715
        [HttpGet("taggroup")]
        public IActionResult Taggroup()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _mongoService.Taggroup()
            });
        }

        [HttpGet("fields")]
        public IActionResult Fields()
        {
            Dictionary<string, string> flds = new Dictionary<string, string>();

            flds.Add("name", "string");
            flds.Add("brigade", "string");
            flds.Add("warBase", "string");
            flds.Add("shock_wave_01", "double");
            flds.Add("shock_wave_02", "double");
            flds.Add("shock_wave_03", "double");
            flds.Add("nuclear_radiation_01", "double");
            flds.Add("nuclear_radiation_02", "double");
            flds.Add("nuclear_radiation_03", "double");
            flds.Add("thermal_radiation_01", "double");
            flds.Add("thermal_radiation_02", "double");
            flds.Add("thermal_radiation_03", "double");
            flds.Add("nuclear_pulse_01", "double");
            flds.Add("nuclear_pulse_02", "double");
            flds.Add("nuclear_pulse_03", "double");
            flds.Add("fallout_01", "double");
            flds.Add("fallout_02", "double");
            flds.Add("fallout_03", "double");
            flds.Add("lon", "double");
            flds.Add("lat", "double");
            flds.Add("alt", "double");
            flds.Add("launchUnit", "string");
            flds.Add("platform", "string");
            flds.Add("warZone", "string");
            flds.Add("combatZone", "string");
            flds.Add("platoon", "string");
            flds.Add("missileNo", "string");
            flds.Add("missileNum", "double");
            flds.Add("prepareTime", "double");
            //flds.Add("targetBindingTime", "double");
            //flds.Add("defenseBindingTime", "double");
            flds.Add("fireRange", "double");
            // 2020-10-10
            flds.Add("useState", "string");
            flds.Add("structureLength", "double");
            flds.Add("structureWidth", "double");
            flds.Add("structureHeight", "double");
            flds.Add("headCount", "int");
            flds.Add("bodyCount", "int");
            flds.Add("platCount", "int");
            flds.Add("notes", "string");

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = flds.ToList()
            });;
        }

        [HttpGet("fieldsExclude")]
        public IActionResult FieldsExclude()
        {
            Dictionary<string, string> flds = new Dictionary<string, string>();

            flds.Add("name", "string");
            flds.Add("brigade", "string");
            flds.Add("warBase", "string");
            flds.Add("shock_wave_01", "double");
            flds.Add("shock_wave_02", "double");
            flds.Add("shock_wave_03", "double");
            flds.Add("nuclear_radiation_01", "double");
            flds.Add("nuclear_radiation_02", "double");
            flds.Add("nuclear_radiation_03", "double");
            flds.Add("thermal_radiation_01", "double");
            flds.Add("thermal_radiation_02", "double");
            flds.Add("thermal_radiation_03", "double");
            flds.Add("nuclear_pulse_01", "double");
            flds.Add("nuclear_pulse_02", "double");
            flds.Add("nuclear_pulse_03", "double");
            flds.Add("fallout_01", "double");
            flds.Add("fallout_02", "double");
            flds.Add("fallout_03", "double");
            flds.Add("lon", "double");
            flds.Add("lat", "double");
            flds.Add("alt", "double");
            flds.Add("launchUnit", "string");
            flds.Add("platform", "string");
            flds.Add("warZone", "string");
            flds.Add("combatZone", "string");
            flds.Add("platoon", "string");
            //flds.Add("missileNo", "string");
            //flds.Add("missileNum", "double");
            //flds.Add("prepareTime", "double");
            //flds.Add("targetBindingTime", "double");
            //flds.Add("defenseBindingTime", "double");
            //flds.Add("fireRange", "double");

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = flds.ToList()
            }); ;
        }
    }
}
