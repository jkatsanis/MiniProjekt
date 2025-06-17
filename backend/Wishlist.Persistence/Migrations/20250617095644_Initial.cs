using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Wishlist.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Wishlist");

            migrationBuilder.CreateTable(
                name: "wishing_list",
                schema: "Wishlist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wishing_list", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wishlist_item",
                schema: "Wishlist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_done = table.Column<bool>(type: "boolean", nullable: false),
                    wishlist_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wishlist_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_wishlist_item_wishing_list_wishlist_id",
                        column: x => x.wishlist_id,
                        principalSchema: "Wishlist",
                        principalTable: "wishing_list",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_wishlist_item_wishlist_id",
                schema: "Wishlist",
                table: "wishlist_item",
                column: "wishlist_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wishlist_item",
                schema: "Wishlist");

            migrationBuilder.DropTable(
                name: "wishing_list",
                schema: "Wishlist");
        }
    }
}
