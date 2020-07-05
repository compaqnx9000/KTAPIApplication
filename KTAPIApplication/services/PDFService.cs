using DinkToPdf;
using DinkToPdf.Contracts;
using KTAPIApplication.core;
using KTAPIApplication.enums;
using KTAPIApplication.services;
using KTAPIApplication.vo;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTAPIApplication.Services
{
    public class PDFService : IPDFService
    {
        private readonly IConverter _converter;
        private readonly IMongoService _mongoService;
        private readonly IDamageAnalysisService _analysisService;


        public PDFService(IConverter converter, IMongoService mongoService, IDamageAnalysisService analysisService)
        {
            _converter = converter;
            _mongoService = mongoService ??
                throw new ArgumentNullException(nameof(mongoService));

            _analysisService = analysisService ??
                throw new ArgumentNullException(nameof(analysisService));
        }

        private string MakeHtml(string warBase, string brigade)
        {
            List<BsonDocument> mocks = _mongoService.QueryMockAll();
            

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
            <html>
                <head>
                <meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
                <style>
                    .title-text {
                      font-size: 8rem;
                      text-align: center;
                      color: #f00;
                    }

                    .title-line {
                      margin: 2rem 0 1.5rem 0;
                      border-bottom: 2px solid #f00;
                    }

                    .time-period {
                      display: flex;
                      padding-bottom: .5rem;
                    }

                    .time-period li {
                      flex: 1;
                      color: #333;
                    }

                    table {
                      border-collapse: collapse;
                      width: 100%;
                    }

                    table,
                    td,
                    tr {
                      text-align: center;
                      border: 1px solid black;
                    }

                    .table-content {
                      width: 100%;
                      margin-top: 1.5rem;
                    }

                    .table-title {
                      text-align: center;
                      font-size: 2.5rem;
                      color: #333;
                    }

                    tr td{
                          font-size: 1.6rem;
                    }

                    .paragraph {
                      line-height: 2.8rem;
                      font-size: 2rem;
                      text-indent: 2rem;
                      color: #333;
                      text-align: justify;
                    }

                    .title-file {
                      text-align: center;
                      font-size: 3rem;
                      font-family: 'SimHei';
                      font-weight: 300;
                      margin-bottom: 1rem;
                      color: #333;
                    }

                    .doc-container {
                      min-width: 1280px;
                      height: 100vh;
                      margin: 0 auto;
                      overflow-y: auto;
                      background: #999;
                    }

                    .doc-content {
                      /* width: 24cm; */
                      height: calc(100% - 4rem);
                      margin: 2rem auto;
                    }

                    .page-content {
                      width: 100%;
                      margin-bottom: 2rem;
                      min-height: 100%;
                      min-width: 1280px;
                    }

                    .page {
                      box-sizing: border-box;
                      padding: 3cm;
                      background: #fff;
                    }

                    .marg {
                      /* margin:  0 1rem; */
                      margin-right: 2rem;
                    }

                    html {
                      overflow-x: auto;
                      overflow-y: auto;
                      font-size: 16px;
                      height: 100vh;
                    }

                    body {
                      min-width: 1280px;
                      height: 100vh;
                    }

                    body,
                    dl,
                    dt,
                    dd,
                    ul,
                    ol,
                    li,
                    pre,
                    form,
                    fieldset,
                    input,
                    p,
                    blockquote,
                    th,
                    td {
                      font-weight: 400;
                      margin: 0;
                      padding: 0;
                    }

                    h1,
                    h2,
                    h3,
                    h4,
                    h4,
                    h5 {
                      margin: 0;
                      padding: 0;
                      font-size: 16px;
                    }

                    body {
                      color: #666666;
                      font-family: Helvetica, Arial, sans-serif;
                      font-size: 12px;
                      padding: 0px;
                      text-align: left;
                    }

                    select {
                      font-size: 12px;
                    }

                    table {
                      border-collapse: collapse;
                    }

                    fieldset,
                    img {
                      border: 0 none;
                    }

                    fieldset {
                      margin: 0;
                      padding: 0;
                    }

                    fieldset p {
                      margin: 0;
                      padding: 0 0 0 8px;
                    }

                    legend {
                      display: none;
                    }

                    address,
                    caption,
                    em,
                    strong,
                    th,
                    i {
                      font-style: normal;
                      font-weight: 400;
                    }

                    table caption {
                      margin-left: -1px;
                    }

                    hr {
                      border-bottom: 1px solid #FFFFFF;
                      border-top: 1px solid #E4E4E4;
                      border-width: 1px 0;
                      clear: both;
                      height: 2px;
                      margin: 5px 0;
                      overflow: hidden;
                    }

                    ol,
                    ul {
                      list-style-image: none;
                      list-style-position: outside;
                      list-style-type: none;
                    }

                    caption,
                    th {
                      text-align: left;
                    }

                    q:before,
                    q:after,
                    blockquote:before,
                    blockquote:after {
                      content: '';
                    }
                    </style>
                </head>
            ");

            sb.Append(@"<body>");
            sb.Append(@"<div class='page'>");

            sb.AppendFormat(@"<div class='head-red-container'>
                                  <h1 class='title-text'>
                                        核爆毁伤分析报告
                                  </h1>
                                  <h2 class='title-line'>
                                    <ul class='time-period'>
                                      <li></li>
                                      <li style='text-align:right'>{0}</li>
                                    </ul>
                                  </h2>
                              </div>", DateTime.Now.ToLongDateString().ToString());

            // xxx旅核爆毁伤分析报告
            sb.AppendFormat(@"<h1 class='title-file'>【{0}】旅核爆毁伤分析报告</h1>", brigade);

            // 表1的文字描述
            sb.AppendFormat(@" <p class='paragraph'>
                            【{0}】基地【{1}】旅，于2020年07月02日22点12分03秒,遭到袭击。遭袭核爆信息具体如下：
                         </p>",warBase,brigade);
            // 表1
            sb.Append(@"<p class='table-title'>表1 遭袭核爆信息</p>
                        <div class='table-content'>
                          <table>
                            <tr>
                              <td>时间</td>
                              <!-- <td>型号</td> -->
                              <td>核当量（t）</td>
                              <td>落点位置</td>
                              <td>爆高（m）</td>
                            </tr>");


            foreach (var mock in mocks)
            {
                DateTime occurTime = mock.GetValue("OccurTime").ToUniversalTime();
                occurTime = occurTime.AddHours(8);
                double lon = mock.GetValue("Lon").AsDouble;
                double lat = mock.GetValue("Lat").AsDouble;
                double alt = mock.GetValue("Alt").AsDouble;
                double yield = mock.GetValue("Yield").AsDouble;

                sb.AppendFormat(@"<tr>
                     <td>{0}</td>
                     <td>{1}</td>
                     <td>{2} , {3}</td>
                     <td>{4}</td>
                     </tr>", occurTime, yield, Math.Round(lon,2), Math.Round(lat,2), alt);
            }
            sb.Append(@"</table></div>");

            // 表2的文字描述
            sb.AppendFormat(@"<p class='paragraph' style='margin-top:2rem'>
                          【{0}】基地【{1}】旅中的核力量遭到破坏，其中
                          <span>{2}有{3}轻微受损，{4}中度受损，{5}重度受损；</span>
                          核力量毁伤情况具体如下：
                        </p>",warBase,brigade,8,19,29,39);
            // 表2
            sb.Append(@"<p class='table-title' style='margin-top:2rem'>表2 核力量毁伤信息</p><div class='table-content'>
                          <table>
                            <tr>
                              <td>核反击力量名称</td>
                              <td>总数</td>
                              <td>安全</td>
                              <td>轻微受损</td>
                              <td>中度受损</td>
                              <td>重度受损</td>
                              <td>毁伤程度约（%）</td>
                              <td>毁伤程度描述</td>
                            </tr>");
            //
            // TODO 循环添加表
            List<BaseVO> bases =  _analysisService.Query();
            BaseVO baseVO = bases.Where(it => it.baseName == warBase).FirstOrDefault();
            if (baseVO != null)
            {
                BrigadeVO brigadeVO = baseVO.brigadeList.Where(it => it.name == brigade).FirstOrDefault();
                if (brigadeVO!=null)
                {
                    foreach(var target in brigadeVO.children)
                    {
                        sb.AppendFormat(@"<tr> 
                                            <td>{0}</td>
                                            <td>{1}</td>
                                            <td>{2}</td>
                                            <td>{3}</td>
                                            <td>{4}</td>
                                            <td>{5}</td>
                                            <td>{6}</td>
                                            <td>{7}</td>
                                        ", target.abilityName,target.total,target.safeNumber,target.mildNumber,
                                        target.moderateNumber,target.severeNumber,
                                        "怎么算的？","TODO");
                    }
                }

            }


            sb.Append(@"<tr>
                          <td colspan='4'>综合毁伤程度约(%)</td>
                          <td colspan='3'>20%</td>
                          <td>xxx</td>
                        </tr>
                      </table>
                    </div>");

            // 表3的文字描述
            sb.AppendFormat(@"<p class='paragraph'>
                                经评估，全旅损失战斗力约为{0}%，无法组织反击任务。毁伤级别评估标准具体如下：
                              </p>", 80);

            // 表3
            sb.Append(@"<p class='table-title'>表3 核力量综合毁伤评估标准</p>
                        <div class='table-content'>
                          <table>
                            <tr>
                              <td>毁伤程度范围（%）</td>
                              <td>程度描述</td>
                            </tr>
                            <tr>
                              <td>70%-100%</td>
                              <td>重度毁伤</td>
                            </tr>
                            <tr>
                              <td>30%-70%</td>
                              <td>中度毁伤</td>
                            </tr>
                            <tr>
                              <td>0%-30%</td>
                              <td>轻度毁伤</td>
                            </tr>
                            <tr>
                              <td>0%</td>
                              <td>安全</td>
                            </tr>
                          </table>
                        </div>");

            // 表4
            sb.Append(@"<p class='table-title' style='margin-top:2rem'>表4 每种核力量毁伤评估标准</p>
                        <div class='table-content'>
                          <table>
                            <tr>
                              <td>核力量类型</td>
                              <td>抗压能力PSI</td>
                              <td>受到的压强范围PSI</td>
                              <td>抗核辐射能力REM</td>
                              <td>受到的核辐射范围REM</td>
                              <td>程度描述</td>
                            </tr>
                            <tr>
                              <td rowspan='4'>发射井</td>
                              <td rowspan='4'>1000</td>
                              <td>2000以上</td>
                              <td rowspan='4'>无</td>
                              <td rowspan='4'>无</td>
                              <td>重度毁伤</td>
                            </tr>
                            <tr>
                              <td>1300-2000</td>
                              <td>中度毁伤</td>
                            </tr>
                            <tr>
                              <!-- <td>1000</td> -->
                              <td>1000-1300</td>
                              <!-- <td>无</td>
                            <td>无</td> -->
                              <td>轻度毁伤</td>
                            </tr>
                            <tr>
                              <!-- <td>1000</td> -->
                              <td>1000以下</td>
                              <!-- <td>无</td>
                            <td>无</td> -->
                              <td>安全</td>
                            </tr>
                            <tr>
                              <td rowspan='4'>发射车</td>
                              <td rowspan='4'>1000</td>
                              <td>2000以上</td>
                              <td rowspan='4'>无</td>
                              <td rowspan='4'>无</td>
                              <td>重度毁伤</td>
                            </tr>
                            <tr>
                              <td>1300-2000</td>
                              <!-- <td>无</td>
                            <td>无</td> -->
                              <td>中度毁伤</td>
                            </tr>
                            <tr>
                              <!-- <td>1000</td> -->
                              <td>1000-1300</td>
                              <!-- <td>无</td>
                            <td>无</td> -->
                              <td>轻度毁伤</td>
                            </tr>
                            <tr>
                              <!-- <td>1000</td> -->
                              <td>1000以下</td>
                              <!-- <td>无</td>
                            <td>无</td> -->
                              <td>安全</td>
                            </tr>
                          </table>
                        </div>");

            // 表5的文字描述
            sb.AppendFormat(@"<p class='paragraph' style='margin-top:2rem'>
                            【{0}】基地【{1}】旅，核火球、冲击波、核辐射、光辐射、电磁脉冲五种毁伤区域如下：
                        </p>",warBase,brigade);
            // 表5
            sb.Append(@" <p class='table-title'>表5 核爆毁伤区域</p>
                        <div class='table-content'>
                            <table>
                                <tr>
                                    <td>时间</td>
                                    <td>核火球</td>
                                    <td>冲击波</td>
                                    <td>光辐射</td>
                                    <td>核辐射</td>
                                    <td>电磁脉冲</td>
                                    <td>核沉降</td>
                                </tr>");

            //TODO: 循环添加
            MyAnalyse myAnalyse = new MyAnalyse();
            foreach (var mock in mocks)
            {
                DateTime occurTime = mock.GetValue("OccurTime").ToUniversalTime();
                occurTime = occurTime.AddHours(8);
                double lon = mock.GetValue("Lon").AsDouble;
                double lat = mock.GetValue("Lat").AsDouble;
                double alt = mock.GetValue("Alt").AsDouble;
                double yield = mock.GetValue("Yield").AsDouble;

               
                double fireball = myAnalyse.CalcfireBallRadius(yield / 1000.0, alt > 0);
                double shockwave = myAnalyse.CalcShockWaveRadius(yield / 1000.0, alt * 3.2808399, 1);
                double thermalRadiation = myAnalyse.GetThermalRadiationR(yield / 1000.0, alt * 3.2808399, 1.9);
                double nuclearRadiation = myAnalyse.CalcNuclearRadiationRadius(yield / 1000.0, alt * 3.2808399, 100);
                double nuclearPulse = myAnalyse.CalcNuclearPulseRadius(yield, alt / 1000.0, 200);

                double maximumDownwindDistance = 0; double maximumWidth = 0;
                myAnalyse.CalcRadioactiveFalloutRegion(lon,lat,alt * 3.2808399 ,yield/1000, 15, 225,DamageEnumeration.Light,
                     ref  maximumDownwindDistance,ref  maximumWidth);



                sb.AppendFormat(@"<tr>
                                 <td>{0}</td>
                                 <td>{1}</td>
                                 <td>{2}</td>
                                 <td>{3}</td>
                                 <td>{4}</td>
                                 <td>{5}</td>
                                 <td>{6} , {7}</td>
                                 </tr>", occurTime, Math.Round(fireball,2), Math.Round(shockwave,2), Math.Round(thermalRadiation,2),
                                 Math.Round(nuclearRadiation,2), Math.Round(nuclearPulse,2), 
                                 Math.Round(maximumDownwindDistance,2), Math.Round(maximumWidth,2));
            }

            sb.Append(@" </table></div>");

            sb.Append(@"</div>");

            sb.Append(@"</body>");

            sb.Append(@"</html>");
            return sb.ToString();
        }
        /// <summary>
        /// 创建PDF
        /// </summary>
        /// <param name="htmlContent">传入html字符串</param>
        /// <returns></returns>
        public byte[] CreatePDF(string warBase, string brigade)
        {
            string htmlContent =  MakeHtml(warBase,brigade);
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                //Margins = new MarginSettings
                //{
                //    Top = 10,
                //    Left = 0,
                //    Right = 0,
                //},
                DocumentTitle = "PDF Report",
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                // Page = "www.baidu.com", //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE  这里是用现有的网页生成PDF
                //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                WebSettings = { DefaultEncoding = "utf-8" },
                //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            //return File(file, "application/pdf");

            return file;

        }

    }
}
