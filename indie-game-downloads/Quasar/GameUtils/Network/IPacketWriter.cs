using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.GameUtils.Network;

public interface IPacketWriter
{
	void Write(byte value);

	void Write(int value);

	void Write(long value);

	void Write(short value);

	void Write(string value);

	void Write(Vector3 value);

	void Write(bool value);

	void Write(Quaternion value);

	void Write(float value);

	void Write(Int2 value);

	void Write(Vector2 value);
}
