using KONSI.CHALLENGE.CACHE;
using KONSI.CHALLENGE.SEARCH;
using KONSI.CHALLENGE.SERVICES.Connections;
using KONSI.CHALLENGE.SERVICES.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace KONSI.CHALLENGE.SERVICES.Services
{
    public class ConsultationBenefitsService : IConsultationBenefitsService
    {
        private readonly IApplicationRestConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly IRedisCache _redisCache;
        private readonly IElasticsearchClient _elasticsearchClient;

        public ConsultationBenefitsService(IApplicationRestConnection connection, IConfiguration configuration, IRedisCache redisCache, IElasticsearchClient elasticsearchClient)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
            _elasticsearchClient = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));
        }

        public async Task ConsultationBenefitsAsync(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentNullException(nameof(cpf), "CPF não pode ser nulo ou vazio");

            try
            {
                string urlAddOn = _configuration.GetSection("RestConnection:UrlAddOnTwo").Value + cpf;

                var jsonData = _redisCache.GetJsonData(cpf);

                if (string.IsNullOrEmpty(jsonData))
                {
                    jsonData = await _connection.Connection(Method.Get, urlAddOn, null);
                    _redisCache.SetJsonData(cpf, jsonData);
                }

                var jsonObject = JObject.Parse(jsonData);
                var cpfValue = jsonObject["data"]?["cpf"]?.ToString();
                var beneficios = jsonObject["data"]?["beneficios"];

                if (!string.IsNullOrEmpty(cpfValue) && beneficios != null)
                {
                    foreach (var beneficio in beneficios)
                    {
                        var numeroBeneficio = beneficio["numero_beneficio"]?.ToString() ?? "";
                        var codigoTipoBeneficio = beneficio["codigo_tipo_beneficio"]?.ToString() ?? "";

                        if (!await _elasticsearchClient.IsDataIndexed(cpfValue, numeroBeneficio, codigoTipoBeneficio))
                        {
                            await _elasticsearchClient.IndexDataInElasticsearch(jsonData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar benefícios: " + ex.Message, ex);
            }
        }
    }
}