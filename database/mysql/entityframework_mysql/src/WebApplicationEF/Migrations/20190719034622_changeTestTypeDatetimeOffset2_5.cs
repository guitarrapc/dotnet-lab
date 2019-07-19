using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class changeTestTypeDatetimeOffset2_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ByteArray",
                table: "TestType",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldMaxLength: 5000,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ByteArray",
                table: "TestType",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldMaxLength: 3000,
                oldNullable: true);
        }
    }
}
