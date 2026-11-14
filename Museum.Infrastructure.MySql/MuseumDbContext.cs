using Microsoft.EntityFrameworkCore;
using Museum.Domain.Entities;

namespace Museum.Infrastructure.MySql;

/// <summary>
/// Связывает сущности музея с таблицами MySQL.
/// </summary>
/// <param name="options">Параметры подключения к базе данных.</param>
public sealed class MuseumDbContext(DbContextOptions<MuseumDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Выставки музея.
    /// </summary>
    public DbSet<Exhibition> Exhibitions => Set<Exhibition>();

    /// <summary>
    /// Экскурсии музея.
    /// </summary>
    public DbSet<Excursion> Excursions => Set<Excursion>();

    /// <summary>
    /// Посетители музея.
    /// </summary>
    public DbSet<Visitor> Visitors => Set<Visitor>();

    /// <summary>
    /// Проданные билеты.
    /// </summary>
    public DbSet<Ticket> Tickets => Set<Ticket>();

    /// <summary>
    /// Состав выставок каждой экскурсии.
    /// </summary>
    public DbSet<ExcursionExhibition> ExcursionExhibitions => Set<ExcursionExhibition>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exhibition>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(item => item.Theme);
        });

        modelBuilder.Entity<Excursion>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Duration).HasConversion<long>();
            entity.HasIndex(item => item.Date);
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.FullName).IsRequired().HasMaxLength(200);
            entity.Property(item => item.Phone).IsRequired().HasMaxLength(32);
            entity.Property(item => item.BirthDate)
                .HasConversion(value => value.ToDateTime(TimeOnly.MinValue), value => DateOnly.FromDateTime(value))
                .HasColumnType("date");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Price).HasPrecision(18, 2);
            entity.HasOne(item => item.Excursion).WithMany(item => item.Tickets)
                .HasForeignKey(item => item.ExcursionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Visitor).WithMany(item => item.Tickets)
                .HasForeignKey(item => item.VisitorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ExcursionExhibition>(entity =>
        {
            entity.HasKey(item => new { item.ExcursionId, item.ExhibitionId });
            entity.HasOne(item => item.Excursion).WithMany(item => item.Exhibitions)
                .HasForeignKey(item => item.ExcursionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(item => item.Exhibition).WithMany(item => item.ExcursionExhibitions)
                .HasForeignKey(item => item.ExhibitionId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
