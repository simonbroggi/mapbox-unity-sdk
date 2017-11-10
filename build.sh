#!/usr/bin/env bash
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod CreateBuild.BuildNow
zip -r Build.zip Build