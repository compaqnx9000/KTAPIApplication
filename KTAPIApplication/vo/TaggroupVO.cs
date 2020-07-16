using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.vo
{
    public class TaggroupVO
    {
        public TaggroupVO(string tagGroupName, string[] tagsInGroup)
        {
            TagGroupName = tagGroupName;
            this.tagsInGroup = tagsInGroup;
        }

        public string TagGroupName { get; set; }
        public string[] tagsInGroup { get; set; }
    }
}
