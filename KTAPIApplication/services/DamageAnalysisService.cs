using KTAPIApplication.bo;
using KTAPIApplication.Services;
using KTAPIApplication.vo;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KTAPIApplication.Services
{
    public class DamageAnalysisService : IDamageAnalysisService
    {
        private readonly IMongoService _mongoService;

        MyCore.MyAnalyse _analyse = new MyCore.MyAnalyse();

        public DamageAnalysisService(IMongoService mongoService)
        {
            _mongoService = mongoService ??
               throw new ArgumentNullException(nameof(mongoService));
        }

        public List<BaseVO> Single(string data)
        {
            // 传入的data：     http://localhost:5000/single?data={"ids":["test001"]}

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
                List<MockBO> mocks = _mongoService.QueryMock(id);

                // (3) 读取全部hb-config表的信息
                Dictionary<string, ConfigBO> configs = _mongoService.QueryConfigAll();

                // (4) 根据hbmock中的Lon、Lat、Alt、Yield计算出记录在hb-info表中的各种target的损伤程度
                TargetEffects(mocks, infos, configs);

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


                List<BaseVO> baseVOs = new List<BaseVO>();

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

                        if (targetVOs.Count > 0)
                            brigadeVOs.Add(new BrigadeVO("ID", brigade, targetVOs));
                    }
                    baseVOs.Add(new BaseVO("id", bs, brigadeVOs));
                }

                return baseVOs;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public List<BaseVO> Query()
        {
            // (1) 读取全部hb-info表的信息
            List<InfoBO> infos = _mongoService.QueryInfoAll();

            // (2) 读取全部hb-hbmock表的信息
            List<MockBO> mocks = _mongoService.QueryMockAll();

            // (3) 读取全部hb-config表的信息
            Dictionary<string, ConfigBO> configs = _mongoService.QueryConfigAll();

            // (4) 根据hbmock中的Lon、Lat、Alt、Yield计算出记录在hb-info表中的各种target的损伤程度
            TargetEffects(mocks, infos, configs);

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


            List<BaseVO> baseVOs = new List<BaseVO>();

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

                    if (targetVOs.Count > 0)
                        brigadeVOs.Add(new BrigadeVO("ID", brigade, targetVOs));
                }
                baseVOs.Add(new BaseVO("id", bs, brigadeVOs));
            }
            return baseVOs;
        }

        public int TargetEffects(List<MockBO> mocks,
                                 List<InfoBO> infos,
                                 Dictionary<string, ConfigBO> configs)
        {
            foreach (var mock in mocks)
            {
                foreach (var info in infos)
                {
                    //求爆点和8种Target的距离
                    double dis = MyCore.Utils.Translate.GetDistance(mock.Lat, mock.Lon, info.lat, info.lon);

                    if (info.platform.Equals("营区"))
                    {
                        // 对《营区》有影响的是 [冲击波 & 光辐射] ，取2种损伤最大的
                        var result1 = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01,info.shock_wave_02,info.shock_wave_03);
                        var result2 = MyCore.NuclearAlgorithm.ThermalRadiation(dis, mock.Yield, mock.Alt, info.thermal_radiation_01,info.thermal_radiation_02,info.thermal_radiation_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("中心库"))
                    {
                        // 对《中心库》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = MyCore.NuclearAlgorithm.NuclearPulse(dis, mock.Yield, mock.Alt, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("待机库"))
                    {
                        // 对《待机库》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = MyCore.NuclearAlgorithm.NuclearPulse(dis, mock.Yield, mock.Alt, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射井"))
                    {
                        // 对《井》有影响的是[冲击波]
                        var result = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射场"))
                    {
                        // 对《发射场》有影响的是[ 冲击波 & 核辐射 ] ，取2种损伤最大的
                        var result1 = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = MyCore.NuclearAlgorithm.NuclearRadiation(dis, mock.Yield, mock.Alt, info.nuclear_radiation_01,info.nuclear_radiation_02,info.nuclear_radiation_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("通信站"))
                    {
                        // 对《通信站》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = MyCore.NuclearAlgorithm.NuclearPulse(dis , mock.Yield, mock.Alt, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射车"))
                    {
                        // 对《车》有影响的是[ 冲击波 & 光辐射 & 核辐射 & 核电磁脉冲 ] ，取4种损伤最大的
                        var result1 = MyCore.NuclearAlgorithm.Airblast(dis, mock.Yield, mock.Alt, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = MyCore.NuclearAlgorithm.ThermalRadiation(dis, mock.Yield, mock.Alt, info.thermal_radiation_01, info.thermal_radiation_02, info.thermal_radiation_03);
                        var result3 = MyCore.NuclearAlgorithm.NuclearRadiation(dis, mock.Yield, mock.Alt, info.nuclear_radiation_01, info.nuclear_radiation_02, info.nuclear_radiation_03);
                        var result4 = MyCore.NuclearAlgorithm.NuclearPulse(dis, mock.Yield, mock.Alt, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);

                        var result12 = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        var result34 = (MyCore.enums.DamageEnumeration)Math.Max(result3.GetHashCode(), result4.GetHashCode());

                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result12.GetHashCode(), result34.GetHashCode());
                        info.nuclear_warheads.Add(mock.NuclearExplosionID + "," + result);
                    }
                }
            }
            //throw new NotImplementedException();
            return 0;
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
