using Gateway.Api.Data;
using Gateway.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/works")]
    public class WorksController : ControllerBase
    {
        private readonly GatewayDbContext _db;

        public WorksController(GatewayDbContext db)
        {
            _db = db;
        }

        // POST /api/works/{assignmentId}/submissions
        [HttpPost("{assignmentId:int}/submissions")]
        public async Task<ActionResult<SubmissionResponseDto>> UploadSubmission(
            int assignmentId,
            [FromForm] SubmissionUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Файл не передан");

            var now = DateTime.UtcNow;

            // вместо настоящего FileService пока просто Guid
            var fakeFileId = Guid.NewGuid().ToString();

            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentName = request.StudentName,
                StudentId = request.StudentId,
                FileId = fakeFileId,
                Status = "DONE",
                CreatedAt = now,
                UpdatedAt = now
            };

            // если студент уже сдавал это задание — считаем плагиатом
            var existing = await _db.Submissions
                .Where(s => s.AssignmentId == assignmentId &&
                            s.StudentId == request.StudentId)
                .OrderBy(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                submission.IsPlagiarism = true;
                submission.PlagiarizedFromSubmissionId = existing.Id;
            }
            else
            {
                submission.IsPlagiarism = false;
                submission.PlagiarizedFromSubmissionId = null;
            }

            _db.Submissions.Add(submission);
            await _db.SaveChangesAsync();

            var response = new SubmissionResponseDto
            {
                SubmissionId = submission.Id,
                Status = submission.Status,
                IsPlagiarism = submission.IsPlagiarism,
                PlagiarizedFromSubmissionId = submission.PlagiarizedFromSubmissionId
            };

            return Ok(response);
        }

        // GET /api/works/{assignmentId}/reports
        [HttpGet("{assignmentId:int}/reports")]
        public async Task<ActionResult<object>> GetReports(int assignmentId)
        {
            var submissions = await _db.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();

            var reports = submissions.Select(s => new ReportItemDto
            {
                SubmissionId = s.Id,
                StudentName = s.StudentName,
                StudentId = s.StudentId,
                Status = s.Status,
                IsPlagiarism = s.IsPlagiarism,
                PlagiarizedFromSubmissionId = s.PlagiarizedFromSubmissionId
            }).ToList();

            return Ok(new
            {
                AssignmentId = assignmentId,
                Reports = reports
            });
        }
    }
}
