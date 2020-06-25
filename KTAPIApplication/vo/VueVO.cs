using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.vo
{
    public class VueVO
    {
        public VueVO(double info_Lon, double info_Lat, double mock_Lon, 
            double mock_Lat, double mock_Alt, double mock_Yield, string mock_Date)
        {
            Info_Lon = info_Lon;
            Info_Lat = info_Lat;
            Mock_Lon = mock_Lon;
            Mock_Lat = mock_Lat;
            Mock_Alt = mock_Alt;
            Mock_Yield = mock_Yield;
            Mock_Date = mock_Date;
        }

        public double Info_Lon { get; set; }
        public double Info_Lat { get; set; }
        public double Mock_Lon { get; set; }
        public double Mock_Lat { get; set; }
        public double Mock_Alt { get; set; }
        public double Mock_Yield { get; set; }
        public string Mock_Date { get; set; }



        // return info：经度、纬度，
        //mock：时间，经、纬、高、当量
    }
}
