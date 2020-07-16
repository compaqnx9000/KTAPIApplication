using KTAPIApplication.bo;
using KTAPIApplication.Controllers;
using KTAPIApplication.services;
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
        List<BaseVO> Query();

        VueVO QueryByBrigade(string brigade);

        List<BsonDocument> QueryMockAll();
        List<BsonDocument> QueryMock(string nuclearExplosionID);

        /* info表操作 */
        List<InfoBO> QueryInfoAll();
        List<InfoBO> QueryInfoByBrigade(string brigade);

        List<TaggroupVO> Taggroup();


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

        /* overlay表操作 */
        List<OverlayBO> GetOverlays();
        OverlayBO GetOverlay(string id);
        string AddOverlay(OverlayBO bo);
        bool UpdateOverlay(string id, OverlayBO bo);
        bool DeleteOverlay(string id);

        /* rule表操作 */
        List<RuleBo> GetRules();
        RuleBo GetRule(string id);
        string AddRule(RuleBo bo);
        bool UpdateRule(string id, RuleBo bo);
        bool DeleteRule(string id);

        /* timeindex表操作 */
        List<TimeindexBO> GetTimeindexs();
        TimeindexBO GetTimeindex(string id);
        string AddTimeindex(TimeindexBO bo);
        bool UpdateTimeindex(string id, TimeindexBO bo);
        bool DeleteTimeindex(string id);
        TimeindexBO QueryTimeindex(string platform,string missileNo);


    }
}
