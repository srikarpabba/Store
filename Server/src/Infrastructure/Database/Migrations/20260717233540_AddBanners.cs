using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banners",
                schema: "store",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    storefront = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    link_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    image_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_banners", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banner_CreatedById",
                schema: "store",
                table: "banners",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_CreatedOn",
                schema: "store",
                table: "banners",
                column: "created_on_utc");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_IsDeleted",
                schema: "store",
                table: "banners",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_IsDeleted_CreatedOn",
                schema: "store",
                table: "banners",
                columns: new[] { "is_deleted", "created_on_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_banners_storefront_sort_order",
                schema: "store",
                table: "banners",
                columns: new[] { "storefront", "sort_order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banners",
                schema: "store");
        }
    }
}
