using System;
using System.Collections;
using Microsoft.Xna.Framework.Net;

namespace SpaceBlast.Networking;

internal class TimeSyncClient
{
	private const int constLatencyArraySize = 12;

	private PacketReader m_PacketReader;

	private PacketWriter m_PacketWriter;

	private LocalNetworkGamer m_Sender;

	private NetworkGamer m_Host;

	private ArrayList m_Latencies;

	private int m_NextLatencyIndex;

	private ushort m_PacketSequenceNumber = 2;

	private int m_SentTime;

	public TimeSyncClient(LocalNetworkGamer sender, NetworkGamer host, PacketReader reader, PacketWriter writer)
	{
		m_Sender = sender;
		m_Host = host;
		m_PacketReader = reader;
		m_PacketWriter = writer;
		m_Latencies = new ArrayList(12);
		for (int i = 0; i < 12; i++)
		{
			m_Latencies.Add(-1);
		}
	}

	public void HostChanged(NetworkGamer newhost)
	{
		m_Host = newhost;
	}

	public void SendTimeSyncRequest(int realTimeMs)
	{
		try
		{
			m_PacketWriter.Write((byte)1);
			m_PacketSequenceNumber++;
			m_PacketWriter.Write(m_PacketSequenceNumber);
			m_Sender.SendData(m_PacketWriter, SendDataOptions.InOrder, m_Host);
			m_SentTime = realTimeMs;
		}
		catch (Exception)
		{
		}
	}

	public void ProcessTimeSyncResponse(int realTimeMs)
	{
		int latency = (realTimeMs - m_SentTime) / 2;
		int num = m_PacketReader.ReadInt32();
		ushort num2 = m_PacketReader.ReadUInt16();
		if (num2 == m_PacketSequenceNumber)
		{
			int num3 = CalcAverageLatency(latency);
			TimeManager.SetTime((double)(num + num3) / 1000.0);
		}
	}

	private int CalcAverageLatency(int latency)
	{
		m_Latencies[m_NextLatencyIndex++] = latency;
		if (m_NextLatencyIndex >= 12)
		{
			m_NextLatencyIndex = 0;
		}
		int num = 0;
		float num2 = 0f;
		foreach (int latency2 in m_Latencies)
		{
			if (latency2 != -1)
			{
				num++;
				num2 += (float)latency2;
				continue;
			}
			break;
		}
		float num4 = num2 / (float)num;
		float num5 = num4 * 1.5f;
		num = 0;
		num2 = 0f;
		foreach (int latency3 in m_Latencies)
		{
			if (latency3 != -1)
			{
				if (!((float)latency3 > num5))
				{
					num++;
					num2 += (float)latency3;
				}
				continue;
			}
			break;
		}
		num4 = num2 / (float)num;
		return (int)num4;
	}
}
