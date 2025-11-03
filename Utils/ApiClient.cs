using RestSharp;

namespace OnlineShoppingTests.Utils
{
    public class ApiClient : IDisposable
    {
        private readonly RestClient _client;

        public ApiClient(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<RestResponse> GetAsync(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Get);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> PostAsync(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddJsonBody(body);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> DeleteAsync(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.Delete);
            return await _client.ExecuteAsync(request);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}