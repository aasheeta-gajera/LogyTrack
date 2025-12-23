//using Microsoft.EntityFrameworkCore;
//using logyTrack.Models;
//using System.Collections.Generic;
//using LogyTrackAPI.Models;

//namespace LogyTrackAPI.Data
//{
//    public class LogyTrackContext : DbContext
//    {
//        public LogyTrackContext(DbContextOptions<LogyTrackContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<User> Users { get; set; }
//    }
//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
//        {
//        }

//        public DbSet<Driver> Drivers { get; set; }
//        public DbSet<Vehicle> Vehicles { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Driver configuration
//            modelBuilder.Entity<Driver>()
//                .HasKey(d => d.DriverId);

//            modelBuilder.Entity<Driver>()
//                .HasIndex(d => d.PhoneNumber)
//                .IsUnique();

//            modelBuilder.Entity<Driver>()
//                .HasIndex(d => d.LicenseNumber)
//                .IsUnique();

//            modelBuilder.Entity<Driver>()
//                .Property(d => d.FullName)
//                .IsRequired()
//                .HasMaxLength(100);

//            // Vehicle configuration
//            modelBuilder.Entity<Vehicle>()
//                .HasKey(v => v.VehicleId);

//            modelBuilder.Entity<Vehicle>()
//                .HasIndex(v => v.VehicleNumber)
//                .IsUnique();

//            modelBuilder.Entity<Vehicle>()
//                .Property(v => v.VehicleNumber)
//                .IsRequired()
//                .HasMaxLength(50);

//            modelBuilder.Entity<Vehicle>()
//                .Property(v => v.Status)
//                .IsRequired()
//                .HasMaxLength(20);

//            // Relationship: One Driver to Many Vehicles
//            modelBuilder.Entity<Vehicle>()
//                .HasOne(v => v.Driver)
//                .WithMany(d => d.Vehicles)
//                .HasForeignKey(v => v.DriverId)
//                .OnDelete(DeleteBehavior.SetNull)
//                .IsRequired(false);

//            // Check constraint for Status
//            modelBuilder.Entity<Vehicle>()
//                .HasCheckConstraint("CK_Vehicle_Status",
//                    "Status IN ('Available', 'OnTrip', 'Maintenance')");
//        }
//    }
//}