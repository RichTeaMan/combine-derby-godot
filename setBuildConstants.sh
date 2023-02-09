#!/bin/bash


COMMIT_HASH=$(git rev-parse --short HEAD)
BUILD_DATE=$(date)

echo "build hash: $COMMIT_HASH"
git show

sed -i "s/var commit_hash = \"dev\"/var commit_hash = \"$COMMIT_HASH\"/" singletons/build_info.gd
sed -i "s/var build_time = \"dev\"/var build_time = \"$BUILD_DATE\"/" singletons/build_info.gd

