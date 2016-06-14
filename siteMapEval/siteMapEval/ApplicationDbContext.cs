using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using wreq.Models.Entities;

namespace wreq.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base()// base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Culture> Cultures { get; set; }
        public DbSet<SoilType> SoilTypes { get; set; }
        public DbSet<IrrigationType> IrrigationTypes { get; set; }
        public DbSet<Field> Fields { get; set; }
        //public DbSet<CropStateRecord> CropStateRecords { get; set; }
        public DbSet<WeatherRecord> WeatherRecords { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Irrigation> Irrigations { get; set; }
        public DbSet<WaterLimit> WaterLimits { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Field>().HasRequired(f => f.SoilType)
                .WithMany(s => s.Fields)
                .HasForeignKey(f => f.SoilTypeId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Field>().HasRequired(f => f.IrrigationType)
                .WithMany(s => s.Fields)
                .HasForeignKey(f => f.IrrigationTypeId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Crop>().HasRequired(f => f.Field)
                .WithMany(s => s.Crops)
                .HasForeignKey(f => f.FieldId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Crop>().HasRequired(f => f.Culture)
                .WithMany(s => s.Crops)
                .HasForeignKey(f => f.CultureId)
                .WillCascadeOnDelete(false);
        }
    }
}