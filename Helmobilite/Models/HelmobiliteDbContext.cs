using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.DependencyResolver;
using System.Reflection.Emit;

namespace Helmobilite.Models
{
    public class HelmobiliteDbContext : IdentityDbContext<ApplicationUser>
    {
        public HelmobiliteDbContext(DbContextOptions<HelmobiliteDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Member>().HasIndex(e => e.Matricule).IsUnique();

			builder.Entity<Client>()
				.HasOne(c => c.Address)
				.WithOne(a => a.Client)
				.HasForeignKey<Address>(a => a.ClientId);

			builder.Entity<Chauffeur>()
                .HasMany(c => c.Licenses)
                .WithOne(cl => cl.Chauffeur)
                .HasForeignKey(cl => cl.ChauffeurId);

			builder.Entity<Chauffeur>()
				.HasMany(c => c.Deliveries)
				.WithOne(d => d.Chauffeur)
				.HasForeignKey(d => d.ChauffeurId);

			builder.Entity<ChauffeurLicense>()
	            .HasKey(cl => new { cl.ChauffeurId, cl.License });

			builder.Entity<ChauffeurLicense>()
				.HasOne(cl => cl.Chauffeur)
                .WithMany(c => c.Licenses)
                .HasForeignKey(cl => cl.ChauffeurId);

			builder.Entity<ChauffeurLicense>()
                .Property(cl => cl.License)
	            .HasConversion<string>();

			builder.Entity<Delivery>()
				.HasOne(d => d.Client)
				.WithMany(c => c.Deliveries)
				.HasForeignKey(d => d.ClientId);

			builder.Entity<Delivery>()
				.HasOne(d => d.Chauffeur)
				.WithMany(c => c.Deliveries)
				.HasForeignKey(d => d.ChauffeurId);

			builder.Entity<Delivery>()
				.HasOne(d => d.Truck)
				.WithMany(t => t.Deliveries)
				.HasForeignKey(d => d.TruckId);

			builder.Entity<Truck>()
				.HasMany(t => t.Deliveries)
				.WithOne(d => d.Truck)
				.HasForeignKey(d => d.TruckId);

			base.OnModelCreating(builder);
        }

        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<Chauffeur> Chauffeurs { get; set; }
		public DbSet<Dispatcher> Dispatchers { get; set; }
		public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Member> Members { get; set; }
	}

}
