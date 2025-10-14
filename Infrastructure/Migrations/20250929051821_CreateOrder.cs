using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CreateOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Thêm cột tạm Role_temp (integer) với default 0
            migrationBuilder.AddColumn<int>(
                name: "Role_temp",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // 2) Copy dữ liệu từ Role (string) sang Role_temp có mapping an toàn:
            //    - trim + lower -> 'admin' => 0, 'customer' => 1
            //    - nếu value là chuỗi số (regex) thì cast sang integer
            //    - ngược lại -> default 0
            migrationBuilder.Sql(@"
                UPDATE ""Users"" SET ""Role_temp"" =
                  CASE
                    WHEN lower(trim(""Role"")) = 'admin' THEN 0
                    WHEN lower(trim(""Role"")) = 'customer' THEN 1
                    WHEN trim(""Role"") ~ '^[0-9]+$' THEN (trim(""Role""))::integer
                    ELSE 0
                  END;
            ");

            // 3) Xóa cột Role (text) cũ
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            // 4) Rename Role_temp -> Role
            migrationBuilder.RenameColumn(
                name: "Role_temp",
                table: "Users",
                newName: "Role");

            // (Bây giờ cột "Role" là integer, NOT NULL)

            // 5) Các cột mới bạn muốn thêm (giữ nguyên logic trước)
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "OrderDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: tạo cột string tạm, chuyển int -> text, xóa int, rename tạm -> Role

            // 1) Thêm cột tạm Role_old (string)
            migrationBuilder.AddColumn<string>(
                name: "Role_old",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // 2) Copy dữ liệu từ Role (int) -> Role_old (text) theo mapping
            migrationBuilder.Sql(@"
                UPDATE ""Users"" SET ""Role_old"" =
                  CASE
                    WHEN ""Role"" = 0 THEN 'Admin'
                    WHEN ""Role"" = 1 THEN 'Customer'
                    ELSE ''
                  END;
            ");

            // 3) Xóa cột Role (integer)
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            // 4) Rename Role_old -> Role
            migrationBuilder.RenameColumn(
                name: "Role_old",
                table: "Users",
                newName: "Role");

            // 5) Xóa các cột mới đã thêm trong Up
            migrationBuilder.DropColumn(name: "RefreshToken", table: "Users");
            migrationBuilder.DropColumn(name: "RejectReason", table: "Users");

            migrationBuilder.DropColumn(name: "Note", table: "Orders");
            migrationBuilder.DropColumn(name: "PaymentMethod", table: "Orders");
            migrationBuilder.DropColumn(name: "PhoneNumber", table: "Orders");
            migrationBuilder.DropColumn(name: "Status", table: "Orders");

            migrationBuilder.DropColumn(name: "UnitPrice", table: "OrderDetails");
        }
    }
}
