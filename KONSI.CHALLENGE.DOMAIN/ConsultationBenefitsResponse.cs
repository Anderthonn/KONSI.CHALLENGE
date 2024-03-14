namespace KONSI.CHALLENGE.DOMAIN
{
    public class ConsultationBenefitsResponse
    {
        public string? Success { get; set; }
        public ConsultationBenefitsData? Data { get; set; }
    }

    public class ConsultationBenefitsData
    {
        public string? Cpf { get; set; }
        public List<ConsultationBenefit>? Benefits { get; set; }
    }

    public class ConsultationBenefit
    {
        public string? BenefitNumber { get; set; }
        public string? BenefitTypeCode { get; set; }
    }
    public class ConsultationBenefits
    {
        public string? Cpf { get; set; }
        public string? BenefitNumber { get; set; }
        public string? BenefitTypeCode { get; set; }
    }
}