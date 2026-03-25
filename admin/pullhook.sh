#!/bin/bash

# This shell script will be run each time I push changes to main and GitHub sends a webhook.

# Move into here
cd /etc/web/destroyere/evans-webpage || exit

# Pull changes from main
git pull origin main