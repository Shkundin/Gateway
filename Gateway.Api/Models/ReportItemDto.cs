namespace Gateway.Api.Models
{
    public class ReportItemDto
    {
        public int SubmissionId { get; set; }
        public string StudentName { get; set; } = null!;
        public string StudentId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public bool? IsPlagiarism { get; set; }
        public int? PlagiarizedFromSubmissionId { get; set; }
    }
}
