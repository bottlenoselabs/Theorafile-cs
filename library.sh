#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
if [[ -z $SCRIPTS_DIRECTORY ]]; then
    SCRIPTS_DIRECTORY="$DIRECTORY/ext/scripts"
    git clone "https://github.com/bottlenoselabs/scripts" "$SCRIPTS_DIRECTORY" 2> /dev/null 1> /dev/null || git -C "$SCRIPTS_DIRECTORY" pull 1> /dev/null
fi

. $SCRIPTS_DIRECTORY/utility.sh

if [[ ! -z "$1" ]]; then
    TARGET_BUILD_OS="$1"
else
    TARGET_BUILD_OS="host"
fi

if [[ ! -z "$2" ]]; then
    TARGET_BUILD_ARCH="$2"
else
    TARGET_BUILD_ARCH="default"
fi

$SCRIPTS_DIRECTORY/c/library/main.sh \
    $DIRECTORY/src/c/production/Theorafile \
    $DIRECTORY/build \
    $DIRECTORY/lib \
    "theorafile" \
    "theorafile" \
    $TARGET_BUILD_OS \
    $TARGET_BUILD_ARCH \
    $CMAKE_SDL2_INCLUDE_DIRS $CMAKE_SDL2_LIBRARIES "-DCMAKE_OSX_DEPLOYMENT_TARGET=10.9"