using UnityEngine;
using System.Collections;
using gNet;

using System.IO;
using UnityEngine.UI;

public class Dispatcher : gMsgDispatch{}

public class ServerTime : MonoBehaviour {
	public Text svrTime;

	void Start () {

		svrTime = GameObject.Find ("Canvas/Text").GetComponent<Text> ();

		LoginReq req = new LoginReq ();
		req.user_id = gGlobal.MyPlayerId.ToString ();
		req.uuid = "xxxxxxxxxxxxxxxxxuuid";

		Dispatcher.SendNetMsg (gNetMsgType.MT_LoginReq, req);
	}

	void Update() {
		if (svrTime != null) {
			svrTime.text = "ServerTime: " + gGlobal.ServerTime.ToString ();
		} else {
			Debug.Log ("can not find server time text to set");
		}
	}

	void test() {
		LoginReq req = new LoginReq ();
		req.user_id = gGlobal.MyPlayerId.ToString ();
		req.uuid = "xxxxxxxxxxxxxxxxxuuid";
		Debug.Log ("test: origin LoginReq:{user_id:" + req.user_id + ", uuid: " + req.uuid + "}");

		byte[] data;

		using(MemoryStream m = new MemoryStream()) {
			ProtoBuf.Serializer.Serialize <LoginReq>(m, req);
			data = m.ToArray ();
		}
		Debug.Log ("test: proto serialized data len: %" + data.Length.ToString());
		using(MemoryStream m = new MemoryStream(data)) {
			LoginReq rr = ProtoBuf.Serializer.Deserialize<LoginReq> (m);
			Debug.Log ("test: restore LoginReq:{user_id:" + rr.user_id + ", uuid: " + rr.uuid + "}");
		}
	}
}
