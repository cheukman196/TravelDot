
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.HotelManagement.Models;
using TravelGroupAssignment1.Areas.RoomManagement.Models;
using TravelGroupAssignment1.Areas.FlightManagement.Models;
using TravelGroupAssignment1.Areas.CustomerManagement.Models;
using TravelGroupAssignment1.Models;
namespace TravelGroupAssignment1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<CarRentalCompany> CarRentalCompanies { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarBooking> CarBookings { get; set; }
        public DbSet<RoomBooking> RoomBookings { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<FlightBooking> FlightBookings { get; set; }
        public DbSet<CarComment> CarComments { get; set; }
        public DbSet<RoomComment> RoomComments { get; set; }
        public DbSet<HotelComment> HotelComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Hotel>()
                .HasMany(r => r.Rooms)
                .WithOne(h => h.Hotel)
                .HasForeignKey(r => r.HotelId);

            modelBuilder.Entity<CarRentalCompany>()
                .HasMany(c => c.Cars)
                .WithOne(cr => cr.Company)
                .HasForeignKey(c => c.CompanyId);

            modelBuilder.Entity<Car>()
                .HasMany(c => c.Bookings)
                .WithOne(cb => cb.Car)
                .HasForeignKey(c => c.CarId);

            modelBuilder.Entity<Room>()
                .HasMany(rb => rb.RoomBookings)
                .WithOne(r => r.Room)
                .HasForeignKey(rb => rb.RoomId);

            modelBuilder.Entity<Car>()
                .HasMany(c => c.CarComments)
                .WithOne(cc => cc.Car)
                .HasForeignKey(c => c.CarId);

            modelBuilder.Entity<Room>()
                .HasMany(r => r.RoomComments)
                .WithOne(rc => rc.Room)
                .HasForeignKey(r => r.RoomId);

            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.HotelComments)
                .WithOne(hc => hc.Hotel)
                .HasForeignKey(h => h.HotelId);

            modelBuilder.Entity<Trip>().HasData(
                new Trip { TripId = 1 }
                 );
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
            }
                );
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            }
               );
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRoles");
            }
             );
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable(name: "UserClaims");
            }
            );
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable(name: "UserLogins");
            }
            );
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable(name: "RoleClaims");
            }
            );
        }

    }
}
