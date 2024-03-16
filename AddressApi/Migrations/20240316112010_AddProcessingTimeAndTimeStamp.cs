using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddressApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessingTimeAndTimeStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnName",
                table: "TableName");

            migrationBuilder.DropColumn(
                name: "ColumnName",
                table: "TableName");

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "InputAddresses",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ProcessingTime",
                table: "InputAddresses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "InputAddresses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "InputAddresses");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "InputAddresses",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ProcessingTime",
                table: "InputAddresses",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
