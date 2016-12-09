using UnityEngine;
using System.Collections;
using gNet;

using System.IO;
using UnityEngine.UI;

public class Dispatcher : gMsgDispatch{}

public class ServerTime : MonoBehaviour {
	public Text svrTime;
	public string serverId;
	int frame = 0;

	void Start () {

		svrTime = GameObject.Find ("Canvas/Text").GetComponent<Text> ();

	}

	void Update() {
		if (svrTime != null) {
			svrTime.text = "ServerTime: " + gGlobal.ServerTime.ToString ();
		} else {
			Debug.Log ("can not find server time text to set");
		}
		if (0 == frame % 50) {
			test_login ();
		}
		frame += 1;
	}

	void test_login() {
		LoginReq req = new LoginReq ();
		req.user_id = "0x" + System.Convert.ToString (gGlobal.MyPlayerId, 16);
		req.uuid = "uuidxxxxxxxxxxxxxxxxx";
		req.server_id = serverId;
		Debug.Log (req.user_id + req.uuid + req.server_id);
		Dispatcher.SendNetMsg (gNetMsgType.MT_LoginReq, req);
	}
		
}
