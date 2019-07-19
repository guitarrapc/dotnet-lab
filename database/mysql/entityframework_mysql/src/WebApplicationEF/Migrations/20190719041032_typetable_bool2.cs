using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class typetable_bool2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Bool",
                table: "TestType",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "TinyInt(1)");

            migrationBuilder.AddColumn<byte>(
                name: "Bool2",
                table: "TestType",
                type: "TinyInt(1)",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bool2",
                table: "TestType");

            migrationBuilder.AlterColumn<byte>(
                name: "Bool",
                table: "TestType",
                type: "TinyInt(1)",
                nullable: false,
                oldClrType: typeof(short));
        }
    }
}
