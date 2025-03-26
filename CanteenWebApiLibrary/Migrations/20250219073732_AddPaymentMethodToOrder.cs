using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenWebApiLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Commented out to avoid table recreation
            /*
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    categoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ParentCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3213E83F52BFCC76", x => x.id);
                    table.ForeignKey(
                        name: "FK__Categorie__Paren__4222D4EF",
                        column: x => x.ParentCategoryID,
                        principalTable: "Categories",
                        principalColumn: "id");
                });
            */

            // Only apply the necessary change
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Cash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove only the column added in this migration
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            // Do NOT drop entire tables!
            /*
            migrationBuilder.DropTable(name: "OrderItems");
            migrationBuilder.DropTable(name: "MenuItem");
            migrationBuilder.DropTable(name: "Orders");
            migrationBuilder.DropTable(name: "Categories");
            migrationBuilder.DropTable(name: "users");
            migrationBuilder.DropTable(name: "roles");
            */
        }
    }
}
