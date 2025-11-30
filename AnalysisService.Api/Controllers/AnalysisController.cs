using AnalysisService.Api.Models;
using AnalysisService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalysisService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly TextAnalysisService _service;

        public AnalysisController(TextAnalysisService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<AnalysisResponseDto> Analyze([FromBody] AnalysisRequestDto request)
        {
            var (isPlagiarism, similarity) = _service.Analyze(request.Text);

            var response = new AnalysisResponseDto
            {
                IsPlagiarism = isPlagiarism,
                SimilarityScore = similarity
            };

            return Ok(response);
        }
    }
}
