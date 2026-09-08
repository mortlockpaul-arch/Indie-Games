using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.TileSets;

public abstract class TileSet
{
	public static TileSet Cave = new TileSetCave();

	public static TileSet Wooden = new TileSetWooden();

	public static TileSet Poison = new TileSetPoison();

	public static TileSet Mine = new TileSetMine();

	public static TileSet Lava = new TileSetLava();

	public static TileSet Dungeon = new TileSetDungeon();

	protected Dictionary<TileId, Tile> Tiles = new Dictionary<TileId, Tile>();

	public abstract TileSetType Type { get; }

	public static TileSet GetTileSet(TileSetType type)
	{
		return type switch
		{
			TileSetType.Cave => Cave, 
			TileSetType.Wooden => Wooden, 
			TileSetType.Poison => Poison, 
			TileSetType.Mine => Mine, 
			TileSetType.Lava => Lava, 
			TileSetType.Dungeon => Dungeon, 
			_ => throw new Exception("Unknown tile set type: " + type), 
		};
	}

	public TileSet()
	{
		Add(TileId.Wall, TileType.Wall);
		Add(TileId.BorderNSEW, TileType.Wall);
		Add(TileId.BorderNE, TileType.Wall);
		Add(TileId.BorderSE, TileType.Wall);
		Add(TileId.BorderSW, TileType.Wall);
		Add(TileId.BorderNW, TileType.Wall);
		Add(TileId.BorderNS, TileType.Wall);
		Add(TileId.BorderEW, TileType.Wall);
		Add(TileId.BorderN, TileType.Wall);
		Add(TileId.BorderS, TileType.Wall);
		Add(TileId.BorderE, TileType.Wall);
		Add(TileId.BorderW, TileType.Wall);
		Add(TileId.BorderSEW, TileType.Wall);
		Add(TileId.BorderNEW, TileType.Wall);
		Add(TileId.BorderNSW, TileType.Wall);
		Add(TileId.BorderNSE, TileType.Wall);
		Add(TileId.BorderN2, TileType.Wall);
		Add(TileId.BorderS2, TileType.Wall);
		Add(TileId.BorderE2, TileType.Wall);
		Add(TileId.BorderW2, TileType.Wall);
		Add(TileId.ClosedDoorNS, TileType.Door);
		Add(TileId.ClosedDoorEW, TileType.Door);
		Add(TileId.OpenDoorNS, TileType.Floor);
		Add(TileId.OpenDoorEW, TileType.Floor);
		Add(TileId.SecretDoorNS, TileType.Door);
		Add(TileId.SecretDoorEW, TileType.Door);
		Add(TileId.Floor, TileType.Floor);
		Add(TileId.Floor1, TileType.Floor);
		Add(TileId.Floor2, TileType.Floor);
		Add(TileId.Floor3, TileType.Floor);
		Add(TileId.Floor4, TileType.Floor);
		Add(TileId.Floor5, TileType.Floor);
		Add(TileId.Floor6, TileType.Floor);
		Add(TileId.Floor7, TileType.Floor);
	}

	protected void Add(TileId id, TileType type)
	{
		Tiles.Add(id, new Tile(id, type, GetSprite(id)));
	}

	public Tile GetTile(TileId id)
	{
		return Tiles[id];
	}

	public Tile RandomFloor()
	{
		TileId key = DM.Random.Next(7) switch
		{
			0 => TileId.Floor1, 
			1 => TileId.Floor2, 
			2 => TileId.Floor3, 
			3 => TileId.Floor4, 
			4 => TileId.Floor5, 
			5 => TileId.Floor6, 
			6 => TileId.Floor7, 
			_ => TileId.Floor, 
		};
		return Tiles[key];
	}

	protected abstract Sprite GetSprite(TileId id);

	public virtual void Update(GameTime gameTime)
	{
	}

	public virtual void DrawTileEffect(SpriteBatch spriteBatch, Tile tile, Location loc, Vector2 pos, Color color, float scale)
	{
	}
}
