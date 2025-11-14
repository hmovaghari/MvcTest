using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToSettingKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SettingKey",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SettingKey",
                keyColumn: "SettingKeyID",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Description",
                value: "تاریخ پیشفرض تراکنش‌های جدید");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SettingKey");
        }
    }
}
