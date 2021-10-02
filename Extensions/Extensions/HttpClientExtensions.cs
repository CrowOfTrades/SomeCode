using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetRequestAsync<T>(this HttpClient http, string path, string token)
        {
            HttpRequestMessage request = CreateRequest(path, HttpMethod.Get);

            T response = await http.SendRequestAsync<T>(request, token);

            return response;
        }

        public static async Task<T> PostRequestAsync<T>(this HttpClient http, string path, object payload, string token)
        {
            HttpRequestMessage request = CreateRequest(path, HttpMethod.Post, payload);

            T response = await http.SendRequestAsync<T>(request, token);

            return response;
        }

        public static async Task<T> PutRequestAsync<T>(this HttpClient http, string path, object payload, string token)
        {
            HttpRequestMessage request = CreateRequest(path, HttpMethod.Put, payload);

            T response = await http.SendRequestAsync<T>(request, token);

            return response;
        }

        public static async Task<T> HeadRequestAsync<T>(this HttpClient http, string path, object payload, string token)
        {
            HttpRequestMessage request = CreateRequest(path, HttpMethod.Head, payload);

            T response = await http.SendRequestAsync<T>(request, token);

            return response;
        }

        public static async Task<T> DeleteRequestAsync<T>(this HttpClient http, string path, object payload, string token)
        {
            HttpRequestMessage request = CreateRequest(path, HttpMethod.Delete, payload);

            T response = await http.SendRequestAsync<T>(request, token);

            return response;
        }

        public static async Task<T> PatchRequestAsync<T>(this HttpClient http, string path, object payload, string token)
        {
            HttpRequestMessage request = CreateRequest(path, HttpMethod.Patch, payload);

            T response = await http.SendRequestAsync<T>(request, token);

            return response;
        }

        #region [ PRIVATE METHODS ]
        private static void SetToken(this HttpClient http, string token)
        {
            if (string.IsNullOrEmpty(token)) return;

            http.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
        }

        private static HttpRequestMessage CreateRequest(string path, HttpMethod method, object payload = null)
        {
            HttpRequestMessage request = new()
            {
                Content = payload?.ConvertToContent(),
                Method = method,
                RequestUri = new Uri(path)
            };
            return request;
        }

        private static StringContent ConvertToContent(this object payload)
        {
            string payloadSerialized = JsonSerializer.Serialize(payload);
            return new StringContent(payloadSerialized, Encoding.UTF8, "application/json");
        }

        private static async Task<T> SendRequestAsync<T>(this HttpClient http, HttpRequestMessage request, string token)
        {
            http.SetToken(token);

            HttpResponseMessage response = await http.SendAsync(request);

            T responseObj = response.IsSuccessStatusCode ? await response.ReadAsAsync<T>() : default;

            return responseObj;
        }

        private static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
        {
            if (response.StatusCode is HttpStatusCode.NoContent) return default;

            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(responseStream);
        }
        #endregion [ PRIVATE METHODS ]
    }
}
