using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "User",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "TransactionType",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "Transaction",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "SettingValue",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "SettingKey",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "Person",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyID",
                table: "CurrencyUnit",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    ApiKeyID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.ApiKeyID);
                    table.ForeignKey(
                        name: "FK_ApiKeys_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "CurrencyUnit",
                keyColumn: "CurrencyUnitID",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "ApiKeyID",
                value: null);

            migrationBuilder.UpdateData(
                table: "CurrencyUnit",
                keyColumn: "CurrencyUnitID",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "ApiKeyID",
                value: null);

            migrationBuilder.UpdateData(
                table: "SettingKey",
                keyColumn: "SettingKeyID",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "ApiKeyID",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "ApiKeyID",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_User_ApiKeyID",
                table: "User",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionType_ApiKeyID",
                table: "TransactionType",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ApiKeyID",
                table: "Transaction",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingValue_ApiKeyID",
                table: "SettingValue",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingKey_ApiKeyID",
                table: "SettingKey",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_ApiKeyID",
                table: "Person",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyUnit_ApiKeyID",
                table: "CurrencyUnit",
                column: "ApiKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UserID",
                table: "ApiKeys",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyUnit_ApiKeys_ApiKeyID",
                table: "CurrencyUnit",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_ApiKeys_ApiKeyID",
                table: "Person",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingKey_ApiKeys_ApiKeyID",
                table: "SettingKey",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValue_ApiKeys_ApiKeyID",
                table: "SettingValue",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ApiKeys_ApiKeyID",
                table: "Transaction",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionType_ApiKeys_ApiKeyID",
                table: "TransactionType",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ApiKeys_ApiKeyID",
                table: "User",
                column: "ApiKeyID",
                principalTable: "ApiKeys",
                principalColumn: "ApiKeyID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyUnit_ApiKeys_ApiKeyID",
                table: "CurrencyUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_ApiKeys_ApiKeyID",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingKey_ApiKeys_ApiKeyID",
                table: "SettingKey");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValue_ApiKeys_ApiKeyID",
                table: "SettingValue");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ApiKeys_ApiKeyID",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionType_ApiKeys_ApiKeyID",
                table: "TransactionType");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ApiKeys_ApiKeyID",
                table: "User");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropIndex(
                name: "IX_User_ApiKeyID",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_TransactionType_ApiKeyID",
                table: "TransactionType");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ApiKeyID",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_SettingValue_ApiKeyID",
                table: "SettingValue");

            migrationBuilder.DropIndex(
                name: "IX_SettingKey_ApiKeyID",
                table: "SettingKey");

            migrationBuilder.DropIndex(
                name: "IX_Person_ApiKeyID",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyUnit_ApiKeyID",
                table: "CurrencyUnit");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "TransactionType");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "SettingValue");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "SettingKey");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "ApiKeyID",
                table: "CurrencyUnit");
        }
    }
}
