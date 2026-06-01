# How to add more stuff

## Calculators

Use an editor that can edit SQLite to open `/EvansWebpage/Data/calcs/calcs.db`.

1. Select the **Exhibits** table and add a new row.
2. Fill in the exhibit details (Id, Name, Category, Manufacturer, etc.). Set boolean columns (`HasCas`, `UnderConstruction`, etc.) to `1` for true or `0` for false.
3. To add specimens, select the **Specimens** table. Add a row and make sure the **ExhibitId** matches the Id you gave your exhibit.
4. Do the same for **GalleryImages** and **MyCalcsLinks** if needed.
5. Click **Write Changes** to save.

> **`manufactureDate`**: Valid formats are:
>
> - `"YYYY"` (e.g., `"1992"`)
> - `"Month YYYY"` (e.g., `"April 2006"`)
> - `"Week n of YYYY"` (e.g., `"Week 35 of 1992"`)
> - `"YYYY-MM-DD"` (e.g., `"2006-04-12"`)
>
> As long as there is a 4-digit year somewhere in the string, the program will automatically extract it to calculate the "Oldest Calc" and "Youngest Calc" stats on the homepage.
>
> Don't put two 4-digit years. I don't know what that does, but it'll probably break shit.
>
> **`acquisitionDate`**: Use the standard ISO 8601 format `"YYYY-MM-DD"` (e.g., `"2026-02-02"`).

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
