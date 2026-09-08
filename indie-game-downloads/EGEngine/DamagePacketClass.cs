using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class DamagePacketClass : NetworkPacket
{
	private byte[] localData = new byte[5];

	public int Damage;

	public byte Damager;

	public DamegePacketType DamageType;

	private static PlayerBase playerRef;

	private static NetworkGamer damagedGamer;

	private static NetworkGamer damagerGamer;

	public static int numDamageRecv;

	private static Vector3 DamageDirection;

	public override void Send()
	{
	}

	static DamagePacketClass()
	{
		numDamageRecv = 0;
		DamageDirection = Vector3.Zero;
	}
}
