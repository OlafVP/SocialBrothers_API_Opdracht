using API_Opdracht.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Opdracht.Data;

public class DatabaseContext :DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Address>? Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>().ToTable("Addresses");
    }
}