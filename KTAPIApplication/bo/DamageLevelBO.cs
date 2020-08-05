using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    public class DamageLevelBO
    {
        public ObjectId _id { get; set; }
        public double min { get; set; }
        public double max { get; set; } 
        public int counter { get; set; }  
        public string summary { get; set; }
        public string description { get; set; }
    }
}
