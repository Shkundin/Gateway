namespace AnalysisService.Api.Models
{
    public class AnalysisResponseDto
    {
        public bool IsPlagiarism { get; set; }
        public double SimilarityScore { get; set; }
    }
}
