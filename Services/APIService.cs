using System.Text;
using Newtonsoft.Json;

namespace pract14mobile.Services
{
    public class APIService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _apiBaseUrl = "http://192.168.1.91:5260/"; // Используйте ваш IP адрес

        public static T Get<T>(string endPoint)
        {
            var response = _httpClient.GetAsync(_apiBaseUrl + endPoint).Result;

            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }

            var content = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<T>(content);
            return data;
        }

        public static T Post<T>(T body, string endpoint)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(_apiBaseUrl + endpoint, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        public static bool Put<T>(T body, int id, string endpoint)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PutAsync(_apiBaseUrl + endpoint + "/" + id, content).Result;

            return response.IsSuccessStatusCode;
        }

        public static bool Delete(int id, string endpoint)
        {
            var response = _httpClient.DeleteAsync(_apiBaseUrl + endpoint + "/" + id).Result;
            return response.IsSuccessStatusCode;
        }
    }
}