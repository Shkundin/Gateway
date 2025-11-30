namespace FileService.Api.Models
{
    public class FileRecord
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string StoragePath { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
