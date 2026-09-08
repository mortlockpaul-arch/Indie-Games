using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Net;

internal class NetworkSimulator
{
	private class LatentPacket : IComparable<LatentPacket>
	{
		public long SendTime;

		public byte[] Data;

		public int Size;

		public SendDataOptions Options;

		public LocalNetworkGamer Sender;

		public NetworkGamer Recipient;

		public bool IsStillValid
		{
			get
			{
				if (Sender.HasLeftSession)
				{
					return false;
				}
				if (Recipient != null && Recipient.HasLeftSession)
				{
					return false;
				}
				return true;
			}
		}

		public int CompareTo(LatentPacket other)
		{
			long num = SendTime - other.SendTime;
			if (num < 0)
			{
				return -1;
			}
			if (num > 0)
			{
				return 1;
			}
			return 0;
		}
	}

	private float simulatedPacketLoss;

	private TimeSpan simulatedLatency;

	private long simulatedLatencyTicks;

	private Random random = new Random();

	private List<LatentPacket> unorderedPackets = new List<LatentPacket>();

	private Queue<LatentPacket> encryptedOrderedPackets = new Queue<LatentPacket>();

	private Queue<LatentPacket> unencryptedOrderedPackets = new Queue<LatentPacket>();

	private Stack<LatentPacket> packetPool = new Stack<LatentPacket>();

	public float SimulatedPacketLoss
	{
		get
		{
			return simulatedPacketLoss;
		}
		set
		{
			if (value < 0f || value > 1f)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			simulatedPacketLoss = value;
		}
	}

	public TimeSpan SimulatedLatency
	{
		get
		{
			return simulatedLatency;
		}
		set
		{
			if (value < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			simulatedLatency = value;
			simulatedLatencyTicks = value.Ticks * Stopwatch.Frequency / 10000000;
		}
	}

	public bool SendData(byte[] data, int offset, int count, SendDataOptions options, LocalNetworkGamer sender, NetworkGamer recipient)
	{
		if (simulatedPacketLoss > 0f && (options & SendDataOptions.Reliable) == 0 && random.NextDouble() <= (double)simulatedPacketLoss)
		{
			return true;
		}
		if (simulatedLatencyTicks > 0)
		{
			StoreLatentPacket(data, offset, count, options, sender, recipient);
			return true;
		}
		return false;
	}

	private void StoreLatentPacket(byte[] data, int offset, int count, SendDataOptions options, LocalNetworkGamer sender, NetworkGamer recipient)
	{
		lock (this)
		{
			LatentPacket latentPacket = ((packetPool.Count <= 0) ? new LatentPacket() : packetPool.Pop());
			latentPacket.SendTime = Stopwatch.GetTimestamp() + NormallyDistributedRandomLatency();
			latentPacket.Size = count;
			latentPacket.Options = options;
			latentPacket.Sender = sender;
			latentPacket.Recipient = recipient;
			if (latentPacket.Data == null || latentPacket.Data.Length < count)
			{
				latentPacket.Data = new byte[count];
			}
			Array.Copy(data, offset, latentPacket.Data, 0, count);
			if ((options & SendDataOptions.InOrder) != SendDataOptions.None)
			{
				if ((options & SendDataOptions.Chat) != SendDataOptions.None)
				{
					unencryptedOrderedPackets.Enqueue(latentPacket);
				}
				else
				{
					encryptedOrderedPackets.Enqueue(latentPacket);
				}
				return;
			}
			int num = unorderedPackets.BinarySearch(latentPacket);
			if (num < 0)
			{
				num = ~num;
			}
			unorderedPackets.Insert(num, latentPacket);
		}
	}

	private long NormallyDistributedRandomLatency()
	{
		long num = simulatedLatencyTicks / 4;
		long num2 = simulatedLatencyTicks * 2;
		double d = random.NextDouble();
		double num3 = random.NextDouble();
		double num4 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Cos(Math.PI * 2.0 * num3);
		num4 *= (double)num;
		num4 += (double)simulatedLatencyTicks;
		long num5 = (long)num4;
		if (num5 < 0)
		{
			return 0L;
		}
		if (num5 > num2)
		{
			return num2;
		}
		return num5;
	}

	public void Update()
	{
		if (unorderedPackets.Count == 0 && encryptedOrderedPackets.Count == 0 && unencryptedOrderedPackets.Count == 0)
		{
			return;
		}
		long timestamp = Stopwatch.GetTimestamp();
		lock (this)
		{
			while (unorderedPackets.Count > 0 && TrySendLatentPacket(unorderedPackets[0], timestamp))
			{
				unorderedPackets.RemoveAt(0);
			}
			TrySendLatentPackets(encryptedOrderedPackets, timestamp);
			TrySendLatentPackets(unencryptedOrderedPackets, timestamp);
		}
	}

	private void TrySendLatentPackets(Queue<LatentPacket> packetQueue, long currentTime)
	{
		while (packetQueue.Count > 0 && TrySendLatentPacket(packetQueue.Peek(), currentTime))
		{
			packetQueue.Dequeue();
		}
	}

	private bool TrySendLatentPacket(LatentPacket packet, long currentTime)
	{
		long num = currentTime - packet.SendTime;
		if (num < 0)
		{
			return false;
		}
		if (packet.IsStillValid)
		{
			packet.Sender.SendDataNow(packet.Data, 0, packet.Size, packet.Options, packet.Recipient);
		}
		packetPool.Push(packet);
		return true;
	}
}
