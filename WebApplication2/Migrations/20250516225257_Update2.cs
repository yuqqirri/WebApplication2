using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rundowns_Currency_id",
                table: "Rundowns",
                column: "Currency_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rundowns_Currencies_Currency_id",
                table: "Rundowns",
                column: "Currency_id",
                principalTable: "Currencies",
                principalColumn: "Currency_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rundowns_Currencies_Currency_id",
                table: "Rundowns");

            migrationBuilder.DropIndex(
                name: "IX_Rundowns_Currency_id",
                table: "Rundowns");
        }
    }
}
