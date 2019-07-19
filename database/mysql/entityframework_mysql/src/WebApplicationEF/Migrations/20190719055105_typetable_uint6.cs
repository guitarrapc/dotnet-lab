using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class typetable_uint6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Ushort",
                table: "TestType",
                type: "INT(11)",
                nullable: false,
                oldClrType: typeof(int));

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

            migrationBuilder.AddColumn<long>(
                name: "Uint",
                table: "TestType",
                type: "BIGINT(20)",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Int",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Long",
                table: "TestType");

            migrationBuilder.DropColumn(
                name: "Uint",
                table: "TestType");

            migrationBuilder.AlterColumn<int>(
                name: "Ushort",
                table: "TestType",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INT(11)");
        }
    }
}
