using System;
using System.IO;
using Loot.Items;
using Loot.NPCs;
using Loot.TileSets;
using Loot.Widgets;
using Microsoft.Xna.Framework;

namespace Loot.Dungeon;

public struct MapCell
{
	public static readonly TimeSpan FadeInDuration = TimeSpan.FromMilliseconds(350.0);

	public bool Discovered;

	public Tile Tile;

	public Ornament Ornament;

	public Widget Widget;

	public Item Item;

	public NPC NPC;

	public void Update(GameTime gameTime)
	{
		if (Item != null)
		{
			Item.Update(gameTime);
		}
		if (Widget != null)
		{
			Widget.Update(gameTime);
		}
	}

	public void Read(BinaryReader reader, TileSet tileSet)
	{
		Discovered = reader.ReadBoolean();
		Tile = tileSet.GetTile((TileId)reader.ReadByte());
		int num = reader.ReadInt32();
		Ornament = ((num == -1) ? null : Loot.Dungeon.Ornament.GetOrnament(num));
		Widget = WidgetRegistry.Load(reader);
		Item = ItemRegistry.Load(reader);
		NPC = NPCRegistry.Load(reader);
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(Discovered);
		writer.Write((byte)Tile.Id);
		writer.Write((Ornament == null) ? (-1) : Ornament.Id);
		WidgetRegistry.Save(writer, Widget);
		ItemRegistry.Save(writer, Item);
		NPCRegistry.Save(writer, NPC);
	}
}
