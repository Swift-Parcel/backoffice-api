using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        // Core Tables
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CaseNote> CaseNotes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Handler> Handlers { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SlaRule> SlaRules { get; set; }
        public DbSet<StatusWorkflow> StatusWorkflows { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        private const string Citext = "citext";
        private const string EnumCaseStatus = "enum_case_status";
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension(Citext);
            
            modelBuilder.HasPostgresEnum<ParcelStatus>("enum_parcel_status");
            modelBuilder.HasPostgresEnum<Timeslot>("enum_timeslot");
            modelBuilder.HasPostgresEnum<ServiceType>("enum_service_type");
            modelBuilder.HasPostgresEnum<CaseType>("enum_case_type");
            modelBuilder.HasPostgresEnum<CaseStatus>(EnumCaseStatus);
            modelBuilder.HasPostgresEnum<Priority>("enum_priority");
            modelBuilder.HasPostgresEnum<Channel>("enum_channel");
            modelBuilder.HasPostgresEnum<DayOfWeek>("enum_day_of_week");
            modelBuilder.HasPostgresEnum<AuditAction>("enum_action");
            modelBuilder.HasPostgresEnum<EntityType>("enum_entity_type");
            
            
            modelBuilder.Entity<AuditLog>(b =>
            {
                b.Property(e => e.AuditAction).HasColumnType("enum_action");
                b.Property(e => e.EntityType).HasColumnType("enum_entity_type");
                b.Property(e => e.IpAddress).HasColumnType("inet");
            });

            modelBuilder.Entity<Case>(b =>
            {
                b.Property(e => e.CaseType).HasColumnType("enum_case_type");
                b.Property(e => e.Status).HasColumnType(EnumCaseStatus).HasDefaultValue(CaseStatus.Open);
                b.Property(e => e.Priority).HasColumnType("enum_priority").HasDefaultValue(Priority.Low);
                b.Property(e => e.Channel).HasColumnType("enum_channel");
                
                b.HasOne(e => e.Handler).WithMany(h => h.Cases).HasForeignKey(e => e.HandlerId);
            });

            modelBuilder.Entity<Country>(b => 
            {
                b.HasKey(e => e.CountryCode);
                b.Property(e => e.CountryCode).HasMaxLength(10);
            });

            modelBuilder.Entity<Customer>(b =>
            {
                b.Property(e => e.Email).HasColumnType(Citext);
                b.HasIndex(e => e.Email).IsUnique();
                b.Property(e => e.Vip).HasDefaultValue(false);
                b.ComplexProperty(c => c.Address, addressBuilder =>
                {
                    addressBuilder.Property(a => a.Street)
                        .HasMaxLength(200)
                        .HasColumnName("Address_Street");

                    addressBuilder.Property(a => a.StreetNumber)
                        .HasMaxLength(30)
                        .HasColumnName("Address_StreetNumber");

                    addressBuilder.Property(a => a.City)
                        .HasMaxLength(100)
                        .HasColumnName("Address_City");

                    addressBuilder.Property(a => a.PostalCode)
                        .HasMaxLength(20)
                        .HasColumnName("Address_PostalCode");

                    addressBuilder.Property(a => a.CountryCode)
                        .HasMaxLength(3)
                        .HasColumnName("Address_CountryCode");
                });
            });
            
            modelBuilder.Entity<Handler>(b =>
            {
                b.HasIndex(e => e.UserId).IsUnique(); 
                b.Property(e => e.MaxCases).HasDefaultValue(10);
            });

            modelBuilder.Entity<Parcel>(b =>
            {
                b.HasIndex(e => e.TrackingNumber).IsUnique();
                b.Property(e => e.Status).HasColumnType("enum_parcel_status").HasDefaultValue(ParcelStatus.PendingPickup);
                b.Property(e => e.ServiceType).HasColumnType("enum_service_type");
                b.ComplexProperty(p => p.RecipientAddress, addressBuilder =>
                {
                    addressBuilder.Property(a => a.Street)
                        .HasMaxLength(200)
                        .HasColumnName("Recipient_Street");

                    addressBuilder.Property(a => a.StreetNumber)
                        .HasMaxLength(30)
                        .HasColumnName("Recipient_StreetNumber");

                    addressBuilder.Property(a => a.City)
                        .HasMaxLength(100)
                        .HasColumnName("Recipient_City");

                    addressBuilder.Property(a => a.PostalCode)
                        .HasMaxLength(20)
                        .HasColumnName("Recipient_PostalCode");

                    addressBuilder.Property(a => a.CountryCode)
                        .HasMaxLength(3)
                        .HasColumnName("Recipient_CountryCode");
                });
            });

            modelBuilder.Entity<Permission>(b =>
            {
                b.HasIndex(e => e.Name).IsUnique();
                b.Property(e => e.Granular).HasDefaultValue(false);
            });

            modelBuilder.Entity<Region>(b =>
            {
                b.HasIndex(e => e.Name).IsUnique();
                b.Property(e => e.ManagerEmail).HasColumnType(Citext);
                b.Property(e => e.BusinessHoursStart).HasColumnType("time");
                b.Property(e => e.BusinessHoursEnd).HasColumnType("time");
                b.Property(e => e.BusinessDays).HasColumnType("enum_day_of_week[]");
                
                b.HasOne(r => r.Country)
                    .WithMany()
                    .HasForeignKey(r => r.CountryCode);
            });

            modelBuilder.Entity<Role>().HasIndex(e => e.RoleName).IsUnique();

            modelBuilder.Entity<SlaRule>(b =>
            {
                b.Property(e => e.CaseType).HasColumnType("enum_case_type");
                b.Property(e => e.Priority).HasColumnType("enum_priority");
                b.Property(e => e.ServiceType).HasColumnType("enum_service_type");
            });

            modelBuilder.Entity<StatusWorkflow>(b =>
            {
                b.Property(e => e.FromStatus).HasColumnType(EnumCaseStatus);
                b.Property(e => e.ToStatus).HasColumnType(EnumCaseStatus);
            });

            modelBuilder.Entity<SystemConfig>(b =>
            {
                b.HasIndex(e => e.ConfigKey).IsUnique();
                b.Property(e => e.ConfigValue).HasColumnType("jsonb"); 
            });

            modelBuilder.Entity<Tag>().HasIndex(e => e.Name).IsUnique();

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(e => e.Username).IsUnique();
                b.HasIndex(e => e.Email).IsUnique();
                b.Property(e => e.Email).HasColumnType(Citext);
                b.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
            });
        }
    }
}