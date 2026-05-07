# How to add more stuff

## Calculators

Use this empty template and add to /EvansWebpage/Data/calcs/calcs.json.

```json
{
  "id": "",
  "name": "",
  "manufacturer": "",
  "manufacturerLogo": "",
  "manufacturerSlug": "",
  "mainImageUrl": "",
  "model": "",
  "modelSlug": "",
  "yearIntroduced": 0,
  "type": "",
  "specimens": [
    {
      "number": 1,
      "variant": "Normal",
      "serialNumber": "",
      "condition": "",
      "imageUrl": ""
    }
  ],
  "gallery": [
    {
      "url": "",
      "altText": "",
      "caption": ""
    }
  ],
  "myCalcsLinks": [
    { "id": "", "name": "" },
    { "id": "", "name": "" }
  ],
  "underConstruction": true,
  "category": ""
}
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
