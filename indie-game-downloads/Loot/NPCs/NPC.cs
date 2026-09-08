using System;
using System.IO;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Loot.Items;
using Loot.PC;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.NPCs;

public abstract class NPC : Character
{
	public bool IsAlly;

	public bool IsEpic;

	private NPCSprite npcSprite;

	protected Sprite activeSprite;

	protected static Texture2D hpBarRed;

	protected static Texture2D hpBarGreen;

	private TimeSpan idleTime = TimeSpan.Zero;

	private static readonly TimeSpan idleDuration = TimeSpan.FromMilliseconds(500.0);

	private static readonly Color poisonedBaseColor = new Color(0, 204, 0);

	public virtual int XP
	{
		get
		{
			int num = (MaxHP / 2 + Stats.DEF + Stats.DEX + (Stats.DMG.Min + Stats.DMG.Max) / 10) / 4;
			if (num <= 0)
			{
				num = 1;
			}
			return num;
		}
	}

	protected abstract NPCSprite.Info SpriteInfo { get; }

	public NPC(int level, Location loc)
	{
		base.level = level;
		SetLocation(loc);
		if (SpriteInfo != null)
		{
			npcSprite = new NPCSprite(SpriteInfo);
			activeSprite = npcSprite.Move;
		}
	}

	public NPC(BinaryReader reader)
		: base(reader)
	{
		if (SpriteInfo != null)
		{
			npcSprite = new NPCSprite(SpriteInfo);
			activeSprite = npcSprite.Move;
		}
	}

	protected void AdjustDifficulty()
	{
		float num = DM.GameOptions.Difficulty switch
		{
			Difficulty.Easy => 0.75f, 
			Difficulty.Normal => 1f, 
			Difficulty.Hard => 1.25f, 
			_ => throw new Exception("Unknown difficulty: " + DM.GameOptions.Difficulty), 
		};
		MaxHP = (int)((double)MaxHP * (double)num);
		HP = MaxHP;
		Stats.DMG = new DMGRange((int)((double)Stats.DMG.Min * (double)num), (int)((double)Stats.DMG.Max * (double)num));
		Stats.DEF = (int)((double)Stats.DEF * (double)num);
		Stats.DEX = (int)((double)Stats.DEX * (double)num);
		Stats.LCK = (int)((double)Stats.LCK * (double)num);
	}

	public static void LoadContent(ContentManager content)
	{
		hpBarRed = content.Load<Texture2D>("Sprites\\Dungeon\\HpBarRed");
		hpBarGreen = content.Load<Texture2D>("Sprites\\Dungeon\\HpBarGreen");
	}

	public virtual void Elite()
	{
		MaxHP *= 2;
		HP = MaxHP;
		Stats.DMG = new DMGRange(Stats.DMG.Min * 2, Stats.DMG.Max * 2);
		IsEpic = true;
	}

	public virtual void Boss()
	{
		MaxHP *= 3;
		HP = MaxHP;
		Stats.DMG = new DMGRange(Stats.DMG.Min * 3, Stats.DMG.Max * 3);
		Stats.DEF = (int)((double)Stats.DEF * 1.5);
		Stats.DEX = (int)((double)Stats.DEX * 1.5);
		IsEpic = true;
	}

	protected virtual Item Loot()
	{
		return new Gold();
	}

	protected virtual Item RareLoot()
	{
		return new Gold();
	}

	public override void OnDeath()
	{
		DM.KillNPC(this);
		int num = (IsEpic ? 66 : 33);
		if (DM.Random.Next(100) < num && DM.Map.GetItem(base.Location) == null && DM.Map.GetWidget(base.Location) == null)
		{
			Item item = ((DM.Random.Next(100) <= 6) ? RareLoot() : Loot());
			if (item != null)
			{
				DM.Map.SetItem(base.Location, item);
			}
		}
	}

	public override void UpdateSprite(GameTime gameTime)
	{
		if (npcSprite != null)
		{
			activeSprite = (Status.Is<StatusFreeze>() ? npcSprite.Frozen : ((!base.IsAttacking) ? npcSprite.Move : npcSprite.Attack));
			activeSprite.Update(gameTime);
		}
	}

	public override void UpdateAI(GameTime gameTime)
	{
		Player player = DM.Player;
		bool flag = !player.Status.Is<StatusInvisible>();
		float num = base.Location.Distance(player.Location);
		if (!IsAlly && (double)num < 1.5)
		{
			if (flag)
			{
				Attack(DM.Player);
				return;
			}
		}
		else if (IsAlly && (double)num < 1.5)
		{
			return;
		}
		float num2 = (float)(5.75 - (double)player.SkillSet.Stealth.Level / 2.5);
		if (((double)num <= (double)num2) & flag)
		{
			Location location = DM.PathToPlayer(base.Location);
			if (location != base.Location)
			{
				DM.MoveNPC(this, location);
				return;
			}
			if (DM.Player.Orb != null && (double)base.Location.Distance(DM.Player.Orb.Location) < 1.5)
			{
				Attack(DM.Player.Orb);
				return;
			}
		}
		else if (DM.Player.Orb != null && (double)base.Location.Distance(DM.Player.Orb.Location) < 1.5)
		{
			Attack(DM.Player.Orb);
			return;
		}
		idleTime += gameTime.ElapsedGameTime;
		if (idleTime > idleDuration)
		{
			idleTime -= idleDuration;
			Location loc = new Location(base.Location.Row + (DM.Random.Next(3) - 1), base.Location.Col + (DM.Random.Next(3) - 1));
			if (DM.Map.IsValid(loc) && DM.Map.IsOpenFloor(loc))
			{
				DM.MoveNPC(this, loc);
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Draw(spriteBatch, Color.White);
	}

	public virtual void Draw(SpriteBatch spriteBatch, Color color)
	{
		if (activeSprite != null)
		{
			Vector2 vector = DungeonView.Camera.Position(this);
			float zoom = DungeonView.Camera.Zoom;
			float num = (float)(int)color.A / 255f;
			if (Status.Is<StatusPoison>())
			{
				color = Color.Lerp(color, poisonedBaseColor, (float)((Math.Sin(DM.TwoPiTimer) + 1.0) / 2.0));
				color *= num;
			}
			DrawHPBar(spriteBatch, vector, color);
			if (IsEpic && HP > 0)
			{
				NPCSprite.Epic.Draw(spriteBatch, vector, Color.White, zoom);
			}
			activeSprite.SpriteEffects = ((base.Location.Col > DM.Player.Location.Col) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			activeSprite.Draw(spriteBatch, vector, color, zoom);
		}
	}

	protected virtual void DrawHPBar(SpriteBatch spriteBatch, Vector2 pos, Color color)
	{
		if (Profile.Preferences.HPBar)
		{
			float zoom = DungeonView.Camera.Zoom;
			Rectangle rectangle = new Rectangle((int)((double)pos.X - 24.0 * (double)zoom), (int)((double)pos.Y - 32.0 * (double)zoom), (int)(48.0 * (double)zoom), (int)(4.0 * (double)zoom));
			Pixel.Draw(spriteBatch, rectangle, color);
			rectangle.Width = (int)((double)rectangle.Width * (double)HP / (double)MaxHP);
			if (Status.Is<StatusPoison>())
			{
				spriteBatch.Draw(hpBarGreen, rectangle, color);
			}
			else
			{
				spriteBatch.Draw(hpBarRed, rectangle, color);
			}
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		IsAlly = reader.ReadBoolean();
		IsEpic = reader.ReadBoolean();
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(IsAlly);
		writer.Write(IsEpic);
	}
}
