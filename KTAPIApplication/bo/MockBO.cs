using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    public class MockBO
    {
        public Object _id { get; set; }
        public string NuclearExplosionID { get; set; }
        public DateTime OccurTime { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Alt { get; set; }
        public double Yield { get; set; }
        public string Producer { get; set; }
        public string DetectionType { get; set; }
        public DateTime ReportTime { get; set; }
        public string Nonce { get; set; }
    }
}
