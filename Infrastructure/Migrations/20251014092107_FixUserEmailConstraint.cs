using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class FixUserEmailConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa index cũ (unique toàn cột)
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Users_Email"";");

            // Tạo lại index unique có điều kiện (bỏ qua giá trị NULL)
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_Users_Email""
                ON ""Users"" (""Email"")
                WHERE ""Email"" IS NOT NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa lại index có điều kiện
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Users_Email"";");

            // Tạo lại index unique toàn cột (nếu rollback)
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_Users_Email""
                ON ""Users"" (""Email"");
            ");
        }
    }
}
