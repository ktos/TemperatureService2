using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TemperatureService3.Migrations
{
    public partial class DbId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorValues_Sensors_SensorName",
                table: "SensorValues");

            migrationBuilder.DropIndex(
                name: "IX_SensorValues_SensorName",
                table: "SensorValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "SensorName",
                table: "SensorValues");

            migrationBuilder.AddColumn<int>(
                name: "SensorId",
                table: "SensorValues",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sensors",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Sensors",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SensorValues_SensorId",
                table: "SensorValues",
                column: "SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorValues_Sensors_SensorId",
                table: "SensorValues",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorValues_Sensors_SensorId",
                table: "SensorValues");

            migrationBuilder.DropIndex(
                name: "IX_SensorValues_SensorId",
                table: "SensorValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "SensorValues");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Sensors");

            migrationBuilder.AddColumn<string>(
                name: "SensorName",
                table: "SensorValues",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sensors",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SensorValues_SensorName",
                table: "SensorValues",
                column: "SensorName");

            migrationBuilder.AddForeignKey(
                name: "FK_SensorValues_Sensors_SensorName",
                table: "SensorValues",
                column: "SensorName",
                principalTable: "Sensors",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
