using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ECP_V2.WebApplication.Helpers
{
    public class CallServiceAPI
    {
        public static async Task SendMsgMobile(string id,string msg)
        {
            using (var stringContent = new StringContent("{ \"id\": \""+id+ "\",\"mess\":\"" + msg + "\" }", System.Text.Encoding.UTF8, "application/json"))
            using (var client = new HttpClient())
            {
                try
                {
                    //var response = await client.PostAsync("http://113.160.100.121:8082/api/EPC_Mobile/SendMsgDevice", stringContent);
                    var response = await client.GetAsync("http://113.160.100.121:8082/api/EPC_Mobile/SendMsgDevice?id="+id+"&mess="+msg);
                    //var response = await client.PostAsync("http://localhost:13406/api/EPC_Mobile/SendMsgDevice", stringContent);
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }        
    }
}