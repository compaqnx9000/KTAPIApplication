using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KTAPIApplication.bo;
using KTAPIApplication.services;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace KTAPIApplication.Controllers
{
    public class Missile
    {
        public Missile(long time, double yield, double lon, double lat, double alt)
        {
            Time = time;
            Yield = yield;
            Lon = lon;
            Lat = lat;
            Alt = alt;
        }

        public long Time { get; set; }
        public double Yield { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Alt { get; set; }
    }

    public class Statistic
    {
        public Statistic(string id, string abilityName, int total, int safeNumber, int mildNumber, int moderateNumber, int severeNumber)
        {
            this.id = id;
            this.abilityName = abilityName;
            this.total = total;
            this.safeNumber = safeNumber;
            this.mildNumber = mildNumber;
            this.moderateNumber = moderateNumber;
            this.severeNumber = severeNumber;
        }

        [JsonIgnore]
        public string id { get; set; }
        [JsonProperty("name")]
        public string abilityName { get; set; }
        public int total { get; set; }
        public int safeNumber { get; set; }
        public int mildNumber { get; set; }
        public int moderateNumber { get; set; }
        public int severeNumber { get; set; }
    }

    public class Report
    {
        public Report()
        {
            Missiles = new List<Missile>();
            DamageList = new List<Statistic>();
        }

        public long AttackTime { get; set; }
        public List<Missile> Missiles { get; set; }
        public List<Statistic> DamageList { get; set; }
    }

    [EnableCors("AllowSameDomain")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        private readonly IDamageAnalysisService _analysisService;

        public ReportController(IMongoService mongoService, IDamageAnalysisService analysisService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));

            _analysisService = analysisService ??
                throw new ArgumentNullException(nameof(analysisService));
        }

        /// <summary>
        /// 根据旅名返回生成报告所需数据。
        /// </summary>
        /// <param name="brigade">旅名。</param>
        /// <returns></returns>
        [HttpPost("report")]
        public IActionResult Report([FromBody]string brigade)
        {

            /// (1) 读取全部hb-info表的信息
            List<InfoBO> infos = _mongoService.QueryInfoByBrigade(brigade);

            // (2) 读取全部hb-hbmock表的信息
            List<BsonDocument> mocks = _mongoService.QueryMockAll();

            // (3) 读取全部hb-config表的信息
            Dictionary<string, ConfigBO> configs = _mongoService.QueryConfigAll();

            // (4) 根据hbmock中的Lon、Lat、Alt、Yield计算出记录在hb-info表中的各种target的损伤程度
            _analysisService.TargetEffects(mocks, infos, configs);

            Report report = new Report();
            report.AttackTime = Utils.Util.GetTimeStamp();

            foreach (var mock in mocks)
            {
                try
                {
                    DateTime occurTime = mock.GetValue("OccurTime").ToUniversalTime();
                    occurTime = occurTime.AddHours(8);
                    double lon = mock.GetValue("Lon").AsDouble;
                    double lat = mock.GetValue("Lat").AsDouble;
                    double alt = mock.GetValue("Alt").AsDouble;
                    double yield = mock.GetValue("Yield").AsDouble;
                    report.Missiles.Add(new Missile(Utils.Util.GetTimeStamp(occurTime), yield, lon, lat, alt));
                }
                catch (Exception)
                {

                }
            }

            List<Statistic> targetVOs = new List<Statistic>();

            AddTargetVOs(brigade, ref infos, ref targetVOs, "发射井");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "发射车");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "发射场");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "营区");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "中心库");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "待机库");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "通信站");
            AddTargetVOs(brigade, ref infos, ref targetVOs, "人员");

            report.DamageList = targetVOs;


            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = report
            });

        }
        private void AddTargetVOs(string brigade, ref List<InfoBO> infos, ref List<Statistic> targetVOs, string clsName)
        {
            var docs_01 = (from info in infos
                           where info.brigade == brigade && info.platform == clsName
                           select info).ToList();

            int level00 = 0; int level01 = 0; int level02 = 0; int level03 = 0;

            if (docs_01.Count > 0)
            {
                foreach (var info in docs_01)
                {
                    int val = Convert.ToInt32(JudgeLevel(info.nuclear_warheads));
                    if (val == 0) level00++;
                    if (val == 1) level01++;
                    if (val == 2) level02++;
                    if (val == 3) level03++;
                }
                targetVOs.Add(new Statistic("id", clsName,
                    docs_01.Count, level00, level01, level02, level03));
            }
        }

        private double JudgeLevel(List<string> damages)
        {
            double level = 0;
            foreach (string damage in damages)
            {
                string s = damage.Split(",")[1];
                if (s.Equals("Light"))
                {
                    level = level == 0 ? 1 : level + 1;
                }
                if (s.Equals("Heavy"))
                {
                    level = level == 0 ? 2 : level + 2;
                }
                if (s.Equals("Destroy"))
                {
                    level = 3;
                }
            }

            if (level > 3) level = 3;
            return level;
        }
    }


}
