﻿// <auto-generated />
using HabitHub_Backend;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HabitHub_Backend.Migrations
{
    [DbContext(typeof(HabitHubDbContext))]
    partial class HabitHubDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HabitHub_Backend.Models.Habit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Habits");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "I would like to work out more.",
                            ImageUrl = "https://th.bing.com/th/id/R.1c9c3d67b55baf0ed910b39c5f833d06?rik=QmmXTGBc19MRUQ&pid=ImgRaw&r=0",
                            Title = "Work out more",
                            UserId = 1
                        },
                        new
                        {
                            Id = 2,
                            Description = "I would like to get 8 hours of sleep a night.",
                            ImageUrl = "https://static0.reviewthisimages.com/wordpress/wp-content/uploads/2019/10/sleeping.jpg",
                            Title = "Sleep better",
                            UserId = 2
                        });
                });

            modelBuilder.Entity("HabitHub_Backend.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Easy"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Slightly difficult"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Challenging"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Extremely Challenging"
                        });
                });

            modelBuilder.Entity("HabitHub_Backend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Bio = "The greatest person you'll ever meet.",
                            Email = "riley@email.com",
                            ImageUrl = "https://th.bing.com/th/id/R.f08431063da214d8c07452cca215447f?rik=7gKQvCXgiLVQXw&pid=ImgRaw&r=0",
                            Name = "Riley Tullis",
                            PhoneNumber = "123-456-7890",
                            Uid = ""
                        },
                        new
                        {
                            Id = 2,
                            Bio = "Eh, he's ok.",
                            Email = "jovanni@email.com",
                            ImageUrl = "https://th.bing.com/th/id/R.e733fb390ae9a3c28ca2389bd2466be7?rik=tGXnQ7Yf6T1kBQ&pid=ImgRaw&r=0",
                            Name = "Jovanni Feliz",
                            PhoneNumber = "098-765-4321",
                            Uid = ""
                        });
                });

            modelBuilder.Entity("HabitTag", b =>
                {
                    b.Property<int>("HabitListId")
                        .HasColumnType("integer");

                    b.Property<int>("TagsId")
                        .HasColumnType("integer");

                    b.HasKey("HabitListId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("HabitTag");
                });

            modelBuilder.Entity("HabitHub_Backend.Models.Habit", b =>
                {
                    b.HasOne("HabitHub_Backend.Models.User", null)
                        .WithMany("HabitList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HabitTag", b =>
                {
                    b.HasOne("HabitHub_Backend.Models.Habit", null)
                        .WithMany()
                        .HasForeignKey("HabitListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HabitHub_Backend.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HabitHub_Backend.Models.User", b =>
                {
                    b.Navigation("HabitList");
                });
#pragma warning restore 612, 618
        }
    }
}
