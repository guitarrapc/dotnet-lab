using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationEF.Migrations
{
    public partial class changeTestTableColumntype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestTable",
                type: "VARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "TestTable",
                type: "VARCHAR(255)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "TestTable");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestTable",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldNullable: true);
        }
    }
}
