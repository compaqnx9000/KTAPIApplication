﻿using MongoDB.Bson;
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
        public string level_01 { get; set; }
        public string level_02 { get; set; }
        public string level_03 { get; set; }

    }
}
