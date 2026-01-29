using Newtonsoft.Json;
using System.Text;

namespace pract14mobile.Services
{
    public static class APIService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _apiBaseUrl = "http://192.168.1.91:5260/";

        static APIService()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        public static async Task<List<T>> GetListAsync<T>(string endPoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl + endPoint);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<T>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<T>>(content);
                return data ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<T>();
            }
        }
        public static async Task<T> GetAsync<T>(string endPoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl + endPoint);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(content);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                throw;
            }
        }

        public static List<T> Get<T>(string endPoint)
        {
            try
            {
                var response = _httpClient.GetAsync(_apiBaseUrl + endPoint).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API Error: {response.StatusCode}");
                    return new List<T>();
                }

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var data = JsonConvert.DeserializeObject<List<T>>(content);
                return data ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<T>();
            }
        }

        public static T GetSingle<T>(string endPoint)
        {
            try
            {
                var response = _httpClient.GetAsync(_apiBaseUrl + endPoint).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }

                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var data = JsonConvert.DeserializeObject<T>(content);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                throw;
            }
        }

        public static async Task<bool> PostAsync<T>(T body, string endpoint)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync(_apiBaseUrl + endpoint, content);
                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Post Error: {ex.Message}");
                return false;
            }
        }

        public static async Task<string> Post<T>(T body, string endpoint)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await _httpClient.PostAsync(_apiBaseUrl + endpoint, content);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error: {result.StatusCode} - {errorContent}");
                }

                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Post Error: {ex.Message}");
            }
        }

        public static async Task<bool> PutAsync<T>(T body, int id, string endpoint)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await _httpClient.PutAsync(_apiBaseUrl + endpoint + "/" + id, content);
                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Put Error: {ex.Message}");
                return false;
            }
        }

        public static async Task<string> Put<T>(T body, int id, string endpoint)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await _httpClient.PutAsync(_apiBaseUrl + endpoint + "/" + id, content);

                if (!result.IsSuccessStatusCode)
                {
                    var errorContent = await result.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Error: {result.StatusCode} - {errorContent}");
                }

                return await result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Put Error: {ex.Message}");
            }
        }

        public static bool Delete(string endpoint, int id)
        {
            try
            {
                var result = _httpClient.DeleteAsync(_apiBaseUrl + endpoint + "/" + id).GetAwaiter().GetResult();
                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> DeleteAsync(string endpoint, int id)
        {
            try
            {
                var result = await _httpClient.DeleteAsync(_apiBaseUrl + endpoint + "/" + id);
                return result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return false;
            }
        }
    }
}