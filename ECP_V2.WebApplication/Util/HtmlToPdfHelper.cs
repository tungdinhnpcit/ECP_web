using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace ECP_V2.WebApplication.Util
{
    public static class HtmlToPdfHelper
    {
        public static Byte[] PdfSharpConvert(String html)
        {
            Byte[] file = null;

            using (MemoryStream ms = new MemoryStream())
            {
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                file = ms.ToArray();
            }

            return file;

        }
    }
}