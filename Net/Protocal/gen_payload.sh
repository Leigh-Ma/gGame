#!/bin/sh

SCRIPT_NAME=$0
SCRIPT_DIR=${SCRIPT_NAME%/*}

PACKAGE_NAME=gNet

cd $SCRIPT_DIR

payloadFiles=`ls *.proto`


for payload in $payloadFiles
do
    echo "processing $payload"
    cp $payload ${PACKAGE_NAME}.proto
    protogen -i:${PACKAGE_NAME}.proto -o:.tmp.cs
    cat .tmp.cs >> ${PACKAGE_NAME}.cs
    rm .tmp.cs
done

mv ${PACKAGE_NAME}.cs ../gNetPayLoad.cs


