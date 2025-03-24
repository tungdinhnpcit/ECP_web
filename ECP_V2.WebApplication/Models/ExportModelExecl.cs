//using GemBox.Spreadsheet;
//using System.Collections.Generic;

//namespace ECP_V2.WebApplication.Models
//{
//    public class ExportModelExecl
//    {
//        public string Format { get; set; }
//        public SaveOptions Options => this.FormatMappingDictionary[this.Format];

//        public IDictionary<string, SaveOptions> FormatMappingDictionary => new Dictionary<string, SaveOptions>()
//        {
//            ["XLSX"] = new XlsxSaveOptions(),
//            ["XLS"] = new XlsSaveOptions(),
//            ["ODS"] = new OdsSaveOptions(),
//            ["CSV"] = new CsvSaveOptions(CsvType.CommaDelimited),
//            ["PDF"] = new PdfSaveOptions(),
//            ["HTML"] = new HtmlSaveOptions() { EmbedImages = true },
//            ["XPS"] = new XpsSaveOptions(),
//            ["BMP"] = new ImageSaveOptions(ImageSaveFormat.Bmp),
//            ["PNG"] = new ImageSaveOptions(ImageSaveFormat.Png),
//            ["JPG"] = new ImageSaveOptions(ImageSaveFormat.Jpeg),
//            ["GIF"] = new ImageSaveOptions(ImageSaveFormat.Gif),
//            ["TIF"] = new ImageSaveOptions(ImageSaveFormat.Tiff)
//        };
//    }
//}