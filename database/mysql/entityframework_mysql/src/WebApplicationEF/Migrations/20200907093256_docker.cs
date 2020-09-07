using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class docker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    BlogId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.BlogId);
                });

            migrationBuilder.CreateTable(
                name: "TestTable",
                columns: table => new
                {
                    Number = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: true),
                    Url = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTable", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "TestType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Sbyte = table.Column<short>(type: "SMALLINT(6)", nullable: false),
                    Byte = table.Column<byte>(nullable: false),
                    ByteArray = table.Column<byte[]>(maxLength: 3000, nullable: true),
                    Short = table.Column<short>(nullable: false),
                    Ushort = table.Column<int>(type: "INT(11)", nullable: false),
                    Int = table.Column<int>(nullable: false),
                    Uint = table.Column<long>(type: "BIGINT(20)", nullable: false),
                    Long = table.Column<long>(nullable: false),
                    Ulong = table.Column<long>(type: "BigInt", nullable: false),
                    Float = table.Column<float>(nullable: false),
                    Double = table.Column<double>(nullable: false),
                    Decimal = table.Column<decimal>(nullable: false),
                    Bool = table.Column<short>(nullable: false),
                    Bool2 = table.Column<byte>(type: "TinyInt(1)", nullable: false),
                    Bool3 = table.Column<short>(type: "BIT(1)", nullable: false),
                    Char = table.Column<int>(type: "INT(11)", nullable: false),
                    String = table.Column<string>(nullable: true),
                    String2 = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    String3 = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    Datetime = table.Column<DateTime>(nullable: false),
                    DatetimeOffset = table.Column<DateTimeOffset>(nullable: false),
                    DatetimeOffset2 = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    BlogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogId",
                table: "Posts",
                column: "BlogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "TestTable");

            migrationBuilder.DropTable(
                name: "TestType");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Blogs");
        }
    }
}
