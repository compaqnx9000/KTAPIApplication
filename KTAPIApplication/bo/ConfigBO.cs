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
    public class ConfigBO
    {
        public ObjectId _id { get; set; }
        public string platform { get; set; }
        public double shock_wave { get; set; }
        public double thermal_radiation { get; set; }
        public double nuclear_radiation { get; set; }
        public double nuclear_pulse { get; set; }
        public double fallout { get; set; }
    }
}
