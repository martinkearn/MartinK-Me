using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using EvangelistSiteWeb.Data;

namespace EvangelistSiteWeb.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20161223090613_ConferenceImageUrl")]
    partial class ConferenceImageUrl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EvangelistSiteWeb.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.Conference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("ConferenceTitle");

                    b.Property<DateTime>("Date");

                    b.Property<string>("ExternalUrl");

                    b.Property<string>("ImageUrl");

                    b.HasKey("Id");

                    b.ToTable("Conference");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ConferenceTalk", b =>
                {
                    b.Property<int>("TalkId");

                    b.Property<int>("ConferenceId");

                    b.HasKey("TalkId", "ConferenceId");

                    b.HasIndex("ConferenceId");

                    b.HasIndex("TalkId");

                    b.ToTable("ConferenceTalk");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("FAIconClass");

                    b.Property<string>("ShortUrl");

                    b.Property<string>("TargetUrl");

                    b.Property<string>("Title");

                    b.Property<bool>("VisibleOnSite");

                    b.HasKey("Id");

                    b.ToTable("Resource");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ResourceGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CssClass");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.Property<bool>("VisibleOnSite");

                    b.HasKey("Id");

                    b.ToTable("ResourceGroup");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ResourceResourceGroup", b =>
                {
                    b.Property<int>("ResourceGroupId");

                    b.Property<int>("ResourceId");

                    b.HasKey("ResourceGroupId", "ResourceId");

                    b.HasIndex("ResourceGroupId");

                    b.HasIndex("ResourceId");

                    b.ToTable("ResourceResourceGroup");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ResourceTalk", b =>
                {
                    b.Property<int>("TalkId");

                    b.Property<int>("ResourceId");

                    b.HasKey("TalkId", "ResourceId");

                    b.HasIndex("ResourceId");

                    b.HasIndex("TalkId");

                    b.ToTable("ResourceTalk");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.Talk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audience");

                    b.Property<string>("Description");

                    b.Property<string>("Technologies");

                    b.Property<string>("Time");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Talk");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ConferenceTalk", b =>
                {
                    b.HasOne("EvangelistSiteWeb.Models.Conference", "Conference")
                        .WithMany("ConferenceTalks")
                        .HasForeignKey("ConferenceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EvangelistSiteWeb.Models.Talk", "Talk")
                        .WithMany("ConferenceTalks")
                        .HasForeignKey("TalkId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ResourceResourceGroup", b =>
                {
                    b.HasOne("EvangelistSiteWeb.Models.ResourceGroup", "ResourceGroup")
                        .WithMany("ResourceResourceGroups")
                        .HasForeignKey("ResourceGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EvangelistSiteWeb.Models.Resource", "Resource")
                        .WithMany("ResourceResourceGroups")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EvangelistSiteWeb.Models.ResourceTalk", b =>
                {
                    b.HasOne("EvangelistSiteWeb.Models.Resource", "Resource")
                        .WithMany("ResourceTalks")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EvangelistSiteWeb.Models.Talk", "Talk")
                        .WithMany("ResourceTalks")
                        .HasForeignKey("TalkId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("EvangelistSiteWeb.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("EvangelistSiteWeb.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EvangelistSiteWeb.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
