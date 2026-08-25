using Microsoft.Xna.Framework.Net;

namespace SpaceBlast.Networking;

internal class TimeSyncServer
{
	private LocalNetworkGamer m_Sender;

	private PacketReader m_PacketReader;

	private PacketWriter m_PacketWriter;

	public TimeSyncServer(LocalNetworkGamer sender, PacketReader reader, PacketWriter writer)
	{
		m_Sender = sender;
		m_PacketReader = reader;
		m_PacketWriter = writer;
	}

	public void ProcessTimeSyncRequest(NetworkGamer from)
	{
		ushort value = m_PacketReader.ReadUInt16();
		m_PacketWriter.Write((byte)2);
		m_PacketWriter.Write((int)(TimeManager.TotalSeconds * 1000.0));
		m_PacketWriter.Write(value);
		m_Sender.SendData(m_PacketWriter, SendDataOptions.InOrder, from);
	}
}
