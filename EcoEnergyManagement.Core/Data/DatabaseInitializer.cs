using System;
using System.Threading.Tasks;
using EcoEnergyManagement.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoEnergyManagement.Core.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync()
    {
        await using var db = new AppDbContext();

        await db.Database.EnsureCreatedAsync();

        if (await db.OrganizationUnits.AnyAsync())
            return;

        // =========================================
        // ROOT
        // =========================================

        var company = new OrganizationUnit
        {
            Name = "ЛЕОЛІНГ, Центр іноземних мов",
            Type = "Підприємство"
        };

        db.OrganizationUnits.Add(company);

        await db.SaveChangesAsync();

        // =========================================
        // STRUCTURAL UNITS
        // =========================================

        var office = new OrganizationUnit
        {
            Name = "Офісно-адміністративний комплекс",
            Type = "Адміністративний підрозділ",
            ParentId = company.Id
        };

        var education = new OrganizationUnit
        {
            Name = "Навчальний центр",
            Type = "Освітній підрозділ",
            ParentId = company.Id
        };

        var translation = new OrganizationUnit
        {
            Name = "Перекладацький відділ",
            Type = "Перекладацький підрозділ",
            ParentId = company.Id
        };

        var online = new OrganizationUnit
        {
            Name = "Центр дистанційного навчання",
            Type = "Дистанційний підрозділ",
            ParentId = company.Id
        };

        db.OrganizationUnits.AddRange(
            office,
            education,
            translation,
            online);

        await db.SaveChangesAsync();

        // =========================================
        // RESOURCES
        // =========================================

        var electricity = new Resource
        {
            Name = "Електрична енергія",
            Unit = "кВт·год"
        };

        var heat = new Resource
        {
            Name = "Теплова енергія",
            Unit = "Гкал"
        };

        var water = new Resource
        {
            Name = "Холодна вода",
            Unit = "тис. м³"
        };

        var wastewater = new Resource
        {
            Name = "Водовідведення",
            Unit = "тис. м³"
        };

        var waste = new Resource
        {
            Name = "Тверді побутові відходи",
            Unit = "т"
        };

        var oil = new Resource
        {
            Name = "Мастильні матеріали",
            Unit = "т"
        };

        db.Resources.AddRange(
            electricity,
            heat,
            water,
            wastewater,
            waste,
            oil);

        await db.SaveChangesAsync();

        // =========================================
        // RESOURCE LIMITS
        // =========================================

        AddLimit(db, office, electricity, 1500, 4000);
        AddLimit(db, office, heat, 20, 100);
        AddLimit(db, office, water, 0.01m, 0.05m);
        AddLimit(db, office, wastewater, 0.01m, 0.05m);
        AddLimit(db, office, waste, 0.05m, 0.20m);

        AddLimit(db, education, electricity, 3000, 8000);
        AddLimit(db, education, heat, 40, 150);
        AddLimit(db, education, water, 0.03m, 0.15m);
        AddLimit(db, education, wastewater, 0.03m, 0.15m);
        AddLimit(db, education, waste, 0.10m, 0.40m);

        AddLimit(db, translation, electricity, 1000, 3000);
        AddLimit(db, translation, heat, 15, 70);
        AddLimit(db, translation, wastewater, 0.01m, 0.04m);
        AddLimit(db, translation, waste, 0.03m, 0.15m);

        AddLimit(db, online, electricity, 500, 2500);
        AddLimit(db, online, heat, 10, 50);
        AddLimit(db, online, waste, 0.01m, 0.08m);

        await db.SaveChangesAsync();

        // =========================================
        // TARIFFS
        // =========================================

        db.Tariffs.AddRange(
            new Tariff
            {
                ResourceId = electricity.Id,
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2023, 5, 31),
                Price = 2.20m
            },
            new Tariff
            {
                ResourceId = electricity.Id,
                StartDate = new DateTime(2023, 6, 1),
                EndDate = new DateTime(2025, 12, 31),
                Price = 2.64m
            },
            new Tariff
            {
                ResourceId = heat.Id,
                StartDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2023, 9, 30),
                Price = 1400.88m
            },
            new Tariff
            {
                ResourceId = heat.Id,
                StartDate = new DateTime(2023, 10, 1),
                EndDate = new DateTime(2025, 12, 31),
                Price = 1550m
            }
        );

        await db.SaveChangesAsync();
    }

    private static void AddLimit(
        AppDbContext db,
        OrganizationUnit unit,
        Resource resource,
        decimal min,
        decimal max)
    {
        db.ResourceLimits.Add(
            new ResourceLimit
            {
                OrganizationUnitId = unit.Id,
                ResourceId = resource.Id,
                MinValue = min,
                MaxValue = max
            });
    }
}
