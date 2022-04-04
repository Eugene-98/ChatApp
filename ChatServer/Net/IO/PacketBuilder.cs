using System;
using System.IO;
using System.Text;

namespace ChatServer.Net.IO
{
	internal class PacketBuilder
	{
		private MemoryStream ms;
		public PacketBuilder()
		{
			ms = new MemoryStream();
		}

		public void WriteOpCode(byte opcode)
		{
			ms.WriteByte(opcode);
		}

		public void WriteMessage (string msg)
		{
			var msgLenght = msg.Length;
			ms.Write(BitConverter.GetBytes(msgLenght));
			ms.Write(Encoding.ASCII.GetBytes(msg));
		}

		public byte[] GetPacketBytes()
		{
			return ms.ToArray();
		}
	}
}