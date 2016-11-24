using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using ProtoBuf;

namespace gNet{
	
	public delegate void NetMsgCallBack(gNetMsg msg);

	public class gNetConnector{
		const int PAYER_ID_BYTES = 8;
		NetMsgCallBack onReceiveMsgCb;
		Thread receiveThread;
		Thread sendThread;
		Socket clientSocket;

		string serverAddr;
	    int serverPort;
		bool isConnected;

		int reTryTimes;

		byte[] recvBuffer = new byte[gGlobal.SocketRecvBuffSize];
		gNetBuffer recvHelper = new gNetBuffer();
		gNetMsg recvedMsg = new gNetMsg ();

		Queue<gNetMsg> sendQueue = new Queue<gNetMsg>();

		public gNetConnector(NetMsgCallBack cb) {
			onReceiveMsgCb = cb;
		}

		public void DoConnect(string addr, int port){
			onClose ();

			serverAddr = addr;
			serverPort = port;
			onConnect ();
		}

		public void DoClose() {
			onClose ();
		}

		public void AsynSendMessage(gNetMsgType msgType, ProtoBuf.IExtensible data) {
			gNetMsg msg = new gNetMsg () {
				type = msgType,
				content = gPB.pbEncode (data),
			};

			lock (sendQueue) {
				sendQueue.Enqueue (msg);
			}
		}

	    void onClose() {
			if (!isConnected) {
				return;
			}

			if (receiveThread != null) {
				receiveThread.Abort ();
				receiveThread = null;
			}

			if (sendThread != null) {
				sendThread.Abort ();
				sendThread = null;
			}

			if (clientSocket != null) {
				if (clientSocket.Connected) {
					clientSocket.Close ();
				}
				clientSocket = null;
			}

			isConnected = false;
		}

		void onConnectTimeOut() {
			reTryTimes += 1;
			onClose ();
			if (reTryTimes < gGlobal.MaxReConnectTimes) {
				onConnect ();
			}
		}

		void onConnectFailure(Exception reason) {
		}

		void onReConnect() {
			reTryTimes += 1;
			onClose ();
			if (reTryTimes < gGlobal.MaxReConnectTimes) {
				onConnect ();
			}
		}

		void onConnectSuccess(IAsyncResult iar) {
			try
			{
				Socket client = (Socket)iar.AsyncState;
				client.EndConnect(iar);

				receiveThread = new Thread(new ThreadStart(threadReceiveSocket));
				receiveThread.IsBackground = true;
				receiveThread.Start();

				sendThread = new Thread(new ThreadStart(threadSendSocket));
				sendThread.IsBackground = true;
				sendThread.Start();

				isConnected = true;
				reTryTimes = 0;
				//Debug.Log("connect success");
			}
			catch (Exception)
			{
				onClose();
			}
		}

		private void onConnect() {
			try{

				//Debug.Log("begin to connect");
				clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
						ProtocolType.Tcp);

				IAsyncResult r =  clientSocket.BeginConnect(serverAddr, serverPort, 
						onConnectSuccess, clientSocket);

				if (!r.AsyncWaitHandle.WaitOne(5000,true)) {
					onConnectTimeOut();
				}


			} catch (Exception e) {
				onConnectFailure (e);
			}
		}

		void threadReceiveSocket() {
			int recvSize;
			while (true) {
				if (!clientSocket.Connected) {
					isConnected = false;
					onReConnect ();
					break;
				}

				try {
					recvSize = clientSocket.Receive(this.recvBuffer);
					if( recvSize > 0 ) {
						this.recvHelper.AppendData(recvBuffer, recvSize);
						while(recvHelper.GetOneNetMsg(out this.recvedMsg)) {
							//Debug.Log ("recv message: size:" + recvedMsg.size.ToString() + ", type:" + recvedMsg.type.ToString());
							if (onReceiveMsgCb != null){
								onReceiveMsgCb(recvedMsg);
							}
						}
					}

				} catch(Exception) {
					clientSocket.Disconnect(true);
					clientSocket.Shutdown(SocketShutdown.Both);
					clientSocket.Close();
					break;
				}

			}
		}

		void threadSendSocket() {
			while (true) {
				if (!clientSocket.Connected) {
					this.isConnected = false;
					onReConnect ();
					break;
				}

				if (sendQueue.Count > 0) {
					try {
						sendMessage (sendQueue.Dequeue ());
					} catch (Exception ) {
						clientSocket.Disconnect (true);
						clientSocket.Shutdown (SocketShutdown.Both);
						clientSocket.Close ();
						break;
					}
				} else {
					Thread.Sleep (50);
				}
			}
		}


	    void sendMessage(gNetMsg msg) {
			int totalSize = msg.size + gNetMsgHead.HEAD_BYTES + PAYER_ID_BYTES;
			int sendSize  = 0;

			Int64 playerId = IPAddress.HostToNetworkOrder (gGlobal.MyPlayerId);

			byte[] data = new byte[totalSize];

			Array.Copy(BitConverter.GetBytes(playerId), 0, data, 0, PAYER_ID_BYTES);

			Array.Copy(gNetMsgHead.EncodeHead(msg), 0, data, PAYER_ID_BYTES, gNetMsgHead.HEAD_BYTES);

			Array.Copy (msg.content, 0, data, PAYER_ID_BYTES + gNetMsgHead.HEAD_BYTES, msg.size);

			while (sendSize < totalSize) {
				sendSize += clientSocket.Send (data, sendSize, totalSize - sendSize, SocketFlags.None);
			}
		}

	}
}