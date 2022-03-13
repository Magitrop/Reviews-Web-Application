﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RazorCoursework.Data;

namespace RazorCoursework.Migrations
{
    [DbContext(typeof(AppContentDbContext))]
    partial class AppContentDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RazorCoursework.Data.Like", b =>
                {
                    b.Property<string>("LikeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ReviewID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LikeID");

                    b.HasIndex("ReviewID");

                    b.ToTable("ReviewLikes");
                });

            modelBuilder.Entity("RazorCoursework.Data.Rating", b =>
                {
                    b.Property<string>("RatingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("RatingValue")
                        .HasColumnType("int");

                    b.Property<string>("ReviewID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RatingID");

                    b.HasIndex("ReviewID");

                    b.ToTable("ReviewRatings");
                });

            modelBuilder.Entity("RazorCoursework.Data.Review", b =>
                {
                    b.Property<string>("ReviewID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AttachedPictureLinks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OwnerRating")
                        .HasColumnType("int");

                    b.Property<string>("ReviewCreatorID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewCreatorName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewSubjectGenre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewSubjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewText")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReviewID");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("RazorCoursework.Data.Tag", b =>
                {
                    b.Property<string>("TagID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TagName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TagID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("RazorCoursework.Data.UserPreferences", b =>
                {
                    b.Property<string>("PreferenceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDarkTheme")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEnglishVersion")
                        .HasColumnType("bit");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PreferenceID");

                    b.ToTable("UserPreferences");
                });

            modelBuilder.Entity("RazorCoursework.Data.UserReviewAndTagRelation", b =>
                {
                    b.Property<string>("RelationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ReviewID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TagID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RelationID");

                    b.HasIndex("ReviewID");

                    b.HasIndex("TagID");

                    b.ToTable("ReviewAndTagRelations");
                });

            modelBuilder.Entity("RazorCoursework.Data.Like", b =>
                {
                    b.HasOne("RazorCoursework.Data.Review", "Review")
                        .WithMany("Likes")
                        .HasForeignKey("ReviewID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Review");
                });

            modelBuilder.Entity("RazorCoursework.Data.Rating", b =>
                {
                    b.HasOne("RazorCoursework.Data.Review", "Review")
                        .WithMany("Ratings")
                        .HasForeignKey("ReviewID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Review");
                });

            modelBuilder.Entity("RazorCoursework.Data.UserReviewAndTagRelation", b =>
                {
                    b.HasOne("RazorCoursework.Data.Review", "Review")
                        .WithMany("TagRelations")
                        .HasForeignKey("ReviewID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RazorCoursework.Data.Tag", "Tag")
                        .WithMany("ReviewRelations")
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Review");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("RazorCoursework.Data.Review", b =>
                {
                    b.Navigation("Likes");

                    b.Navigation("Ratings");

                    b.Navigation("TagRelations");
                });

            modelBuilder.Entity("RazorCoursework.Data.Tag", b =>
                {
                    b.Navigation("ReviewRelations");
                });
#pragma warning restore 612, 618
        }
    }
}
