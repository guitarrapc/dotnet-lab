using BlazorAppEF.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace BlazorAppEF.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TestType> TestType { get; set; }
        public DbSet<TestTable> TestTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // custom type conversion: https://docs.microsoft.com/ja-jp/ef/core/modeling/value-conversions
            // DateTimeOffset(clr) should map to DateTime(mysql).
            // offset is always 0.(UTC)
            var datetimeOffsetConverter = new ValueConverter<DateTimeOffset, DateTime>(datetimeoffset => datetimeoffset.DateTime, value => new DateTimeOffset(value, TimeSpan.Zero));
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.DatetimeOffset2)
                .HasConversion(datetimeOffsetConverter);

            modelBuilder
                .Entity<TestType>()
                .Property(e => e.String3)
                .HasColumnType("VARCHAR(255)");
            // do not
            //.HasColumnType("VARCHAR")
            //.HasMaxLength(255);

            // InvalidCastException: Unable to cast object of type 'System.SByte' to type 'System.Int16'.
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Bool)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Bool2)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Bool3)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Sbyte)
                .HasColumnType("SMALLINT(6)");
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Ushort)
                .HasColumnType("INT(11)");
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Uint)
                .HasColumnType("BIGINT(20)");
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Char)
                .HasColumnType("INT(11)");
        }
    }
}
