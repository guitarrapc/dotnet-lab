using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class typetable_uint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uint",
                table: "TestType");

            migrationBuilder.AlterColumn<short>(
                name: "Sbyte",
                table: "TestType",
                type: "SMALLINT(6)",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<short>(
                name: "Bool",
                table: "TestType",
                nullable: false,
                oldClrType: typeof(short));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Sbyte",
                table: "TestType",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT(6)");

            migrationBuilder.AlterColumn<short>(
                name: "Bool",
                table: "TestType",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<long>(
                name: "Uint",
                table: "TestType",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
