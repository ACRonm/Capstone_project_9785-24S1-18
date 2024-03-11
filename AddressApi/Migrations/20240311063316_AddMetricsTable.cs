using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMetricsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ProcessingTime",
                table: "InputAddresses",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "InputAddresses",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalAddresses = table.Column<int>(type: "int", nullable: false),
                    CorrectedAddresses = table.Column<int>(type: "int", nullable: false),
                    FailedAddresses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropColumn(
                name: "ProcessingTime",
                table: "InputAddresses");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "InputAddresses");
        }
    }
}
