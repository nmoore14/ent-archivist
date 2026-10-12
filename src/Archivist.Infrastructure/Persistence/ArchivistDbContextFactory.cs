using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Archivist.Infrastructure.Persistence;

public sealed class ArchivistDbContextFactory : IDesignTimeDbContextFactory<ArchivistDbContext>
{
    public ArchivistDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ArchivistDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=archivist_design;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new ArchivistDbContext(options);
    }
}
