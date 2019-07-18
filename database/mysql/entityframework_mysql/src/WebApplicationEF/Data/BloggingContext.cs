using Microsoft.EntityFrameworkCore;
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
        public int Id { get; set; }
        public sbyte Sbyte { get; set; }
        public byte Byte { get; set; }
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
        public string String { get; set; }
        public DateTime Datetime { get; set; }
        public DateTimeOffset DatetimeOffset { get; set; }
    }

    public class TestTable
    {
        [Key]
        public int Number { get; set; }
        public string Name { get; set; }
    }
}
