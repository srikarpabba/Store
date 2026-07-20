using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "promotions",
                schema: "store",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    discount_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    starts_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ends_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_promotions", x => x.id);
                    table.ForeignKey(
                        name: "fk_promotions_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "store",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_promotions_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "store",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_CreatedById",
                schema: "store",
                table: "promotions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_CreatedOn",
                schema: "store",
                table: "promotions",
                column: "created_on_utc");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_IsDeleted",
                schema: "store",
                table: "promotions",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_IsDeleted_CreatedOn",
                schema: "store",
                table: "promotions",
                columns: new[] { "is_deleted", "created_on_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_promotions_brand_id",
                schema: "store",
                table: "promotions",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_promotions_product_id",
                schema: "store",
                table: "promotions",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promotions",
                schema: "store");
        }
    }
}
