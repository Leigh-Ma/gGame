using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using gNet;
using gEvent;

public class gMsgDispatch :MonoBehaviour {

	private static gNetConnector connector = new gNetConnector(OnReceiveNetMsg);

	private static Dictionary<gNetMsgType, NetMsgCallBack> netMsgCallbackMap = 
				new Dictionary<gNetMsgType, NetMsgCallBack> ();
	
	private static Dictionary<gGameEventType, GameEventCallBack> gameEvtCallbackMap = 
				new Dictionary<gGameEventType, GameEventCallBack> ();

	private static Queue<gNetMsg> netMsgQueue = new Queue<gNetMsg>();

	private static Queue<gGameEvent> gameEvtQueue = new Queue<gGameEvent>();


	public static void AddNetMsgHandler(gNetMsgType msgType, NetMsgCallBack handler) {
		if (netMsgCallbackMap.ContainsKey(msgType)) {
			netMsgCallbackMap[msgType] += handler;
		} else {
			netMsgCallbackMap.Add(msgType, handler);
		}
	}

	public static void DelNetMsgHandler(gNetMsgType msgType, NetMsgCallBack handler) {
		if (netMsgCallbackMap.ContainsKey (msgType)) {
			netMsgCallbackMap[msgType] -= handler;
			if (netMsgCallbackMap [msgType] == null) {
				netMsgCallbackMap.Remove (msgType);
			}
		}
	}

	public static void AddGameEventCallBack(gGameEventType evtType, GameEventCallBack handler) {
		if (gameEvtCallbackMap.ContainsKey (evtType)) {
			gameEvtCallbackMap[evtType] += handler;
		} else {
			gameEvtCallbackMap.Add (evtType, handler);
		}
	}

	public static void DelGameEventCallBack(gGameEventType evtType, GameEventCallBack handler) {
		if (gameEvtCallbackMap.ContainsKey (evtType)) {
			gameEvtCallbackMap[evtType] -= handler;
			if (gameEvtCallbackMap [evtType] == null) {
				gameEvtCallbackMap.Remove (evtType);
			}
		}
	}

	public static void SendNetMsg(gNetMsgType msgType, ProtoBuf.IExtensible data) {
		connector.AsynSendMessage (msgType, data);
	}

	public static void OnReciveGameEvent(gGameEvent evt) {
		lock (gameEvtQueue) {
			gameEvtQueue.Enqueue (evt);
		}
	}

	public static void CloseConnection() {
		connector.DoClose ();
	}

	private static void OnReceiveNetMsg(gNetMsg msg) {
		lock (netMsgQueue) {
			netMsgQueue.Enqueue (msg);
		}
	}

	void Start() {
		gNetMsgHandler.Register ();
		connector.DoConnect (gGlobal.ServerAddress, gGlobal.ServerPort);
	}

	void Update() {
		
		while (gameEvtQueue.Count > 0) {
			gGameEvent evt = gameEvtQueue.Dequeue ();
			if (gameEvtCallbackMap.ContainsKey (evt.type)) {
				gameEvtCallbackMap [evt.type] (evt);
			}
		}

		while (netMsgQueue.Count > 0) {
			gNetMsg msg = netMsgQueue.Dequeue ();
			Debug.Log ("Message center: get message of type:" + msg.type.ToString ());
			if (netMsgCallbackMap.ContainsKey (msg.type)) {
				netMsgCallbackMap [msg.type] (msg);
			} else {
				Debug.Log ("Message center: no handler for type: " + msg.type.ToString ());
			}
		}
			
	}
}
