using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public abstract class Weapon : Equipment
{
	public override string Name => (!Identified) ? "Weapon (?)" : name;

	protected abstract WeaponSprite WeaponSprite { get; }

	public override Sprite Sprite => WeaponSprite.Sprite;

	public Weapon(string name)
		: base(Equipment.RandomTier(), name)
	{
	}

	public Weapon(EquipmentTier tier, string name)
		: base(tier, name)
	{
	}

	public Weapon(BinaryReader reader, string name)
		: base(reader, name)
	{
	}

	public void DrawAttack(SpriteBatch spriteBatch, Vector2 offset, float scale, float rotation)
	{
		if (DM.Player.Frenzy)
		{
			for (int num = 4; num >= 0; num--)
			{
				Color color = Color.White * (float)(1.0 - (double)num * 0.20000000298023224);
				WeaponSprite.AttackSprite.Rotation = DM.Player.SkillSet.Frenzy.Rotation - (float)((double)num * Math.PI / 8.0);
				WeaponSprite.AttackSprite.Draw(spriteBatch, offset, color, scale);
			}
		}
		else
		{
			WeaponSprite.AttackSprite.Rotation = rotation;
			WeaponSprite.AttackSprite.Draw(spriteBatch, offset, Color.White, scale);
		}
	}
}
