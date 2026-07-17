using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReworkCategoryGenderPhotoAndBrandLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photo_file_name",
                schema: "store",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "logo_url",
                schema: "store",
                table: "brands");

            migrationBuilder.AddColumn<string>(
                name: "photo_file_name",
                schema: "store",
                table: "category_genders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_file_name",
                schema: "store",
                table: "brands",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photo_file_name",
                schema: "store",
                table: "category_genders");

            migrationBuilder.DropColumn(
                name: "logo_file_name",
                schema: "store",
                table: "brands");

            migrationBuilder.AddColumn<string>(
                name: "photo_file_name",
                schema: "store",
                table: "categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                schema: "store",
                table: "brands",
                type: "text",
                nullable: true);
        }
    }
}
