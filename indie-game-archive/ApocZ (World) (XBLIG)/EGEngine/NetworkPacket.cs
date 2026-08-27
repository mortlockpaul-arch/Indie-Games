using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class NetworkPacket
{
	private byte bit3Mask = 4;

	private byte bit4Mask = 8;

	public bool use;

	public byte Id;

	public byte SenderId;

	protected void InCodeBuffer(byte[] e)
	{
		int num = e.Length - 1;
		if ((e[0] & bit3Mask) > 0 && (e[num] & bit4Mask) > 0 && ((e[0] & bit4Mask) <= 0 || (e[1] & bit3Mask) <= 0))
		{
			byte b = e[num];
			for (int i = num; i > 0; i++)
			{
				e[i] = e[i - 1];
			}
			e[0] = b;
		}
	}

	protected void DeCodeBuffer(byte[] e)
	{
		int num = e.Length - 1;
		if ((e[0] & bit4Mask) > 0 && (e[1] & bit3Mask) > 0)
		{
			byte b = e[0];
			for (int i = 0; i < num; i++)
			{
				e[i] = e[i + 1];
			}
			e[num] = b;
		}
	}

	public virtual void Initialize<T>()
	{
	}

	public virtual void QueuePacket()
	{
	}

	public virtual void Send()
	{
	}

	public virtual void ReadData(PacketReader pReader, LocalNetworkGamer gamer)
	{
	}

	public virtual void WriteData(PacketWriter pWriter, LocalNetworkGamer gamer)
	{
	}
}
