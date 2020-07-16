using KTAPIApplication.bo;
using KTAPIApplication.Controllers;
using KTAPIApplication.services;
using KTAPIApplication.vo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SystemAPIApplication;

namespace KTAPIApplication.Services
{
    public class MongoService : IMongoService
    {
        private MongoSetting _config;
        private MongoClient _client = null;

        private MongoOtherSetting _otherConfig;
        private MongoClient _otherClient = null;

        private ServiceUrls _urlConfig;

        public MongoService(IOptions<MongoSetting> setting, IOptions<MongoOtherSetting> otherSetting, IOptions<ServiceUrls> urls)
        {
            // 自己的库
            _config = setting.Value;
            string conn = "mongodb://" + _config.IP + ":" + _config.Port;
            _client = new MongoClient(conn);

            // 别人的库
            _otherConfig = otherSetting.Value;
            string otherConn = "mongodb://" + _otherConfig.IP + ":" + _otherConfig.Port;
            _otherClient = new MongoClient(otherConn);

            _urlConfig = urls.Value;
        }

        //{
        //    id: 142555,
        //    baseName: "基地01",
        //    brigadeList: [
        //        {
        //            id: 478,
        //            name: "45旅",
        //            children: [
        //                {
        //                    id: 78,
        //                    abilityName: "车",
        //                    total: "20",
        //                    safeNumber: "1",
        //                    mildNumber: "12",
        //                    moderateNumber: "6",
        //                    severeNumber: "3"
        //                },
        //                {
        //                    id: 4788,
        //                    abilityName: "井",
        //                    total: "20",
        //                    safeNumber: "1",
        //                    mildNumber: "12",
        //                    moderateNumber: "3",
        //                    severeNumber: "6",
        //                }
        //            ]
        //        }
        //    ]
        //}

        private void Classfi(List<BsonDocument> lst, ref int level00, ref int level01, ref int level02, ref int level03)
        {
            foreach (BsonDocument item in lst)
            {
                int value = int.Parse(item.GetValue("health").ToString());
                switch (value)
                {
                    case 0:
                        level00++;
                        break;
                    case 1:
                        level01++;
                        break;
                    case 2:
                        level02++;
                        break;
                    case 3:
                        level03++;
                        break;
                }
            }
        }
        public List<InfoBO> QueryInfoAll()
        {
            List<InfoBO> infos = new List<InfoBO>();
            var collection = _client.GetDatabase(_config.InfoSetting.Database).
              GetCollection<BsonDocument>(_config.InfoSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var info = BsonSerializer.Deserialize<InfoBO>(doc);
                infos.Add(info);
            }
            return infos;
        }
        public List<BsonDocument> QueryMockAll()
        {
            var collection = _otherClient.GetDatabase(_otherConfig.MockSetting.Database).GetCollection<BsonDocument>(_otherConfig.MockSetting.Collection);
            return collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        }

        public List<BsonDocument> QueryMock(string nuclearExplosionID)
        {
            var collection = _otherClient.GetDatabase(_otherConfig.MockSetting.Database).GetCollection<BsonDocument>(_otherConfig.MockSetting.Collection);
            var filter = Builders<BsonDocument>.Filter;
            return collection.Find(filter.Eq("NuclearExplosionID", nuclearExplosionID)).ToList();
        }

        public List<InfoBO> QueryInfoByBrigade(string brigade)
        {
            var collection = _otherClient.GetDatabase(_config.InfoSetting.Database).GetCollection<BsonDocument>(_config.InfoSetting.Collection);
            var filter = Builders<BsonDocument>.Filter;
            //return collection.Find(filter.Eq("brigade", brigade)).ToList();

            var list = collection.Find(filter.Eq("brigade", brigade)).ToList();

            List<InfoBO> infos = new List<InfoBO>();

            foreach (var doc in list)
            {
                var info = BsonSerializer.Deserialize<InfoBO>(doc);
                infos.Add(info);
            }
            return infos;
        }


        public VueVO QueryByBrigade(string brigade)
        {
            var collection = _client.GetDatabase(_config.InfoSetting.Database).
                GetCollection<BsonDocument>(_config.InfoSetting.Collection);



            double info_Lon = 0; double info_Lat = 0; double mock_Lon = 0;
            double mock_Lat = 0; double mock_Alt = 0; double mock_Yield = 0; string mock_Date = "";

            var filter = Builders<BsonDocument>.Filter;
            var docs_01 = collection.Find(filter.Eq("brigade", brigade)).ToList();
            if (docs_01.Count > 0)
            {
                info_Lon = docs_01[0].GetValue("lon").AsDouble;
                info_Lat = docs_01[0].GetValue("lat").AsDouble;

            }

            var collection2 = _client.GetDatabase(_config.MockSetting.Database).
                GetCollection<BsonDocument>(_config.MockSetting.Collection);
            var filter2 = Builders<BsonDocument>.Filter;
            var docs_02 = collection2.Find(filter2.Empty).ToList();
            if (docs_02.Count > 0)
            {
                mock_Lon = docs_02[0].GetValue("Lon").AsDouble;
                mock_Lat = docs_02[0].GetValue("Lat").AsDouble;
                mock_Alt = docs_02[0].GetValue("Alt").AsDouble;
                mock_Yield = docs_02[0].GetValue("Yield").AsDouble;
                mock_Date = docs_02[0].GetValue("OccurTime").ToString();
            }

            return new VueVO(info_Lon, info_Lat, mock_Lon,
             mock_Lat, mock_Alt, mock_Yield, mock_Date);
        }

        public List<BaseVO> Query()
        {
            // 一个旅：通信站1个、发射场6~36个、中心库2个、待机库2个、营区（主营区1个，小营区2个）
            var collection = _client.GetDatabase(_config.InfoSetting.Database).
                GetCollection<BsonDocument>(_config.InfoSetting.Collection);

            // 查询不重复基地名称
            FieldDefinition<BsonDocument, string> field = "base";  // 需要distinct字段
            var wheres = Builders<BsonDocument>.Filter.Empty; // 查询条件
            var bases = collection.Distinct(field, wheres).ToList();  // 返回list

            // 查询不重复旅名称
            FieldDefinition<BsonDocument, string> field2 = "brigade";  // 需要distinct字段
            var brigades = collection.Distinct(field2, wheres).ToList();  // 返回list

            //var filter = Builders<BsonDocument>.Filter;
            //var docs_01 = collection.Find(filter.Eq("base", "大凉山基地")
            //                            & filter.Eq("brigade", "45旅")
            //                            & filter.Eq("classification","井")
            //                            ).ToList();
            List<BaseVO> baseVOs = new List<BaseVO>();

            // 循环查询
            foreach (string bs in bases)
            {
                List<BrigadeVO> brigadeVOs = new List<BrigadeVO>();

                foreach (string brigade in brigades)
                {
                    var filter = Builders<BsonDocument>.Filter;
                    ProjectionDefinitionBuilder<BsonDocument> builderProjection = Builders<BsonDocument>.Projection;
                    ProjectionDefinition<BsonDocument> projection = builderProjection.Include("health").Exclude("_id");

                    List<TargetVO> targetVOs = new List<TargetVO>();

                    var docs_01 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "井")).Project(projection).ToList();
                    if (docs_01.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_01, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "井",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    var docs_02 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "车")).Project(projection).ToList();
                    if (docs_02.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_02, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "车",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    var docs_03 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "营区")).Project(projection).ToList();
                    if (docs_03.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_03, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "营区",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    var docs_04 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "中心库")).Project(projection).ToList();
                    if (docs_04.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_04, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "中心库",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    var docs_05 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "发射场")).Project(projection).ToList();
                    if (docs_05.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_05, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "发射场",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    var docs_06 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "通信站")).Project(projection).ToList();
                    if (docs_06.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_06, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "通信站",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    var docs_07 = collection.Find(filter.Eq("base", bs)
                                                & filter.Eq("brigade", brigade)
                                                & filter.Eq("classification", "待机库")).Project(projection).ToList();
                    if (docs_07.Count > 0)
                    {
                        int level00 = 0, level01 = 0, level02 = 0, level03 = 0;
                        Classfi(docs_07, ref level00, ref level01, ref level02, ref level03);
                        targetVOs.Add(new TargetVO("id", "待机库",
                            level00 + level01 + level02 + level03, level00, level01, level02, level03));

                    }

                    if (targetVOs.Count > 0)
                        brigadeVOs.Add(new BrigadeVO("ID", brigade, targetVOs));
                }
                baseVOs.Add(new BaseVO("id", bs, brigadeVOs));
            }

            return baseVOs;
        }
        #region Config表增删改查
        public Dictionary<string, ConfigBO> QueryConfigAll()
        {
            Dictionary<string, ConfigBO> kv = new Dictionary<string, ConfigBO>();

            var collection = _client.GetDatabase(_config.ConfigSetting.Database).GetCollection<BsonDocument>(_config.ConfigSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var config = BsonSerializer.Deserialize<ConfigBO>(doc);
                kv.Add(config.platform, config);
            }


            return kv;
        }
        public List<ConfigBO> GetConfigs()
        {
            List<ConfigBO> configs = new List<ConfigBO>();

            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<BsonDocument>(_config.ConfigSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var config = BsonSerializer.Deserialize<ConfigBO>(doc);
                configs.Add(config);
            }
            return configs;
        }

        public ConfigBO GetConfig(string id)
        {
            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<BsonDocument>(_config.ConfigSetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))).ToList();
            foreach (var doc in list)
            {
                var config = BsonSerializer.Deserialize<ConfigBO>(doc);
                return config;
            }
            return null;
        }
        public bool UpdateConfig(string id, ConfigBO config)
        {
            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<BsonDocument>(_config.ConfigSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));


            var update = Builders<BsonDocument>.Update.Set("shock_wave_01", config.shock_wave_01)
                                                        .Set("shock_wave_02", config.shock_wave_02)
                                                        .Set("shock_wave_03", config.shock_wave_03)
                                                        .Set("nuclear_radiation_01", config.nuclear_radiation_01)
                                                        .Set("nuclear_radiation_02", config.nuclear_radiation_02)
                                                        .Set("nuclear_radiation_03", config.nuclear_radiation_03)
                                                        .Set("thermal_radiation_01", config.thermal_radiation_01)
                                                        .Set("thermal_radiation_02", config.thermal_radiation_02)
                                                        .Set("thermal_radiation_03", config.thermal_radiation_03)
                                                        .Set("nuclear_pulse_01", config.nuclear_pulse_01)
                                                        .Set("nuclear_pulse_02", config.nuclear_pulse_02)
                                                        .Set("nuclear_pulse_03", config.nuclear_pulse_03)
                                                        .Set("fallout_01", config.fallout_01)
                                                        .Set("fallout_02", config.fallout_02)
                                                        .Set("fallout_03", config.fallout_03)
                                                        ;

            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteConfig(string id)
        {
            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<BsonDocument>(_config.ConfigSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }
        public string AddConfig(ConfigBO config)
        {
            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<BsonDocument>(_config.ConfigSetting.Collection);

            var document = config.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();

        }
        #endregion

        #region Description表增删改查
        /// <summary>
        /// 获取所有的Description表记录。
        /// </summary>
        /// <returns></returns>
        public List<DescriptionBO> GetDescriptions()
        {
            List<DescriptionBO> descriptions = new List<DescriptionBO>();

            var collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                    .GetCollection<BsonDocument>(_config.DescriptionSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var description = BsonSerializer.Deserialize<DescriptionBO>(doc);
                descriptions.Add(description);
            }
            return descriptions;
        }
        public DescriptionBO GetDescription(string id)
        {
            var collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                    .GetCollection<BsonDocument>(_config.DescriptionSetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))).ToList();
            foreach (var doc in list)
            {
                var config = BsonSerializer.Deserialize<DescriptionBO>(doc);
                return config;
            }
            return null;
        }
        public string AddDescription(DescriptionBO bo)
        {
            var collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                    .GetCollection<BsonDocument>(_config.DescriptionSetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();

        }
        public bool UpdateDescription(string id, DescriptionBO bo)
        {
            var collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                    .GetCollection<BsonDocument>(_config.DescriptionSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));




            var update = Builders<BsonDocument>.Update.Set("name", bo.name)
                                                    .Set("level_01", bo.level_01)
                                                    .Set("level_02", bo.level_02)
                                                    .Set("level_03", bo.level_03);
            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteDescription(string id)
        {
            var collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                        .GetCollection<BsonDocument>(_config.DescriptionSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }

        #endregion

        #region Info表增删改查
        public List<InfoBO> GetInfos()
        {
            List<InfoBO> infos = new List<InfoBO>();

            var collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<BsonDocument>(_config.InfoSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var info = BsonSerializer.Deserialize<InfoBO>(doc);
                infos.Add(info);
            }
            return infos;
        }
        public InfoBO GetInfo(string id)
        {
            var collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<BsonDocument>(_config.InfoSetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))).ToList();
            foreach (var doc in list)
            {
                var info = BsonSerializer.Deserialize<InfoBO>(doc);
                return info;
            }
            return null;
        }
        public string AddInfo(InfoBO bo)
        {
            var collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<BsonDocument>(_config.InfoSetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);

            InfoChanged();


            return document.GetValue("_id").ToString();
        }
        public bool UpdateInfo(string id, InfoBO bo)
        {
            var collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<BsonDocument>(_config.InfoSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var update = Builders<BsonDocument>.Update.Set("brigade", bo.brigade)
                                                    .Set("lon", bo.lon)
                                                    .Set("lat", bo.lat)
                                                    .Set("alt", bo.alt)
                                                    .Set("launchUnit", bo.launchUnit)
                                                    .Set("platform", bo.platform)
                                                    .Set("warZone", bo.warZone)
                                                    .Set("combatZone", bo.combatZone)
                                                    .Set("platoon", bo.platoon)
                                                    .Set("missileNo", bo.missileNo)
                                                    .Set("missileNum", bo.missileNum)
                                                    .Set("shock_wave_01", bo.shock_wave_01)
                                                    .Set("shock_wave_02", bo.shock_wave_02)
                                                    .Set("shock_wave_03", bo.shock_wave_03)
                                                    .Set("nuclear_radiation_01", bo.nuclear_radiation_01)
                                                    .Set("nuclear_radiation_02", bo.nuclear_radiation_02)
                                                    .Set("nuclear_radiation_03", bo.nuclear_radiation_03)
                                                    .Set("thermal_radiation_01", bo.thermal_radiation_01)
                                                    .Set("thermal_radiation_02", bo.thermal_radiation_02)
                                                    .Set("thermal_radiation_03", bo.thermal_radiation_03)
                                                    .Set("nuclear_pulse_01", bo.nuclear_pulse_01)
                                                    .Set("nuclear_pulse_02", bo.nuclear_pulse_02)
                                                    .Set("nuclear_pulse_03", bo.nuclear_pulse_03)
                                                    .Set("fallout_01", bo.fallout_01)
                                                    .Set("fallout_02", bo.fallout_02)
                                                    .Set("fallout_03", bo.fallout_03)
                                                    .Set("warBase", bo.warBase)
                                                    .Set("fireRange",bo.fireRange);





            var result = collection.UpdateOne(filter, update);
            InfoChanged();

            return result.ModifiedCount > 0;
        }
        public bool DeleteInfo(string id)
        {
            var collection = _client.GetDatabase(_config.InfoSetting.Database)
                                         .GetCollection<BsonDocument>(_config.InfoSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            InfoChanged();

            return result.DeletedCount > 0;

        }
        public List<TaggroupVO> Taggroup()
        {
            List<TaggroupVO> taggroups = new List<TaggroupVO>();
            var collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<BsonDocument>(_config.InfoSetting.Collection);

            var bases = collection.Distinct<string>("warBase", Builders<BsonDocument>.Filter.Empty).ToList().ToArray();
            var missileNos = collection.Distinct<string>("missileNo", Builders<BsonDocument>.Filter.Empty).ToList().ToArray();
            var platforms = collection.Distinct<string>("platform", Builders<BsonDocument>.Filter.Empty).ToList().ToArray();

            taggroups.Add(new TaggroupVO("基地", bases));
            taggroups.Add(new TaggroupVO("弹型", missileNos));
            taggroups.Add(new TaggroupVO("发射平台", platforms));

            return taggroups;
        }

        #endregion

        #region overlay表增删改查
        public List<OverlayBO> GetOverlays()
        {
            List<OverlayBO> bos = new List<OverlayBO>();

            var collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                    .GetCollection<BsonDocument>(_config.OverlaySetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<OverlayBO>(doc);
                bos.Add(bo);
            }
            return bos;
        }
        public OverlayBO GetOverlay(string id)
        {
            var collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                   .GetCollection<BsonDocument>(_config.OverlaySetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<OverlayBO>(doc);
                return bo;
            }
            return null;
        }
        public string AddOverlay(OverlayBO bo)
        {
            var collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                    .GetCollection<BsonDocument>(_config.OverlaySetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();
        }
        public bool UpdateOverlay(string id, OverlayBO bo)
        {
            var collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                   .GetCollection<BsonDocument>(_config.OverlaySetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var update = Builders<BsonDocument>.Update.Set("addend", bo.addend)
                                                    .Set("augend", bo.augend)
                                                    .Set("result", bo.result);

            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteOverlay(string id)
        {
            var collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                   .GetCollection<BsonDocument>(_config.OverlaySetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }
        #endregion

        #region Rule表增删改查
        public List<RuleBo> GetRules()
        {
            List<RuleBo> bos = new List<RuleBo>();

            var collection = _client.GetDatabase(_config.RuleSetting.Database)
                                    .GetCollection<BsonDocument>(_config.RuleSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<RuleBo>(doc);
                bos.Add(bo);
            }
            return bos;
        }
        public RuleBo GetRule(string id)
        {
            var collection = _client.GetDatabase(_config.RuleSetting.Database)
                                   .GetCollection<BsonDocument>(_config.RuleSetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<RuleBo>(doc);
                return bo;
            }
            return null;
        }
        public string AddRule(RuleBo bo)
        {
            var collection = _client.GetDatabase(_config.RuleSetting.Database)
                                    .GetCollection<BsonDocument>(_config.RuleSetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();
        }
        public bool UpdateRule(string id, RuleBo bo)
        {
            var collection = _client.GetDatabase(_config.RuleSetting.Database)
                                  .GetCollection<BsonDocument>(_config.RuleSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var update = Builders<BsonDocument>.Update.Set("name", bo.name)
                                                    .Set("unit", bo.unit)
                                                    .Set("limits", bo.limits)
                                                    .Set("description", bo.description);

            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteRule(string id)
        {
            var collection = _client.GetDatabase(_config.RuleSetting.Database)
                                   .GetCollection<BsonDocument>(_config.RuleSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }
        #endregion

        #region timeindex表增删改查
        public List<TimeindexBO> GetTimeindexs()
        {
            List<TimeindexBO> bos = new List<TimeindexBO>();

            var collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                    .GetCollection<BsonDocument>(_config.TimeindexSetting.Collection);

            var list = collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<TimeindexBO>(doc);
                bos.Add(bo);
            }
            return bos;
        }
        public TimeindexBO GetTimeindex(string id)
        {
            var collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                   .GetCollection<BsonDocument>(_config.TimeindexSetting.Collection);
            var list = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id))).ToList();
            foreach (var doc in list)
            {
                var bo = BsonSerializer.Deserialize<TimeindexBO>(doc);
                return bo;
            }
            return null;
        }
        public string AddTimeindex(TimeindexBO bo)
        {
            var collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                   .GetCollection<BsonDocument>(_config.TimeindexSetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();
        }
        public bool UpdateTimeindex(string id, TimeindexBO bo)
        {
            var collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                 .GetCollection<BsonDocument>(_config.TimeindexSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var update = Builders<BsonDocument>.Update.Set("platform", bo.platform)
                                                    .Set("missileNo", bo.missileNo)
                                                    .Set("prepareTime", bo.prepareTime)
                                                    .Set("targetBindingTime", bo.targetBindingTime)
                                                    .Set("defenseBindingTime", bo.defenseBindingTime);
            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteTimeindex(string id)
        {
            var collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                   .GetCollection<BsonDocument>(_config.TimeindexSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }

        public TimeindexBO QueryTimeindex(string platform, string missileNo)
        {
            var collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                   .GetCollection<BsonDocument>(_config.TimeindexSetting.Collection);

            var filter = Builders<BsonDocument>.Filter;

            var list = collection.Find(filter.Eq("platform", platform)
                                                & filter.Eq("missileNo", missileNo)
                                                ).ToList();
            foreach (var doc in list)
            {
                var bo = (TimeindexBO)BsonSerializer.Deserialize<TimeindexBO>(doc);
                return bo;
            }
            return null;
        }

        #endregion


        private void InfoChanged()
        {
            // 修改了调用HFJ的接口通知
            string url = _urlConfig.InfoChanged;// "http://localhost:7011/infochanged";
            try
            {
                Task<string> s = GetAsyncJson(url);
                s.Wait();
                Console.WriteLine("HFJ的infochanged接口被调用了");

            }
            catch (Exception e)
            {
                Console.WriteLine("HFJ的infochanged接口出错了");
            }
        }
        private static async Task<string> GetAsyncJson(string url)
        {
            HttpClient client = new HttpClient();
            //HttpContent content = new StringContent();
            //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
