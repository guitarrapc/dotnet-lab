﻿// <auto-generated />
using System;
using BlazorAppEF.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlazorAppEF.Migrations
{
    [DbContext(typeof(BloggingContext))]
    [Migration("20200907193639_docker")]
    partial class docker
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("BlazorAppEF.Data.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("BlogId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("BlazorAppEF.Data.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BlogId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("PostId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("BlazorAppEF.Data.TestTable", b =>
                {
                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(50)");

                    b.Property<string>("Url")
                        .HasColumnType("VARCHAR(255)");

                    b.HasKey("Number");

                    b.ToTable("TestTable");
                });

            modelBuilder.Entity("BlazorAppEF.Data.TestType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<short>("Bool")
                        .HasColumnType("smallint");

                    b.Property<short>("Bool2")
                        .HasColumnType("TinyInt(1)");

                    b.Property<ulong>("Bool3")
                        .HasColumnType("BIT(1)");

                    b.Property<byte>("Byte")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte[]>("ByteArray")
                        .HasColumnType("varbinary(3000)")
                        .HasMaxLength(3000);

                    b.Property<int>("Char")
                        .HasColumnType("INT(11)");

                    b.Property<DateTime>("Datetime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("DatetimeOffset")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DatetimeOffset2")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("Decimal")
                        .HasColumnType("decimal(65,30)");

                    b.Property<double>("Double")
                        .HasColumnType("double");

                    b.Property<float>("Float")
                        .HasColumnType("float");

                    b.Property<int>("Int")
                        .HasColumnType("int");

                    b.Property<long>("Long")
                        .HasColumnType("bigint");

                    b.Property<short>("Sbyte")
                        .HasColumnType("SMALLINT(6)");

                    b.Property<short>("Short")
                        .HasColumnType("smallint");

                    b.Property<string>("String")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("String2")
                        .HasColumnType("VARCHAR(255)");

                    b.Property<string>("String3")
                        .HasColumnType("VARCHAR(255)");

                    b.Property<long>("Uint")
                        .HasColumnType("BIGINT(20)");

                    b.Property<long>("Ulong")
                        .HasColumnType("BigInt");

                    b.Property<int>("Ushort")
                        .HasColumnType("INT(11)");

                    b.HasKey("Id");

                    b.ToTable("TestType");
                });

            modelBuilder.Entity("BlazorAppEF.Data.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BlazorAppEF.Data.Post", b =>
                {
                    b.HasOne("BlazorAppEF.Data.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}