namespace qa_dotnet_cucumber.Models
{
    public class CertificationModel
    {
        public string? Scenario { get; set; }
        public string? CertificateOrAward { get; set; }
        public string? CertifiedFrom { get; set; }
        public string? Year { get; set; }


        public string? OldCertificateOrAward { get; set; }
        public string? ExpectedMessage { get; set; }
    }

    public class CertificationRoot
    {
        public List<CertificationModel> Certifications { get; set; }
    }
}
