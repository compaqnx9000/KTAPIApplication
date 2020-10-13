using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Services
{
    /// <summary>
    /// 与pdf相关
    /// </summary>
    public interface IPDFService
    {
        public string MakeHtml(string warBase, string brigade);

    }
}
