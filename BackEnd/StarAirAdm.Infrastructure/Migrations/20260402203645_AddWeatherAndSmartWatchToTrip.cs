using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarAirAdm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherAndSmartWatchToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlightTripId",
                table: "SmartWatchReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsManualEntry",
                table: "SmartWatchReadings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SmartWatchReadingId",
                table: "FlightTrips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherRiskLevel",
                table: "FlightTrips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherSummary",
                table: "FlightTrips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmartWatchReadings_FlightTripId",
                table: "SmartWatchReadings",
                column: "FlightTripId",
                unique: true,
                filter: "[FlightTripId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SmartWatchReadings_FlightTrips_FlightTripId",
                table: "SmartWatchReadings",
                column: "FlightTripId",
                principalTable: "FlightTrips",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmartWatchReadings_FlightTrips_FlightTripId",
                table: "SmartWatchReadings");

            migrationBuilder.DropIndex(
                name: "IX_SmartWatchReadings_FlightTripId",
                table: "SmartWatchReadings");

            migrationBuilder.DropColumn(
                name: "FlightTripId",
                table: "SmartWatchReadings");

            migrationBuilder.DropColumn(
                name: "IsManualEntry",
                table: "SmartWatchReadings");

            migrationBuilder.DropColumn(
                name: "SmartWatchReadingId",
                table: "FlightTrips");

            migrationBuilder.DropColumn(
                name: "WeatherRiskLevel",
                table: "FlightTrips");

            migrationBuilder.DropColumn(
                name: "WeatherSummary",
                table: "FlightTrips");
        }
    }
}
