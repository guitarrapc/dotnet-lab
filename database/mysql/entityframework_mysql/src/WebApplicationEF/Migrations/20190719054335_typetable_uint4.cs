using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class typetable_uint4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Byte",
                table: "TestType",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte[]>(
                name: "ByteArray",
                table: "TestType",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Sbyte",
                table: "TestType",
                type: "SMALLINT(6)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Short",
                table: "TestType",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Byte",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "ByteArray",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Sbyte",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Short",
                table: "TestType");
        }
    }
}
