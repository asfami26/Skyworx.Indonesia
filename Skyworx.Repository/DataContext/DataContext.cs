using Microsoft.EntityFrameworkCore;
using Skyworx.Repository.Entity;

namespace Skyworx.Repository.DataContext;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<PengajuanKredit> PengajuanKredits { get; set; }
    public DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PengajuanKredit>().HasKey(p => p.Id);
        modelBuilder.Entity<UserAccount>().HasKey(u => u.Id);
    }
}