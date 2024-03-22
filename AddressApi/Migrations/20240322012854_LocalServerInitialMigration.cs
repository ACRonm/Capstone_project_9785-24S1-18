using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressApi.Migrations
{
    /// <inheritdoc />
    public partial class LocalServerInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //drop all tables
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "InputAddresses");

            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Postcode = table.Column<int>(type: "int", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Accuracy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InputAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Postcode = table.Column<int>(type: "int", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: true),
                    ProcessingTime = table.Column<long>(type: "bigint", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalAddresses = table.Column<int>(type: "int", nullable: false),
                    CorrectedAddresses = table.Column<int>(type: "int", nullable: false),
                    FailedAddresses = table.Column<int>(type: "int", nullable: false),
                    MiscorrectedAddresses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Street_City_Postcode_Region",
                table: "Addresses",
                columns: new[] { "Street", "City", "Postcode", "Region" });

            migrationBuilder.CreateIndex(
                name: "IX_InputAddresses_Street_City_Postcode_Region_Result",
                table: "InputAddresses",
                columns: new[] { "Street", "City", "Postcode", "Region", "Result" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "InputAddresses");

            migrationBuilder.DropTable(
                name: "Metrics");
        }
    }
}
