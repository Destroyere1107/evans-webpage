# Evan's Webpage - Overkill Branch

> [!NOTE]
> This branch is dedicated to transitioning from plain HTML to ASP.NET. As of now, a full ASP.NET transition is way
> overkill, thus the branch name. This is all to facilitate more fluid changes in the future.

> [!WARNING]
> Almost all work in this branch is AI generated! These changes will not be merged to `main` until I've had a chance to
> fully review and, if needed, rewrite them, as shown below.

## CURRENT REVIEW STATUS

### DONE – Off to the races with this one!

- [X] ~~Review `_Layout.cshtml` and nested partial structure~~ All good!
- [X] ~~Review `Program.cs` and adjust it to my needs~~ Whole reason I did this
- [X] ~~Review stylesheet (`wwwroot/css/style.css`)~~ It's exactly the same
- [X] ~~Verify Root Pages:~~ Everything's good.
  - [X] ~~`Index.cshtml`~~
  - [X] ~~`About.cshtml`~~
  - [X] ~~`Happenings.cshtml`~~
  - [X] ~~`Guestbook.cshtml`~~
  - [X] ~~`Yugoslavia.cshtml`~~
  - [X] ~~`AI.cshtml`~~
  - [X] ~~`Contact.cshtml`~~
  - [X] ~~`CalculatorMuseum.cshtml`~~
  - [X] ~~`Photos.cshtml`~~ Nothing here.
  - [X] ~~`Links.cshtml`~~ Still not much.
  - [X] ~~`Privacy.cshtml`~~ Just as empty.
  - [X] ~~`Sitemap.cshtml`~~ You get the point.
- [X] ~~Verify Calculator Pages:~~
  - [X] ~~`Calculators/Directory.cshtml`~~
  - [X] ~~`Calculators/Hp50g.cshtml`~~
  - [X] ~~`Calculators/Hp28c.cshtml`~~
  - [X] ~~`Calculators/Hp48.cshtml`~~
  - [X] ~~`Calculators/Prime.cshtml`~~
  - [X] ~~`Calculators/NspireCx.cshtml`~~
  - [X] ~~`Calculators/FxCg50.cshtml`~~
- [X] ~~Verify Lite Pages:~~ Apparently there ain't damn shit to verify in pages where there aren't even any divs
  - [X] ~~`Lite/Simple.cshtml`~~
  - [X] ~~`Lite/AiSimple.cshtml`~~
- [X] ~~Test Theme Toggle visual functionality~~ I still don't know why I kept this
- [X] ~~Verify active navigation state bindings~~

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
- Microsoft.Extensions.FileProviders.Embedded



### Running on-VM

To run in production on Yugoslavia:

1.  ```bash
        ./EvansWebpage --urls "http://0.0.0.0:5000"
    ```
2.  It should be accessible from port 5000. I will eventually set it up to run continually in the background via a `systemd` service.
3.  Make sure Caddy is config'd right.

> [!NOTE]
> In step 1, make sure the runtime is correct! Use `linux-musl-x64` for an Alpine machine and `linux-x64` for everything else.

---

This is Evan's webpage and Calculator Museum.

This repo is primarily hosted on GitHub so I can use my existing key signing and extensions, but it is mirrored onto git.yugoslavia.dev.

## For Adjustments

This repo can be pushed to on Yugoslavia. I have it set so that changes here are sent to GitHub every hour, and changes to GitHub are pulled to Yugoslavia every time a commit is made there (webhooks baby!)

## Setup on yuws

On the server, ~~I have a service to pull from the GitHub repo every 12 hours. Check evans-webpage-pull.service and site-pull.timer.~~

That service is now redundant. I have set up a webhook to pull from GitHub every time a commit is made there, too. Check webhook.service and /etc/webhook.conf.

## About AI Contributions

I decided to try and test out Google's 'Jules' coding agent. I accidentally turned 'automatically look for shit to do' on. I do not know how to turn it off. Other than the security changes it suggests, I'm keeping those contained to non-prod branches.

The important thing is that **I will mark any commit that I relied primarily on AI for.**
