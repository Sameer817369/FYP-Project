using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class bookingToDBUpdatAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ArrivalDate",
                table: "Bookings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ArrivalTime",
                table: "Bookings",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "Bookings");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArrivalDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
