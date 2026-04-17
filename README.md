# Evan's Webpage - Overkill Branch

> [!NOTE]
> This branch is dedicated to transitioning from plain HTML to ASP.NET. As of now, a full ASP.NET transition is way
> overkill, thus the branch name. This is all to facilitate more fluid changes in the future.

> [!WARNING]
> Almost all work in this branch is AI generated! These changes will not be merged to `main` until I've had a chance to
> fully review and, if needed, rewrite them, as shown below.

## CURRENT REVIEW STATUS

### 0/25 DONE - 0%

- [ ] Review `_Layout.cshtml` and nested partial structure
- [ ] Review `Program.cs` and adjust it to my needs
- [ ] Review stylesheet (`wwwroot/css/style.css`)
- [ ] Verify Root Pages:
  - [ ] `Index.cshtml`
  - [ ] `About.cshtml`
  - [ ] `Happenings.cshtml`
  - [ ] `Guestbook.cshtml`
  - [ ] `Yugoslavia.cshtml`
  - [ ] `AiTopic.cshtml`
  - [ ] `Contact.cshtml`
  - [ ] `CalculatorMuseum.cshtml`
  - [ ] `Photos.cshtml`
  - [ ] `Links.cshtml`
  - [ ] `Privacy.cshtml`
  - [ ] `Sitemap.cshtml`
- [ ] Verify Calculator Pages:
  - [ ] `Calculators/Directory.cshtml`
  - [ ] `Calculators/Hp50g.cshtml`
  - [ ] `Calculators/Hp28c.cshtml`
  - [ ] `Calculators/Hp48.cshtml`
  - [ ] `Calculators/Prime.cshtml`
  - [ ] `Calculators/NspireCx.cshtml`
  - [ ] `Calculators/FxCg50.cshtml`
- [ ] Verify Lite Pages:
  - [ ] `Lite/Simple.cshtml`
  - [ ] `Lite/AiSimple.cshtml`
- [ ] Test Theme Toggle visual functionality
- [ ] Verify active navigation state bindings

## Running the Server

(This is for me, not for anyone else.)

### Locally

To run the server locally, use `dotnet run` from inside the `EvansWebpage` directory:

```bash
cd EvansWebpage
dotnet run --urls "http://localhost:5050"
```

The site will be available on `http://localhost:5050` (or whatever ports you configure in `Properties/launchSettings.json`).

### Running on-VM

To run in production on Yugoslavia:

1.  ```bash
    cd EvansWebpage
    dotnet publish -c Release
    ```
2.  The compiled binary will be located in `bin/Release/net10.0/publish/`. I will eventually set it up to run continually in the background via a `systemd` service.
3.  Make sure Caddy is config'd right.

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
