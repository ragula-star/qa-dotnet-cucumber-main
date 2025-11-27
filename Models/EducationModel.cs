namespace qa_dotnet_cucumber.Models
{
    public class EducationModel
    {
        public string? University { get; set; }
        public string? Country { get; set; }
        public string? Title { get; set; }
        public string? Degree { get; set; }
        public string? Year { get; set; }

        public string Scenario { get; set; } = string.Empty;
    }
    public class EducationRoot
    {
        public List<EducationModel> Educations { get; set; }
    }

}
