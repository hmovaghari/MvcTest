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
VALUES ('20251107065501_InitialCreate', '8.0.22');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "SettingKey" ADD "Description" TEXT NOT NULL DEFAULT '';

UPDATE "SettingKey" SET "Description" = 'تاریخ پیشفرض تراکنش‌های جدید'
WHERE "SettingKeyID" = '11111111-1111-1111-1111-111111111111';
SELECT changes();


INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251114100540_AddDescriptionToSettingKey', '8.0.22');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Person" (
    "PersonID" TEXT NOT NULL CONSTRAINT "PK_Person" PRIMARY KEY,
    "BankAccountNumber" TEXT NULL,
    "BankCard" TEXT NULL,
    "BankShaba" TEXT NULL,
    "CurrencyUnitID" TEXT NULL,
    "Description" TEXT NULL,
    "IsPerson" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "PersonAddress" TEXT NULL,
    "PersonEmail" TEXT NULL,
    "PersonMobile" TEXT NULL,
    "PersonTell" TEXT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_Person_CurrencyUnit_CurrencyUnitID" FOREIGN KEY ("CurrencyUnitID") REFERENCES "CurrencyUnit" ("CurrencyUnitID"),
    CONSTRAINT "FK_Person_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Person" ("PersonID", "BankAccountNumber", "BankCard", "BankShaba", "CurrencyUnitID", "Description", "IsPerson", "Name", "PersonAddress", "PersonEmail", "PersonMobile", "PersonTell", "UserID")
SELECT "PersonID", "BankAccountNumber", "BankCard", "BankShaba", "CurrencyUnitID", "Description", "IsPerson", "Name", "PersonAddress", "PersonEmail", "PersonMobile", "PersonTell", "UserID"
FROM "Person";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Person";

ALTER TABLE "ef_temp_Person" RENAME TO "Person";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Person_CurrencyUnitID" ON "Person" ("CurrencyUnitID");

CREATE INDEX "IX_Person_UserID" ON "Person" ("UserID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251116112641_ModificationOfNon-mandatoryTextFields', '8.0.22');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "User" ADD "ApiKeyID" TEXT NULL;

ALTER TABLE "TransactionType" ADD "ApiKeyID" TEXT NULL;

ALTER TABLE "Transaction" ADD "ApiKeyID" TEXT NULL;

ALTER TABLE "SettingValue" ADD "ApiKeyID" TEXT NULL;

ALTER TABLE "SettingKey" ADD "ApiKeyID" TEXT NULL;

ALTER TABLE "Person" ADD "ApiKeyID" TEXT NULL;

ALTER TABLE "CurrencyUnit" ADD "ApiKeyID" TEXT NULL;

CREATE TABLE "ApiKeys" (
    "ApiKeyID" TEXT NOT NULL CONSTRAINT "PK_ApiKeys" PRIMARY KEY,
    "UserID" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsAdmin" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiKeys_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

UPDATE "CurrencyUnit" SET "ApiKeyID" = NULL
WHERE "CurrencyUnitID" = '11111111-1111-1111-1111-111111111111';
SELECT changes();


UPDATE "CurrencyUnit" SET "ApiKeyID" = NULL
WHERE "CurrencyUnitID" = '22222222-2222-2222-2222-222222222222';
SELECT changes();


UPDATE "SettingKey" SET "ApiKeyID" = NULL
WHERE "SettingKeyID" = '11111111-1111-1111-1111-111111111111';
SELECT changes();


UPDATE "User" SET "ApiKeyID" = NULL
WHERE "UserID" = '11111111-1111-1111-1111-111111111111';
SELECT changes();


CREATE INDEX "IX_User_ApiKeyID" ON "User" ("ApiKeyID");

CREATE INDEX "IX_TransactionType_ApiKeyID" ON "TransactionType" ("ApiKeyID");

CREATE INDEX "IX_Transaction_ApiKeyID" ON "Transaction" ("ApiKeyID");

CREATE INDEX "IX_SettingValue_ApiKeyID" ON "SettingValue" ("ApiKeyID");

CREATE INDEX "IX_SettingKey_ApiKeyID" ON "SettingKey" ("ApiKeyID");

CREATE INDEX "IX_Person_ApiKeyID" ON "Person" ("ApiKeyID");

CREATE INDEX "IX_CurrencyUnit_ApiKeyID" ON "CurrencyUnit" ("ApiKeyID");

CREATE INDEX "IX_ApiKeys_UserID" ON "ApiKeys" ("UserID");

CREATE TABLE "ef_temp_CurrencyUnit" (
    "CurrencyUnitID" TEXT NOT NULL CONSTRAINT "PK_CurrencyUnit" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "IsDesimal" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "FK_CurrencyUnit_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID")
);

INSERT INTO "ef_temp_CurrencyUnit" ("CurrencyUnitID", "ApiKeyID", "IsDesimal", "Name")
SELECT "CurrencyUnitID", "ApiKeyID", "IsDesimal", "Name"
FROM "CurrencyUnit";

CREATE TABLE "ef_temp_Person" (
    "PersonID" TEXT NOT NULL CONSTRAINT "PK_Person" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "BankAccountNumber" TEXT NULL,
    "BankCard" TEXT NULL,
    "BankShaba" TEXT NULL,
    "CurrencyUnitID" TEXT NULL,
    "Description" TEXT NULL,
    "IsPerson" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "PersonAddress" TEXT NULL,
    "PersonEmail" TEXT NULL,
    "PersonMobile" TEXT NULL,
    "PersonTell" TEXT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_Person_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID"),
    CONSTRAINT "FK_Person_CurrencyUnit_CurrencyUnitID" FOREIGN KEY ("CurrencyUnitID") REFERENCES "CurrencyUnit" ("CurrencyUnitID"),
    CONSTRAINT "FK_Person_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Person" ("PersonID", "ApiKeyID", "BankAccountNumber", "BankCard", "BankShaba", "CurrencyUnitID", "Description", "IsPerson", "Name", "PersonAddress", "PersonEmail", "PersonMobile", "PersonTell", "UserID")
SELECT "PersonID", "ApiKeyID", "BankAccountNumber", "BankCard", "BankShaba", "CurrencyUnitID", "Description", "IsPerson", "Name", "PersonAddress", "PersonEmail", "PersonMobile", "PersonTell", "UserID"
FROM "Person";

CREATE TABLE "ef_temp_SettingKey" (
    "SettingKeyID" TEXT NOT NULL CONSTRAINT "PK_SettingKey" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "Description" TEXT NOT NULL,
    "Key" TEXT NOT NULL,
    "UserID" TEXT NULL,
    CONSTRAINT "FK_SettingKey_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID"),
    CONSTRAINT "FK_SettingKey_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID")
);

INSERT INTO "ef_temp_SettingKey" ("SettingKeyID", "ApiKeyID", "Description", "Key", "UserID")
SELECT "SettingKeyID", "ApiKeyID", "Description", "Key", "UserID"
FROM "SettingKey";

CREATE TABLE "ef_temp_SettingValue" (
    "SettingValueID" TEXT NOT NULL CONSTRAINT "PK_SettingValue" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "SettingKeyID" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    CONSTRAINT "FK_SettingValue_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID"),
    CONSTRAINT "FK_SettingValue_SettingKey_SettingKeyID" FOREIGN KEY ("SettingKeyID") REFERENCES "SettingKey" ("SettingKeyID") ON DELETE CASCADE,
    CONSTRAINT "FK_SettingValue_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_SettingValue" ("SettingValueID", "ApiKeyID", "SettingKeyID", "UserID", "Value")
SELECT "SettingValueID", "ApiKeyID", "SettingKeyID", "UserID", "Value"
FROM "SettingValue";

CREATE TABLE "ef_temp_Transaction" (
    "TransactionID" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "Amount" TEXT NOT NULL,
    "ApiKeyID" TEXT NULL,
    "Date" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "PayerPersonID" TEXT NULL,
    "ReceiverPersonID" TEXT NULL,
    "TransactionTypeID" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_Transaction_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID"),
    CONSTRAINT "FK_Transaction_Person_PayerPersonID" FOREIGN KEY ("PayerPersonID") REFERENCES "Person" ("PersonID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transaction_Person_ReceiverPersonID" FOREIGN KEY ("ReceiverPersonID") REFERENCES "Person" ("PersonID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transaction_TransactionType_TransactionTypeID" FOREIGN KEY ("TransactionTypeID") REFERENCES "TransactionType" ("TransactionTypeID") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Transaction" ("TransactionID", "Amount", "ApiKeyID", "Date", "Description", "PayerPersonID", "ReceiverPersonID", "TransactionTypeID", "UserID")
SELECT "TransactionID", "Amount", "ApiKeyID", "Date", "Description", "PayerPersonID", "ReceiverPersonID", "TransactionTypeID", "UserID"
FROM "Transaction";

CREATE TABLE "ef_temp_TransactionType" (
    "TransactionTypeID" TEXT NOT NULL CONSTRAINT "PK_TransactionType" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "IsCost" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "ParentTransactionTypeID" TEXT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_TransactionType_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID"),
    CONSTRAINT "FK_TransactionType_TransactionType_ParentTransactionTypeID" FOREIGN KEY ("ParentTransactionTypeID") REFERENCES "TransactionType" ("TransactionTypeID"),
    CONSTRAINT "FK_TransactionType_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_TransactionType" ("TransactionTypeID", "ApiKeyID", "IsCost", "Name", "ParentTransactionTypeID", "UserID")
SELECT "TransactionTypeID", "ApiKeyID", "IsCost", "Name", "ParentTransactionTypeID", "UserID"
FROM "TransactionType";

CREATE TABLE "ef_temp_User" (
    "UserID" TEXT NOT NULL CONSTRAINT "PK_User" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "Email" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsAdmin" INTEGER NOT NULL,
    "Password" TEXT NOT NULL,
    "Salt1" TEXT NOT NULL,
    "Salt2" TEXT NOT NULL,
    "Username" TEXT NOT NULL,
    CONSTRAINT "FK_User_ApiKeys_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKeys" ("ApiKeyID") ON DELETE SET NULL
);

INSERT INTO "ef_temp_User" ("UserID", "ApiKeyID", "Email", "IsActive", "IsAdmin", "Password", "Salt1", "Salt2", "Username")
SELECT "UserID", "ApiKeyID", "Email", "IsActive", "IsAdmin", "Password", "Salt1", "Salt2", "Username"
FROM "User";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "CurrencyUnit";

ALTER TABLE "ef_temp_CurrencyUnit" RENAME TO "CurrencyUnit";

DROP TABLE "Person";

ALTER TABLE "ef_temp_Person" RENAME TO "Person";

DROP TABLE "SettingKey";

ALTER TABLE "ef_temp_SettingKey" RENAME TO "SettingKey";

DROP TABLE "SettingValue";

ALTER TABLE "ef_temp_SettingValue" RENAME TO "SettingValue";

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

DROP TABLE "TransactionType";

ALTER TABLE "ef_temp_TransactionType" RENAME TO "TransactionType";

DROP TABLE "User";

ALTER TABLE "ef_temp_User" RENAME TO "User";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_CurrencyUnit_ApiKeyID" ON "CurrencyUnit" ("ApiKeyID");

CREATE INDEX "IX_Person_ApiKeyID" ON "Person" ("ApiKeyID");

CREATE INDEX "IX_Person_CurrencyUnitID" ON "Person" ("CurrencyUnitID");

CREATE INDEX "IX_Person_UserID" ON "Person" ("UserID");

CREATE INDEX "IX_SettingKey_ApiKeyID" ON "SettingKey" ("ApiKeyID");

CREATE INDEX "IX_SettingKey_UserID" ON "SettingKey" ("UserID");

CREATE INDEX "IX_SettingValue_ApiKeyID" ON "SettingValue" ("ApiKeyID");

CREATE INDEX "IX_SettingValue_SettingKeyID" ON "SettingValue" ("SettingKeyID");

CREATE INDEX "IX_SettingValue_UserID" ON "SettingValue" ("UserID");

CREATE INDEX "IX_Transaction_ApiKeyID" ON "Transaction" ("ApiKeyID");

CREATE INDEX "IX_Transaction_PayerPersonID" ON "Transaction" ("PayerPersonID");

CREATE INDEX "IX_Transaction_ReceiverPersonID" ON "Transaction" ("ReceiverPersonID");

CREATE INDEX "IX_Transaction_TransactionTypeID" ON "Transaction" ("TransactionTypeID");

CREATE INDEX "IX_Transaction_UserID" ON "Transaction" ("UserID");

CREATE INDEX "IX_TransactionType_ApiKeyID" ON "TransactionType" ("ApiKeyID");

CREATE INDEX "IX_TransactionType_ParentTransactionTypeID" ON "TransactionType" ("ParentTransactionTypeID");

CREATE INDEX "IX_TransactionType_UserID" ON "TransactionType" ("UserID");

CREATE INDEX "IX_User_ApiKeyID" ON "User" ("ApiKeyID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251121083546_AddApiKey', '8.0.22');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "ApiKeys" RENAME TO "ApiKey";

DROP INDEX "IX_ApiKeys_UserID";

CREATE INDEX "IX_ApiKey_UserID" ON "ApiKey" ("UserID");

CREATE TABLE "ef_temp_ApiKey" (
    "ApiKeyID" TEXT NOT NULL CONSTRAINT "PK_ApiKey" PRIMARY KEY,
    "IsActive" INTEGER NOT NULL,
    "IsAdmin" INTEGER NOT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_ApiKey_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_ApiKey" ("ApiKeyID", "IsActive", "IsAdmin", "UserID")
SELECT "ApiKeyID", "IsActive", "IsAdmin", "UserID"
FROM "ApiKey";

CREATE TABLE "ef_temp_CurrencyUnit" (
    "CurrencyUnitID" TEXT NOT NULL CONSTRAINT "PK_CurrencyUnit" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "IsDesimal" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "FK_CurrencyUnit_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID")
);

INSERT INTO "ef_temp_CurrencyUnit" ("CurrencyUnitID", "ApiKeyID", "IsDesimal", "Name")
SELECT "CurrencyUnitID", "ApiKeyID", "IsDesimal", "Name"
FROM "CurrencyUnit";

CREATE TABLE "ef_temp_Person" (
    "PersonID" TEXT NOT NULL CONSTRAINT "PK_Person" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "BankAccountNumber" TEXT NULL,
    "BankCard" TEXT NULL,
    "BankShaba" TEXT NULL,
    "CurrencyUnitID" TEXT NULL,
    "Description" TEXT NULL,
    "IsPerson" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "PersonAddress" TEXT NULL,
    "PersonEmail" TEXT NULL,
    "PersonMobile" TEXT NULL,
    "PersonTell" TEXT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_Person_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID"),
    CONSTRAINT "FK_Person_CurrencyUnit_CurrencyUnitID" FOREIGN KEY ("CurrencyUnitID") REFERENCES "CurrencyUnit" ("CurrencyUnitID"),
    CONSTRAINT "FK_Person_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Person" ("PersonID", "ApiKeyID", "BankAccountNumber", "BankCard", "BankShaba", "CurrencyUnitID", "Description", "IsPerson", "Name", "PersonAddress", "PersonEmail", "PersonMobile", "PersonTell", "UserID")
SELECT "PersonID", "ApiKeyID", "BankAccountNumber", "BankCard", "BankShaba", "CurrencyUnitID", "Description", "IsPerson", "Name", "PersonAddress", "PersonEmail", "PersonMobile", "PersonTell", "UserID"
FROM "Person";

CREATE TABLE "ef_temp_SettingKey" (
    "SettingKeyID" TEXT NOT NULL CONSTRAINT "PK_SettingKey" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "Description" TEXT NOT NULL,
    "Key" TEXT NOT NULL,
    "UserID" TEXT NULL,
    CONSTRAINT "FK_SettingKey_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID"),
    CONSTRAINT "FK_SettingKey_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID")
);

INSERT INTO "ef_temp_SettingKey" ("SettingKeyID", "ApiKeyID", "Description", "Key", "UserID")
SELECT "SettingKeyID", "ApiKeyID", "Description", "Key", "UserID"
FROM "SettingKey";

CREATE TABLE "ef_temp_SettingValue" (
    "SettingValueID" TEXT NOT NULL CONSTRAINT "PK_SettingValue" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "SettingKeyID" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    CONSTRAINT "FK_SettingValue_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID"),
    CONSTRAINT "FK_SettingValue_SettingKey_SettingKeyID" FOREIGN KEY ("SettingKeyID") REFERENCES "SettingKey" ("SettingKeyID") ON DELETE CASCADE,
    CONSTRAINT "FK_SettingValue_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_SettingValue" ("SettingValueID", "ApiKeyID", "SettingKeyID", "UserID", "Value")
SELECT "SettingValueID", "ApiKeyID", "SettingKeyID", "UserID", "Value"
FROM "SettingValue";

CREATE TABLE "ef_temp_Transaction" (
    "TransactionID" TEXT NOT NULL CONSTRAINT "PK_Transaction" PRIMARY KEY,
    "Amount" TEXT NOT NULL,
    "ApiKeyID" TEXT NULL,
    "Date" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "PayerPersonID" TEXT NULL,
    "ReceiverPersonID" TEXT NULL,
    "TransactionTypeID" TEXT NOT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_Transaction_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID"),
    CONSTRAINT "FK_Transaction_Person_PayerPersonID" FOREIGN KEY ("PayerPersonID") REFERENCES "Person" ("PersonID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transaction_Person_ReceiverPersonID" FOREIGN KEY ("ReceiverPersonID") REFERENCES "Person" ("PersonID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transaction_TransactionType_TransactionTypeID" FOREIGN KEY ("TransactionTypeID") REFERENCES "TransactionType" ("TransactionTypeID") ON DELETE CASCADE,
    CONSTRAINT "FK_Transaction_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_Transaction" ("TransactionID", "Amount", "ApiKeyID", "Date", "Description", "PayerPersonID", "ReceiverPersonID", "TransactionTypeID", "UserID")
SELECT "TransactionID", "Amount", "ApiKeyID", "Date", "Description", "PayerPersonID", "ReceiverPersonID", "TransactionTypeID", "UserID"
FROM "Transaction";

CREATE TABLE "ef_temp_TransactionType" (
    "TransactionTypeID" TEXT NOT NULL CONSTRAINT "PK_TransactionType" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "IsCost" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "ParentTransactionTypeID" TEXT NULL,
    "UserID" TEXT NOT NULL,
    CONSTRAINT "FK_TransactionType_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID"),
    CONSTRAINT "FK_TransactionType_TransactionType_ParentTransactionTypeID" FOREIGN KEY ("ParentTransactionTypeID") REFERENCES "TransactionType" ("TransactionTypeID"),
    CONSTRAINT "FK_TransactionType_User_UserID" FOREIGN KEY ("UserID") REFERENCES "User" ("UserID") ON DELETE CASCADE
);

INSERT INTO "ef_temp_TransactionType" ("TransactionTypeID", "ApiKeyID", "IsCost", "Name", "ParentTransactionTypeID", "UserID")
SELECT "TransactionTypeID", "ApiKeyID", "IsCost", "Name", "ParentTransactionTypeID", "UserID"
FROM "TransactionType";

CREATE TABLE "ef_temp_User" (
    "UserID" TEXT NOT NULL CONSTRAINT "PK_User" PRIMARY KEY,
    "ApiKeyID" TEXT NULL,
    "Email" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsAdmin" INTEGER NOT NULL,
    "Password" TEXT NOT NULL,
    "Salt1" TEXT NOT NULL,
    "Salt2" TEXT NOT NULL,
    "Username" TEXT NOT NULL,
    CONSTRAINT "FK_User_ApiKey_ApiKeyID" FOREIGN KEY ("ApiKeyID") REFERENCES "ApiKey" ("ApiKeyID") ON DELETE SET NULL
);

INSERT INTO "ef_temp_User" ("UserID", "ApiKeyID", "Email", "IsActive", "IsAdmin", "Password", "Salt1", "Salt2", "Username")
SELECT "UserID", "ApiKeyID", "Email", "IsActive", "IsAdmin", "Password", "Salt1", "Salt2", "Username"
FROM "User";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "ApiKey";

ALTER TABLE "ef_temp_ApiKey" RENAME TO "ApiKey";

DROP TABLE "CurrencyUnit";

ALTER TABLE "ef_temp_CurrencyUnit" RENAME TO "CurrencyUnit";

DROP TABLE "Person";

ALTER TABLE "ef_temp_Person" RENAME TO "Person";

DROP TABLE "SettingKey";

ALTER TABLE "ef_temp_SettingKey" RENAME TO "SettingKey";

DROP TABLE "SettingValue";

ALTER TABLE "ef_temp_SettingValue" RENAME TO "SettingValue";

DROP TABLE "Transaction";

ALTER TABLE "ef_temp_Transaction" RENAME TO "Transaction";

DROP TABLE "TransactionType";

ALTER TABLE "ef_temp_TransactionType" RENAME TO "TransactionType";

DROP TABLE "User";

ALTER TABLE "ef_temp_User" RENAME TO "User";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_ApiKey_UserID" ON "ApiKey" ("UserID");

CREATE INDEX "IX_CurrencyUnit_ApiKeyID" ON "CurrencyUnit" ("ApiKeyID");

CREATE INDEX "IX_Person_ApiKeyID" ON "Person" ("ApiKeyID");

CREATE INDEX "IX_Person_CurrencyUnitID" ON "Person" ("CurrencyUnitID");

CREATE INDEX "IX_Person_UserID" ON "Person" ("UserID");

CREATE INDEX "IX_SettingKey_ApiKeyID" ON "SettingKey" ("ApiKeyID");

CREATE INDEX "IX_SettingKey_UserID" ON "SettingKey" ("UserID");

CREATE INDEX "IX_SettingValue_ApiKeyID" ON "SettingValue" ("ApiKeyID");

CREATE INDEX "IX_SettingValue_SettingKeyID" ON "SettingValue" ("SettingKeyID");

CREATE INDEX "IX_SettingValue_UserID" ON "SettingValue" ("UserID");

CREATE INDEX "IX_Transaction_ApiKeyID" ON "Transaction" ("ApiKeyID");

CREATE INDEX "IX_Transaction_PayerPersonID" ON "Transaction" ("PayerPersonID");

CREATE INDEX "IX_Transaction_ReceiverPersonID" ON "Transaction" ("ReceiverPersonID");

CREATE INDEX "IX_Transaction_TransactionTypeID" ON "Transaction" ("TransactionTypeID");

CREATE INDEX "IX_Transaction_UserID" ON "Transaction" ("UserID");

CREATE INDEX "IX_TransactionType_ApiKeyID" ON "TransactionType" ("ApiKeyID");

CREATE INDEX "IX_TransactionType_ParentTransactionTypeID" ON "TransactionType" ("ParentTransactionTypeID");

CREATE INDEX "IX_TransactionType_UserID" ON "TransactionType" ("UserID");

CREATE INDEX "IX_User_ApiKeyID" ON "User" ("ApiKeyID");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251121131607_EditApiKeyTableName', '8.0.22');

COMMIT;

