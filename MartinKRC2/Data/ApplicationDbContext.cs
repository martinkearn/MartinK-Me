using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MartinKRC2.Models;

namespace MartinKRC2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ResourceResourceGroup>()
                .HasKey(t => new { t.ResourceGroupId, t.ResourceId });

            builder.Entity<ResourceResourceGroup>()
                .HasOne(rg => rg.ResourceGroup)
                .WithMany(r => r.ResourceResourceGroups)
                .HasForeignKey(rg => rg.ResourceGroupId);

            builder.Entity<ResourceResourceGroup>()
                .HasOne(rg => rg.Resource)
                .WithMany(r => r.ResourceResourceGroups)
                .HasForeignKey(rg => rg.ResourceId);
        }

        public DbSet<ResourceGroup> ResourceGroup { get; set; }

        public DbSet<Resource> Resource { get; set; }

        public DbSet<Talk> Talk { get; set; }

        public DbSet<ResourceResourceGroup> ResourceResourceGroup { get; set; }
    }
}
