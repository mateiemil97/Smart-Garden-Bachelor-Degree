﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Smart_garden.Entites;

namespace Smart_garden.Migrations
{
    [DbContext(typeof(SmartGardenContext))]
    partial class SmartGardenContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Smart_garden.Entites.BoardsKeys", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Registered");

                    b.Property<string>("SeriesKey")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("BoardKey");
                });

            modelBuilder.Entity("Smart_garden.Entites.IrigationSystem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BoardKeyId");

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("BoardKeyId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("IrigationSystem");
                });

            modelBuilder.Entity("Smart_garden.Entites.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("SensorId");

                    b.Property<float>("Value");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("Measurement");
                });

            modelBuilder.Entity("Smart_garden.Entites.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Manual");

                    b.Property<DateTime>("Start");

                    b.Property<DateTime>("Stop");

                    b.Property<int>("SystemId");

                    b.Property<float>("TemperatureMax");

                    b.Property<float>("TemperatureMin");

                    b.HasKey("Id");

                    b.HasIndex("SystemId");

                    b.ToTable("Schedule");
                });

            modelBuilder.Entity("Smart_garden.Entites.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PortId");

                    b.Property<int>("SystemId");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("PortId");

                    b.HasIndex("SystemId");

                    b.ToTable("Sensor");
                });

            modelBuilder.Entity("Smart_garden.Entites.SensorPort", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Port")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("SensorPort");
                });

            modelBuilder.Entity("Smart_garden.Entites.SystemState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("SystemId");

                    b.Property<bool>("Working");

                    b.HasKey("Id");

                    b.HasIndex("SystemId");

                    b.ToTable("SystemState");
                });

            modelBuilder.Entity("Smart_garden.Entites.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(70);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("SecurityStamp");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Smart_garden.Entites.Zone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("MoistureStart");

                    b.Property<int>("MoistureStop");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("SensorId");

                    b.Property<bool>("WaterSwitch");

                    b.HasKey("Id");

                    b.HasIndex("SensorId")
                        .IsUnique();

                    b.ToTable("Zone");
                });

            modelBuilder.Entity("Smart_garden.Entites.IrigationSystem", b =>
                {
                    b.HasOne("Smart_garden.Entites.BoardsKeys", "BoardKey")
                        .WithOne("IrigationSystem")
                        .HasForeignKey("Smart_garden.Entites.IrigationSystem", "BoardKeyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Smart_garden.Entites.User", "User")
                        .WithMany("System")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Smart_garden.Entites.Measurement", b =>
                {
                    b.HasOne("Smart_garden.Entites.Sensor", "Sensor")
                        .WithMany("Measurement")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Smart_garden.Entites.Schedule", b =>
                {
                    b.HasOne("Smart_garden.Entites.IrigationSystem", "System")
                        .WithMany()
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Smart_garden.Entites.Sensor", b =>
                {
                    b.HasOne("Smart_garden.Entites.SensorPort", "SensorPort")
                        .WithMany("Sensor")
                        .HasForeignKey("PortId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Smart_garden.Entites.IrigationSystem", "System")
                        .WithMany("Sensors")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Smart_garden.Entites.SystemState", b =>
                {
                    b.HasOne("Smart_garden.Entites.IrigationSystem", "System")
                        .WithMany()
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Smart_garden.Entites.Zone", b =>
                {
                    b.HasOne("Smart_garden.Entites.Sensor", "Sensor")
                        .WithOne("Zone")
                        .HasForeignKey("Smart_garden.Entites.Zone", "SensorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
