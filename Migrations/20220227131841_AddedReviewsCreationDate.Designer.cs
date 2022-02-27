﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RazorCoursework.Data;

namespace RazorCoursework.Migrations
{
    [DbContext(typeof(AppContentDbContext))]
    [Migration("20220227131841_AddedReviewsCreationDate")]
    partial class AddedReviewsCreationDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Coursework_Itransition.Data.Review", b =>
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

                    b.Property<string>("ReviewSubjectGenre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewSubjectName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewText")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReviewID");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Coursework_Itransition.Data.Tag", b =>
                {
                    b.Property<string>("TagID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TagName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TagID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Coursework_Itransition.Data.UserReviewAndTagRelation", b =>
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

            modelBuilder.Entity("Coursework_Itransition.Data.UserReviewAndTagRelation", b =>
                {
                    b.HasOne("Coursework_Itransition.Data.Review", "Review")
                        .WithMany("TagRelations")
                        .HasForeignKey("ReviewID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Coursework_Itransition.Data.Tag", "Tag")
                        .WithMany("ReviewRelations")
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Review");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Coursework_Itransition.Data.Review", b =>
                {
                    b.Navigation("TagRelations");
                });

            modelBuilder.Entity("Coursework_Itransition.Data.Tag", b =>
                {
                    b.Navigation("ReviewRelations");
                });
#pragma warning restore 612, 618
        }
    }
}
