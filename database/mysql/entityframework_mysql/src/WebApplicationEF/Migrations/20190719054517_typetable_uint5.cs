using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class typetable_uint5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ushort",
                table: "TestType",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ushort",
                table: "TestType");
        }
    }
}
