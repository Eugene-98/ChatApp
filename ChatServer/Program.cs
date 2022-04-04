using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ChatServer.Net.IO;

namespace ChatServer
{
	internal class Program
	{
		static List<Client> users = new List<Client>();
		private static TcpListener _listener;
		static void Main(string[] args)
		{
			users = new List<Client>();
			_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7800);
			_listener.Start();

			while (true)
			{
				var client = new Client(_listener.AcceptTcpClient());
				users.Add(client);

				BroadcastConnection();
			}
		}

		private static void BroadcastConnection()
		{
			foreach (var user in users)
			{
				foreach (var usr in users)
				{
					var broadcastPacket = new PacketBuilder();
					broadcastPacket.WriteOpCode(1);
					broadcastPacket.WriteMessage(usr.Username);
					broadcastPacket.WriteMessage(usr.UserId.ToString());

					user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
				}
			}
		}
		public static void BroadcastDisconnect(string userId)
		{
			var disconnectedUser = users.Where(x => x.UserId.ToString() == userId).FirstOrDefault();
			users.Remove(disconnectedUser);
			foreach (var user in users)
			{
				var broadcastPacket = new PacketBuilder();
				broadcastPacket.WriteOpCode(10);
				broadcastPacket.WriteMessage(userId);
				user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
			}

			BroadcastMessage($"[{disconnectedUser.Username}] Disconnected!");
		}

		public static void BroadcastMessage(string message)
		{
			foreach (var user in users)
			{
				var msgPacket = new PacketBuilder();
				msgPacket.WriteOpCode(5);
				msgPacket.WriteMessage(message);
				user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
			}
		}
	}
}
