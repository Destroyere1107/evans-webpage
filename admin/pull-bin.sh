#!/bin/bash
set -e

# This is an exact copy of the same pull-bin.sh run on the production server every time GitHub webhooks into it.

# Configuration
REPO="destroyere1107/evans-webpage"
TARGET_DIR="/home/evan/web"
BINARY_NAME="EvansWebpage"
ARTIFACT_NAME="evans-webpage-bin" 

cd $TARGET_DIR || exit

echo "Fetching latest artifact from GitHub"
gh run download --name "$ARTIFACT_NAME" --repo "$REPO" --dir ./temp_deploy

if [ -f "./temp_deploy/$BINARY_NAME" ]; then
    echo "New binary found. Starting update"
    
    # 1. Stop the existing process
    sudo systemctl stop evans-webpage.service

    # 2. Replace the old binary with the new one
    mv ./temp_deploy/$BINARY_NAME ./$BINARY_NAME
    chmod +x ./$BINARY_NAME

    # 3. Start new binary
    sudo systemctl start evans-webpage.service
    
    echo "Cleaning up..."
    rm -rf ./temp_deploy
    echo "Deployment successful"
else
    echo "Error: Binary '$BINARY_NAME' not found in the downloaded artifact."
    rm -rf ./temp_deploy
    exit 1
fi