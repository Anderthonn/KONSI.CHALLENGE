using KONSI.CHALLENGE.DOMAIN;

namespace KONSI.CHALLENGE.SERVICES.Interfaces
{
    public interface IMainService
    {
        Task<List<ConsultationBenefits>> ProcessCPF(string cpf);
    }
}