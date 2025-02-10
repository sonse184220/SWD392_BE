using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<ChatHistory> ChatHistories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<OpeningHour> OpeningHours { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=swd392.database.windows.net;Database=CityScout;User Id=thien;Password=Swd2102025;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account__1788CCAC0A2A0CBA");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__A9D105347232A0DE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Account__RoleID__60A75C0F");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__54379E50193625BF");

            entity.ToTable("Blog");

            entity.Property(e => e.BlogId).HasColumnName("BlogID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DestinationId).HasColumnName("DestinationID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Destination).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__Blog__Destinatio__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Blog__UserID__6E01572D");
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__ChatHist__A9FBE626D2B9EBB2");

            entity.ToTable("ChatHistory");

            entity.Property(e => e.ChatId).HasColumnName("ChatID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ChatHistoryReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__ChatHisto__Recei__7A672E12");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatHistorySenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__ChatHisto__Sende__797309D9");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contact__5C6625BB616615C9");

            entity.ToTable("Contact");

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.DestinationId).HasColumnName("DestinationID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.Destination).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__Contact__Destina__656C112C");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__DB5FE4ACFB00342D");

            entity.ToTable("Destination");

            entity.Property(e => e.DestinationId).HasColumnName("DestinationID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.DestinationName).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C870DBEF416C");

            entity.ToTable("Event");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.DestinationId).HasColumnName("DestinationID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventName).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Destination).WithMany(p => p.Events)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__Event__Destinati__75A278F5");
        });

        modelBuilder.Entity<OpeningHour>(entity =>
        {
            entity.HasKey(e => e.OpeningId).HasName("PK__OpeningH__808F8713FB7A33FC");

            entity.Property(e => e.OpeningId).HasColumnName("OpeningID");
            entity.Property(e => e.DayOfWeek).HasMaxLength(15);
            entity.Property(e => e.DestinationId).HasColumnName("DestinationID");
            entity.Property(e => e.IsClosed).HasDefaultValue(false);

            entity.HasOne(d => d.Destination).WithMany(p => p.OpeningHours)
                .HasForeignKey(d => d.DestinationId)
                .HasConstraintName("FK__OpeningHo__Desti__693CA210");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Review__74BC79AE7BB3F2BF");

            entity.ToTable("Review");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Review__UserID__72C60C4A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3AB97217E9");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
