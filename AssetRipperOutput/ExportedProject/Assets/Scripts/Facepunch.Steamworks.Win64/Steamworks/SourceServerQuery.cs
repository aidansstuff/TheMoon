using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	internal static class SourceServerQuery
	{
		private static readonly byte[] A2S_SERVERQUERY_GETCHALLENGE = new byte[5] { 85, 255, 255, 255, 255 };

		private const byte A2S_RULES = 86;

		private static readonly Dictionary<IPEndPoint, Task<Dictionary<string, string>>> PendingQueries = new Dictionary<IPEndPoint, Task<Dictionary<string, string>>>();

		internal static Task<Dictionary<string, string>> GetRules(ServerInfo server)
		{
			IPEndPoint endpoint = new IPEndPoint(server.Address, server.QueryPort);
			lock (PendingQueries)
			{
				if (PendingQueries.TryGetValue(endpoint, out var value))
				{
					return value;
				}
				Task<Dictionary<string, string>> task = GetRulesImpl(endpoint).ContinueWith(delegate(Task<Dictionary<string, string>> t)
				{
					lock (PendingQueries)
					{
						PendingQueries.Remove(endpoint);
					}
					return t;
				}).Unwrap();
				PendingQueries.Add(endpoint, task);
				return task;
			}
		}

		private static async Task<Dictionary<string, string>> GetRulesImpl(IPEndPoint endpoint)
		{
			try
			{
				using UdpClient client = new UdpClient();
				client.Client.SendTimeout = 3000;
				client.Client.ReceiveTimeout = 3000;
				client.Connect(endpoint);
				return await GetRules(client);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static async Task<Dictionary<string, string>> GetRules(UdpClient client)
		{
			byte[] challengeBytes = await GetChallengeData(client);
			challengeBytes[0] = 86;
			await Send(client, challengeBytes);
			byte[] ruleData = await Receive(client);
			Dictionary<string, string> rules = new Dictionary<string, string>();
			using (BinaryReader br = new BinaryReader(new MemoryStream(ruleData)))
			{
				if (br.ReadByte() != 69)
				{
					throw new Exception("Invalid data received in response to A2S_RULES request");
				}
				ushort numRules = br.ReadUInt16();
				for (int index = 0; index < numRules; index++)
				{
					rules.Add(br.ReadNullTerminatedUTF8String(), br.ReadNullTerminatedUTF8String());
				}
			}
			return rules;
		}

		private static async Task<byte[]> Receive(UdpClient client)
		{
			byte[][] packets = null;
			do
			{
				byte[] buffer = (await client.ReceiveAsync()).Buffer;
				using BinaryReader br = new BinaryReader(new MemoryStream(buffer));
				switch (br.ReadInt32())
				{
				case -1:
				{
					byte[] unsplitdata = new byte[buffer.Length - br.BaseStream.Position];
					Buffer.BlockCopy(buffer, (int)br.BaseStream.Position, unsplitdata, 0, unsplitdata.Length);
					return unsplitdata;
				}
				case -2:
				{
					br.ReadInt32();
					byte packetNumber = br.ReadByte();
					byte packetCount = br.ReadByte();
					br.ReadInt32();
					if (packets == null)
					{
						packets = new byte[packetCount][];
					}
					byte[] data = new byte[buffer.Length - br.BaseStream.Position];
					Buffer.BlockCopy(buffer, (int)br.BaseStream.Position, data, 0, data.Length);
					packets[packetNumber] = data;
					break;
				}
				default:
					throw new Exception("Invalid Header");
				}
			}
			while (packets.Any((byte[] p) => p == null));
			return Combine(packets);
		}

		private static async Task<byte[]> GetChallengeData(UdpClient client)
		{
			await Send(client, A2S_SERVERQUERY_GETCHALLENGE);
			byte[] challengeData = await Receive(client);
			if (challengeData[0] != 65)
			{
				throw new Exception("Invalid Challenge");
			}
			return challengeData;
		}

		private static async Task Send(UdpClient client, byte[] message)
		{
			byte[] sendBuffer = new byte[message.Length + 4];
			sendBuffer[0] = byte.MaxValue;
			sendBuffer[1] = byte.MaxValue;
			sendBuffer[2] = byte.MaxValue;
			sendBuffer[3] = byte.MaxValue;
			Buffer.BlockCopy(message, 0, sendBuffer, 4, message.Length);
			await client.SendAsync(sendBuffer, message.Length + 4);
		}

		private static byte[] Combine(byte[][] arrays)
		{
			byte[] array = new byte[arrays.Sum((byte[] a) => a.Length)];
			int num = 0;
			foreach (byte[] array2 in arrays)
			{
				Buffer.BlockCopy(array2, 0, array, num, array2.Length);
				num += array2.Length;
			}
			return array;
		}
	}
}
