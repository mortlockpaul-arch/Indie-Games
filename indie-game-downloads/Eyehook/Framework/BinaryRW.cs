using System.IO;

namespace Eyehook.Framework;

public interface BinaryRW
{
	void Read(BinaryReader reader);

	void Write(BinaryWriter writer);
}
