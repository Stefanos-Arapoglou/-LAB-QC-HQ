using _LAB__QC_HQ.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace _LAB__QC_HQ.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //The following DbSets are commented out since they will be mapped by the builder from Application user
    //ITS WRONG!! Professor SAID TO AVOID DOING THIS!! keep Aspnet and ApplicationUser separate!!
    //not sure how to fix it now though..

    /*  public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }  */

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentDepartment> ContentDepartments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<KnowHowDetail> KnowHowDetails { get; set; }

    public virtual DbSet<KnowHowUser> KnowHowUsers { get; set; }

    public virtual DbSet<Legislation> Legislations { get; set; }

    public virtual DbSet<UserDepartment> UserDepartments { get; set; }

    /*    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=aspnet-_LAB__QC_HQ-0d3d813e-7c46-40f3-87f1-a1aec4576ba0;Trusted_Connection=True;");*/

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=aspnet-_LAB__QC_HQ-0d3d813e-7c46-40f3-87f1-a1aec4576ba0;Trusted_Connection=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call base FIRST to setup Identity relationships
        base.OnModelCreating(modelBuilder);

        // Tell EF Core these tables already exist
        modelBuilder.Entity<Content>().ToTable("Content", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<ContentDepartment>().ToTable("ContentDepartments", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Department>().ToTable("Departments", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Item>().ToTable("Items", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<KnowHowDetail>().ToTable("KnowHowDetails", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<KnowHowUser>().ToTable("KnowHowUsers", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<Legislation>().ToTable("Legislation", t => t.ExcludeFromMigrations());
        modelBuilder.Entity<UserDepartment>().ToTable("UserDepartments", t => t.ExcludeFromMigrations());



        // Remove ALL AspNet... entity configurations since they come from base

        // KEEP ONLY your custom entity configurations:
        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__Content__655FE510F55A53B5");

            entity.ToTable("Content");

            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.ContentType)
                .HasMaxLength(50)
                .HasColumnName("content_type");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TimesViewed).HasColumnName("times_viewed");
            entity.Property(e => e.Title).HasMaxLength(255);

            // IMPORTANT: Update this foreign key reference
            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Contents)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Content_Creator");

            entity.HasMany(d => d.Legislations).WithMany(p => p.Contents)
                .UsingEntity<Dictionary<string, object>>(
                    "ContentLegislation",
                    r => r.HasOne<Legislation>().WithMany()
                        .HasForeignKey("LegislationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ContentLegislation_Legislation"),
                    l => l.HasOne<Content>().WithMany()
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ContentLegislation_Content"),
                    j =>
                    {
                        j.HasKey("ContentId", "LegislationId");
                        j.ToTable("ContentLegislation");
                        j.IndexerProperty<int>("ContentId").HasColumnName("content_id");
                        j.IndexerProperty<int>("LegislationId").HasColumnName("legislation_id");
                    });
        });

        modelBuilder.Entity<ContentDepartment>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.DepartmentId });

            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.ClearanceLevelRequired).HasColumnName("Clearance_Level_Required");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentDepartments)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDepartments_Content");

            entity.HasOne(d => d.Department).WithMany(p => p.ContentDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentDepartments_Departments");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__C2232422EAC7C993");

            entity.HasIndex(e => e.Name, "UQ__Departme__737584F6B1641052").IsUnique();

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__52020FDDBECE9195");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.ItemType)
                .HasMaxLength(50)
                .HasColumnName("item_type");
            entity.Property(e => e.ItemValue).HasColumnName("item_value");
            //i added this for the ordering of items
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");

            entity.HasOne(d => d.Content).WithMany(p => p.Items)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Items_Content");
        });

        modelBuilder.Entity<KnowHowDetail>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__KnowHowD__655FE51027F1893A");

            entity.Property(e => e.ContentId)
                .ValueGeneratedNever()
                .HasColumnName("content_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RiskLevel).HasColumnName("risk_level");

            entity.HasOne(d => d.Content).WithOne(p => p.KnowHowDetail)
                .HasForeignKey<KnowHowDetail>(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KnowHowDetails_Content");
        });

        modelBuilder.Entity<KnowHowUser>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.UserId });

            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Role).HasMaxLength(100);

            entity.HasOne(d => d.Content).WithMany(p => p.KnowHowUsers)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KnowHowUsers_KnowHow");

            // IMPORTANT: Update this foreign key reference
            entity.HasOne(d => d.User).WithMany(p => p.KnowHowUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KnowHowUsers_Users");
        });

        modelBuilder.Entity<Legislation>(entity =>
        {
            entity.HasKey(e => e.LegislationId).HasName("PK__Legislat__A1EF0CB62EBF1802");

            entity.ToTable("Legislation");

            entity.Property(e => e.LegislationId).HasColumnName("legislation_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<UserDepartment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.DepartmentId });

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ClearanceLevel).HasColumnName("Clearance_Level");

            entity.HasOne(d => d.Department).WithMany(p => p.UserDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDepartments_Departments");

            // IMPORTANT: Update this foreign key reference
            entity.HasOne(d => d.User).WithMany(p => p.UserDepartments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserDepartments_Users");
        });

        // Configure ApplicationUser custom properties
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            // Map to existing AspNetUsers table
            // THIS IS THE PART THE PROFESSOR SAID TO AVOID!! 
            // Future me: Keep AspNetUsers and ApplicaitonUsers separate!
            // Now not sure how to fix issue without breaking Identity functionality..
            entity.ToTable("AspNetUsers");

            // Configure your custom properties
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired(false); // nullable for existing users

            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(e => e.JobTitle)
                .HasMaxLength(200)
                .IsRequired(false);

            entity.Property(e => e.HireDate)
                .IsRequired(false);

            entity.Property(e => e.ProfileImage)
                .IsRequired(false);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
