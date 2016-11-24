//Auto generated, do not modify unless you know clearly what you are doing.
using System;
using UnityEngine;
using gNet;

public static partial class gNetMsgHandler
{

	public static void Handle_LoginAck(gNetMsg msg) {
		LoginAck ack = gPB.pbDecode<LoginAck> (msg.content);
		Debug.Log (ack.common.timeStamp.ToString ());
		gGlobal.SetServerTime (ack.common.timeStamp);
	}

	public static void Handle_LogoutAck(gNetMsg msg) {
		LogoutAck ack = gPB.pbDecode<LogoutAck> (msg.content);
		//TO DO

	}

}
