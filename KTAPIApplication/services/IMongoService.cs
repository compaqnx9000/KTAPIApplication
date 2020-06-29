using KTAPIApplication.bo;
using KTAPIApplication.Controllers;
using KTAPIApplication.vo;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Services
{
    public interface IMongoService
    {
        List<Base> Query();

        VueVO QueryByBrigade(string brigade);

        List<BsonDocument> QueryMockAll();
        List<BsonDocument> QueryMock(string nuclearExplosionID);

        /* info表操作 */
        List<InfoBO> QueryInfoAll();
        List<InfoBO> QueryInfoByBrigade(string brigade);


        /* config表操作 */
        Dictionary<string, ConfigBO> QueryConfigAll();
        List<ConfigBO> GetConfigs();
        ConfigBO GetConfig(string id);
        bool UpdateConfig(string id,ConfigBO config);
        bool DeleteConfig(string id);
        string AddConfig(ConfigBO config);

        /* description表操作 */
        List<DescriptionBO> GetDescriptions();
        DescriptionBO GetDescription(string id);
        string AddDescription(DescriptionBO bo);
        bool UpdateDescription(string id, DescriptionBO bo);
        bool DeleteDescription(string id);

        /* info表操作 */
        List<InfoBO> GetInfos();
        InfoBO GetInfo(string id);
        string AddInfo(InfoBO bo);
        bool UpdateInfo(string id, InfoBO bo);
        bool DeleteInfo(string id);

    }
}
