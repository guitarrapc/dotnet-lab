using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationEF.Data
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
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
    }

    public class TestType
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        public sbyte Sbyte { get; set; }
        public byte Byte { get; set; }
        [MaxLength(3000)]
        public byte[] ByteArray { get; set; }
        public short Short { get; set; }
        public ushort Ushort { get; set; }
        public int Int { get; set; }
        public uint Uint { get; set; }
        public long Long { get; set; }
        // cannot map ulong to premitive, specify type.
        [Column(TypeName = "BigInt")]
        public ulong Ulong { get; set; }
        public float Float { get; set; }
        public double Double { get; set; }
        public bool Bool { get; set; }
        [Column(TypeName = "TinyInt(1)")]
        public bool Bool2 { get; set; }
        public string String { get; set; }
        [Column(TypeName = "VARCHAR(255)")]
        public string String2 { get; set; }
        public string String3 { get; set; }
        public DateTime Datetime { get; set; }
        public DateTimeOffset DatetimeOffset { get; set; }
        public DateTimeOffset DatetimeOffset2 { get; set; }
    }

    public class TestTable
    {
        [Key]
        public int Number { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string Name { get; set; }
        [Column(TypeName = "VARCHAR(255)")]
        public string Url { get; set; }
    }
}
