using FileService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FileService.Api.Data
{
    public class FileServiceDbContext : DbContext
    {
        public FileServiceDbContext(DbContextOptions<FileServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileRecord> Files => Set<FileRecord>();
    }
}
