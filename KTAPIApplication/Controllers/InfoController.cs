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


        // add 0715
        [HttpGet("taggroup")]
        public ActionResult Taggroup()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _mongoService.Taggroup()
            });
        }

        [HttpGet("fields")]
        public ActionResult Fields()
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

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = flds.ToList()
            });;
        }

        [HttpGet("fieldsExclude")]
        public ActionResult FieldsExclude()
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
