using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProfitCalculator;

public partial class ProfitCalculatorDataBaseContext : DbContext
{
    public ProfitCalculatorDataBaseContext()
    {
    }

    public ProfitCalculatorDataBaseContext(DbContextOptions<ProfitCalculatorDataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#pragma warning disable CS1030 // Директива #warning
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source= DataBase\\\\\\\\ProfitCalculatorDataBase.db");
#pragma warning restore CS1030 // Директива #warning

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("Customer_id");
            entity.Property(e => e.CompanyName).HasColumnName("Company_name");
            entity.Property(e => e.Inn).HasColumnName("inn");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerId).HasColumnName("Customer_id");
            entity.Property(e => e.Data).HasColumnName("data");
            //entity.Property(e => e.EndDate)
            //    .HasColumnType("DATETIME")
            //    .HasColumnName("endDate");
            //entity.Property(e => e.StartDate)
            //    .HasColumnType("DATETIME")
            //    .HasColumnName("startDate");
            entity.Property(e => e.СontentsOfTransportation).HasColumnName("Сontents_of_transportation");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
