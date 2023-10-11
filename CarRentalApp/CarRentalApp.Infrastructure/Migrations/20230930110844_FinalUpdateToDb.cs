using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalApp.Infrastructure.Migrations
{
    public partial class FinalUpdateToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalCost",
                table: "Rents",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalCost",
                table: "Rents",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
