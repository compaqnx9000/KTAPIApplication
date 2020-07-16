using KTAPIApplication.bo;
using KTAPIApplication.vo;
using MongoDB.Bson;
using System.Collections.Generic;

namespace KTAPIApplication.services
{
    public interface IDamageAnalysisService
    {
        int TargetEffects(List<BsonDocument> mocks,List<InfoBO> infos, Dictionary<string, ConfigBO> configs);

        MyCore.enums.DamageEnumeration Fallout(double lon, double lat, double alt_ft, double equivalent_kt, double windSpeed_mph, double angle, double rads01, double rads02, double rads03);

        List<BaseVO> Query();

        List<BaseVO> Single(string data);
    }
}
