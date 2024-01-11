#!/bin/bash

EXPORT_NAME=combine_derby

rm build -rf
rm .godot -rf

mkdir -p build/linux

echo "Build imports"
godot -v --headless --editor --quit

echo "Export 1"
godot -v --headless --export-release "Linux/X11" build/linux/$EXPORT_NAME.x86_64

echo "Export 2"
godot -v --headless --export-release "Linux/X11" build/linux/$EXPORT_NAME.x86_64

echo "Complete"
ls build/linux/ -l
