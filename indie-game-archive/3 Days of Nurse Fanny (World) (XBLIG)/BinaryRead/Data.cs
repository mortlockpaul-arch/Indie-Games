using Microsoft.Xna.Framework.Content;

namespace BinaryRead;

public class Data
{
	public byte[] data;

	public void Read(ContentReader input)
	{
		int count = (int)input.BaseStream.Length;
		data = input.ReadBytes(count);
	}
}
