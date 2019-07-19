using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class typetable_uint3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Byte",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "ByteArray",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Int",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Sbyte",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Short",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Ushort",
                table: "TestType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "Int",
                table: "TestType",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "Long",
                table: "TestType",
                nullable: false,
                defaultValue: 0L);

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

            migrationBuilder.AddColumn<int>(
                name: "Ushort",
                table: "TestType",
                nullable: false,
                defaultValue: 0);
        }
    }
}
