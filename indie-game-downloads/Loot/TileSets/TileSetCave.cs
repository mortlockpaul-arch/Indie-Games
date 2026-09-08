using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.TileSets;

public class TileSetCave : TileSet
{
	private static Dictionary<TileId, Sprite> Sprites = new Dictionary<TileId, Sprite>();

	private static Sprite[] sparkle;

	public override TileSetType Type => TileSetType.Cave;

	protected override Sprite GetSprite(TileId id)
	{
		return Sprites[id];
	}

	public static void LoadContent(ContentManager content)
	{
		Vector2 value = new Vector2(32f, 32f);
		Texture2D texture = content.Load<Texture2D>("Sprites\\Tiles\\Cave");
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
		sparkle = new Sprite[2];
		sparkle[0] = new StillSprite(texture, new Rectangle(412, 4, 64, 64), value);
		sparkle[1] = new StillSprite(texture, new Rectangle(412, 72, 64, 64), value);
	}

	public override void DrawTileEffect(SpriteBatch spriteBatch, Tile tile, Location loc, Vector2 pos, Color color, float scale)
	{
		if (DM.Player.Depth >= 25 && (tile.IsWall() || tile.IsSecretDoor()))
		{
			Vector2 vector = DungeonView.Center - pos;
			float num = (float)(((double)vector.Length() - 64.0 * (double)scale) / (64.0 * (double)scale));
			float num2 = DM.Player.Lantern.Radius * 4f;
			if (!((double)num > (double)num2))
			{
				float num3 = (float)(1.0 - (double)num / (double)num2);
				float num4 = (float)Math.Atan2(vector.Y, vector.X) + DM.TwoPiTimer / 2f;
				float num5 = MathHelper.Clamp((float)(Math.Abs(Math.Sin(num4)) - 0.33000001311302185) * 2f, 0f, 1f);
				float num6 = MathHelper.Clamp((float)(Math.Abs(Math.Sin((double)num4 - 1.5707963705062866)) - 0.33000001311302185) * 2f, 0f, 1f);
				sparkle[0].Draw(spriteBatch, pos, Color.White * num5 * num3, scale);
				sparkle[1].Draw(spriteBatch, pos, Color.White * num6 * num3, scale);
			}
		}
	}
}
