#!/bin/bash

if [ $# -ne 1 ]
then
    echo "Usage: $0 <dest>"
    exit 1
fi

DEST_DIR=${1}

if [ ! -d plugin/plugin-srcs ]
then
    echo "ERROR: can not find plugin/plugin-srcs"
    exit 1
fi

if [ ! -d ${DEST_DIR}/ ]
then
    echo "ERROR: can not find ${DEST_DIR}/"
    exit 1
fi

cp plugin/plugin-srcs/*.json ${DEST_DIR}/
cp -rp plugin/plugin-srcs/ros_types ${DEST_DIR}/

exit 0
