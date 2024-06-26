using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Models;

public partial class RealocationAppContext : DbContext
{
    public RealocationAppContext()
    {
    }

    public RealocationAppContext(DbContextOptions<RealocationAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<RelocationDetail> RelocationDetails { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskComment> TaskComments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCategory> UserCategories { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=DESKTOP-N2Q4FF9\\SQLEXPRESS;Database=RealocationApp;Trusted_Connection=True;Encrypt=false");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
        String connStr = config.GetConnectionString("DefaultConnectionString");
        optionsBuilder.UseSqlServer(connStr);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B2BFB19A2");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__10D1609F05F448EA");

            entity.Property(e => e.CountryName).HasMaxLength(100);
        });

        modelBuilder.Entity<RelocationDetail>(entity =>
        {
            entity.HasKey(e => e.RelocationId).HasName("PK__Relocati__5047ADD90DAAE5CD");

            entity.ToTable("RelocationDetail");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DestinationCountry).HasMaxLength(100);
            entity.Property(e => e.MoveDate).HasColumnType("date");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__7C6949B15361C1BE");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.Priority).HasMaxLength(20);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tasks__UserId__34C8D9D1");
        });

        modelBuilder.Entity<TaskComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__TaskComm__C3B4DFCA7213EFA6");

            entity.Property(e => e.Comment).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaskComme__TaskI__38996AB5");

            entity.HasOne(d => d.User).WithMany(p => p.TaskComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TaskComme__UserI__398D8EEE");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5168692E");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E434E6C1D7").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserCategory>(entity =>
        {
            entity.HasKey(e => e.UserCategoryId).HasName("PK__UserCate__2421C384F6C30681");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.UserCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserCateg__Categ__30F848ED");

            entity.HasOne(d => d.User).WithMany(p => p.UserCategories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserCateg__UserI__300424B4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
