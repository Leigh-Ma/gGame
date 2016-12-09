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
    cat .tmp.cs >> ${payload%.*}.cs
    rm .tmp.cs
    rm ${PACKAGE_NAME}.proto
    mv ${payload%.*}.cs ../NetMsg/
done


