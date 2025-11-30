using FileService.Api.Data;
using FileService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly FileServiceDbContext _db;
        private readonly IWebHostEnvironment _env;

        public FileController(FileServiceDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не передан");

            var id = Guid.NewGuid();

            var storageFolder = Path.Combine(_env.ContentRootPath, "Storage");
            Directory.CreateDirectory(storageFolder);

            var ext = Path.GetExtension(file.FileName);
            var storedFileName = id + ext;
            var fullPath = Path.Combine(storageFolder, storedFileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var record = new FileRecord
            {
                Id = id,
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                StoragePath = fullPath,
                CreatedAt = DateTime.UtcNow
            };

            _db.Files.Add(record);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                fileId = record.Id,
                fileName = record.FileName
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var record = await _db.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (record == null)
                return NotFound("Запись о файле не найдена");

            if (!System.IO.File.Exists(record.StoragePath))
                return NotFound("Файл не найден");

            var bytes = await System.IO.File.ReadAllBytesAsync(record.StoragePath);
            return File(bytes, record.ContentType, record.FileName);
        }
    }
}
