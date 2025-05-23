﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repository.Models;

public partial class CityScoutContext : DbContext
{
    public CityScoutContext()
    {
    }

    public CityScoutContext(DbContextOptions<CityScoutContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatHistory> ChatHistories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<OpeningHour> OpeningHours { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account__1788CCAC39E9A09A");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__Email").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Account__RoleID__4CA06362");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2B72299A3A");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(255)
                .HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__ChatHist__A9FBE62650BB9356");

            entity.ToTable("ChatHistory");

            entity.Property(e => e.ChatId).HasColumnName("ChatID");
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReceiverId)
                .HasMaxLength(255)
                .HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId)
                .HasMaxLength(255)
                .HasColumnName("SenderID");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ChatHistoryReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__ChatHisto__Recei__4D94879B");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatHistorySenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__ChatHisto__Sende__4E88ABD4");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__City__F2D21A961FD1405A");

            entity.ToTable("City");

            entity.Property(e => e.CityId)
                .HasMaxLength(255)
                .HasColumnName("CityID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__DB5FE4ACE791AF25");

            entity.ToTable("Destination");

            entity.Property(e => e.DestinationId)
                .HasMaxLength(255)
                .HasColumnName("DestinationID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CategoryId)
                .HasMaxLength(255)
                .HasColumnName("CategoryID");
            entity.Property(e => e.DestinationName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.DistrictId)
                .HasMaxLength(255)
                .HasColumnName("DistrictID");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Ward).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Destinations)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Destinati__Categ__4F7CD00D");

            entity.HasOne(d => d.District).WithMany(p => p.Destinations)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Destinati__Distr__5070F446");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DistrictId).HasName("PK__District__85FDA4A666C563E2");

            entity.ToTable("District");

            entity.Property(e => e.DistrictId)
                .HasMaxLength(255)
                .HasColumnName("DistrictID");
            entity.Property(e => e.CityId)
                .HasMaxLength(255)
                .HasColumnName("CityID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.City).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__District__CityID__5165187F");
        });

        modelBuilder.Entity<OpeningHour>(entity =>
        {
            entity.HasKey(e => new { e.DestinationId, e.DayOfWeek }).HasName("PK__OpeningH__0B52A4A105E00327");

            entity.Property(e => e.DestinationId)
                .HasMaxLength(255)
                .HasColumnName("DestinationID");
            entity.Property(e => e.DayOfWeek).HasMaxLength(15);
            entity.Property(e => e.IsClosed).HasDefaultValue(false);

            entity.HasOne(d => d.Destination).WithMany(p => p.OpeningHours)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__OpeningHo__Desti__52593CB8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3AF355183D");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__RoleName").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(255);
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoryId).HasName("PK__SubCateg__26BE5BF949A5C961");

            entity.ToTable("SubCategory");

            entity.Property(e => e.SubCategoryId)
                .HasMaxLength(255)
                .HasColumnName("SubCategoryID");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(255)
                .HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SubCatego__Categ__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}