using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.vo
{
    public class BrigadeVO
    {
        public BrigadeVO(string id, string name, List<TargetVO> children)
        {
            this.id = id;
            this.name = name;
            this.children = children;
        }

        public string id { get; set; }
        public string name { get; set; }
        public List<TargetVO> children { get; set; }
    }
}
