﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatClient.Net.IO;

namespace ChatClient.Net
{
	internal class Server
	{
		TcpClient _client;
		public PacketReader PacketReader;

		public event Action connectedEvent;
		public event Action messageEvent;
		public event Action disconnectedEvent;

		public Server()
		{
			_client = new TcpClient();
		}

		public void ConnectToServer(string username)
		{
			if (!_client.Connected)
			{
				_client.Connect("127.0.0.1", 7800);
				PacketReader = new PacketReader(_client.GetStream());

				if (!string.IsNullOrEmpty(username))
				{
					var connectPacket = new PacketBuilder();
					connectPacket.WriteOpCode(0);
					connectPacket.WriteMessage(username);
					_client.Client.Send(connectPacket.GetPacketBytes());
				}

				ReadPackets();

			}
		}

		private void ReadPackets()
		{
			Task.Run(() =>
			{
				while (true)
				{
					var opcode = PacketReader.ReadByte();
					switch (opcode)
					{
						case 1:
							connectedEvent?.Invoke();
							break;
						case 5:
							messageEvent?.Invoke();
							break;
						case 10:
							disconnectedEvent?.Invoke();
							break;
						default:
							Console.WriteLine("error");
							break;
					}
				}
			});
		}


		public void SendMessageToServer(string message)
		{
			var messagePacket = new PacketBuilder();
			messagePacket.WriteOpCode(5);
			messagePacket.WriteMessage(message);
			_client.Client.Send(messagePacket.GetPacketBytes());
		}
	}
}
