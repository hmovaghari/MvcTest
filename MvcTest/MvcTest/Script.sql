CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "CurrencyUnit" (
    "CurrencyUnitID" TEXT NOT NULL CONSTRAINT "PK_CurrencyUnit" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "IsDesimal" INTEGER NOT NULL
);

CREATE TABLE "User" (
    "UserID" TEXT NOT NULL CONSTRAINT "PK_User" PRIMARY KEY,
    "Username" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Salt1" TEXT NOT NULL,
    "Salt2" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsAdmin" INTEGER NOT NULL
);

CREATE TABLE "Person" (
    "PersonID" TEXT NOT NULL CONSTRAINT "PK_Person" PRIMARY KEY,
    "UserID" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "IsPerson" INTEGER NOT NULL,
    "PersonTell" TEXT NOT NULL,
    "PersonMobile" TEXT NOT NULL,
    "PersonEmail" TEXT NOT NULL,
    "PersonAddress" TEXT NOT NULL,
    "BankAccountNumber" TEXT NOT NULL,
    "BankShaba" TEXT NOT NULL,
    "BankCard" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CurrencyUnitID" TEXT NULL,
    CONSTRAINT "FK_Person_CurrencyUnit_CurrencyUnitID" FOREIGN KEY ("CurrencyUnitID") REFERENCES "CurrencyUnit" ("CurrencyUnitID"),
    CONSTRAINT "FK_Person_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "SettingKey" (
    "SettingKeyID" TEXT NOT NULL CONSTRAINT "PK_SettingKey" PRIMARY KEY,
    "Key" TEXT NOT NULL,
    "UserID" TEXT NULL,
    CONSTRAINT "FK_SettingKey_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID")
);

CREATE TABLE "TransactionType" (
    "TransactionTypeID" TEXT NOT NULL CONSTRAINT "PK_TransactionType" PRIMARY KEY,
    "ParentTransactionTypeID" TEXT NULL,
    "UserID" TEXT NOT NULL,
    "IsCost" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "FK_TransactionType_TransactionType_ParentTransactionTypeID" FOREIGN KEY ("ParentTransactionTypeID") REFERENCES "TransactionType" ("TransactionTypeID"),
    CONSTRAINT "FK_TransactionType_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "SettingValue" (
    "SettingValueID" TEXT NOT NULL CONSTRAINT "PK_SettingValue" PRIMARY KEY,
    "SettingKeyID" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    CONSTRAINT "FK_SettingValue_SettingKey_SettingKeyID" FOREIGN KEY ("SettingKeyID") REFERENCES "SettingKey" ("SettingKeyID") ON DELETE CASCADE,
    CONSTRAINT "FK_SettingValue_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

CREATE TABLE "Transaction" (
    "TransactionID" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "TransactionTypeID" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    "ReceiverPersonID" TEXT NULL,
    "PayerPersonID" TEXT NULL,
    "Date" TEXT NOT NULL,
    "Amount" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    CONSTRAINT "FK_Transaction_Person_PayerPersonID" FOREIGN KEY ("PayerPersonID") REFERENCES "Person" ("PersonID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transaction_Person_ReceiverPersonID" FOREIGN KEY ("ReceiverPersonID") REFERENCES "Person" ("PersonID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transaction_TransactionType_TransactionTypeID" FOREIGN KEY ("TransactionTypeID") REFERENCES "TransactionType" ("TransactionTypeID") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "CurrencyUnit" ("CurrencyUnitID", "IsDesimal", "Name")
VALUES ('11111111-1111-1111-1111-111111111111', 0, 'ریال');
SELECT changes();

INSERT INTO "CurrencyUnit" ("CurrencyUnitID", "IsDesimal", "Name")
VALUES ('22222222-2222-2222-2222-222222222222', 0, 'تومان');
SELECT changes();


INSERT INTO "SettingKey" ("SettingKeyID", "Key", "UserID")
VALUES ('11111111-1111-1111-1111-111111111111', 'NewTrasactionDateType', NULL);
SELECT changes();


INSERT INTO "User" ("UserID", "Email", "IsActive", "IsAdmin", "Password", "Salt1", "Salt2", "Username")
VALUES ('11111111-1111-1111-1111-111111111111', '', 1, 1, '0130fd5601a7addede0fb3dfac1657353497fe54d9a54a7a83b82ef77e5e6212', 'bcfbf4a8-67fc-4db9-ba7e-908afb4de0f7', 'ddc46224-2fdc-4320-b821-f7f7c2e65bba', 'admin');
SELECT changes();


CREATE INDEX "IX_Person_CurrencyUnitID" ON "Person" ("CurrencyUnitID");

CREATE INDEX "IX_Person_UserID" ON "Person" ("UserID");

CREATE INDEX "IX_SettingKey_UserID" ON "SettingKey" ("UserID");

CREATE INDEX "IX_SettingValue_SettingKeyID" ON "SettingValue" ("SettingKeyID");

CREATE INDEX "IX_SettingValue_UserID" ON "SettingValue" ("UserID");

CREATE INDEX "IX_Transaction_PayerPersonID" ON "Transaction" ("PayerPersonID");

CREATE INDEX "IX_Transaction_ReceiverPersonID" ON "Transaction" ("ReceiverPersonID");

CREATE INDEX "IX_Transaction_TransactionTypeID" ON "Transaction" ("TransactionTypeID");

CREATE INDEX "IX_Transaction_UserID" ON "Transaction" ("UserID");

CREATE INDEX "IX_TransactionType_ParentTransactionTypeID" ON "TransactionType" ("ParentTransactionTypeID");

CREATE INDEX "IX_TransactionType_UserID" ON "TransactionType" ("UserID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251107065501_InitialCreate', '8.0.21');

COMMIT;

