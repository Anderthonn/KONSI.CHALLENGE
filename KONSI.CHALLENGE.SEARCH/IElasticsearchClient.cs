using KONSI.CHALLENGE.DOMAIN;

namespace KONSI.CHALLENGE.SEARCH
{
    public interface IElasticsearchClient
    {
        Task IndexDataInElasticsearch(string jsonData);
        Task<List<ConsultationBenefits>> SearchDataInElasticsearch(string cpf);
        Task<bool> IsDataIndexed(string cpf, string numeroBeneficio, string codigoTipoBeneficio);
    }
}