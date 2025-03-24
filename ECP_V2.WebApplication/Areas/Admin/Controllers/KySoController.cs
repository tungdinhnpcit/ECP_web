
using ECP_V2.Common.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using static ECP_V2.WebApplication.Areas.Admin.Controllers.BCGBATController;

namespace ECP_V2.WebApplication.Areas.Admin.Controllers
{
    [Authorize]
    public class KySoController : UTController
    {

        string APIKySo_Image = System.Configuration.ConfigurationManager.AppSettings["APIKySo_Image"].ToString();
        string strAppCode = System.Configuration.ConfigurationManager.AppSettings["strAppCode"].ToString();
        string strPassword = System.Configuration.ConfigurationManager.AppSettings["strPassword"].ToString();
        string _strSerial = " ";


        public class data_KySo { public string data { get; set; } }
        public class Model_Ky_CA
        {
            public string strAppCode { get; set; }
            public string strPassword { get; set; }
            public string strSerial { get; set; }
            public string strAlias { get; set; }
            public string fileAsBase64String { get; set; }
            public string strHashAlogrithm { get; set; }
            public int? marginBottom { get; set; }
            public int? marginLeft { get; set; }
        }



        public async Task<ResponseData> KyCA(Model_Ky_CA Model)
        {
            ResponseData response = new ResponseData();
            response.Status = false;
            try
            {
                ServicePointManager.Expect100Continue = false;

                using (HttpClient client = new HttpClient())
                {
                    if (client.BaseAddress == null)
                    {
                        client.BaseAddress = new Uri(APIKySo_Image);
                    }
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.ConnectionClose = true;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                    //new MediaTypeWithQualityHeaderValue("application/json"));
                    new MediaTypeWithQualityHeaderValue("text/xml"));
                    String signedFileBase64 = "";

                    int numberPageSign = DisplayConfigConsts.NUMBER_PAGE_SIGN_DEFAULT;
                    float widthRectangle = DisplayConfigConsts.WIDTH_RECTANGLE_DEFAULT;
                    float heightRectangle = DisplayConfigConsts.HEIGHT_RECTANGLE_DEFAULT;
                    int locateSign = DisplayConfigConsts.LOCATE_SIGN_DEFAULT;
                    float marginTopOfRectangle = DisplayConfigConsts.MARGIN_TOP_OF_RECTANGLE_DEFAULT;
                    float marginBottomOfRectangle = Model.marginBottom ?? 0;// DisplayConfigConsts.MARGIN_BOTTOM_OF_RECTANGLE_DEFAULT;
                    float marginRightOfRectangle = DisplayConfigConsts.MARGIN_RIGHT_OF_RECTANGLE_DEFAULT;
                    float marginLeftOfRectangle = Model.marginLeft ?? 0;// DisplayConfigConsts.MARGIN_LEFT_OF_RECTANGLE_DEFAULT;
                    String displayText = DisplayConfigConsts.DISPLAY_TEXT_DEFAULT_EMPTY;
                    String formatRectangleText = DisplayConfigConsts.FORMAT_RECTANGLE_TEXT_DEFAULT;
                    String contact = DisplayConfigConsts.CONTACT_DEFAULT_EMPTY;
                    String reason = DisplayConfigConsts.REASON_DEFAULT_EMPTY;
                    String location = DisplayConfigConsts.LOCATION_DEFAULT_EMPTY;
                    String dateFormatString = DisplayConfigConsts.DATE_FORMAT_STRING_DEFAULT;
                    String fontPath = DisplayConfigConsts.FONT_PATH_DEFAULT;
                    int sizeFont = DisplayConfigConsts.SIZE_FONT_DEFAULT;
                    String organizationUnit = DisplayConfigConsts.ORGANIZATION_UNIT_DEFAULT_EMPTY;
                    String organization = DisplayConfigConsts.ORGANIZATION_DEFAULT_EMPTY;

                    String dspRec = "";
                    dspRec += "<contact>" + contact + "</contact>";
                    dspRec += "<dateFormatString>" + dateFormatString + "</dateFormatString>";
                    dspRec += "<displayText>" + displayText + "</displayText>";
                    dspRec += "<fontPath>" + fontPath + "</fontPath>";
                    dspRec += "<formatRectangleText>" + formatRectangleText + "</formatRectangleText>";
                    dspRec += "<heightRectangle>" + heightRectangle + "</heightRectangle>";
                    dspRec += "<locateSign>" + locateSign + "</locateSign>";
                    dspRec += "<location>" + location + "</location>";
                    dspRec += "<marginBottomOfRectangle>" + marginBottomOfRectangle + "</marginBottomOfRectangle>";
                    dspRec += "<marginLeftOfRectangle>" + marginLeftOfRectangle + "</marginLeftOfRectangle>";
                    dspRec += "<marginRightOfRectangle>" + marginRightOfRectangle + "</marginRightOfRectangle>";
                    dspRec += "<marginTopOfRectangle>" + marginTopOfRectangle + "</marginTopOfRectangle>";
                    dspRec += "<numberPageSign>" + numberPageSign + "</numberPageSign>";
                    dspRec += "<organization>" + organization + "</organization>";
                    dspRec += "<organizationUnit>" + organizationUnit + "</organizationUnit>";
                    dspRec += "<reason>" + reason + "</reason>";
                    dspRec += "<sizeFont>" + sizeFont + "</sizeFont>";
                    dspRec += "<widthRectangle>" + widthRectangle + "</widthRectangle>";
                    var content = "";
                    content += "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Body><signPdfBase64RectangleText xmlns=\"http://ws.signer.com/\">";
                    content += "<arg0 xmlns=\"\">" + strAppCode + "</arg0>";
                    content += "<arg1 xmlns=\"\">" + strPassword + "</arg1>";
                    //content += "<arg2 xmlns=\"\">" + Model.strSerial + "</arg2>"; ;
                    content += "<arg2 xmlns=\"\">" + " " + "</arg2>"; ;
                    content += "<arg3 xmlns=\"\">" + Model.strAlias + "</arg3>";
                    content += "<arg4 xmlns=\"\">" + Model.fileAsBase64String + "</arg4>";
                    content += "<arg5 xmlns=\"\">" + "SHA-1" + "</arg5>";
                    content += "<arg6 xmlns=\"\">" + dspRec + "</arg6>";
                    content += "<arg7 xmlns=\"\"><useTimestamp>false</useTimestamp></arg7>";
                    content += "</signPdfBase64RectangleText></soap:Body></soap:Envelope>";
                    var data = new data_KySo();
                    data.data = content;

                    var httpContent = new StringContent(content, Encoding.UTF8, "text/xml");

                    try
                    {
                        var result = client.PostAsync(client.BaseAddress, httpContent).Result;
                        string resultContent = await result.Content.ReadAsStringAsync();
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(resultContent);
                        string jsonText = JsonConvert.SerializeXmlNode(doc);
                        object transactObject1 = JsonConvert.DeserializeObject(jsonText);
                        JObject datafile = JObject.Parse(jsonText);
                        var dataBase64 = datafile["S:Envelope"]["S:Body"]["ns2:signPdfBase64RectangleTextResponse"]["return"]["signedFileBase64"].ToString();
                        response.Status = true;
                        response.Data = dataBase64;
                        return response;
                        //return Ok(transactObject1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        response.Message = ex.Message;
                        return response;

                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.Message = ex.Message;
                return response;

            }

        }
    }
    public class DisplayConfigConsts
    {
        /**
         * Begin Configuration Display Rectangle
         */
        //location of rectangle
        public static int TOP_LEFT_LOCATION = 1;
        public static int TOP_RIGHT_LOCATION = 2;
        public static int BOTTOM_LEFT_LOCATION = 3;
        public static int BOTTOM_RIGHT_LOCATION = 4;
        public static int USER_DEFINE_LOCATION = 5;
        public static int LOCATE_SIGN_DEFAULT = BOTTOM_LEFT_LOCATION;

        public static int NUMBER_PAGE_SIGN_DEFAULT = 1;
        public static float WIDTH_RECTANGLE_DEFAULT = 200;
        public static float HEIGHT_RECTANGLE_DEFAULT = 50;
        public static float MARGIN_TOP_OF_RECTANGLE_DEFAULT = 20f;
        public static float MARGIN_BOTTOM_OF_RECTANGLE_DEFAULT = 20f;
        public static float MARGIN_RIGHT_OF_RECTANGLE_DEFAULT = 220f;
        public static float MARGIN_LEFT_OF_RECTANGLE_DEFAULT = 230f;

        //text format of Rectangle to show contact, signDate, reason, location.
        public static String SIGN_TEXT_FORMAT_1 = "Người Ký: %s";
        public static String SIGN_TEXT_FORMAT_2 = "Người Ký: %s\r\nNgày Ký: %s";
        public static String SIGN_TEXT_FORMAT_3 = "Người Ký: %s\r\nNgày Ký: %s\r\nLý Do: %s";
        public static String SIGN_TEXT_FORMAT_4 = "Người Ký: %s\r\nNgày Ký: %s\r\nLý Do: %s\r\nĐịa Điểm: %s";
        public static String SIGN_TEXT_FORMAT_5 = "Digital signed by: %s";
        public static String SIGN_TEXT_FORMAT_6 = "Digital signed by: %s \r\nDate: %s";
        public static String SIGN_TEXT_FORMAT_7 = "Digital signed by: %s \r\nDate: %s \r\nReason: %s";
        public static String SIGN_TEXT_FORMAT_8 = "Digital signed by: %s \r\nDate: %s \r\nReason: %s \r\nLocation: %s";
        public static String SIGN_TEXT_FORMAT_9 = "Người Ký: %s\r\n%sNgày ký: %s";
        public static String SIGN_TEXT_FORMAT_10 = "Người Ký: %s\r\n%s\r\n%s\r\nNgày ký: %s";
        public static String FORMAT_RECTANGLE_TEXT_DEFAULT = SIGN_TEXT_FORMAT_2;

        public static String DATE_FORMAT_1 = "HH:mm:ss dd/MM/yyyy";
        public static String DATE_FORMAT_2 = "yyyy/MM/dd HH:mm:ss";

        public static String CONTACT_DEFAULT_EMPTY = "";
        public static String REASON_DEFAULT_EMPTY = "";
        public static String LOCATION_DEFAULT_EMPTY = "";
        public static String ORGANIZATION_UNIT_DEFAULT_EMPTY = "";
        public static String ORGANIZATION_DEFAULT_EMPTY = "";
        public static String DISPLAY_TEXT_DEFAULT_EMPTY = "";
        public static String DATE_FORMAT_STRING_DEFAULT = DATE_FORMAT_1;

        /**
         * End Configuration Display Rectangle
         */
        /**
         * Begin Configuration Display Table
         */
        public static String PAGE_SIZE_DEFAULT = "A4";
        public static int TOTAL_PAGE_SIGN_DEFAULT = 1;
        public static int MAX_PAGE_SIGN_DEFAULT = 10;
        public static String[] TITLES_DEFAULT = new String[] { "STT", "Người Ký", "Đơn vị", "Thời gian ký", "Ý kiến" };
        public static float[] WIDTHS_PERCEN_DEFAULT = new float[] { 0.06f, 0.18f, 0.2f, 0.14f, 0.42f };

        public static float HEIGHT_TITLE_DEFAULT = 30f;
        public static int[] BACKGROUND_COLOR_TITLE_DEFAULT = new int[] { 240, 240, 240 };
        public static int SIZE_FONT_DEFAULT = 9;
        public static float MARGIN_TOP_OF_TABLE_DEFAULT = 80f;
        public static float MARGIN_BOTTOM_OF_TABLE_DEFAULT = 80f;
        public static float MARGIN_RIGHT_OF_TABLE_DEFAULT = 60f;
        public static Boolean IS_DISPLAY_TITLE_PAGE_SIGN_DEFAULT = true;
        public static String TITLE_PAGE_SIGN_DEFAULT = "TRANG KÝ";
        public static float HEIGHT_ROW_TITLE_PAGE_SIGN_DEFAULT = 40f;
        public static int FONT_SIZE_TITLE_PAGE_SIGN_DEFAULT = 15;

        public static int ALIGN_UNDEFINED = -1;
        public static int ALIGN_LEFT = 0;
        public static int ALIGN_CENTER = 1;
        public static int ALIGN_RIGHT = 2;
        public static int ALIGN_JUSTIFIED = 3;
        public static int ALIGN_TOP = 4;
        public static int ALIGN_MIDDLE = 5;
        public static int ALIGN_BOTTOM = 6;
        public static int ALIGN_BASELINE = 7;
        public static int ALIGN_JUSTIFIED_ALL = 8;

        public static int[] ALIGNMENT_ARRAY_DEFAULT = new int[] { ALIGN_CENTER, ALIGN_LEFT, ALIGN_JUSTIFIED, ALIGN_LEFT, ALIGN_JUSTIFIED };

        public static String FONT_PATH_TIMESNEWROMAN_WINDOWS = "C:/windows/fonts/times.ttf";
        public static String FONT_PATH_TAHOMA_WINDOWS = "C:/windows/fonts/tahoma.ttf";
        public static String FONT_PATH_ARIAL_WINDOWS = "C:/windows/fonts/arial.ttf";

        //font text of table
        public static String FONT_PATH_DEFAULT = FONT_PATH_TIMESNEWROMAN_WINDOWS;
    }

}
