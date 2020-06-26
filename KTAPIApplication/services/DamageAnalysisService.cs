using KTAPIApplication.bo;
using KTAPIApplication.core;
using KTAPIApplication.enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace KTAPIApplication.services
{
    public class DamageAnalysisService : IDamageAnalysisService
    {
        MyAnalyse _analyse = new MyAnalyse();
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
                    double dis = Utils.Translate.GetDistance(lat, lon, info.lat, info.lon);

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
                        var result = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("中心库"))
                    {
                        // 对《中心库》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearPulse(dis / 1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("待机库"))
                    {
                        // 对《待机库》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearPulse(dis / 1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
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
                        var result = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("通信站"))
                    {
                        // 对《通信站》有影响的是[ 冲击波 & 核电磁脉冲 ] ，取2种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = NuclearPulse(dis / 1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);
                        var result = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                    else if (info.platform.Equals("发射车"))
                    {
                        // 对《车》有影响的是[ 冲击波 & 光辐射 & 核辐射 & 核电磁脉冲 ] ，取4种损伤最大的
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = ThermalRadiation(dis, yield/1000, alt * 3.2808399, info.thermal_radiation_01, info.thermal_radiation_02, info.thermal_radiation_03);
                        var result3 = NuclearRadiation(dis, yield/1000, alt * 3.2808399, info.nuclear_radiation_01, info.nuclear_radiation_02, info.nuclear_radiation_03);
                        var result4 = NuclearPulse(dis/1000, yield, alt/1000, info.nuclear_pulse_01, info.nuclear_pulse_02, info.nuclear_pulse_03);

                        var result12 = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());
                        var result34 = (DamageEnumeration)Math.Max(result3.GetHashCode(), result4.GetHashCode());

                        var result = (DamageEnumeration)Math.Max(result12.GetHashCode(), result34.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);

                    }
                    else if (info.platform.Equals("人员"))
                    {
                        //[ 冲击波 | 光辐射 | 核辐射 ]
                        var result1 = Airblast(dis, yield/1000, alt * 3.2808399, info.shock_wave_01, info.shock_wave_02, info.shock_wave_03);
                        var result2 = ThermalRadiation(dis, yield/1000, alt * 3.2808399, info.thermal_radiation_01, info.thermal_radiation_02, info.thermal_radiation_03);
                        var result3 = NuclearRadiation(dis, yield/1000, alt * 3.2808399, info.nuclear_radiation_01, info.nuclear_radiation_02, info.nuclear_radiation_03);

                        var result12 = (DamageEnumeration)Math.Max(result1.GetHashCode(), result2.GetHashCode());

                        var result = (DamageEnumeration)Math.Max(result12.GetHashCode(), result3.GetHashCode());
                        info.nuclear_warheads.Add(nuclearExplosionID + "," + result);
                    }
                }
            }
            //throw new NotImplementedException();
            return 0;
        }

        private DamageEnumeration Airblast(double dis, double yield, double ft, double psi01, double psi02, double psi03)
        {
            // 冲击波。

            double r1 = _analyse.CalcShockWaveRadius(yield,ft, psi01);
            double r2 = _analyse.CalcShockWaveRadius(yield,ft, psi02);
            double r3 = _analyse.CalcShockWaveRadius(yield,ft, psi03);

            if (dis <= r3) return DamageEnumeration.Destroy;
            if (dis <= r2) return DamageEnumeration.Heavy;
            if (dis <= r1) return DamageEnumeration.Light;

            return DamageEnumeration.Safe;
        }
        private DamageEnumeration NuclearRadiation(double dis, double yield, double altitude, double rem01, double rem02, double rem03)
        {
            // 核辐射

           
            double r1 = _analyse.CalcNuclearRadiationRadius(yield, altitude, Utils.Helpers.Convert.ToRem(1));
            double r2 = _analyse.CalcNuclearRadiationRadius(yield, altitude, Utils.Helpers.Convert.ToRem(2));
            double r3 = _analyse.CalcNuclearRadiationRadius(yield, altitude, Utils.Helpers.Convert.ToRem(3));

            if (dis <= r3) return DamageEnumeration.Destroy;
            if (dis <= r2) return DamageEnumeration.Heavy;
            if (dis <= r1) return DamageEnumeration.Light;

            return DamageEnumeration.Safe;
        }
        private DamageEnumeration ThermalRadiation(double dis, double yield, double ft, double cal01, double cal02, double cal03)
        {
            // 光辐射

            double r1 = _analyse.GetThermalRadiationR(yield, ft,cal01);
            double r2 = _analyse.GetThermalRadiationR(yield, ft,cal02);
            double r3 = _analyse.GetThermalRadiationR(yield, ft,cal03);

            if (dis <= r3) return DamageEnumeration.Destroy;
            if (dis <= r2) return DamageEnumeration.Heavy;
            if (dis <= r1) return DamageEnumeration.Light;

            return DamageEnumeration.Safe;
        }
        private DamageEnumeration NuclearPulse(double dis, double yield, double km, double vm01, double vm02, double vm03)
        {
            // 核电磁脉冲

            double r1 = _analyse.CalcNuclearPulseRadius(yield, km, vm01);
            double r2 = _analyse.CalcNuclearPulseRadius(yield, km, vm02);
            double r3 = _analyse.CalcNuclearPulseRadius(yield, km, vm03);

            if (dis <= r3) return DamageEnumeration.Destroy;
            if (dis <= r2) return DamageEnumeration.Heavy;
            if (dis <= r1) return DamageEnumeration.Light;

            return DamageEnumeration.Safe;
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
        public DamageEnumeration Fallout(double lon,double lat,double alt_ft,double equivalent_kt,double windSpeed_mph, double angle,double rads01, double rads02, double rads03)
        {
            // 放射性核沉降 =》人员
            List<Coor> g1 =_analyse.CalcRadioactiveFalloutRegion(lon, lat, alt_ft, equivalent_kt, windSpeed_mph, angle,DamageEnumeration.Light);
            List<Coor> g2 = _analyse.CalcRadioactiveFalloutRegion(lon, lat, alt_ft, equivalent_kt, windSpeed_mph, angle, DamageEnumeration.Heavy);
            List<Coor> g3 = _analyse.CalcRadioactiveFalloutRegion(lon, lat, alt_ft, equivalent_kt, windSpeed_mph, angle, DamageEnumeration.Destroy);
            return DamageEnumeration.Safe;
        }


    }
}
