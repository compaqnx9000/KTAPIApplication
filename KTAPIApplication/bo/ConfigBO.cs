using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    /// <summary>
    /// 8种类型对5种损伤的抵抗默认值。
    /// </summary>
    [Serializable]
    public class ConfigBO
    {
        public ObjectId _id { get; set; }
        public string platform { get; set; }
        public double shock_wave_01 { get; set; }
        public double shock_wave_02 { get; set; }
        public double shock_wave_03 { get; set; }
        public double nuclear_radiation_01 { get; set; }
        public double nuclear_radiation_02 { get; set; }
        public double nuclear_radiation_03 { get; set; }
        public double thermal_radiation_01 { get; set; }
        public double thermal_radiation_02 { get; set; }
        public double thermal_radiation_03 { get; set; }
        public double nuclear_pulse_01 { get; set; }
        public double nuclear_pulse_02 { get; set; }
        public double nuclear_pulse_03 { get; set; }
        public double fallout_01 { get; set; }
        public double fallout_02 { get; set; }
        public double fallout_03 { get; set; }
    }
}
