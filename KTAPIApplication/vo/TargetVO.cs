using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.vo
{
    public class TargetVO
    {
        public TargetVO(string id, string abilityName, int total, int safeNumber, int mildNumber, int moderateNumber, int severeNumber)
        {
            this.id = id;
            this.abilityName = abilityName;
            this.total = total;
            this.safeNumber = safeNumber;
            this.mildNumber = mildNumber;
            this.moderateNumber = moderateNumber;
            this.severeNumber = severeNumber;
        }

        public string id { get; set; }
        public string abilityName { get; set; }
        public int total { get; set; }
        public int safeNumber { get; set; }
        public int mildNumber { get; set; }
        public int moderateNumber { get; set; }
        public int severeNumber { get; set; }
    }
}
