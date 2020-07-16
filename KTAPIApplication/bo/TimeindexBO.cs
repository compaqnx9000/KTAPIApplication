using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    public class TimeindexBO
    {
        public ObjectId _id { get; set; }
        public string brigade { get; set; }
        public string  platform { get; set; }
        public string missileNo { get; set; }
        public double prepareTime { get; set; }
        public double targetBindingTime { get; set; }
        public double defenseBindingTime { get; set; }
    }
}
