PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE "locations" (
    "_id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "unlocode" TEXT NOT NULL,
    "name" TEXT NOT NULL
);
INSERT INTO "locations" VALUES('ITVCE','Venezia',1);
INSERT INTO "locations" VALUES('ITPSA','Pisa',2);
INSERT INTO "locations" VALUES('ITLIV','Livorno',3);
INSERT INTO "locations" VALUES('ITROM','Roma',4);
CREATE TABLE "voyages" (
    "_id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "number" TEXT NOT NULL,
    "last_known_location" TEXT REFERENCES locations (unlocode),
    "next_expected_location" TEXT REFERENCES locations (unlocode)
);
CREATE TABLE "cargos" (
    "_id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "tracking_id" TEXT NOT NULL,
    "transport_status" TEXT,
    "current_voyage" TEXT REFERENCES voyages (number)
);
INSERT INTO "cargos" VALUES(1,'CARGO01',NULL,NULL);
INSERT INTO "cargos" VALUES(2,'CARGO02','NotRecieved',NULL);
INSERT INTO "cargos" VALUES(3,'CARGO03','Claimed',NULL);
DELETE FROM sqlite_sequence;
INSERT INTO "sqlite_sequence" VALUES('locations',4);
INSERT INTO "sqlite_sequence" VALUES('cargos',3);
CREATE UNIQUE INDEX "unique_unlocode" on locations (unlocode ASC);
CREATE UNIQUE INDEX "unique_tid" on cargos (tracking_id ASC);
CREATE UNIQUE INDEX "unique_number" on voyages (number ASC);
COMMIT;
