#!/bin/bash

set -e

docker build -t godot .

docker run -it --rm -v "$(pwd):/proj"  --entrypoint bash godot
