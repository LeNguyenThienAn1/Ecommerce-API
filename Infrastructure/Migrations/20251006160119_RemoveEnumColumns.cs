using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RemoveEnumColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Xóa constraint và cột cũ nếu còn
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.table_constraints
        WHERE constraint_name = 'FK_Products_Categories_CategoryEntityId'
    ) THEN
        ALTER TABLE ""Products"" DROP CONSTRAINT ""FK_Products_Categories_CategoryEntityId"";
    END IF;

    IF EXISTS (
        SELECT 1 FROM pg_indexes WHERE indexname = 'IX_Products_CategoryEntityId'
    ) THEN
        DROP INDEX ""IX_Products_CategoryEntityId"";
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_name = 'Products' AND column_name = 'Brand') THEN
        ALTER TABLE ""Products"" DROP COLUMN ""Brand"";
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_name = 'Products' AND column_name = 'Category') THEN
        ALTER TABLE ""Products"" DROP COLUMN ""Category"";
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns 
               WHERE table_name = 'Products' AND column_name = 'CategoryEntityId') THEN
        ALTER TABLE ""Products"" DROP COLUMN ""CategoryEntityId"";
    END IF;
END $$;
");

            // 2️⃣ Tạo bảng Brands (phải có trước khi add FK)
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateBy = table.Column<string>(type: "text", nullable: false),
                    UpdateBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            // 3️⃣ Thêm cột mới vào Products
            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Products",
                type: "uuid",
                nullable: true);

            // 4️⃣ Tạo bảng Wishlists (sau khi Products đã có Id hợp lệ)
            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateBy = table.Column<string>(type: "text", nullable: false),
                    UpdateBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 5️⃣ Tạo index
            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ProductId",
                table: "Wishlists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId");

            // 6️⃣ Thêm lại các khóa ngoại mới
            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa khóa ngoại
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            // Xóa bảng phụ
            migrationBuilder.DropTable(name: "Brands");
            migrationBuilder.DropTable(name: "Wishlists");

            // Xóa index
            migrationBuilder.DropIndex(name: "IX_Products_BrandId", table: "Products");
            migrationBuilder.DropIndex(name: "IX_Products_CategoryId", table: "Products");

            // Xóa cột
            migrationBuilder.DropColumn(name: "BrandId", table: "Products");
            migrationBuilder.DropColumn(name: "CategoryId", table: "Products");

            // Thêm lại enum cũ (nếu rollback)
            migrationBuilder.AddColumn<int>(
                name: "Brand",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryEntityId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryEntityId",
                table: "Products",
                column: "CategoryEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryEntityId",
                table: "Products",
                column: "CategoryEntityId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
