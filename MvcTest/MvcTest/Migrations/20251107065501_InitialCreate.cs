using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MvcTest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyUnit",
                columns: table => new
                {
                    CurrencyUnitID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsDesimal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyUnit", x => x.CurrencyUnitID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Salt1 = table.Column<string>(type: "TEXT", nullable: false),
                    Salt2 = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsPerson = table.Column<bool>(type: "INTEGER", nullable: false),
                    PersonTell = table.Column<string>(type: "TEXT", nullable: false),
                    PersonMobile = table.Column<string>(type: "TEXT", nullable: false),
                    PersonEmail = table.Column<string>(type: "TEXT", nullable: false),
                    PersonAddress = table.Column<string>(type: "TEXT", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "TEXT", nullable: false),
                    BankShaba = table.Column<string>(type: "TEXT", nullable: false),
                    BankCard = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CurrencyUnitID = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.PersonID);
                    table.ForeignKey(
                        name: "FK_Person_CurrencyUnit_CurrencyUnitID",
                        column: x => x.CurrencyUnitID,
                        principalTable: "CurrencyUnit",
                        principalColumn: "CurrencyUnitID");
                    table.ForeignKey(
                        name: "FK_Person_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingKey",
                columns: table => new
                {
                    SettingKeyID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingKey", x => x.SettingKeyID);
                    table.ForeignKey(
                        name: "FK_SettingKey_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "TransactionType",
                columns: table => new
                {
                    TransactionTypeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentTransactionTypeID = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsCost = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionType", x => x.TransactionTypeID);
                    table.ForeignKey(
                        name: "FK_TransactionType_TransactionType_ParentTransactionTypeID",
                        column: x => x.ParentTransactionTypeID,
                        principalTable: "TransactionType",
                        principalColumn: "TransactionTypeID");
                    table.ForeignKey(
                        name: "FK_TransactionType_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingValue",
                columns: table => new
                {
                    SettingValueID = table.Column<Guid>(type: "TEXT", nullable: false),
                    SettingKeyID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingValue", x => x.SettingValueID);
                    table.ForeignKey(
                        name: "FK_SettingValue_SettingKey_SettingKeyID",
                        column: x => x.SettingKeyID,
                        principalTable: "SettingKey",
                        principalColumn: "SettingKeyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettingValue_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    TransactionID = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionTypeID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReceiverPersonID = table.Column<Guid>(type: "TEXT", nullable: true),
                    PayerPersonID = table.Column<Guid>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transaction_Person_PayerPersonID",
                        column: x => x.PayerPersonID,
                        principalTable: "Person",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Person_ReceiverPersonID",
                        column: x => x.ReceiverPersonID,
                        principalTable: "Person",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_TransactionType_TransactionTypeID",
                        column: x => x.TransactionTypeID,
                        principalTable: "TransactionType",
                        principalColumn: "TransactionTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CurrencyUnit",
                columns: new[] { "CurrencyUnitID", "IsDesimal", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), false, "ریال" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), false, "تومان" }
                });

            migrationBuilder.InsertData(
                table: "SettingKey",
                columns: new[] { "SettingKeyID", "Key", "UserID" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "NewTrasactionDateType", null });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Email", "IsActive", "IsAdmin", "Password", "Salt1", "Salt2", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "", true, true, "0130fd5601a7addede0fb3dfac1657353497fe54d9a54a7a83b82ef77e5e6212", "bcfbf4a8-67fc-4db9-ba7e-908afb4de0f7", "ddc46224-2fdc-4320-b821-f7f7c2e65bba", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Person_CurrencyUnitID",
                table: "Person",
                column: "CurrencyUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_UserID",
                table: "Person",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingKey_UserID",
                table: "SettingKey",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingValue_SettingKeyID",
                table: "SettingValue",
                column: "SettingKeyID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingValue_UserID",
                table: "SettingValue",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PayerPersonID",
                table: "Transaction",
                column: "PayerPersonID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ReceiverPersonID",
                table: "Transaction",
                column: "ReceiverPersonID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransactionTypeID",
                table: "Transaction",
                column: "TransactionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_UserID",
                table: "Transaction",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionType_ParentTransactionTypeID",
                table: "TransactionType",
                column: "ParentTransactionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionType_UserID",
                table: "TransactionType",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingValue");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "SettingKey");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "TransactionType");

            migrationBuilder.DropTable(
                name: "CurrencyUnit");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
