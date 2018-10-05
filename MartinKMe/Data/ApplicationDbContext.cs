using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MartinKMe.Models;

namespace MartinKMe.Data
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

            //see this https://docs.efproject.net/en/latest/modeling/relationships.html for guidance on creating many-to-many EF relationships

            //Conference <> Talk many:many mapping table
            builder.Entity<ConferenceTalk>()
                .HasKey(t => new { t.TalkId, t.ConferenceId });

            builder.Entity<ConferenceTalk>()
                .HasOne(r => r.Conference)
                .WithMany(rt => rt.ConferenceTalks)
                .HasForeignKey(r => r.ConferenceId);
        }

        public DbSet<Resource> Resource { get; set; }

        public DbSet<Talk> Talk { get; set; }

        public DbSet<Conference> Conference { get; set; }

        public DbSet<ConferenceTalk> ConferenceTalk { get; set; }
    }
}
