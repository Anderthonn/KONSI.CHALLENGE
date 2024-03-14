using KONSI.CHALLENGE.DOMAIN;
using KONSI.CHALLENGE.QUEUE;
using KONSI.CHALLENGE.SEARCH;
using KONSI.CHALLENGE.SERVICES.Interfaces;
using System.Text.RegularExpressions;

namespace KONSI.CHALLENGE.SERVICES.Services
{
    public class MainService : IMainService
    {
        private readonly IRabbitMQConsumer _rabbitMQConsumer;
        private readonly IElasticsearchClient _elasticsearchClient;
        private readonly IConsultationBenefitsService _consultationBenefitsService;

        public MainService(IRabbitMQConsumer rabbitMQConsumer, IElasticsearchClient elasticsearchClient, IConsultationBenefitsService consultationBenefitsService)
        {
            _rabbitMQConsumer = rabbitMQConsumer ?? throw new ArgumentNullException(nameof(rabbitMQConsumer));
            _elasticsearchClient = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));
            _consultationBenefitsService = consultationBenefitsService ?? throw new ArgumentNullException(nameof(consultationBenefitsService));
        }

        public async Task<List<ConsultationBenefits>> ProcessCPF(string cpf)
        {
            try
            {
                if (string.IsNullOrEmpty(cpf))
                {
                    throw new ArgumentException("O CPF não pode ser nulo ou vazio.", nameof(cpf));
                }

                _rabbitMQConsumer.EnqueueCPFList(cpf);
                await _consultationBenefitsService.ConsultationBenefitsAsync(cpf);

                var cpfFormat = Regex.Replace(cpf, @"[^\d]", "");
                var consultationBenefits = await _elasticsearchClient.SearchDataInElasticsearch(cpfFormat);

                return consultationBenefits;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar o CPF.", ex);
            }
        }
    }
}