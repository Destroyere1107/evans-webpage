# How to add more stuff

## Calculators

Open `/EvansWebpage/Data/calcs/calcs.db` in an SQL editor.

The database contains five tables: `Manufacturers`, `Exhibits`, `Specimens`, `GalleryImages`, and `MyCalcsLinks`. Add a row to `Exhibits` first, and then add rows to the other tables linking back to it. Ensure the `Manufacturer` is documented in the `Manufacturers` table _before_ linking it.

#### Manufacturers Table

This table stores data for the manufacturers.

- **`Id`**: A URL-friendly identifier for the manufacturer, used as the linking key for other tables. (e.g., `hp`, `ti`, `casio`).
- **`Name`**: The full display name of the manufacturer (e.g., `Hewlett-Packard`, `Texas Instruments`).
- **`LogoUrl`**: Path to the logo image file (e.g., `/assets/logos/hp-logo.png`).
- **`LegalName`**: The full legal name of the company (e.g., `Hewlett-Packard Company`).
- **`HomeCountry`**: The country of the company's headquarters (e.g., `USA`, `Japan`).
- **`Website`**: URL to the company's website (e.g., `https://www.hp.com`).

### Exhibits Table

This table holds the high-level information for individual exhibits.

- **`Id`**: The identifier used solely for determining the URL (e.g., `50g`, `nspire-cx`). This is **not** used for linking child tables anymore.
- **`Name`**: The full display name of the exhibit (e.g., `50g`).
- **`Category`**: The broad classification of the exhibit, used for organizing in the directory (e.g., `Calculator`, `Accessory`, `Computer`). The directory groups these dynamically. If left blank, it defaults to `Other`.
- **`ManufacturerId`**: The foreign key linking to the `Id` column in the `Manufacturers` table (e.g., `hp`, `ti`).
- **`Model`**: The shortened model name (e.g., `HP 50g`).
- **`ModelSlug`**: The internal identifier used to link child tables (`Specimens`, `GalleryImages`, `MyCalcsLinks`) and to resolve the markdown file directory. Must be unique.
- **`Type`**: The type of calculator. Any string is valid, but the museum has special colored badges for `Graphing`, `Scientific`, `Financial`, and `Basic`. If the calculator has CAS, append ` (CAS)` to the end (e.g., `Graphing (CAS)`).
- **`YearIntroduced`**: The 4-digit year the calculator was released (integer).
- **`MainImageUrl`**: URL to the main feature image displayed above the Quick Facts table.
- **`UnderConstruction`**: Determines whether the 'Under Construction' banner and tag are displayed (`1` for true, `0` for false).
- **`HasColor`**: Set to `1` (true) if the calculator features a color screen, or `0` (false) otherwise.
- **`IsHidden`**: Set to `1` to completely hide this exhibit from the museum. In this case, the Exhibit service will ignore it completely. Defaults to `0`.

#### Specimens Table

This table logs the specific specimens in the collection.

- **`Id`**: Auto-incrementing primary key (leave blank or let SQLite handle it).
- **`ExhibitId`**: Must exactly match the `ModelSlug` from the `Exhibits` table.
- **`Number`**: The sequential specimen number for this exhibit (integer).
- **`Variant`**: Any specific variant information (e.g., `Normal`, `Blue Prototype`).
- **`SerialNumber`**: The serial number printed on the device.
- **`Condition`**: The condition of the unit (e.g., `Mint`, `Good`, `Fair`, `Poor`, `Broken`). Any string is valid.
- **`ImageUrl`**: URL to the hosted image of this specimen. Use `[coming soon]` if you don't have a picture yet.
- **`ManufactureDate`**: Valid formats are:
  - `"YYYY"` (e.g., `"1992"`)
  - `"Month YYYY"` (e.g., `"April 2006"`)
  - `"Week n of YYYY"` (e.g., `"Week 35 of 1992"`)
  - `"YYYY-MM-DD"` (e.g., `"2006-04-12"`)

  > As long as there is a 4-digit year somewhere in the string, the program will automatically extract it to calculate the "Oldest Calc" and "Youngest Calc" stats on the homepage.
  > Don't put two 4-digit years. I don't know what that does, but it'll probably break shit.

- **`Datecode`**: The raw datecode stamped on the unit.
- **`CountryOfManufacture`**: Where it was made (e.g., `China`, `USA`).
- **`HardwareRevision`**: Hardware revision code if known.
- **`AcquisitionDate`**: Use the standard ISO 8601 format `"YYYY-MM-DD"` (e.g., `"2026-02-02"`).

#### GalleryImages Table

This table holds the links to additional photos for the gallery section.

- **`Id`**: Auto-incrementing primary key (leave blank).
- **`ExhibitId`**: Must exactly match the `ModelSlug` from the `Exhibits` table.
- **`Url`**: URL of the image.
- **`AltText`**: Accessibility text for screen readers.
- **`Caption`**: Optional caption to display underneath the image.

#### MyCalcsLinks Table

This table holds links to similar specimens on MyCalcs.

- **`Id`**: Auto-incrementing primary key (leave blank).
- **`ExhibitId`**: Must exactly match the `ModelSlug` from the `Exhibits` table.
- **`LinkId`**: The numeric ID used in the MyCalcs URL.
- **`Name`**: Optional display name for the link (e.g., `48SX`). If left blank, the ID will be displayed.

### Exhibit Markdown Files

Each exhibit has its own text in Markdown. These files must be placed in a directory matching the exhibit's manufacturer and model:
`/EvansWebpage/Data/calcs/md/{ManufacturerId}/{ModelSlug}/`

The directory names _must_ exactly match the `ManufacturerId` (e.g., `hp`) and `ModelSlug` (e.g., `50g`) defined in the `Exhibits` table.

Inside this folder, there can be three files:

- **`description.md`**: The main body of the exhibit page, displayed under History and Background. If missing, it defaults to a _"Content coming soon."_ placeholder.
- **`specimens.md`**: Notes about the physical specimens in the collection, displayed below the table. If missing, it defaults to a _"Content coming soon."_ placeholder.
- **`notes.md`**: Optional technical, programming, or miscellaneous notes. If this file is missing or empty, the entire 'Notes' section will be fully omitted.

### Featured Exhibits

The Calculator Museum landing page has three manually-curated highlight cards: **Calc of the Month**, **Curator's Choice**, and **Newest Addition**.

To change these, open `/EvansWebpage/Pages/CalculatorMuseum.cshtml.cs` and modify the following string constants near the top of the file to match the `Id` of the desired exhibit from the `Exhibits` table:

```csharp
public static readonly string ExhibitOfTheMonthId = "28";        // Calc of the Month
private const string CuratorsChoiceId    = "50g";       // Curator's Choice
private const string NewestAdditionId    = "nspire-cx"; // Newest Addition
```

## Themes

1. Copy `theme-template.css` to `whatever-new-theme.css`.
2. Fill in the new theme with the appropriate colors and uncomment any needed overrides.
3. Link the theme in `<head>` in `_Layout.cshtml`:

```html
<link
  rel="stylesheet"
  type="text/css"
  href="~/css/theme-dark.css"
  asp-append-version="true"
/>
<link
  rel="stylesheet"
  type="text/css"
  href="~/css/theme-light.css"
  asp-append-version="true"
/>
<link
  rel="stylesheet"
  type="text/css"
  href="~/css/theme-whatever.css"
  asp-append-version="true"
/>
```

4. Add the theme just above that to the function:

```javascript
const themes = ["dark", "light", "whatever"];
```

5. Add it to the `themes` array in the theme toggle script at the bottom of `_Layout.cshtml`:

```javascript
const themes = [
  { id: "dark", label: "Dark" },
  { id: "light", label: "Light" },
  { id: "blue", label: "Blue" },
  { id: "whatever", label: "Whatever" }, // New theme
];
```
