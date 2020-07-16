using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.bo
{
    /// <summary>
    /// hb-overlay表的增删改查
    /// </summary>
    public class OverlayBO
    {
        public ObjectId _id { get; set; }
        public string  addend { get; set; }
        public string augend { get; set; }
        public string result { get; set; }
    }
}
