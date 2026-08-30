using Microsoft.Xna.Framework.Content;

namespace GKEngine.Edit.Gysmos;

public class GysmoModelReader : ContentTypeReader<GysmoModel>
{
	protected override GysmoModel Read(ContentReader input, GysmoModel existingInstance)
	{
		return new GysmoModel(input);
	}
}
