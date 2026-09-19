using EcoEnergyManagement.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.Core.Data;

public class AppDbContext : DbContext
{
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();

    public DbSet<Resource> Resources => Set<Resource>();

    public DbSet<Tariff> Tariffs => Set<Tariff>();

    public DbSet<ResourceLimit> ResourceLimits => Set<ResourceLimit>();

    public DbSet<ResourceConsumption> ResourceConsumptions => Set<ResourceConsumption>();

    public DbSet<ServiceOutput> ServiceOutputs => Set<ServiceOutput>();

    public DbSet<TemperatureRecord> TemperatureRecords => Set<TemperatureRecord>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            "Data Source=eco_energy.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // -----------------------------------------
        // OrganizationUnit
        // -----------------------------------------

        modelBuilder.Entity<OrganizationUnit>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<OrganizationUnit>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------------------------------
        // Resource
        // -----------------------------------------

        modelBuilder.Entity<Resource>()
            .HasKey(x => x.Id);

        // -----------------------------------------
        // Tariff
        // -----------------------------------------

        modelBuilder.Entity<Tariff>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Tariff>()
            .HasOne(x => x.Resource)
            .WithMany(x => x.Tariffs)
            .HasForeignKey(x => x.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------------------
        // ResourceLimit
        // -----------------------------------------

        modelBuilder.Entity<ResourceLimit>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ResourceLimit>()
            .HasOne(x => x.OrganizationUnit)
            .WithMany(x => x.ResourceLimits)
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResourceLimit>()
            .HasOne(x => x.Resource)
            .WithMany(x => x.Limits)
            .HasForeignKey(x => x.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        // One limit per unit/resource
        modelBuilder.Entity<ResourceLimit>()
            .HasIndex(x => new
            {
                x.OrganizationUnitId,
                x.ResourceId
            })
            .IsUnique();

        // -----------------------------------------
        // ResourceConsumption
        // -----------------------------------------

        modelBuilder.Entity<ResourceConsumption>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ResourceConsumption>()
            .HasOne(x => x.OrganizationUnit)
            .WithMany(x => x.Consumptions)
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResourceConsumption>()
            .HasOne(x => x.Resource)
            .WithMany(x => x.Consumptions)
            .HasForeignKey(x => x.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        // One consumption record per:
        // subdivision + resource + year + month
        modelBuilder.Entity<ResourceConsumption>()
            .HasIndex(x => new
            {
                x.OrganizationUnitId,
                x.ResourceId,
                x.Year,
                x.Month
            })
            .IsUnique();

        // -----------------------------------------
        // ServiceOutput
        // -----------------------------------------

        modelBuilder.Entity<ServiceOutput>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ServiceOutput>()
            .HasOne(x => x.OrganizationUnit)
            .WithMany(x => x.ServiceOutputs)
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceOutput>()
            .HasIndex(x => new
            {
                x.OrganizationUnitId,
                x.Year,
                x.Month
            })
            .IsUnique();

        // -----------------------------------------
        // Temperature
        // -----------------------------------------

        modelBuilder.Entity<TemperatureRecord>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<TemperatureRecord>()
            .HasOne(x => x.OrganizationUnit)
            .WithMany(x => x.TemperatureRecords)
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TemperatureRecord>()
            .HasIndex(x => new
            {
                x.OrganizationUnitId,
                x.Year,
                x.Month
            })
            .IsUnique();

        // -----------------------------------------
        // Decimal precision
        // -----------------------------------------

        modelBuilder.Entity<ResourceConsumption>()
            .Property(x => x.Amount)
            .HasPrecision(18, 3);

        modelBuilder.Entity<ResourceLimit>()
            .Property(x => x.MinValue)
            .HasPrecision(18, 3);

        modelBuilder.Entity<ResourceLimit>()
            .Property(x => x.MaxValue)
            .HasPrecision(18, 3);

        modelBuilder.Entity<Tariff>()
            .Property(x => x.Price)
            .HasPrecision(18, 4);

        modelBuilder.Entity<ServiceOutput>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TemperatureRecord>()
            .Property(x => x.Temperature)
            .HasPrecision(8, 2);
    }
}
