using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Configuration;
using System.Windows;

namespace SpiWpf.Data
{
    public class Repository
    {
        private static JsonSerializerOptions JsonDefaultOptions => new()
        {
            PropertyNameCaseInsensitive = true,
        };


        public static async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            if (CheckToken())
            {
                string token = Preferences.Token!;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var responseHttp = await _httpClient.GetAsync(url);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<T>(responseHttp, JsonDefaultOptions);
                return new HttpResponseWrapper<T>(response, false, responseHttp);
            }
            else
            {
                return new HttpResponseWrapper<T>(default!, true, responseHttp);
            }

        }

        public static async Task<HttpResponseWrapper<object>> Post<T>(string url, T model)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            if (CheckToken())
            {
                string token = Preferences.Token!;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var mesageJSON = JsonSerializer.Serialize(model);
            var messageContet = new StringContent(mesageJSON, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PostAsync(url, messageContet);
            return new HttpResponseWrapper<object>(null!, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public static async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T model)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            if (CheckToken())
            {
                string token = Preferences.Token!;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var messageJSON = JsonSerializer.Serialize(model);
            var messageContet = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PostAsync(url, messageContet);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<TResponse>(responseHttp, JsonDefaultOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }
            return new HttpResponseWrapper<TResponse>(default!, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public static async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            if (CheckToken())
            {
                string token = Preferences.Token!;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var responseHTTP = await _httpClient.DeleteAsync(url);
            return new HttpResponseWrapper<object>(null!, !responseHTTP.IsSuccessStatusCode, responseHTTP);
        }

        public static async Task<HttpResponseWrapper<object>> Put<T>(string url, T model)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            if (CheckToken())
            {
                string token = Preferences.Token!;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var messageJSON = JsonSerializer.Serialize(model);
            var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PutAsync(url, messageContent);
            return new HttpResponseWrapper<object>(null!, !responseHttp.IsSuccessStatusCode, responseHttp);
        }

        public static async Task<HttpResponseWrapper<TResponse>> Put<T, TResponse>(string url, T model)
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            if (CheckToken())
            {
                string token = Preferences.Token!;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var messageJSON = JsonSerializer.Serialize(model);
            var messageContent = new StringContent(messageJSON, Encoding.UTF8, "application/json");
            var responseHttp = await _httpClient.PutAsync(url, messageContent);
            if (responseHttp.IsSuccessStatusCode)
            {
                var response = await UnserializeAnswer<TResponse>(responseHttp, JsonDefaultOptions);
                return new HttpResponseWrapper<TResponse>(response, false, responseHttp);
            }

            return new HttpResponseWrapper<TResponse>(default!, !responseHttp.IsSuccessStatusCode, responseHttp);
        }



        private static async Task<T> UnserializeAnswer<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var respuestaString = await httpResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(respuestaString, jsonSerializerOptions)!;
        }

        private static bool CheckToken()
        {
            string token = Preferences.Token!;
            if (!string.IsNullOrEmpty(token))
            {
                string _DateToken = Preferences.DateToken!;
                DateTime fechaToken = DateTime.Parse(_DateToken);
                var dateCurrent = DateTime.Now;
                if (fechaToken >= dateCurrent)
                {
                    return true; //Token Activo
                }
                else
                {
                    return false; //Token Vencido
                }
            }
            return false;  //no se ha guardado un Token.
        }
    }
}
