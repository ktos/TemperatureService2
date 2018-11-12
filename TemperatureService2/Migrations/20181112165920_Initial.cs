using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TemperatureService2.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    InternalId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SensorValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SensorName = table.Column<string>(nullable: true),
                    Data = table.Column<float>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorValues_Sensors_SensorName",
                        column: x => x.SensorName,
                        principalTable: "Sensors",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorValues_SensorName",
                table: "SensorValues",
                column: "SensorName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorValues");

            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
