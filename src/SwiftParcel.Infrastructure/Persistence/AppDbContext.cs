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
        public DbSet<Address> Addresses { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("citext");
            
            modelBuilder.HasPostgresEnum<ParcelStatus>("enum_parcel_status");
            modelBuilder.HasPostgresEnum<Timeslot>("enum_timeslot");
            modelBuilder.HasPostgresEnum<ServiceType>("enum_service_type");
            modelBuilder.HasPostgresEnum<CaseType>("enum_case_type");
            modelBuilder.HasPostgresEnum<CaseStatus>("enum_case_status");
            modelBuilder.HasPostgresEnum<Priority>("enum_priority");
            modelBuilder.HasPostgresEnum<Channel>("enum_channel");
            modelBuilder.HasPostgresEnum<DayOfWeek>("enum_day_of_week");
            modelBuilder.HasPostgresEnum<AuditAction>("enum_action");
            modelBuilder.HasPostgresEnum<EntityType>("enum_entity_type");

            modelBuilder.Entity<Address>(b =>
            {
                b.HasKey(a => a.Id);
                b.HasOne(a => a.Country).WithMany().HasForeignKey(a => a.CountryCode);
            });
            
            modelBuilder.Entity<AuditLog>(b =>
            {
                b.Property(e => e.AuditAction).HasColumnType("enum_action");
                b.Property(e => e.EntityType).HasColumnType("enum_entity_type");
                b.Property(e => e.IpAddress).HasColumnType("inet");
            });

            modelBuilder.Entity<Case>(b =>
            {
                b.Property(e => e.CaseType).HasColumnType("enum_case_type");
                b.Property(e => e.Status).HasColumnType("enum_case_status").HasDefaultValue(CaseStatus.Open);
                b.Property(e => e.Priority).HasColumnType("enum_priority").HasDefaultValue(Priority.Low);
                b.Property(e => e.Channel).HasColumnType("enum_channel");
                
                b.HasOne(e => e.Handler).WithMany(h => h.Cases).HasForeignKey(e => e.HandlerId);
            });
            
            modelBuilder.Entity<CaseNote>(b =>
            {
                b.HasKey(e => e.Id);
        
                b.HasOne(n => n.Handler)
                    .WithMany()
                    .HasForeignKey(n => n.HandlerId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(n => n.Customer)
                    .WithMany()
                    .HasForeignKey(n => n.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.ToTable(t => t.HasCheckConstraint(
                    "CK_CaseNote_Author", 
                    "(\"HandlerId\" IS NOT NULL AND \"CustomerId\" IS NULL) OR (\"HandlerId\" IS NULL AND \"CustomerId\" IS NOT NULL)"
                ));
            });

            modelBuilder.Entity<Country>(b => 
            {
                b.HasKey(e => e.CountryCode);
                b.Property(e => e.CountryCode).HasMaxLength(10);
            });

            modelBuilder.Entity<Customer>(b =>
            {
                b.Property(e => e.Email).HasColumnType("citext");
                b.HasIndex(e => e.Email).IsUnique();
                b.Property(e => e.Vip).HasDefaultValue(false);
                b.HasOne(c => c.Address).WithMany(a => a.Customers).HasForeignKey(c => c.AddressId);
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
                b.HasOne(p => p.RecipientAddress).WithMany(a => a.Parcels).HasForeignKey(p => p.RecipientAddressId);
            });

            modelBuilder.Entity<Permission>(b =>
            {
                b.HasIndex(e => e.Name).IsUnique();
                b.Property(e => e.Granular).HasDefaultValue(false);
            });

            modelBuilder.Entity<Region>(b =>
            {
                b.HasIndex(e => e.Name).IsUnique();
                b.Property(e => e.ManagerEmail).HasColumnType("citext");
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
                b.Property(e => e.FromStatus).HasColumnType("enum_case_status");
                b.Property(e => e.ToStatus).HasColumnType("enum_case_status");
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
                b.Property(e => e.Email).HasColumnType("citext");
                b.HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
            });
        }
    }
}