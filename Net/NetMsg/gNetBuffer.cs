using System;
using UnityEngine;

namespace gNet{
	
	public class gNetBuffer {
		private byte[] buffer;
		private int bufferSize;
		private int bufferOffset;


		public gNetBuffer(int initSize = 1024){
			if (initSize <= 0) {
				this.bufferSize = 1024;
			} else {
				this.bufferSize = initSize;
			}
			this.buffer = new byte[this.bufferSize];
		}

		public void AppendData(byte[] data, int dataLen){
			if(dataLen <= this.bufferSize - this.bufferOffset) {
				Array.Copy(data, this.buffer, dataLen);
				this.bufferOffset += dataLen;
			} else {
				this.bufferSize = this.bufferOffset + dataLen;

				byte[] tmpBuff = new byte[this.bufferSize];
				Array.Copy(buffer, 0, tmpBuff, 0, this.bufferOffset);
				Array.Copy(data, 0, tmpBuff, this.bufferOffset, dataLen);

				this.buffer = tmpBuff;
				this.bufferOffset = this.bufferSize;
			}
		}

		public bool GetOneNetMsg(out gNetMsg msg) {

			msg = new gNetMsg ();

			if (!gNetMsgHead.DecodeHead (this.buffer, ref msg)) {
				return false;
			}
				

			int remain = this.bufferOffset - gNetMsgHead.HEAD_BYTES - msg.size;

			if (remain < 0) {
				return false;
			}

			if (msg.size > 0) {
				msg.content = new byte[msg.size];
				Array.Copy (this.buffer, gNetMsgHead.HEAD_BYTES, msg.content, 0, msg.size);
			}

			if (remain == 0) {
				this.bufferOffset = 0;
				this.buffer.Initialize ();
			} else {
				byte[] residual = new byte[remain];
				Array.Copy (this.buffer, this.bufferOffset - remain, residual, 0, remain);

				this.buffer.Initialize ();
				Array.Copy (this.buffer, 0, residual, 0, remain);
				this.bufferOffset = remain;
			}

			return true;
		}
	}
}