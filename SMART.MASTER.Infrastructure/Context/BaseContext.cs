using Microsoft.EntityFrameworkCore;
using SMART.MASTER.Domain.Entities;

namespace SMART.MASTER.Infrastructure.Context
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Client> Customers { get; set; } = null!;
        public DbSet<ClientGender> Genders { get; set; } = null!;
        public DbSet<ClientHeading> Headings { get; set; } = null!;
        public DbSet<ClientSocialReason> SocialReasons { get; set; } = null!;
        public DbSet<ClientCurrency> Currencies { get; set; } = null!;
        public DbSet<ClientType> CustomerTypes { get; set; } = null!;
        public DbSet<ClientCity> ClientCities { get; set; } = null!;
        public DbSet<ClientDepartment> ClientDepartments { get; set; } = null!;
        public DbSet<ClientCountry> ClientCountries { get; set; } = null!;
        public DbSet<DeliveryDirection> DeliveryDirections { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Customer
            modelBuilder.Entity<Client>().ToTable("Customer");
            modelBuilder.Entity<Client>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Client>()
                .HasOne(x => x.Heading)
                .WithMany()
                .HasForeignKey(x => x.HeadingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(x => x.SocialReason)
                .WithMany()
                .HasForeignKey(x => x.SocialReasonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(x => x.Gender)
                .WithMany()
                .HasForeignKey(x => x.GenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(x => x.CustomerType)
                .WithMany()
                .HasForeignKey(x => x.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Client>()
                .HasMany(x => x.DeliveryDirections)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            //Gender
            modelBuilder.Entity<ClientGender>().ToTable("Gender");
            modelBuilder.Entity<ClientGender>(o => o.HasKey(x => x.Id));

            //Heading
            modelBuilder.Entity<ClientHeading>().ToTable("Heading");
            modelBuilder.Entity<ClientHeading>(o => o.HasKey(x => x.Id));

            //SocialReason
            modelBuilder.Entity<ClientSocialReason>().ToTable("SocialReason");
            modelBuilder.Entity<ClientSocialReason>(o => o.HasKey(x => x.Id));

            //CustomerType
            modelBuilder.Entity<ClientType>().ToTable("CustomerType");
            modelBuilder.Entity<ClientType>(o => o.HasKey(x => x.Id));

            //Currency
            modelBuilder.Entity<ClientCurrency>().ToTable("Currency");
            modelBuilder.Entity<ClientCurrency>(o => o.HasKey(x => x.Id));

            //Delivery Direction
            modelBuilder.Entity<DeliveryDirection>().ToTable("DeliveryDirection");
            modelBuilder.Entity<DeliveryDirection>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<DeliveryDirection>()
                .HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            //Country
            modelBuilder.Entity<ClientCountry>().ToTable("Country");
            modelBuilder.Entity<ClientCountry>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<ClientCountry>()
                .HasMany(x => x.Departments)
                .WithOne()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            //Department
            modelBuilder.Entity<ClientDepartment>().ToTable("Department");
            modelBuilder.Entity<ClientDepartment>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<ClientDepartment>()
                .HasMany(x => x.Cities)
                .WithOne()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            //City
            modelBuilder.Entity<ClientCity>().ToTable("City");
            modelBuilder.Entity<ClientCity>(o => o.HasKey(x => x.Id));
        }
    }
}
