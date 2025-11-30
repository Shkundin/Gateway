namespace Gateway.Api.Models
{
    public class Submission
    {
        public int Id { get; set; }

        public int AssignmentId { get; set; }

        public string StudentName { get; set; } = null!;
        public string StudentId { get; set; } = null!;

        public string FileId { get; set; } = null!;

        public string Status { get; set; } = "DONE";

        public bool? IsPlagiarism { get; set; }
        public int? PlagiarizedFromSubmissionId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
