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

	public static int numDamageRecv = 0;

	private static Vector3 DamageDirection = Vector3.Zero;

	public override void Send()
	{
	}
}
