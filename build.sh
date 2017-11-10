#!/usr/bin/env bash
jenkins ALL=NOPASSWD: /bin/sh, build.sh
sudo /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod CreateBuild.BuildNow
zip -r Build.zip Build