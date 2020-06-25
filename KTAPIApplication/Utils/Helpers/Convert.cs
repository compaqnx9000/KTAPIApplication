﻿using KTAPIApplication.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Utils.Helpers
{
    public static partial class Convert
    {
        /// <summary>
        /// 根据输入的level，返回psi的值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static double ToPsi(DamageEnumeration level)
        {
            if (level == DamageEnumeration.Light) return 1;
            if (level == DamageEnumeration.Heavy) return 20;
            if (level == DamageEnumeration.Destroy) return 3000;
            return -1;
        }


        public static double ToRem(int level)
        {
            if (level == 1) return 100;
            if (level == 2) return 600;
            if (level == 3) return 5000;
            return -1;
        }

        public static string ToThrem(int level)
        {
            if (level == 1) return "_noharm-100";
            if (level == 2) return "_2nd-50";
            if (level == 3) return "_3rd-100";
            return "";
        }

        public static int ToPluse(int level)
        {
            if (level == 1) return 200;
            if (level == 2) return 2000;
            if (level == 3) return 50000;
            return -1;
        }
    }
}
