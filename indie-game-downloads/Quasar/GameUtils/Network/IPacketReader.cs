using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.GameUtils.Network;

public interface IPacketReader
{
	byte ReadByte();

	bool ReadBoolean();

	Quaternion ReadQuaternion();

	int ReadInt32();

	long ReadInt64();

	short ReadInt16();

	string ReadString();

	Vector3 ReadVector3();

	float ReadSingle();

	Int2 ReadInt2();

	Vector2 ReadVector2();
}
