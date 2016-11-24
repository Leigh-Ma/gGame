using System;
using UnityEngine;

public class gGlobal
{	
	public static string ServerAddress  = "192.168.1.103";
	public static int ServerPort        = 7788;

	public static int MaxReConnectTimes = 5;
	public static int SocketRecvBuffSize= 4096;

	public static Int64 MyPlayerId      = 737346;

	public static UInt32 ServerTime;

	public static void SetServerTime(UInt32 time) {
		ServerTime = time;
		Debug.Log ("gGlobal set ServerTime = " + ServerTime.ToString ());
	}
}
