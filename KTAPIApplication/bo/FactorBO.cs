using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    public class FactorBO
    {
        public ObjectId _id { get; set; }
        public double level_01 { get; set; }
        public double level_02 { get; set; }
        public double level_03 { get; set; }
    }
}
