using KONSI.CHALLENGE.DOMAIN;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace KONSI.CHALLENGE.SERVICES.Connections
{
    public class ApplicationRestConnection : IApplicationRestConnection
    {
        private readonly IConfiguration _configuration;

        public ApplicationRestConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Connection(Method method, string urlAddOn, string? content)
        {
            try
            {
                var url = _configuration.GetSection("RestConnection").GetSection("Url").Value + urlAddOn;

                var client = new RestClient(url);

                var request = new RestRequest();

                switch (method.ToString().ToUpper())
                {
                    case "GET":
                        request.Method = Method.Get;
                        break;
                    case "POST":
                        request.Method = Method.Post;
                        request.AddParameter("application/json", JsonConvert.SerializeObject(content), ParameterType.RequestBody);
                        break;
                    case "PUT":
                        request.Method = Method.Put;
                        request.AddParameter("application/json", JsonConvert.SerializeObject(content), ParameterType.RequestBody);
                        break;
                    case "DELETE":
                        request.Method = Method.Delete;
                        break;
                }

                var token = await GetToken();
                request.AddHeader("Authorization", $"Bearer {token.Data.Token}");

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return response.Content ?? "";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("O recurso solicitado não foi encontrado.");
                }
                else
                {
                    throw new Exception($"Falha na solicitação. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Erro na solicitação HTTP: {ex.Message}");
            }
        }

        private async Task<TokenRequest> GetToken()
        {
            try
            {
                TokenRequest tokenRequest;
                var url = _configuration.GetSection("RestConnection").GetSection("Url").Value + _configuration.GetSection("RestConnection").GetSection("UrlAddOnOne").Value;
                var username = _configuration.GetSection("RestConnection").GetSection("Username").Value;
                var password = _configuration.GetSection("RestConnection").GetSection("Password").Value;

                var client = new RestClient(url);
                var request = new RestRequest();

                request.Method = Method.Post;
                request.AddParameter("application/json", JsonConvert.SerializeObject(new { username, password }), ParameterType.RequestBody);

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    JObject jsonResponse = JObject.Parse(response.Content ?? "");

                    tokenRequest = new TokenRequest();
                    tokenRequest.Success = jsonResponse["success"]?.ToString() ?? "";
                    tokenRequest.Data = new TokenData();
                    tokenRequest.Data.Token = jsonResponse["data"]?["token"]?.ToString() ?? "";
                    tokenRequest.Data.Type = jsonResponse["data"]?["type"]?.ToString() ?? "";
                    tokenRequest.Data.ExpiresIn = jsonResponse["data"]?["expiresIn"]?.ToString() ?? "";

                    return tokenRequest;
                }
                else
                {
                    throw new Exception($"Falha na solicitação. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}