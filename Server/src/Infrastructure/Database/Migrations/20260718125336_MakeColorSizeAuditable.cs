using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeColorSizeAuditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "store",
                table: "sizes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on_utc",
                schema: "store",
                table: "sizes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_on_utc",
                schema: "store",
                table: "sizes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "store",
                table: "sizes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "modified_by",
                schema: "store",
                table: "sizes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_on_utc",
                schema: "store",
                table: "sizes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "store",
                table: "colors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on_utc",
                schema: "store",
                table: "colors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_on_utc",
                schema: "store",
                table: "colors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "store",
                table: "colors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "modified_by",
                schema: "store",
                table: "colors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_on_utc",
                schema: "store",
                table: "colors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Size_CreatedById",
                schema: "store",
                table: "sizes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Size_CreatedOn",
                schema: "store",
                table: "sizes",
                column: "created_on_utc");

            migrationBuilder.CreateIndex(
                name: "IX_Size_IsDeleted",
                schema: "store",
                table: "sizes",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Size_IsDeleted_CreatedOn",
                schema: "store",
                table: "sizes",
                columns: new[] { "is_deleted", "created_on_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_Color_CreatedById",
                schema: "store",
                table: "colors",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Color_CreatedOn",
                schema: "store",
                table: "colors",
                column: "created_on_utc");

            migrationBuilder.CreateIndex(
                name: "IX_Color_IsDeleted",
                schema: "store",
                table: "colors",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Color_IsDeleted_CreatedOn",
                schema: "store",
                table: "colors",
                columns: new[] { "is_deleted", "created_on_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Size_CreatedById",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropIndex(
                name: "IX_Size_CreatedOn",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropIndex(
                name: "IX_Size_IsDeleted",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropIndex(
                name: "IX_Size_IsDeleted_CreatedOn",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropIndex(
                name: "IX_Color_CreatedById",
                schema: "store",
                table: "colors");

            migrationBuilder.DropIndex(
                name: "IX_Color_CreatedOn",
                schema: "store",
                table: "colors");

            migrationBuilder.DropIndex(
                name: "IX_Color_IsDeleted",
                schema: "store",
                table: "colors");

            migrationBuilder.DropIndex(
                name: "IX_Color_IsDeleted_CreatedOn",
                schema: "store",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropColumn(
                name: "created_on_utc",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropColumn(
                name: "deleted_on_utc",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropColumn(
                name: "modified_by",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropColumn(
                name: "modified_on_utc",
                schema: "store",
                table: "sizes");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "store",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "created_on_utc",
                schema: "store",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "deleted_on_utc",
                schema: "store",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "store",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "modified_by",
                schema: "store",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "modified_on_utc",
                schema: "store",
                table: "colors");
        }
    }
}
