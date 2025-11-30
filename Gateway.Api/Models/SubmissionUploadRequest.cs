using Microsoft.AspNetCore.Http;

namespace Gateway.Api.Models
{
    public class SubmissionUploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string StudentId { get; set; } = null!;
    }
}
