using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.vo
{
    public class BaseVO
    {
        public BaseVO(string id, string baseName, List<BrigadeVO> brigadeList)
        {
            this.id = id;
            this.baseName = baseName;
            this.brigadeList = brigadeList;
        }

        public string id { get; set; }
        public string baseName { get; set; }
        public List<BrigadeVO> brigadeList { get; set; }
    }
}
