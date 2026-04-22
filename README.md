# Evan's Webpage

> [!NOTE]
> Merged to Main, we are fully running ASP.NET now! Next step: Make sure it can pull without my oversight.

## Running the Server

(This is for me, not for anyone else.)

### Locally

To run the server locally, use `dotnet run` from inside the `EvansWebpage` directory:

```bash
cd EvansWebpage
dotnet run --urls "http://localhost:5050"
```

The site will be available on `http://localhost:5050` (or whatever ports config'd in `Properties/launchSettings.json`).

THE PAGE REQUIRES THE FOLLOWING:
- `Microsoft.Extensions.FileProviders.Embedded`
- `Markdig`



### Running on-VM

To run in production on Yugoslavia:

1.  ```bash
        ./EvansWebpage --urls "http://0.0.0.0:5000"
    ```
2.  It will run on port 5000 as set up in Caddy. I will eventually set it up to run continually in the background via a `systemd` service.
3.  Make sure Caddy is config'd right.

> [!NOTE]
> In step 1, make sure the runtime is correct! Use `linux-musl-x64` for an Alpine machine and `linux-x64` for everything else.


## Adding more calculators

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
      { "id":  "", "name": "" },
      { "id":  "", "name": "" }
    ],
    "underConstruction": true,
    "category": ""
  }
```
---

This is Evan's webpage and Calculator Museum.

This repo is primarily hosted on GitHub so I can use my existing key signing and extensions, but it is mirrored onto git.yugoslavia.dev.

## For Adjustments

This repo can be pushed to on Yugoslavia. I have it set so that changes here are sent to GitHub every hour, and changes to GitHub are pulled to Yugoslavia every time a commit is made there (webhooks baby!)

## Setup on yuws

Currently, there is a script (pull-bin.sh in my home folder) that pulls the latest artifact, copies the binary, and restarts evans-webpage.service.
Current goal: Set this up on a webhook so I don't have to go into the console every update.

## About AI Contributions

I decided to try and test out Google's 'Jules' coding agent. I accidentally turned 'automatically look for shit to do' on. I do not know how to turn it off. Other than the security changes it suggests, I'm keeping those contained to non-prod branches.

The important thing is that **I will mark any commit that I relied primarily on AI for.**

## Planned Additions

### Front-facing
- [ ] Make happenings dynamically built like the calcpages
  - [ ] RSS Feed for happenings
- [ ] RPN Calculator widget
  - [ ] RPN challenge game (?)
    - [ ] With leaderboard (?)
- [ ] Projects Page (?)
- [ ] Add sortable Attributes/Traits to calculators
- [ ] Add Benchmarks for calculators
- [ ] Stylize more/Add more themes
  - [ ] 'Fancy' theme with more gradients and elegant colors

### Backend
- [ ] Refine theme code and CSS
