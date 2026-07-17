using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryGenderAndCategoryPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "photo_file_name",
                schema: "store",
                table: "categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "category_genders",
                schema: "store",
                columns: table => new
                {
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_genders", x => new { x.category_id, x.gender_id });
                    table.ForeignKey(
                        name: "fk_category_genders_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "store",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_category_genders_genders_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "store",
                        principalTable: "genders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_category_genders_gender_id",
                schema: "store",
                table: "category_genders",
                column: "gender_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "category_genders",
                schema: "store");

            migrationBuilder.DropColumn(
                name: "photo_file_name",
                schema: "store",
                table: "categories");
        }
    }
}
