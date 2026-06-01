#!/usr/bin/env python3
"""One-shot migration: calcs.json → calcs.db"""
"""This is left over from the JSON → SQLite migration."""

import json
import sqlite3
import os

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
JSON_PATH = os.path.join(SCRIPT_DIR, "calcs.json")
DB_PATH = os.path.join(SCRIPT_DIR, "calcs.db")

# Safety check
if os.path.exists(DB_PATH):
    print(f"ERROR: {DB_PATH} already exists. Delete it first if you want to re-run.")
    exit(1)

with open(JSON_PATH, "r", encoding="utf-8") as f:
    exhibits = json.load(f)

conn = sqlite3.connect(DB_PATH)
cur = conn.cursor()

# Create tables
cur.executescript("""
CREATE TABLE Exhibits (
    Id              TEXT PRIMARY KEY,
    Name            TEXT NOT NULL,
    Category        TEXT,
    Manufacturer    TEXT,
    ManufacturerLogo TEXT,
    ManufacturerSlug TEXT,
    Model           TEXT,
    ModelSlug       TEXT,
    Type            TEXT,
    YearIntroduced  INTEGER,
    MainImageUrl    TEXT,
    UnderConstruction INTEGER NOT NULL DEFAULT 0,
    HasCas          INTEGER NOT NULL DEFAULT 0,
    HasGraphing     INTEGER NOT NULL DEFAULT 0,
    HasColor        INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE Specimens (
    Id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    ExhibitId            TEXT NOT NULL REFERENCES Exhibits(Id),
    Number               INTEGER NOT NULL,
    Variant              TEXT,
    SerialNumber         TEXT,
    Condition            TEXT,
    ImageUrl             TEXT,
    ManufactureDate      TEXT,
    Datecode             TEXT,
    CountryOfManufacture TEXT,
    HardwareRevision     TEXT,
    AcquisitionDate      TEXT
);

CREATE TABLE GalleryImages (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    ExhibitId TEXT NOT NULL REFERENCES Exhibits(Id),
    Url       TEXT,
    AltText   TEXT,
    Caption   TEXT
);

CREATE TABLE MyCalcsLinks (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    ExhibitId TEXT NOT NULL REFERENCES Exhibits(Id),
    LinkId    TEXT,
    Name      TEXT
);
""")

# Insert data
for ex in exhibits:
    eid = ex.get("id", "")
    cur.execute("""
        INSERT INTO Exhibits
            (Id, Name, Category, Manufacturer, ManufacturerLogo, ManufacturerSlug,
             Model, ModelSlug, Type, YearIntroduced, MainImageUrl,
             UnderConstruction, HasCas, HasGraphing, HasColor)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, (
        eid,
        ex.get("name", ""),
        ex.get("category", ""),
        ex.get("manufacturer", ""),
        ex.get("manufacturerLogo", ""),
        ex.get("manufacturerSlug", ""),
        ex.get("model", ""),
        ex.get("modelSlug", ""),
        ex.get("type", ""),
        ex.get("yearIntroduced"),
        ex.get("mainImageUrl", ""),
        int(ex.get("underConstruction", False)),
        int(ex.get("hasCas", False)),
        int(ex.get("hasGraphing", False)),
        int(ex.get("hasColor", False)),
    ))

    for spec in ex.get("specimens", []):
        cur.execute("""
            INSERT INTO Specimens
                (ExhibitId, Number, Variant, SerialNumber, Condition, ImageUrl,
                 ManufactureDate, Datecode, CountryOfManufacture, HardwareRevision, AcquisitionDate)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (
            eid,
            spec.get("number", 0),
            spec.get("variant", ""),
            spec.get("serialNumber", ""),
            spec.get("condition", ""),
            spec.get("imageUrl", ""),
            spec.get("manufactureDate", ""),
            spec.get("datecode", ""),
            spec.get("countryOfManufacture", ""),
            spec.get("hardwareRevision", ""),
            spec.get("acquisitionDate", ""),
        ))

    for img in ex.get("gallery", []):
        cur.execute("""
            INSERT INTO GalleryImages (ExhibitId, Url, AltText, Caption)
            VALUES (?, ?, ?, ?)
        """, (
            eid,
            img.get("url", ""),
            img.get("altText", ""),
            img.get("caption", ""),
        ))

    for link in ex.get("myCalcsLinks", []):
        cur.execute("""
            INSERT INTO MyCalcsLinks (ExhibitId, LinkId, Name)
            VALUES (?, ?, ?)
        """, (
            eid,
            link.get("id", ""),
            link.get("name", ""),
        ))

conn.commit()

# Verify
print("=== Migration Summary ===")
for table in ["Exhibits", "Specimens", "GalleryImages", "MyCalcsLinks"]:
    count = cur.execute(f"SELECT COUNT(*) FROM {table}").fetchone()[0]
    print(f"  {table}: {count} rows")

conn.close()
print(f"\nDone! Database written to: {DB_PATH}")
