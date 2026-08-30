using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MaximinusDataTypes;

public class TextureLayeredReader : ContentTypeReader<TextureLayered>
{
	protected override TextureLayered Read(ContentReader input, TextureLayered existingInstance)
	{
		int num = input.ReadInt32();
		Layer[] array = new Layer[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = new Layer(input.ReadObject<Texture2D>(), input.ReadString(), input.ReadBoolean());
		}
		return new TextureLayered(array);
	}
}
