﻿// <auto-generated />
using System;
using DahuaTracker.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DahuaTracker.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.27");

            modelBuilder.Entity("DahuaTracker.CameraCredentials", b =>
                {
                    b.Property<int>("Mode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ip")
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Port")
                        .HasColumnType("TEXT");

                    b.HasKey("Mode");

                    b.ToTable("CameraCredentials");
                });

            modelBuilder.Entity("DahuaTracker.EventInfo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Count")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Index")
                        .HasColumnType("TEXT");

                    b.Property<string>("LaneNumber")
                        .HasColumnType("TEXT");

                    b.Property<int>("Mode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlateColor")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("PlateType")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<string>("VehicleColor")
                        .HasColumnType("TEXT");

                    b.Property<string>("VehicleSize")
                        .HasColumnType("TEXT");

                    b.Property<string>("VehicleType")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EventInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
