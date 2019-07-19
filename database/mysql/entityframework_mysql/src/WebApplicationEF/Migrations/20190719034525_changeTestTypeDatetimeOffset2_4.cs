using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class changeTestTypeDatetimeOffset2_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Bool",
                table: "TestType",
                type: "TinyInt(1)",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<byte[]>(
                name: "ByteArray",
                table: "TestType",
                maxLength: 5000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByteArray",
                table: "TestType");

            migrationBuilder.AlterColumn<short>(
                name: "Bool",
                table: "TestType",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "TinyInt(1)");
        }
    }
}
