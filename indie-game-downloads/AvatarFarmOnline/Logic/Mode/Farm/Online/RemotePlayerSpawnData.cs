using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal struct RemotePlayerSpawnData(AvatarFarmOnline.Logic.Stage.Player p)
{
	private short playerId = p.Selection.Id;

	private Vector2 position = p.Position;

	private int level = p.Level;

	public short PlayerId => playerId;

	public Vector2 Position => position;

	public int Level => level;

	public void WritePacket(IPacketWriter packetWriter)
	{
		packetWriter.Write(playerId);
		packetWriter.Write(position);
		packetWriter.Write((short)level);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData ReadPacket(IPacketReader packetReader)
	{
		return new AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData
		{
			playerId = packetReader.ReadInt16(),
			position = packetReader.ReadVector2(),
			level = packetReader.ReadInt16()
		};
	}
}
