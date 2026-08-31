using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetStarUniverse.Sprites;

public interface IGameObject
{
	Vector2 Position { get; set; }

	Vector2 CenterRight { get; }

	Texture2D Texture2D { get; set; }

	bool Hidden { get; set; }

	Rectangle BoxRectangle { get; }

	int Width { get; set; }

	int Height { get; set; }

	bool Hit { get; set; }
}
