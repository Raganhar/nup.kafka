﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using nup.kafka.DatabaseStuff;

namespace nup.kafka.Migrations
{
    [DbContext(typeof(KafkaMysqlDbContext))]
    [Migration("20230118213723_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("nup.kafka.DatabaseStuff.KafkaMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("FinishedProcessingAtUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("OffSet")
                        .HasColumnType("bigint");

                    b.Property<int>("Partition")
                        .HasColumnType("int");

                    b.Property<string>("PartitionKey")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("ProcessedSuccefully")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ReasonText")
                        .HasMaxLength(2000)
                        .HasColumnType("varchar(2000)");

                    b.Property<DateTime>("RecievedCreatedAtUtc")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("PartitionKey");

                    b.HasIndex("RecievedCreatedAtUtc");

                    b.HasIndex("Topic");

                    b.HasIndex("Partition", "OffSet");

                    b.ToTable("KafkaEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
