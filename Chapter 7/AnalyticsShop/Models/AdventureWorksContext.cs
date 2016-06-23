namespace AnalyticsShop.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AdventureWorksContext : DbContext
    {
        public AdventureWorksContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<vGetAllCategories> vGetAllCategories { get; set; }
        public virtual DbSet<vProductAndDescription> vProductAndDescription { get; set; }
        public virtual DbSet<vProductModelCatalogDescription> vProductModelCatalogDescription { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<vProductAndDescription>()
                .Property(e => e.Culture)
                .IsFixedLength();
        }
    }
}
