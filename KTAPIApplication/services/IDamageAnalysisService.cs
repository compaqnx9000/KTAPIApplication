using KTAPIApplication.bo;
using KTAPIApplication.vo;
using MongoDB.Bson;
using System.Collections.Generic;

namespace KTAPIApplication.Services
{
    public interface IDamageAnalysisService
    {
        int TargetEffects(List<MockBO> mocks,List<InfoBO> infos, Dictionary<string, ConfigBO> configs);

        List<BaseVO> Query();

        List<BaseVO> Single(string data);
    }
}
