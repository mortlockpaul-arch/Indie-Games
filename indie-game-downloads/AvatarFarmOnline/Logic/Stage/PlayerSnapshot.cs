using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Stage;

internal struct PlayerSnapshot
{
	public Vector2 position;

	public float rotation;

	public AvatarFarmOnline.Logic.Stage.Player.PlayerState playerState;

	public Vector2 speed;

	public bool isRunning;

	public void WritePacket(IPacketWriter writer)
	{
		writer.Write(position);
		writer.Write(rotation);
		writer.Write((byte)playerState);
		writer.Write(speed);
		writer.Write(isRunning);
	}

	public void ReadPacket(IPacketReader reader)
	{
		position = reader.ReadVector2();
		rotation = reader.ReadSingle();
		playerState = (AvatarFarmOnline.Logic.Stage.Player.PlayerState)reader.ReadByte();
		speed = reader.ReadVector2();
		isRunning = reader.ReadBoolean();
	}
}
