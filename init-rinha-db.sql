CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE payments (
    "Id" text NOT NULL,
    "CorrelationId" text NOT NULL,
    "Amount" real NOT NULL,
    CONSTRAINT "PK_payments" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250816231628_InitialMigration', '8.0.11');

COMMIT;

START TRANSACTION;

CREATE TABLE "payment-processing" (
    "Id" text NOT NULL,
    "ProcessingMethod" integer NOT NULL,
    "Status" integer NOT NULL,
    "PaymentId" text NOT NULL,
    CONSTRAINT "PK_payment-processing" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_payment-processing_payments_PaymentId" FOREIGN KEY ("PaymentId") REFERENCES payments ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_payment-processing_PaymentId" ON "payment-processing" ("PaymentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250817024035_AddPaymentProcessingTable', '8.0.11');

COMMIT;

START TRANSACTION;

ALTER TABLE "payment-processing" DROP COLUMN "ProcessingMethod";

ALTER TABLE "payment-processing" DROP COLUMN "Status";

ALTER TABLE payments ADD "Status" text NOT NULL DEFAULT '';

ALTER TABLE "payment-processing" ADD "Method" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250817035602_ChangeTables', '8.0.11');

COMMIT;

START TRANSACTION;

ALTER TABLE payments ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE payments ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "payment-processing" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "payment-processing" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250817160139_AddsDateColumnToPaymentEntities', '8.0.11');

COMMIT;

START TRANSACTION;

ALTER TABLE payments DROP COLUMN "Status";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250818011350_RemoveStatusColumnFromPayment', '8.0.11');

COMMIT;


