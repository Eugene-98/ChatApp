using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Net.IO;

namespace ChatServer
{
	internal class Client
	{
		public string Username { get; set; }
		public Guid UserId { get; set; }
		public TcpClient ClientSocket { get; set; }

		private PacketReader _packedReader;

		public Client(TcpClient client)
		{
			ClientSocket = client;
			UserId = Guid.NewGuid();
			_packedReader = new PacketReader(ClientSocket.GetStream());

			var opcode = _packedReader.ReadByte();
			Username = _packedReader.ReadMessage();

			Console.WriteLine($"{DateTime.Now} : Client has connected with the username: {Username}");

			Task.Run(() => Process());

		}

		void Process()
		{
			while (true)
			{
				try
				{
					var opcode = _packedReader.ReadByte();
					switch (opcode)
					{
						case 5:
							var msg = _packedReader.ReadMessage();
							Console.WriteLine($"{DateTime.Now} : {msg}");
							Program.BroadcastMessage($"{DateTime.Now}: {Username}: {msg}");
							break;
						default:
							break;
					}
				}
				catch (Exception)
				{
					Console.WriteLine($"{UserId.ToString()} : Disconnected!");
					Program.BroadcastDisconnect(UserId.ToString());
					ClientSocket.Close();
				}
			}
		}
	}
}
