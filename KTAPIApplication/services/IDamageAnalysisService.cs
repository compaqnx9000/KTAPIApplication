using KTAPIApplication.bo;
using KTAPIApplication.enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.services
{
    public interface IDamageAnalysisService
    {
        int TargetEffects(List<BsonDocument> mocks,List<InfoBO> infos, Dictionary<string, ConfigBO> configs);

        DamageEnumeration Fallout(double lon, double lat, double alt_ft, double equivalent_kt, double windSpeed_mph, double angle, double rads01, double rads02, double rads03);
    }
}
