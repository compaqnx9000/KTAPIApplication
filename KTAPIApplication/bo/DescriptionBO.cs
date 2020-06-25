using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    public class DescriptionBO
    {
        public ObjectId _id { get; set; }
        public string name { get; set; }
        public string level { get; set; }
        public string detail { get; set; }
    }
}
