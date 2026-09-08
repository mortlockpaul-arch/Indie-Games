using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public abstract class Armor : Equipment
{
	public override string Name => (!Identified) ? "Armor (?)" : name;

	protected abstract ArmorSprite ArmorSprite { get; }

	public override Sprite Sprite => ArmorSprite.S;

	public Armor(string name)
		: base(Equipment.RandomTier(), name)
	{
	}

	public Armor(EquipmentTier tier, string name)
		: base(tier, name)
	{
	}

	public Armor(BinaryReader reader, string name)
		: base(reader, name)
	{
	}

	public void DrawOnPlayer(SpriteBatch spriteBatch, Vector2 position, float scale)
	{
		(DM.Player.Facing switch
		{
			Direction.North => ArmorSprite.N, 
			Direction.South => ArmorSprite.S, 
			Direction.East => ArmorSprite.E, 
			Direction.West => ArmorSprite.W, 
			_ => throw new Exception("Unknown direction: " + DM.Player.Facing), 
		}).Draw(spriteBatch, position, Color.White, scale);
	}

	public void DrawOnPlayer(SpriteBatch spriteBatch, Direction facing, Vector2 position, float scale)
	{
		(facing switch
		{
			Direction.North => ArmorSprite.N, 
			Direction.South => ArmorSprite.S, 
			Direction.East => ArmorSprite.E, 
			Direction.West => ArmorSprite.W, 
			_ => throw new Exception("Unknown direction: " + DM.Player.Facing), 
		}).Draw(spriteBatch, position, Color.White, scale);
	}
}
