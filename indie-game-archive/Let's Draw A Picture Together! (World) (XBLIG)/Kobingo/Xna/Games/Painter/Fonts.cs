using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Games.Painter;

internal class Fonts
{
	public static SpriteFont DefaultFont { get; private set; }

	public static SpriteFont HeaderFont { get; private set; }

	public static void Load(GraphicsDevice graphicsDevice, ContentManager content)
	{
		DefaultFont = content.Load<SpriteFont>("Fonts/DefaultFont");
		HeaderFont = content.Load<SpriteFont>("Fonts/HeaderFont");
	}
}
