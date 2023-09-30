#!/bin/bash

cd project
godot -v --headless --editor --quit

godot -v --headless --editor --quit

find .godot/ -type f | wc -l
