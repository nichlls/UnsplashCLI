using Microsoft.EntityFrameworkCore;
using UnsplashCLI.Models;

namespace UnsplashCLI.Data;

// Setup database context
public class PhotoDb : DbContext
{
    public PhotoDb(DbContextOptions<PhotoDb> options)
        : base(options) { }

    public DbSet<Photo> Photos { get; set; }
}
