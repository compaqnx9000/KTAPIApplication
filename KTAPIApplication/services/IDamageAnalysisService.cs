using KTAPIApplication.bo;
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
    }
}
