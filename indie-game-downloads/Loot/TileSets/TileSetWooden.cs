using System.Collections.Generic;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.TileSets;

public class TileSetWooden : TileSet
{
	private static Dictionary<TileId, Sprite> Sprites = new Dictionary<TileId, Sprite>();

	public override TileSetType Type => TileSetType.Wooden;

	protected override Sprite GetSprite(TileId id)
	{
		return Sprites[id];
	}

	public static void LoadContent(ContentManager content)
	{
		Vector2 value = new Vector2(32f, 32f);
		Texture2D texture = content.Load<Texture2D>("Sprites\\Tiles\\Wooden");
		Sprites[TileId.Wall] = new StillSprite(texture, new Rectangle(4, 4, 64, 64), value);
		Sprites[TileId.BorderNSEW] = new StillSprite(texture, new Rectangle(72, 4, 64, 64), value);
		Sprites[TileId.BorderNE] = new StillSprite(texture, new Rectangle(140, 4, 64, 64), value);
		Sprites[TileId.BorderSE] = new StillSprite(texture, new Rectangle(208, 4, 64, 64), value);
		Sprites[TileId.BorderSW] = new StillSprite(texture, new Rectangle(4, 72, 64, 64), value);
		Sprites[TileId.BorderNW] = new StillSprite(texture, new Rectangle(72, 72, 64, 64), value);
		Sprites[TileId.BorderNS] = new StillSprite(texture, new Rectangle(140, 72, 64, 64), value);
		Sprites[TileId.BorderEW] = new StillSprite(texture, new Rectangle(208, 72, 64, 64), value);
		Sprites[TileId.BorderN] = new StillSprite(texture, new Rectangle(4, 140, 64, 64), value);
		Sprites[TileId.BorderS] = new StillSprite(texture, new Rectangle(72, 140, 64, 64), value);
		Sprites[TileId.BorderE] = new StillSprite(texture, new Rectangle(140, 140, 64, 64), value);
		Sprites[TileId.BorderW] = new StillSprite(texture, new Rectangle(208, 140, 64, 64), value);
		Sprites[TileId.BorderSEW] = new StillSprite(texture, new Rectangle(4, 208, 64, 64), value);
		Sprites[TileId.BorderNEW] = new StillSprite(texture, new Rectangle(72, 208, 64, 64), value);
		Sprites[TileId.BorderNSW] = new StillSprite(texture, new Rectangle(140, 208, 64, 64), value);
		Sprites[TileId.BorderNSE] = new StillSprite(texture, new Rectangle(208, 208, 64, 64), value);
		Sprites[TileId.BorderN2] = Sprites[TileId.BorderN];
		Sprites[TileId.BorderS2] = Sprites[TileId.BorderS];
		Sprites[TileId.BorderE2] = Sprites[TileId.BorderE];
		Sprites[TileId.BorderW2] = Sprites[TileId.BorderW];
		Sprites[TileId.Floor] = new StillSprite(texture, new Rectangle(4, 276, 64, 64), value);
		Sprites[TileId.Floor1] = new StillSprite(texture, new Rectangle(72, 276, 64, 64), value);
		Sprites[TileId.Floor2] = new StillSprite(texture, new Rectangle(140, 276, 64, 64), value);
		Sprites[TileId.Floor3] = new StillSprite(texture, new Rectangle(208, 276, 64, 64), value);
		Sprites[TileId.Floor4] = new StillSprite(texture, new Rectangle(4, 344, 64, 64), value);
		Sprites[TileId.Floor5] = new StillSprite(texture, new Rectangle(72, 344, 64, 64), value);
		Sprites[TileId.Floor6] = new StillSprite(texture, new Rectangle(140, 344, 64, 64), value);
		Sprites[TileId.Floor7] = new StillSprite(texture, new Rectangle(208, 344, 64, 64), value);
		Sprites[TileId.ClosedDoorNS] = new StillSprite(texture, new Rectangle(4, 412, 64, 64), value);
		Sprites[TileId.ClosedDoorEW] = new StillSprite(texture, new Rectangle(72, 412, 64, 64), value);
		Sprites[TileId.OpenDoorNS] = new StillSprite(texture, new Rectangle(140, 412, 64, 64), value);
		Sprites[TileId.OpenDoorEW] = new StillSprite(texture, new Rectangle(208, 412, 64, 64), value);
		Sprites[TileId.SecretDoorNS] = Sprites[TileId.BorderNS];
		Sprites[TileId.SecretDoorEW] = Sprites[TileId.BorderEW];
	}
}
