using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMiscorrectedToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MiscorrectedAddresses",
                table: "Metrics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiscorrectedAddresses",
                table: "Metrics");
        }
    }
}
