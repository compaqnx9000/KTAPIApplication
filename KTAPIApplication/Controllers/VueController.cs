using KTAPIApplication.bo;
using KTAPIApplication.services;
using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    public class Base
    {
        public Base(string id, string baseName, List<BrigadeVO> brigadeList)
        {
            this.id = id;
            this.baseName = baseName;
            this.brigadeList = brigadeList;
        }

        public string id { get; set; }
        public string baseName { get; set; }
        public List<BrigadeVO> brigadeList { get; set; }
    }
    public class BrigadeVO
    {
        public BrigadeVO(string id, string name, List<TargetVO> children)
        {
            this.id = id;
            this.name = name;
            this.children = children;
        }

        public string id { get; set; }
        public string name { get; set; }
        public List<TargetVO> children { get; set; }
    }
    public class TargetVO
    {
        public TargetVO(string id, string abilityName, int total, int safeNumber, int mildNumber, int moderateNumber, int severeNumber)
        {
            this.id = id;
            this.abilityName = abilityName;
            this.total = total;
            this.safeNumber = safeNumber;
            this.mildNumber = mildNumber;
            this.moderateNumber = moderateNumber;
            this.severeNumber = severeNumber;
        }

        public string id { get; set; }
        public string abilityName { get; set; }
        public int total { get; set; }
        public int safeNumber { get; set; }
        public int mildNumber { get; set; }
        public int moderateNumber { get; set; }
        public int severeNumber { get; set; }
    }
    [EnableCors("AllowSameDomain")]
    [ApiController]
    public class VueController : ControllerBase
    {
        private readonly IMongoService _mongoService;
        private readonly IDamageAnalysisService _analysisService;

        public VueController(IMongoService mongoService, IDamageAnalysisService analysisService)
        {
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));

            _analysisService = analysisService ??
                throw new ArgumentNullException(nameof(analysisService));
        }

        [HttpGet("single")]
        public ActionResult Single(string data)
        {
            // 传入的data：     http://localhost:5000/test?data={"ids":["test001"]}

            // (1) 解析出来核弹id，比如"test001","test002"
            // (2) 去HB库里的hbmock找"test001"和"test002"对应的经纬度和当量
            // (3) 计算info库里的井和车的level，计算完返回给前端


            // http://localhost:5000/test?data="{\"lon\":116.123,\"lat\":39.456,\"alt\":2187.62802276,\"ImpactTimeUtc\":1590044321.0}"
            // http://localhost:5000/test?data="{\"ids\":[\"test001\",\"test002\"]}"

            //TestBO testBO = new TestBO();
            //testBO.name = new List<string>();
            //testBO.name.Add("test001");
            //testBO.name.Add("test002");
            //string postBody = Newtonsoft.Json.JsonConvert.SerializeObject(testBO);

            //JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(postBody);
            try
            {
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                string id = jo["ids"].First.ToString();

                // (1) 读取全部hb-info表的信息
                List<InfoBO> infos = _mongoService.QueryInfoAll();

                // (2) 读取全部hb-hbmock表的信息
                List<BsonDocument> mocks = _mongoService.QueryMock(id);

                // (3) 读取全部hb-config表的信息
                Dictionary<string, ConfigBO> configs = _mongoService.QueryConfigAll();

                // (4) 根据hbmock中的Lon、Lat、Alt、Yield计算出记录在hb-info表中的各种target的损伤程度
                _analysisService.TargetEffects(mocks, infos, configs);

                // (5) 封装成前端需要的格式，返回给前端

                Dictionary<string, List<string>> kv = new Dictionary<string, List<string>>();
                //5.1 查询有多少基地
                List<string> bases = new List<string>();
                bases = infos.Select(p => p.warBase).ToList();  //只取base_name字段，重新生成新的List集合
                bases = bases.Distinct().ToList(); //去重复，绑定数据后面要加ToList()

                foreach (string base_name in bases)
                {
                    List<string> brigades = (from info in infos
                                             where info.warBase == base_name
                                             select info.brigade).ToList();
                    brigades = brigades.Distinct().ToList();
                    kv.Add(base_name, brigades);
                }


                List<Base> baseVOs = new List<Base>();

                foreach (string bs in bases)
                {
                    List<string> brigades = kv[bs];
                    List<BrigadeVO> brigadeVOs = new List<BrigadeVO>();
                    foreach (string brigade in brigades)
                    {
                        List<TargetVO> targetVOs = new List<TargetVO>();

                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "发射井");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "发射车");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "发射场");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "营区");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "中心库");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "待机库");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "通信站");
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "人员");

                        if (targetVOs.Count > 0)
                            brigadeVOs.Add(new BrigadeVO("ID", brigade, targetVOs));
                    }
                    baseVOs.Add(new Base("id", bs, brigadeVOs));
                }

                return new JsonResult(new
                {
                    return_status = 0,
                    return_msg = "",
                    return_data = baseVOs
                });
            }
            catch (Exception)
            {

                return new JsonResult(new
                {
                    return_status = 1,
                    return_msg = "数据库查询异常",
                    return_data = ""
                });
            }
            

           
            //return new JsonResult("1");
        }

        [HttpGet("fallout")]
        public ActionResult Fallout()
        {
            double lon = 116.391667;
            double lat = 39.903333;
            double alt_ft = 0;
            double equivalent_kt = 1000;
            double windSpeed_mph = 15;
            double angle = 225;
            double rads01 = 1;
            double rads02 = 20;
            double rads03 = 0;

            _analysisService.Fallout(lon,lat,alt_ft,equivalent_kt,windSpeed_mph,angle,rads01,rads02,rads03);

                 return new JsonResult(new
                 {
                     return_status = 1,
                     return_msg = "数据库查询异常",
                     return_data = ""
                 });
        }

        [HttpGet("query")]
        [HttpPost("query")]
        public ActionResult Query()
        {
            // (1) 读取全部hb-info表的信息
            List<InfoBO> infos = _mongoService.QueryInfoAll();

            // (2) 读取全部hb-hbmock表的信息
            List<BsonDocument> mocks = _mongoService.QueryMockAll();

            // (3) 读取全部hb-config表的信息
            Dictionary<string, ConfigBO> configs = _mongoService.QueryConfigAll();

            // (4) 根据hbmock中的Lon、Lat、Alt、Yield计算出记录在hb-info表中的各种target的损伤程度
            _analysisService.TargetEffects(mocks, infos, configs);

            // (5) 封装成前端需要的格式，返回给前端

            Dictionary<string, List<string>> kv = new Dictionary<string, List<string>>();
            //5.1 查询有多少基地
            List<string> bases = new List<string>();
            bases = infos.Select(p => p.warBase).ToList();  //只取base_name字段，重新生成新的List集合
            bases = bases.Distinct().ToList(); //去重复，绑定数据后面要加ToList()

            foreach (string base_name in bases)
            {
                List<string> brigades = (from info in infos
                                         where info.warBase == base_name
                                         select info.brigade).ToList();
                brigades = brigades.Distinct().ToList();
                kv.Add(base_name, brigades);
            }


            List<Base> baseVOs = new List<Base>();

            foreach (string bs in bases)
            {
                List<string> brigades = kv[bs];
                List<BrigadeVO> brigadeVOs = new List<BrigadeVO>();
                foreach (string brigade in brigades)
                {
                    List<TargetVO> targetVOs = new List<TargetVO>();

                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "发射井");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "发射车");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "发射场");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "营区");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "中心库");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "待机库");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "通信站");
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "人员");

                    if (targetVOs.Count > 0)
                        brigadeVOs.Add(new BrigadeVO("ID", brigade, targetVOs));
                }
                baseVOs.Add(new Base("id", bs, brigadeVOs));
            }

            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = baseVOs
            });
        }



        private void AddTargetVOs(string bs, string brigade, ref List<InfoBO> infos, ref List<TargetVO> targetVOs, string clsName)
        {
            var docs_01 = (from info in infos
                           where info.warBase == bs && info.brigade == brigade && info.platform == clsName
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
                targetVOs.Add(new TargetVO("id", clsName,
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
