using KTAPIApplication.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Install-Package Haukcode.DinkToPdf -Version 1.1.2

namespace KTAPIApplication.Controllers
{
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
            string html = _PDFService.MakeHtml(warBase, brigade);
            var pdfBytes = MyPdfLib.PDFUtil.GenPDF(html);


            return File(pdfBytes, "application/pdf");

        }
    }
}
