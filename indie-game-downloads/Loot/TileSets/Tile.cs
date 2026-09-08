using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.TileSets;

public class Tile
{
	public readonly TileId Id;

	public readonly TileType Type;

	public readonly Sprite Sprite;

	public Tile(TileId id, TileType type, Sprite sprite)
	{
		Id = id;
		Type = type;
		Sprite = sprite;
	}

	public bool IsWall()
	{
		return Type == TileType.Wall;
	}

	public bool IsDoor()
	{
		return Type == TileType.Door;
	}

	public bool IsSecretDoor()
	{
		return Id == TileId.SecretDoorNS || Id == TileId.SecretDoorEW;
	}

	public bool IsOpenDoor()
	{
		return Id == TileId.OpenDoorEW || Id == TileId.OpenDoorNS;
	}

	public bool IsFloor()
	{
		return Type == TileType.Floor;
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
	{
		Sprite.Draw(spriteBatch, position, color, scale);
	}
}
