using UnityEngine;
using System.Collections;

namespace gEvent{
	public delegate void GameEventCallBack(gGameEvent evt);

	public struct gGameEvent {
		public gGameEventType type;
		public int size;
		public Object obj;
	}

	public class gEvent{

	}
}
