﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TemperatureService3.Data;

namespace TemperatureService3.Migrations
{
    [DbContext(typeof(SensorsDbContext))]
    [Migration("20191215185403_ChangeIdToInternalId")]
    partial class ChangeIdToInternalId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TemperatureService3.Models.Sensor", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("InternalId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("TemperatureService3.Models.SensorValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Data")
                        .HasColumnType("float");

                    b.Property<string>("SensorName")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("SensorName");

                    b.ToTable("SensorValues");
                });

            modelBuilder.Entity("TemperatureService3.Models.SensorValue", b =>
                {
                    b.HasOne("TemperatureService3.Models.Sensor", "Sensor")
                        .WithMany("Values")
                        .HasForeignKey("SensorName");
                });
#pragma warning restore 612, 618
        }
    }
}
