using Microsoft.Xna.Framework.Content;

namespace BinaryRead;

public class BinaryReader : ContentTypeReader<Data>
{
	protected override Data Read(ContentReader input, Data data)
	{
		Data data2 = new Data();
		data2.Read(input);
		return data2;
	}
}
