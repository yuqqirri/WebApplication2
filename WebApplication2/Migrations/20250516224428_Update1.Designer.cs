﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication2.Data;

#nullable disable

namespace WebApplication2.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250516224428_Update1")]
    partial class Update1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication2.Models.Currency", b =>
                {
                    b.Property<int>("Currency_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Currency_id"));

                    b.Property<string>("Currency_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Currency_id");

                    b.HasIndex("Currency_name")
                        .IsUnique();

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("WebApplication2.Models.Rundown", b =>
                {
                    b.Property<int>("Rundown_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Rundown_id"));

                    b.Property<int>("Currency_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("Rundown_date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Rundown_value")
                        .HasColumnType("decimal(9, 4)");

                    b.HasKey("Rundown_id");

                    b.ToTable("Rundowns");
                });
#pragma warning restore 612, 618
        }
    }
}
