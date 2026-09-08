using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.NPCs;

public class SandWorm : NPC
{
	private AnimatedSprite goingUp;

	private AnimatedSprite move;

	private AnimatedSprite attack;

	private static Texture2D texture;

	private static StillSprite frozen;

	public override string Name => "Sand Worm";

	protected override NPCSprite.Info SpriteInfo => null;

	public SandWorm(BinaryReader reader)
		: base(reader)
	{
		init();
	}

	public SandWorm(int level, Location loc)
		: base(level, loc)
	{
		MoveSpeed = 3.5f;
		MaxHP = base.Level * 5;
		HP = MaxHP;
		Stats.DMG = new DMGRange(base.Level * 4 + 15, base.Level * 4 + 15);
		Stats.DEX = base.Level;
		Stats.DEF = base.Level;
		Stats.LCK = 0;
		AdjustDifficulty();
		init();
	}

	private void init()
	{
		goingUp = new AnimatedSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(100.0));
		move = new AnimatedSprite(texture, new Rectangle(0, 64, 64, 64), new Vector2(32f, 32f), 2, TimeSpan.FromMilliseconds(250.0));
		attack = new AnimatedSprite(texture, new Rectangle(128, 64, 64, 64), new Vector2(32f, 32f), 2, TimeSpan.FromMilliseconds(250.0));
		if (Status.Is<StatusFreeze>())
		{
			goingUp.Frame = 7;
			activeSprite = frozen;
		}
		else if ((double)base.Location.Distance(DM.Player.Location) < 4.0)
		{
			goingUp.Frame = 7;
			activeSprite = move;
		}
		else
		{
			activeSprite = goingUp;
		}
	}

	public static void Load(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Mobs\\SandWorm");
		frozen = new StillSprite(texture, new Rectangle(256, 64, 64, 64), new Vector2(32f, 32f));
	}

	public override void UpdateSprite(GameTime gameTime)
	{
		if (Status.Is<StatusFreeze>())
		{
			activeSprite = frozen;
		}
		else if (goingUp.Frame == 7 && base.IsAttacking)
		{
			attack.Update(gameTime);
			activeSprite = attack;
		}
		else if ((double)base.Location.Distance(DM.Player.Location) < 4.0)
		{
			if (goingUp.Frame < 7)
			{
				goingUp.Direction = 1;
				goingUp.Update(gameTime);
				activeSprite = goingUp;
			}
			else
			{
				move.Update(gameTime);
				activeSprite = move;
			}
		}
		else
		{
			if (goingUp.Frame > 0)
			{
				goingUp.Direction = -1;
				goingUp.Update(gameTime);
			}
			activeSprite = goingUp;
		}
	}

	protected override Item RareLoot()
	{
		return new PotionHealth();
	}
}
