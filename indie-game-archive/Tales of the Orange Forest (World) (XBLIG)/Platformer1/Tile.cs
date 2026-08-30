using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal struct Tile(Texture2D texture, TileCollision collision)
{
	public const int Width = 64;

	public const int Height = 48;

	public Texture2D Texture = texture;

	public TileCollision Collision = collision;

	public static readonly Vector2 Size = new Vector2(64f, 48f);
}
