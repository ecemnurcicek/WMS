using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class TransferStatusIntMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "Transfers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transfers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transfers");

            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "Transfers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
