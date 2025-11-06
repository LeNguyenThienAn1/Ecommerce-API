using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchemaAfterManualChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---- USERS ----
            migrationBuilder.AlterColumn<Guid>(
                name: "UpdateBy",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreateBy",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // ---- PRODUCTS ----
            migrationBuilder.AlterColumn<Guid>(
                name: "UpdateBy",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreateBy",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // ---- ORDERS ----
            migrationBuilder.AlterColumn<Guid>(
                name: "UpdateBy",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreateBy",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // ---- CHATMESSAGES ----
            migrationBuilder.AlterColumn<Guid>(
                name: "UpdateBy",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreateBy",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // ---- CATEGORIES ----
            migrationBuilder.AlterColumn<Guid>(
                name: "UpdateBy",
                table: "Categories",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreateBy",
                table: "Categories",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // ---- BRANDS ----
            migrationBuilder.AlterColumn<Guid>(
                name: "UpdateBy",
                table: "Brands",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreateBy",
                table: "Brands",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ---- USERS ----
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // ---- PRODUCTS ----
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // ---- ORDERS ----
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                table: "Orders",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Orders",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // ---- CHATMESSAGES ----
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                table: "ChatMessages",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "ChatMessages",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // ---- CATEGORIES ----
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // ---- BRANDS ----
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                table: "Brands",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                table: "Brands",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
