using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressApi.Migrations
{
    /// <inheritdoc />
    public partial class ResultsColumnInputAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InputAddresses_Street_City_Postcode_Region",
                table: "InputAddresses");

            migrationBuilder.CreateIndex(
                name: "IX_InputAddresses_Street_City_Postcode_Region_Result",
                table: "InputAddresses",
                columns: new[] { "Street", "City", "Postcode", "Region", "Result" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InputAddresses_Street_City_Postcode_Region_Result",
                table: "InputAddresses");

            migrationBuilder.CreateIndex(
                name: "IX_InputAddresses_Street_City_Postcode_Region",
                table: "InputAddresses",
                columns: new[] { "Street", "City", "Postcode", "Region" });
        }
    }
}
