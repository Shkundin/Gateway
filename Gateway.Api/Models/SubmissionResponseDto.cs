namespace Gateway.Api.Models
{
    public class SubmissionResponseDto
    {
        public int SubmissionId { get; set; }
        public string Status { get; set; } = null!;
        public bool? IsPlagiarism { get; set; }
        public int? PlagiarizedFromSubmissionId { get; set; }
    }
}
