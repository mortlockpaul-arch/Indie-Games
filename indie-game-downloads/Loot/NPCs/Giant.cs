using System.IO;
using Loot.Core;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Amulets;
using Loot.Items.Rings;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.NPCs;

public class Giant : NPC
{
	private static Color hpBarBgColor = new Color(255, 255, 255);

	public override string Name => "Giant";

	protected override NPCSprite.Info SpriteInfo => NPCSprite.Giant;

	public Giant(BinaryReader reader)
		: base(reader)
	{
	}

	public Giant(int level, Location loc)
		: base(level, loc)
	{
		MoveSpeed = 4f;
		MaxHP = base.Level * 4 + 30;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 3 + 30, base.Level * 3 + 30);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
	}

	protected override void DrawHPBar(SpriteBatch spriteBatch, Vector2 pos, Color color)
	{
		if (Profile.Preferences.HPBar)
		{
			float zoom = DungeonView.Camera.Zoom;
			Rectangle rectangle = new Rectangle((int)((double)pos.X - 24.0 * (double)zoom), (int)((double)pos.Y - 96.0 * (double)zoom), (int)(48.0 * (double)zoom), (int)(4.0 * (double)zoom));
			Pixel.Draw(spriteBatch, rectangle, color);
			rectangle.Width = (int)((double)rectangle.Width * (double)HP / (double)MaxHP);
			if (Status.Is<StatusPoison>())
			{
				spriteBatch.Draw(NPC.hpBarGreen, rectangle, color);
			}
			else
			{
				spriteBatch.Draw(NPC.hpBarRed, rectangle, color);
			}
		}
	}

	protected override Item RareLoot()
	{
		if (DM.Player.Depth < 40)
		{
			return new Gold();
		}
		return (DM.Random.Next(100) < 50) ? ((Equipment)new AmuletBone()) : ((Equipment)new RingRoyal());
	}
}
