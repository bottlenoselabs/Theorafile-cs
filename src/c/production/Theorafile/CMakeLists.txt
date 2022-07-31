cmake_minimum_required(VERSION 3.16)

project(theorafile)
set(CMAKE_C_STANDARD 99)
set (CMAKE_SYSTEM_VERSION 10.0 (latest installed version))

get_filename_component(THEORAFILE_DIRECTORY_PATH "../../../../ext/Theorafile" REALPATH BASE_DIR "${CMAKE_CURRENT_SOURCE_DIR}")

include_directories(theorafile PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/include
    ${THEORAFILE_DIRECTORY_PATH}
    ${THEORAFILE_DIRECTORY_PATH}/lib
    )


message(STATUS "foo include dir: ${THEORAFILE_DIRECTORY_PATH}")

file(GLOB_RECURSE SOURCES
    ${CMAKE_CURRENT_SOURCE_DIR}/src/*.c;
    ${THEORAFILE_DIRECTORY_PATH}/theorafile.c;
    ${THEORAFILE_DIRECTORY_PATH}/lib/**/*.c)

add_library(theorafile SHARED ${SOURCES})