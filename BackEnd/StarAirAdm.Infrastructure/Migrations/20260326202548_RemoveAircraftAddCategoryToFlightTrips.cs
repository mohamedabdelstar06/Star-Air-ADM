using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarAirAdm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAircraftAddCategoryToFlightTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaveAssessments_Aircraft_AircraftId",
                table: "PaveAssessments");

            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropIndex(
                name: "IX_PaveAssessments_AircraftId",
                table: "PaveAssessments");

            migrationBuilder.DropColumn(
                name: "AircraftId",
                table: "PaveAssessments");

            migrationBuilder.AddColumn<string>(
                name: "AircraftRegistration",
                table: "PaveAssessments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlightTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PilotId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlightCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Arrival = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ImSafeAssessmentId = table.Column<int>(type: "int", nullable: true),
                    PaveAssessmentId = table.Column<int>(type: "int", nullable: true),
                    DecideSessionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightTrips_AspNetUsers_PilotId",
                        column: x => x.PilotId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightTrips_DecideSessions_DecideSessionId",
                        column: x => x.DecideSessionId,
                        principalTable: "DecideSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FlightTrips_ImSafeAssessments_ImSafeAssessmentId",
                        column: x => x.ImSafeAssessmentId,
                        principalTable: "ImSafeAssessments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FlightTrips_PaveAssessments_PaveAssessmentId",
                        column: x => x.PaveAssessmentId,
                        principalTable: "PaveAssessments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightTrips_DecideSessionId",
                table: "FlightTrips",
                column: "DecideSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTrips_ImSafeAssessmentId",
                table: "FlightTrips",
                column: "ImSafeAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTrips_PaveAssessmentId",
                table: "FlightTrips",
                column: "PaveAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTrips_PilotId",
                table: "FlightTrips",
                column: "PilotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightTrips");

            migrationBuilder.DropColumn(
                name: "AircraftRegistration",
                table: "PaveAssessments");

            migrationBuilder.AddColumn<int>(
                name: "AircraftId",
                table: "PaveAssessments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaveAssessments_AircraftId",
                table: "PaveAssessments",
                column: "AircraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaveAssessments_Aircraft_AircraftId",
                table: "PaveAssessments",
                column: "AircraftId",
                principalTable: "Aircraft",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
