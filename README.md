# Evan's Webpage

This is Evan's webpage and Calculator Museum.

This repo is primarily hosted on GitHub so I can use my existing key signing and extensions, but it is mirrored onto git.yugoslavia.dev.

## For changes made by not me

This repo can be pushed to on Yugoslavia. I have it set so that changes here are sent to GitHub every hour, and changes to GitHub are pulled to Yugoslavia every time a commit is made there (webhooks baby!)

## Setup on yuws

Every time this repo's main branch is pushed to, GitHub Actions builds it, the server downloads it (triggered by webhook), and redeploys it.

## About AI Contributions

A decent amount of the work on this website was made with the assistance of LLMs, often (somewhat inaccurately) called 'AI'. 

I want to make this as clear as possible: I do not, and I do not endorse or support, the process of 'vibe coding', which is where a wannabe programmer who couldn't make a Hello World with a gun to his head gets a shitty idea and spends a bit of the world's oil to make a really shitty version of it.

Every single line of code that I didn't write (and I o try to limit it as much as possible) passes my eyes and is brought up to my standards as much as possible. 

In addition, under no circumstances will I ever use an LLM to generate any **content**, meaning any text or images that your average John Taxpayer will ever lay his eyes on, such as my news posts or the museum.

-# Also, I did finally figure out how to turn off Jules. I am quite happy about that.

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
- [x] Stylize more/Add more themes
  - [x] 'Fancy' theme with more gradients and elegant colors

### Backend

- [x] Refine theme code and CSS

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

- `Microsoft.Extensions.FileProviders.Embedded` - Allows for everything to be bundled as one big binary
- `Markdig` - For the dynamic Happenings and Calcpages.
- `System.ServiceModel.Syndication` - For the RSS feed.

### Running on-VM

To run in production on Yugoslavia:

1.  ```bash
        ./EvansWebpage --urls "http://0.0.0.0:5000"
    ```
2.  It will run on port 5000 as set up in Caddy. I will eventually set it up to run continually in the background via a `systemd` service.
3.  Make sure Caddy is config'd right.

> [!NOTE]
> In step 1, make sure the runtime is correct! Use `linux-musl-x64` for an Alpine machine and `linux-x64` for everything else.

---
