using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TemperatureService2.Models;

namespace TemperatureService2
{
    public partial class TempdataDbContext : DbContext
    {
        public virtual DbSet<Tempdata> Tempdata { get; set; }

        public TempdataDbContext(DbContextOptions<TempdataDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tempdata>(entity =>
            {
                entity.HasKey(e => e.Timestamp);

                entity.ToTable("tempdata");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Sensor)
                    .IsRequired()
                    .HasColumnName("sensor")
                    .HasMaxLength(50);

                entity.Property(e => e.Value).HasColumnName("value");
            });
        }
    }
}
