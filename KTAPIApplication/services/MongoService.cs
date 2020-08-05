using KTAPIApplication.bo;
using KTAPIApplication.Controllers;
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
            var collection = _client.GetDatabase(_config.InfoSetting.Database).
              GetCollection<InfoBO>(_config.InfoSetting.Collection);
            return collection.Find(Builders<InfoBO>.Filter.Empty).ToList();
        }
        public List<MockBO> QueryMockAll()
        {
            var collection = _otherClient.GetDatabase(_otherConfig.MockSetting.Database).GetCollection<MockBO>(_otherConfig.MockSetting.Collection);
            return collection.Find(Builders<MockBO>.Filter.Empty).ToList();
        }

        public List<MockBO> QueryMock(string nuclearExplosionID)
        {
            var collection = _otherClient.GetDatabase(_otherConfig.MockSetting.Database).GetCollection<MockBO>(_otherConfig.MockSetting.Collection);
            return collection.Find(Builders<MockBO>.Filter.Eq("NuclearExplosionID", nuclearExplosionID)).ToList();
        }

        public List<InfoBO> QueryInfoByBrigade(string brigade)
        {
            IMongoCollection<InfoBO> collection = _otherClient.GetDatabase(_config.InfoSetting.Database)
                                         .GetCollection<InfoBO>(_config.InfoSetting.Collection);
            return collection.Find(Builders<InfoBO>.Filter.Eq("brigade", brigade)).ToList();
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
            IMongoCollection<ConfigBO> collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<ConfigBO>(_config.ConfigSetting.Collection);

            return collection.Find(Builders<ConfigBO>.Filter.Empty).ToList();
        }

        public ConfigBO GetConfig(string id)
        {
            IMongoCollection<ConfigBO> collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<ConfigBO>(_config.ConfigSetting.Collection);
            return collection.Find(Builders<ConfigBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
        }

        public bool UpdateConfig(string id, ConfigBO config)
        {
            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<BsonDocument>(_config.ConfigSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));


            var update = Builders<BsonDocument>.Update.Set("platform", config.platform)
                                                        .Set("shock_wave_01", config.shock_wave_01)
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
                                                        .Set("nuclear_pulse_03", config.nuclear_pulse_03);

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

        public async  Task<IEnumerable<ConfigBO>> GetConfigsAsync()
        {
            var collection = _client.GetDatabase(_config.ConfigSetting.Database)
                                    .GetCollection<ConfigBO>(_config.ConfigSetting.Collection);

            return await collection.Find(Builders<ConfigBO>.Filter.Empty).ToListAsync();
        }

        #endregion

        #region Description表增删改查
        /// <summary>
        /// 获取所有的Description表记录。
        /// </summary>
        /// <returns></returns>
        public List<DescriptionBO> GetDescriptions()
        {
            IMongoCollection<DescriptionBO> collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                    .GetCollection<DescriptionBO>(_config.DescriptionSetting.Collection);
            return collection.Find(Builders<DescriptionBO>.Filter.Empty).ToList();
        }
        public DescriptionBO GetDescription(string id)
        {
            IMongoCollection<DescriptionBO> collection = _client.GetDatabase(_config.DescriptionSetting.Database)
                                    .GetCollection<DescriptionBO>(_config.DescriptionSetting.Collection);
            return collection.Find(Builders<DescriptionBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
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

        #region Factor表增删改查
        /// <summary>
        /// 获取所有的Factor表记录。
        /// </summary>
        /// <returns></returns>
        public List<FactorBO> GetFactors()
        {
            IMongoCollection<FactorBO> collection = _client.GetDatabase(_config.FactorSetting.Database)
                                    .GetCollection<FactorBO>(_config.FactorSetting.Collection);
            return collection.Find(Builders<FactorBO>.Filter.Empty).ToList();
        }
        public FactorBO GetFactor(string id)
        {
            IMongoCollection<FactorBO> collection = _client.GetDatabase(_config.FactorSetting.Database)
                                    .GetCollection<FactorBO>(_config.FactorSetting.Collection);
            return collection.Find(Builders<FactorBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
        }
        public string AddFactor(FactorBO bo)
        {
            var collection = _client.GetDatabase(_config.FactorSetting.Database)
                                    .GetCollection<BsonDocument>(_config.FactorSetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();

        }
        public bool UpdateFactor(string id, FactorBO bo)
        {
            var collection = _client.GetDatabase(_config.FactorSetting.Database)
                                    .GetCollection<BsonDocument>(_config.FactorSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var update = Builders<BsonDocument>.Update.Set("level_01", bo.level_01)
                                                    .Set("level_02", bo.level_02)
                                                    .Set("level_03", bo.level_03);
            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteFactor(string id)
        {
            var collection = _client.GetDatabase(_config.FactorSetting.Database)
                                        .GetCollection<BsonDocument>(_config.FactorSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }

        #endregion

        #region DamageLevel表增删改查
        /// <summary>
        /// 获取所有的DamageLevel表记录。
        /// </summary>
        /// <returns></returns>
        public List<DamageLevelBO> GetDamageLevels()
        {
            IMongoCollection<DamageLevelBO> collection = _client.GetDatabase(_config.DamageLevelSetting.Database)
                                    .GetCollection<DamageLevelBO>(_config.DamageLevelSetting.Collection);
            return collection.Find(Builders<DamageLevelBO>.Filter.Empty).ToList();
        }
        public DamageLevelBO GetDamageLevel(string id)
        {
            IMongoCollection<DamageLevelBO> collection = _client.GetDatabase(_config.DamageLevelSetting.Database)
                                    .GetCollection<DamageLevelBO>(_config.DamageLevelSetting.Collection);
            return collection.Find(Builders<DamageLevelBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
        }
        public string AddDamageLevel(DamageLevelBO bo)
        {
            var collection = _client.GetDatabase(_config.DamageLevelSetting.Database)
                                    .GetCollection<BsonDocument>(_config.DamageLevelSetting.Collection);

            var document = bo.ToBsonDocument();
            collection.InsertOne(document);
            return document.GetValue("_id").ToString();

        }
        public bool UpdateDamageLevel(string id, DamageLevelBO bo)
        {
            var collection = _client.GetDatabase(_config.DamageLevelSetting.Database)
                                    .GetCollection<BsonDocument>(_config.DamageLevelSetting.Collection);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var update = Builders<BsonDocument>.Update.Set("min", bo.min)
                                                    .Set("max", bo.max)
                                                    .Set("counter", bo.counter)
                                                    .Set("description", bo.description)
                                                    .Set("summary", bo.summary);
            var result = collection.UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }
        public bool DeleteDamageLevel(string id)
        {
            var collection = _client.GetDatabase(_config.DamageLevelSetting.Database)
                                        .GetCollection<BsonDocument>(_config.DamageLevelSetting.Collection);
            var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));

            var result = collection.DeleteOne(deleteFilter);
            return result.DeletedCount > 0;
        }

        #endregion

        #region Info表增删改查
        public List<InfoBO> GetInfos()
        {
            IMongoCollection<InfoBO> collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<InfoBO>(_config.InfoSetting.Collection);
            return collection.Find(Builders<InfoBO>.Filter.Empty).ToList();
        }
        public InfoBO GetInfo(string id)
        {
            IMongoCollection<InfoBO> collection = _client.GetDatabase(_config.InfoSetting.Database)
                                    .GetCollection<InfoBO>(_config.InfoSetting.Collection);
            return collection.Find(Builders<InfoBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
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
                                                    .Set("prepareTime" ,bo.prepareTime)
                                                    .Set("targetBindingTime" ,bo.targetBindingTime)
                                                    .Set("defenseBindingTime" ,bo.defenseBindingTime)
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
            IMongoCollection<OverlayBO> collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                    .GetCollection<OverlayBO>(_config.OverlaySetting.Collection);
            return collection.Find(Builders<OverlayBO>.Filter.Empty).ToList();
        }
        public OverlayBO GetOverlay(string id)
        {
            IMongoCollection<OverlayBO> collection = _client.GetDatabase(_config.OverlaySetting.Database)
                                   .GetCollection<OverlayBO>(_config.OverlaySetting.Collection);
            return collection.Find(Builders<OverlayBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
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
            IMongoCollection<RuleBo> collection = _client.GetDatabase(_config.RuleSetting.Database)
                                    .GetCollection<RuleBo>(_config.RuleSetting.Collection);
            return collection.Find(Builders<RuleBo>.Filter.Empty).ToList();
        }
        public RuleBo GetRule(string id)
        {
            IMongoCollection<RuleBo> collection = _client.GetDatabase(_config.RuleSetting.Database)
                                   .GetCollection<RuleBo>(_config.RuleSetting.Collection);
            return collection.Find(Builders<RuleBo>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
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
            IMongoCollection<TimeindexBO> collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                    .GetCollection<TimeindexBO>(_config.TimeindexSetting.Collection);
            return collection.Find(Builders<TimeindexBO>.Filter.Empty).ToList();
        }
        public TimeindexBO GetTimeindex(string id)
        {
            IMongoCollection<TimeindexBO> collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                   .GetCollection<TimeindexBO>(_config.TimeindexSetting.Collection);
            return collection.Find(Builders<TimeindexBO>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefault();
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
            IMongoCollection<TimeindexBO> collection = _client.GetDatabase(_config.TimeindexSetting.Database)
                                   .GetCollection<TimeindexBO>(_config.TimeindexSetting.Collection);

            var filter = Builders<TimeindexBO>.Filter;

            return collection.Find(filter.Eq("platform", platform)
                                                & filter.Eq("missileNo", missileNo)
                                                ).FirstOrDefault();
        }

        #endregion


        private void InfoChanged()
        {
            // 修改了调用HFJ的接口通知
            string url = _urlConfig.InfoChanged;// "http://localhost:7011/infochanged";
            try
            {
                Task<string> s = MyCore.Utils.HttpCli.GetAsyncJson(url);
                s.Wait();
                Console.WriteLine("HFJ的infochanged接口被调用了");

            }
            catch (Exception e)
            {
                Console.WriteLine("HFJ的infochanged接口出错了");
            }
        }
       
    }
}
