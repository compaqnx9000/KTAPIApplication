using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTAPIApplication.Controllers
{
    [EnableCors("AllowSameDomain")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private IPDFService _PDFService;

        public PdfController(IPDFService pDFService)
        {
            _PDFService = pDFService;
        }
        /// <summary>
        /// http://localhost:5000/pdf?warBase=12&brigade=121
        /// </summary>
        /// <param name="warBase">基地名。</param>
        /// <param name="brigade">旅名。</param>
        /// <returns></returns>
        [HttpGet("pdf")]
        public FileResult GetPDF(string warBase, string brigade)
        {
            //获取html模板
            //var htmlContent = TemplateGenerator.GetPDFHTMLString(warBase, brigade);

            //生成PDF
            var pdfBytes = _PDFService.CreatePDF(warBase, brigade);

            return File(pdfBytes, "application/pdf");
        }
    }
}
