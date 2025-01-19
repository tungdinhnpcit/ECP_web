using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ECP_V2.WebApplication.NotificationService;
using Newtonsoft.Json;

namespace ECP_V2.WebApplication.NotifyService
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiNotifyUrl;

        public NotificationService(HttpClient httpClient, string apiNotifyUrl)
        {
            _httpClient = httpClient;
            _apiNotifyUrl = apiNotifyUrl;
        }

        public async Task<bool> SendNotificationsAsync(string[] userIds, NotificationRequest2 requestData)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    // Thay đổi requestData cho từng userId nếu cần thiết
                    var request = new
                    {
                        IDConect = requestData.IDConect,
                        userId = userId,
                        title = requestData.Title,
                        name = requestData.Name,
                        header = requestData.Header,
                        subtitle = requestData.Subtitle,
                        contents = requestData.Contents
                    };

                    // Serialize request data thành JSON
                    var jsonContent = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Gửi request đến API để gửi thông báo
                    var response = await _httpClient.PostAsync(_apiNotifyUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Xử lý nếu API trả về lỗi
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                // Bạn có thể thêm logging hoặc xử lý exception ở đây
                return false;
            }
        }

      
    }
}
