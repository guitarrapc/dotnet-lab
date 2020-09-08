using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppEF.Shared.Models
{
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
        // INT(11)
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        // SMALLINT(6)
        public sbyte Sbyte { get; set; }
        // TINYINT(4)
        public byte Byte { get; set; }
        // VARBINARY(3000)
        [MaxLength(3000)]
        public byte[] ByteArray { get; set; }
        // SMALLINT(6)
        public short Short { get; set; }
        // INT(11) UNSIGNED
        public ushort Ushort { get; set; }
        // INT(11)
        public int Int { get; set; }
        // BIGINT(20)
        public uint Uint { get; set; }
        // BIGINT(20)
        public long Long { get; set; }
        // BIGINT(20)
        // cannot map ulong to premitive, specify type.
        [Column(TypeName = "BigInt")]
        public ulong Ulong { get; set; }
        // FLOAT
        public float Float { get; set; }
        // DOUBLE
        public double Double { get; set; }
        // DECIMAL
        public decimal Decimal { get; set; }
        // SMALLINT(6)
        public bool Bool { get; set; }
        // TINYINT(1)
        [Column(TypeName = "TinyInt(1)")]
        public bool Bool2 { get; set; }
        // BIT(1)
        [Column(TypeName = "BIT(1)")]
        public bool Bool3 { get; set; }
        // INT(11)
        public char Char { get; set; }
        // TEXT
        public string String { get; set; }
        // VARCHAR(255)
        [Column(TypeName = "VARCHAR(255)")]
        public string String2 { get; set; }
        // VARCHAR
        public string String3 { get; set; }
        // DATETIME
        public DateTime Datetime { get; set; }
        // TIMESTAMP
        public DateTimeOffset DatetimeOffset { get; set; }
        // DATETIME
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
