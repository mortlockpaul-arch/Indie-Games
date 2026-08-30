using Microsoft.Xna.Framework.Content;

namespace GKEngine.Entities;

public class MaxModelReader : ContentTypeReader<MaxModel>
{
	protected override MaxModel Read(ContentReader input, MaxModel existingInstance)
	{
		return new MaxModel(input);
	}
}
