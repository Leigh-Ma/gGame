using System;
using System.IO;
using System.Net;

namespace gNet {

	public struct gNetMsg {
		public gNetMsgType type;
		public int size;
		public byte[] content;
	}

	public class gNetMsgHead{
		
		public static int TYPE_BYTES = 4;
		public static int SIZE_BYTES = 4;

		public static int HEAD_BYTES {
			get{ return TYPE_BYTES + SIZE_BYTES;}
		}

		public static int OFFSET_TYPE {
			get{ return 0; }
		}

		public static int OFFSET_SIZE {
			get{ return TYPE_BYTES; }
		}

		public static bool DecodeHead(byte[] headBytes, ref gNetMsg msg){
			int type, size;

			if(headBytes.Length >= HEAD_BYTES) {
				type = BitConverter.ToInt32 (headBytes, OFFSET_TYPE);
				size = BitConverter.ToInt32 (headBytes, OFFSET_SIZE);

				msg.type = (gNetMsgType)IPAddress.NetworkToHostOrder (type);
				msg.size = IPAddress.NetworkToHostOrder (size);

				return true;
			}

			return false;
		}

		public static byte[] EncodeHead(gNetMsg msg) {
			int type = IPAddress.HostToNetworkOrder ((int)msg.type);
			int size = IPAddress.HostToNetworkOrder (msg.size);

			byte[] typeBytes = BitConverter.GetBytes (type);
			byte[] sizeBytes = BitConverter.GetBytes (size);

			byte[] headBytes = new byte[HEAD_BYTES];
			Array.Copy (typeBytes, 0, headBytes, OFFSET_TYPE, TYPE_BYTES);
			Array.Copy (sizeBytes, 0, headBytes, OFFSET_SIZE, SIZE_BYTES);

			return headBytes;
		}
	}

}