using KTAPIApplication.bo;
using KTAPIApplication.Services;
using KTAPIApplication.vo;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KTAPIApplication.services
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
                List<BsonDocument> mocks = _mongoService.QueryMock(id);

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
                        AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "人员");

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
            List<BsonDocument> mocks = _mongoService.QueryMockAll();

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
                    AddTargetVOs(bs, brigade, ref infos, ref targetVOs, "人员");

                    if (targetVOs.Count > 0)
                        brigadeVOs.Add(new BrigadeVO("ID", brigade, targetVOs));
                }
                baseVOs.Add(new BaseVO("id", bs, brigadeVOs));
            }
            return baseVOs;
        }

        public int TargetEffects(List<BsonDocument> mocks,
                                 List<InfoBO> infos,
                                 Dictionary<string, ConfigBO> configs)
        {
            foreach (var mock in mocks)
            {

                string nuclearExplosionID = mock.GetValue("NuclearExplosionID").AsString;

                try
                {
                    DateTime OccurTime = mock.GetValue("OccurTime").ToUniversalTime();
                    OccurTime = OccurTime.AddHours(8);
                }
                catch (Exception)
                {

                }

                double lon = mock.GetValue("Lon").AsDouble;
                double lat = mock.GetValue("Lat").AsDouble;
                double alt = mock.GetValue("Alt").AsDouble;
                double yield = mock.GetValue("Yield").AsDouble;

                foreach (var info in infos)
                {
                    //string brigade  = info.GetValue("brigade").AsString;
                    //string bs = info.GetValue("base").AsString;
                    //double shock_wave = info.GetValue("shock_wave").ToDouble();
                    //double nuclear_radiation = info.GetValue("nuclear_radiation").ToDouble();
                    //double thermal_radiation = info.GetValue("thermal_radiation").ToDouble();
                    //double nuclear_pulse = info.GetValue("nuclear_pulse").ToDouble();
                    //string classification = info.GetValue("classification").AsString;
                    //double x = info.GetValue("x").ToDouble();
                    //double y = info.GetValue("y").ToDouble();
                    //double z = info.GetValue("z").ToDouble();

                    //求爆点和8种Target的距离
                    double dis = MyCore.Utils.Translate.GetDistance(lat, lon, info.lat, info.lon);

                    //先把5种损伤计算的值求出来
                    double psi = info.shock_wave_01;
                    double cal = info.thermal_radiation_01;
                    double rem = info.nuclear_radiation_01;
                    double vm = info.nuclear_pulse_01 ;
                    double rads = info.fallout_01;

                    if (info.platform.Equals("营区"))
                    {
                        // 对《营区》有影响的是 [冲击波 & 光辐射] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01,info.shock_wave_02,info.shock_wave_03);
                        var result2 = ThermalRadiation(dis, yield/1000, alt * 3.2808399, info.thermal_radiation_01,info.thermal_radiation_02,info.thermal_radiation_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("中心库"))
                    {
                        // 对《中心库》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearPulse(dis / 1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("待机库"))
                    {
                        // 对《待机库》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearPulse(dis / 1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射井"))
                    {
                        // 对《井》有影响的是[冲击波]
                        var result = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射场"))
                    {
                        // 对《发射场》有影响的是[ 冲击波 & 核辐射 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearRadiation(dis, yield/1000, alt * 3.2808399, info.nuclear_radiation_01,info.nuclear_radiation_02,info.nuclear_radiation_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("通信站"))
                    {
                        // 对《通信站》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearPulse(dis / 1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射车"))
                    {
                        // 对《车》有影响的是[ 冲击波 & 光辐射 & 核辐射 & 核电磁脉冲 ] ，取4种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = ThermalRadiation(dis, yield/1000, alt * 3.2808399, info.thermal_radiation_01, info.thermal_radiation_02, info.thermal_radiation_03);
                        var result3 = NuclearRadiation(dis, yield/1000, alt * 3.2808399, info.nuclear_radiation_01, info.nuclear_radiation_02, info.nuclear_radiation_03);
                        var result4 = NuclearPulse(dis/1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);

                        var result12 = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        var result34 = (MyCore.enums.DamageEnumeration)Math.Max(result3.GetHashCode(), result4.GetHashCode());

                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result12.GetHashCode(), result34.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);

                    }
                    else if (info.platform.Equals("人员"))
                    {
                        //[ 冲击波 | 光辐射 | 核辐射 ]
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = ThermalRadiation(dis, yield/1000, alt * 3.2808399, info.thermal_radiation_01, info.thermal_radiation_02, info.thermal_radiation_03);
                        var result3 = NuclearRadiation(dis, yield/1000, alt * 3.2808399, info.nuclear_radiation_01, info.nuclear_radiation_02, info.nuclear_radiation_03);

                        var result12 = (MyCore.enums.DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());

                        var result = (MyCore.enums.DamageEnumeration)Math.Max(result12.GetHashCode(), result3.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                }
            }
            //throw new NotImplementedException();
            return 0;
        }

        private MyCore.enums.DamageEnumeration Airblast(double dis, double yield, double ft, double psi01, double psi02, double psi03)
        {
            // 冲击波。

            double r1 = _analyse.CalcShockWaveRadius(yield,ft, psi01);
            double r2 = _analyse.CalcShockWaveRadius(yield,ft, psi02);
            double r3 = _analyse.CalcShockWaveRadius(yield,ft, psi03);

            if (dis <= r3) return MyCore.enums.DamageEnumeration.Destroy;
            if (dis <= r2) return MyCore.enums.DamageEnumeration.Heavy;
            if (dis <= r1) return MyCore.enums.DamageEnumeration.Light;

            return MyCore.enums.DamageEnumeration.Safe;
        }
        private MyCore.enums.DamageEnumeration NuclearRadiation(double dis, double yield, double altitude, double rem01, double rem02, double rem03)
        {
            // 核辐射

           
            double r1 = _analyse.CalcNuclearRadiationRadius(yield, altitude, MyCore.Utils.Helpers.Convert.ToRem(1));
            double r2 = _analyse.CalcNuclearRadiationRadius(yield, altitude, MyCore.Utils.Helpers.Convert.ToRem(2));
            double r3 = _analyse.CalcNuclearRadiationRadius(yield, altitude, MyCore.Utils.Helpers.Convert.ToRem(3));

            if (dis <= r3) return MyCore.enums.DamageEnumeration.Destroy;
            if (dis <= r2) return MyCore.enums.DamageEnumeration.Heavy;
            if (dis <= r1) return MyCore.enums.DamageEnumeration.Light;

            return MyCore.enums.DamageEnumeration.Safe;
        }
        private MyCore.enums.DamageEnumeration ThermalRadiation(double dis, double yield, double ft, double cal01, double cal02, double cal03)
        {
            // 光辐射

            double r1 = _analyse.GetThermalRadiationR(yield, ft,cal01);
            double r2 = _analyse.GetThermalRadiationR(yield, ft,cal02);
            double r3 = _analyse.GetThermalRadiationR(yield, ft,cal03);

            if (dis <= r3) return MyCore.enums.DamageEnumeration.Destroy;
            if (dis <= r2) return MyCore.enums.DamageEnumeration.Heavy;
            if (dis <= r1) return MyCore.enums.DamageEnumeration.Light;

            return MyCore.enums.DamageEnumeration.Safe;
        }
        private MyCore.enums.DamageEnumeration NuclearPulse(double dis, double yield, double km, double vm01, double vm02, double vm03)
        {
            // 核电磁脉冲

            double r1 = _analyse.CalcNuclearPulseRadius(yield, km, vm01);
            double r2 = _analyse.CalcNuclearPulseRadius(yield, km, vm02);
            double r3 = _analyse.CalcNuclearPulseRadius(yield, km, vm03);

            if (dis <= r3) return MyCore.enums.DamageEnumeration.Destroy;
            if (dis <= r2) return MyCore.enums.DamageEnumeration.Heavy;
            if (dis <= r1) return MyCore.enums.DamageEnumeration.Light;

            return MyCore.enums.DamageEnumeration.Safe;
        }
        /// <summary>
        /// 返回核沉降
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt_ft"></param>
        /// <param name="equivalent_kt"></param>
        /// <param name="windSpeed_mph"></param>
        /// <param name="angle"></param>
        /// <param name="rads01"></param>
        /// <param name="rads02"></param>
        /// <param name="rads03"></param>
        /// <returns></returns>
        public MyCore.enums.DamageEnumeration Fallout(double lon,double lat,double alt_ft,double equivalent_kt,double windSpeed_mph, double angle,double rads01, double rads02, double rads03)
        {
            // 放射性核沉降 =》人员
            //List<Coor> g1 =_analyse.CalcRadioactiveFalloutRegion(lon, lat, alt_ft, equivalent_kt, windSpeed_mph, angle,DamageEnumeration.Light);
            //List<Coor> g2 = _analyse.CalcRadioactiveFalloutRegion(lon, lat, alt_ft, equivalent_kt, windSpeed_mph, angle, DamageEnumeration.Heavy);
            //List<Coor> g3 = _analyse.CalcRadioactiveFalloutRegion(lon, lat, alt_ft, equivalent_kt, windSpeed_mph, angle, DamageEnumeration.Destroy);
            return MyCore.enums.DamageEnumeration.Safe;
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
