using KONSI.CHALLENGE.DOMAIN;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json.Linq;

namespace KONSI.CHALLENGE.SEARCH
{
    public class ElasticsearchClient : IElasticsearchClient
    {
        private readonly IConfiguration _configuration;

        public ElasticsearchClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ElasticClient ElasticsearchConnection()
        {
            try
            {
                var settings = new ConnectionSettings(new Uri(_configuration.GetSection("ElasticsearchClient").GetSection("ElasticAddress").Value ?? ""))
                    .BasicAuthentication(_configuration.GetSection("ElasticsearchClient").GetSection("Username").Value, _configuration.GetSection("ElasticsearchClient").GetSection("Password").Value)
                    .DefaultIndex("cpf_index");

                var client = new ElasticClient(settings);

                return client;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task EnsureIndexExistsAsync()
        {
            try
            {
                var client = ElasticsearchConnection();
                var indexExistsResponse = await client.Indices.ExistsAsync("cpf_index");

                if (!indexExistsResponse.IsValid || !indexExistsResponse.Exists)
                {
                    var createIndexResponse = await client.Indices.CreateAsync("cpf_index", c => c
                        .Map<object>(m => m
                            .Properties(p => p
                                .Keyword(k => k.Name("cpf"))
                                .Text(t => t.Name("numero_beneficio"))
                                .Text(t => t.Name("codigo_tipo_beneficio"))
                            )
                        )
                    );

                    if (!createIndexResponse.IsValid)
                    {
                        throw new Exception($"Erro ao criar o índice 'cpf_index': {createIndexResponse.ServerError?.Error?.Reason ?? "Razão desconhecida"}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao garantir a existência do índice 'cpf_index': {ex.Message}");
            }
        }

        public async Task IndexDataInElasticsearch(string jsonData)
        {
            try
            {
                await EnsureIndexExistsAsync();

                var client = ElasticsearchConnection();

                var jsonObject = JObject.Parse(jsonData);

                var cpf = jsonObject["data"]?["cpf"]?.ToString();
                var beneficios = jsonObject["data"]?["beneficios"];

                if (!string.IsNullOrEmpty(cpf) && beneficios != null)
                {
                    foreach (var beneficio in beneficios)
                    {
                        var numeroBeneficio = beneficio["numero_beneficio"]?.ToString();
                        var codigoTipoBeneficio = beneficio["codigo_tipo_beneficio"]?.ToString();

                        var indexResponse = await client.IndexAsync(new
                        {
                            cpf = cpf,
                            numero_beneficio = numeroBeneficio,
                            codigo_tipo_beneficio = codigoTipoBeneficio
                        }, idx => idx.Index("cpf_index"));

                        if (!indexResponse.IsValid)
                        {
                            throw new Exception($"Erro ao indexar os dados no Elasticsearch: {indexResponse.ServerError?.Error?.Reason ?? "Razão desconhecida"}");
                        }
                    }
                }
                else
                {
                    throw new Exception("JSON fornecido não contém CPF ou benefícios válidos.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao indexar os dados no Elasticsearch: {ex.Message}");
            }
        }

        public async Task<List<ConsultationBenefits>> SearchDataInElasticsearch(string cpf)
        {
            try
            {
                var client = ElasticsearchConnection();

                var searchResponse = await client.SearchAsync<object>(s => s
                    .Index("cpf_index")
                    .Query(q => q
                        .Match(m => m
                            .Field("cpf")
                            .Query(cpf)
                        )
                    )
                );

                if (searchResponse.IsValid)
                {
                    var hits = searchResponse.Documents;

                    if (hits.Any())
                    {
                        var consultationBenefitsList = new List<ConsultationBenefits>();

                        foreach (var hit in hits)
                        {
                            var hitObject = hit as IDictionary<string, object>;
                            if (hitObject != null)
                            {
                                var consultationBenefits = new ConsultationBenefits
                                {
                                    Cpf = hitObject.ContainsKey("cpf") ? hitObject["cpf"].ToString() : null,
                                    BenefitNumber = hitObject.ContainsKey("numero_beneficio") ? hitObject["numero_beneficio"].ToString() : null,
                                    BenefitTypeCode = hitObject.ContainsKey("codigo_tipo_beneficio") ? hitObject["codigo_tipo_beneficio"].ToString() : null
                                };

                                consultationBenefitsList.Add(consultationBenefits);
                            }
                        }

                        return consultationBenefitsList;
                    }
                    else
                    {
                        throw new Exception("Nenhum resultado encontrado.");
                    }
                }
                else
                {
                    throw new Exception(searchResponse.DebugInformation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar dados no Elasticsearch: {ex.Message}");
            }
        }

        public async Task<bool> IsDataIndexed(string cpf, string numeroBeneficio, string codigoTipoBeneficio)
        {
            try
            {
                var client = ElasticsearchConnection();

                var searchResponse = await client.SearchAsync<object>(s => s
                    .Index("cpf_index")
                    .Query(q => q
                        .Bool(b => b
                            .Must(
                                m => m.Match(mt => mt.Field("cpf").Query(cpf)),
                                m => m.Match(mt => mt.Field("numero_beneficio").Query(numeroBeneficio)),
                                m => m.Match(mt => mt.Field("codigo_tipo_beneficio").Query(codigoTipoBeneficio))
                            )
                        )
                    )
                );

                return searchResponse.IsValid && searchResponse.Documents.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar dados no Elasticsearch: {ex.Message}");
            }
        }
    }
}