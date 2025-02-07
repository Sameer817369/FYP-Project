using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "ImgUrl", "Name", "Price", "Status", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 0, null, null, null, "Kimchi Pancakes", 0.0, 0, null },
                    { 2, 0, null, null, null, "Bibimbap", 0.0, 0, null },
                    { 3, 0, null, null, null, "Kimchi Fried Rice", 0.0, 0, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
