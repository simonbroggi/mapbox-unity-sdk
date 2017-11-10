#!/usr/bin/env bash
sudo /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod CreateBuild.BuildNow
zip -r Build.zip Build