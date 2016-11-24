#!/bin/sh

SCRIPT_NAME=$0
TARGET_DIR_2_LEVEL_ABOVE=Game/Handler

SCRIPT_DIR=${SCRIPT_NAME%/*}
PROTO_FILE=protocol.txt

OUTPUT_FILE=

function doAwkAnalysis() {
bash << EOF
    cat ${PROTO_FILE} | grep -v "^[ \t]*#" | grep ":" | awk -F"[,;]" '${printFunction} {
                for(i=1; i < NF; i++) {
                split(\$i, kv, ":");
                for(j=1; j<=2; j++) {
                    if(kv[j] ~ /[ \t]+"/) {
                        gsub(/[ \t]+"/, "\"", kv[j]);
                    } else {
                        gsub(/[ \t]+/,  "", kv[j]);
                    }
                }

                key = kv[1]; value = kv[2];
                delete kv;
                if(key == "code") {
                    printACode(codeDes);
                    curCode = value;
                }
                codeDes[key] = value;
            }
        }END{
            printACode(codeDes);
        }'
EOF
}

#=======================================================
#BEGIN TO PROCESS FILE
#=======================================================

#=======================================================
# second: generate msg handler map for net
#=======================================================
cd ${SCRIPT_DIR}
OUTPUT_FILE=gNetMsgHandlerRegister.cs
PACKAGE=gNet

cat << EOF | tee ${OUTPUT_FILE}
//Auto generated, do not modify unless you know clearly what you are doing.
using System;
using gNet;

public static partial class gNetMsgHandler
{
    public static void Register()
    {
EOF

printFunction='function printACode(codeDesc){
            reaction = codeDes["reaction"]
            if(reaction  == "") {
                delete codeDesc
                return
            }
            printf("        gMsgDispatch.AddNetMsgHandler (gNetMsgType.MT_%s, Handle_%s);\n",
                            reaction, reaction);

            delete codeDesc
        }'

doAwkAnalysis | tee -a ${OUTPUT_FILE}
cat << EOF | tee -a ${OUTPUT_FILE}
    }
}
EOF
mv ${OUTPUT_FILE} ../../${TARGET_DIR_2_LEVEL_ABOVE}/

#=======================================================
# third: generate msg handler for each net payload
#=======================================================
OUTPUT_FILE=gNetMsgHandler.cs.auto

cat << EOF | tee ${OUTPUT_FILE}
//Auto generated, do not modify unless you know clearly what you are doing.
using System;
using gNet;

public static partial class gNetMsgHandler
{
EOF

printFunction='function printACode(codeDesc){
            reaction = codeDes["reaction"]
            if(reaction  == "") {
                delete codeDesc
                return
            }
            printf("        public static void Handle_%s(gNetMsg msg) {\n", reaction);
            printf("            %s ack = gPB.pbDecode<%s> (msg.content);\n", reaction, reaction);
            printf("            //TODO\n        }\n\n")
            delete codeDesc
        }'

doAwkAnalysis | tee -a ${OUTPUT_FILE}
cat << EOF | tee -a ${OUTPUT_FILE}
    }
}
EOF
mv ${OUTPUT_FILE} ../../${TARGET_DIR_2_LEVEL_ABOVE}/
