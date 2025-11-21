using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class EditApiKeyTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_User_UserID",
                table: "ApiKeys");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                newName: "ApiKey");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKeys_UserID",
                table: "ApiKey",
                newName: "IX_ApiKey_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKey",
                table: "ApiKey",
                column: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKey_User_UserID",
                table: "ApiKey",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyUnit_ApiKey_ApiKeyID",
                table: "CurrencyUnit",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_ApiKey_ApiKeyID",
                table: "Person",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingKey_ApiKey_ApiKeyID",
                table: "SettingKey",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingValue_ApiKey_ApiKeyID",
                table: "SettingValue",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_ApiKey_ApiKeyID",
                table: "Transaction",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionType_ApiKey_ApiKeyID",
                table: "TransactionType",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_ApiKey_ApiKeyID",
                table: "User",
                column: "ApiKeyID",
                principalTable: "ApiKey",
                principalColumn: "ApiKeyID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKey_User_UserID",
                table: "ApiKey");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyUnit_ApiKey_ApiKeyID",
                table: "CurrencyUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_ApiKey_ApiKeyID",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingKey_ApiKey_ApiKeyID",
                table: "SettingKey");

            migrationBuilder.DropForeignKey(
                name: "FK_SettingValue_ApiKey_ApiKeyID",
                table: "SettingValue");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_ApiKey_ApiKeyID",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionType_ApiKey_ApiKeyID",
                table: "TransactionType");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ApiKey_ApiKeyID",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKey",
                table: "ApiKey");

            migrationBuilder.RenameTable(
                name: "ApiKey",
                newName: "ApiKeys");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKey_UserID",
                table: "ApiKeys",
                newName: "IX_ApiKeys_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys",
                column: "ApiKeyID");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_User_UserID",
                table: "ApiKeys",
                column: "UserID",
                principalTable: "User",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

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
    }
}
