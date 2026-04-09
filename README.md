# Evan's Webpage - Overkill Branch

> [!NOTE]
> This branch is dedicated to transitioning from plain HTML to ASP.NET. As of now, a full ASP.NET transition is way
> overkill, thus the branch name.

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
