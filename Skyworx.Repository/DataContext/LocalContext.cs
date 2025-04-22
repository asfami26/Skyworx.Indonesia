using Microsoft.EntityFrameworkCore;
using Skyworx.Repository.Entity;

namespace Skyworx.Repository.DataContext;

public class LocalContext(DbContextOptions<LocalContext> options) : DbContext(options)
{
    public DbSet<PengajuanKredit> PengajuanKredits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PengajuanKredit>().HasKey(p => p.Id);
    }
}