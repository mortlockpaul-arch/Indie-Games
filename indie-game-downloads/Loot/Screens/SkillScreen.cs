using System;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Loot.Dungeon;
using Loot.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class SkillScreen : SlidingScreen
{
	public bool Disabled;

	private Skill[,] Skills;

	private int row;

	private int col;

	private static Texture2D pixel;

	private static Sprite star;

	private static Sprite skill10;

	private static Sprite box;

	private static Sprite upA;

	private float twoPiTimer;

	private Color shadowColor = Color.Black * 0.5f;

	private Vector2 skillWheelOffset = new Vector2(-16f, -6f);

	private static Color titleTextColor = new Color(151, 100, 0);

	private static Color titleTextShadowColor = new Color(255, 255, 0);

	private static Vector2 titleShadowOffset = new Vector2(2f, 2f);

	private Vector2 longNameOffset = new Vector2(0f, -244f);

	private static readonly Color pointColor = Color.Black;

	private static readonly Vector2 pointOffset = new Vector2(60f, 0f);

	private static readonly Vector2 starOffset = new Vector2(-40f, -44f);

	private static readonly Color selectedColor = new Color(255, 204, 0);

	private static readonly Vector2 arrowOffset = new Vector2(42f, -46f);

	private string pointText = "Points: ";

	private Vector2 pointsOffset = new Vector2(0f, 236f);

	public SkillScreen()
		: base(new Vector2(1512f, 362f), new Vector2(932f, 362f), TimeSpan.FromMilliseconds(500.0))
	{
		SkillSet skillSet = DM.Player.SkillSet;
		Skills = new Skill[3, 3]
		{
			{ skillSet.Perception, skillSet.Orb, skillSet.StatBoost },
			{ skillSet.Freeze, skillSet.Regen, skillSet.Frenzy },
			{ skillSet.Stealth, skillSet.Poison, skillSet.Thorns }
		};
		setRowCol(DM.Player.PrimarySkill ?? skillSet.Regen);
	}

	private void setRowCol(Skill skill)
	{
		for (int i = 0; i < Skills.GetLength(0); i++)
		{
			for (int j = 0; j < Skills.GetLength(1); j++)
			{
				if (Skills[i, j] == skill)
				{
					row = i;
					col = j;
					return;
				}
			}
		}
		row = 0;
		col = 0;
	}

	private Skill getCurSkill()
	{
		return Skills[row, col];
	}

	public static void Load(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
		upA = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\UpA"));
		star = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\Star"));
		skill10 = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\Skill10"));
		box = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\SkillBox"));
	}

	public override void update(GameTime gameTime)
	{
		base.update(gameTime);
		twoPiTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds * 6.2831854820251465);
		if ((double)twoPiTimer > 6.2831854820251465)
		{
			twoPiTimer -= (float)Math.PI * 2f;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.RightShoulder))
		{
			if (base.IsOff || base.IsSlideOff)
			{
				SetMode(Mode.SlideOn);
			}
			else if (base.IsOn || base.IsSlideOn)
			{
				SetMode(Mode.SlideOff);
			}
		}
		else
		{
			if (!base.IsOn || Disabled)
			{
				return;
			}
			int length = Skills.GetLength(0);
			int length2 = Skills.GetLength(1);
			if (MC.GamePadManager.isNewDirRight())
			{
				if (col < length2 - 1)
				{
					col++;
				}
			}
			else if (MC.GamePadManager.isNewDirLeft())
			{
				if (col > 0)
				{
					col--;
				}
			}
			else if (MC.GamePadManager.isNewDirUp())
			{
				if (row > 0)
				{
					row--;
				}
			}
			else if (MC.GamePadManager.isNewDirDown() && row < length - 1)
			{
				row++;
			}
			Skill curSkill = getCurSkill();
			if (DM.Player.SkillPoints <= 0 || curSkill.Level >= 10 || !MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				return;
			}
			PlaySound.MenuClick();
			DM.Player.SkillPoints--;
			if (curSkill == DM.Player.PrimarySkill)
			{
				curSkill.Level += 2;
			}
			else
			{
				curSkill.Level++;
			}
			if (curSkill is StatBoostSkill statBoostSkill)
			{
				statBoostSkill.BoostStats();
			}
			if (curSkill.Level == 10)
			{
				Skill skill = curSkill;
				Skill skill2 = skill;
				if (!(skill2 is PoisonSkill))
				{
					if (!(skill2 is FrenzySkill))
					{
						if (!(skill2 is FreezeSkill))
						{
							if (!(skill2 is OrbSkill))
							{
								if (skill2 is RegenSkill)
								{
									Profile.Awardments.Unlock(Awardment.RegenMaster);
								}
							}
							else
							{
								Profile.Awardments.Unlock(Awardment.OrbMaster);
							}
						}
						else
						{
							Profile.Awardments.Unlock(Awardment.FreezeMaster);
						}
					}
					else
					{
						Profile.Awardments.Unlock(Awardment.FrenzyMaster);
					}
				}
				else
				{
					Profile.Awardments.Unlock(Awardment.PoisonMaster);
				}
			}
			SkillSet skillSet = DM.Player.SkillSet;
			if (skillSet.Poison.Level > 0 && skillSet.Frenzy.Level > 0 && skillSet.Freeze.Level > 0 && skillSet.Orb.Level > 0 && skillSet.Regen.Level > 0 && skillSet.Perception.Level > 0 && skillSet.StatBoost.Level > 0 && skillSet.Stealth.Level > 0 && skillSet.Thorns.Level > 0)
			{
				Profile.Awardments.Unlock(Awardment.JackOfAllTrades);
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		box.Draw(base.spriteBatch, Offset);
		drawName();
		drawSkills();
		drawPoints();
		if (Disabled)
		{
			box.Draw(base.spriteBatch, Offset, shadowColor, 1f);
		}
		base.spriteBatch.End();
	}

	private void drawName()
	{
		Skill curSkill = getCurSkill();
		Text.DrawCentered(base.spriteBatch, Offset + longNameOffset + titleShadowOffset, curSkill.Name, titleTextShadowColor);
		Text.DrawCentered(base.spriteBatch, Offset + longNameOffset, curSkill.Name, titleTextColor);
	}

	private void drawSkills()
	{
		Vector2 vector = Offset + skillWheelOffset;
		SkillSet skillSet = DM.Player.SkillSet;
		for (int i = 0; i < Skills.GetLength(0); i++)
		{
			for (int j = 0; j < Skills.GetLength(1); j++)
			{
				Skill skill = Skills[i, j];
				Vector2 vector2 = new Vector2(j * 128 - 128, i * 128 - 128);
				if (i == row && j == col)
				{
					highlight(base.spriteBatch, vector + vector2);
				}
				skill.Sprite.Draw(base.spriteBatch, vector + vector2);
				drawSkillPoints(skill.Level, vector + vector2 + pointOffset);
				if (DM.Player.PrimarySkill == skill)
				{
					star.Draw(base.spriteBatch, vector + vector2 + starOffset);
				}
				if (i == row && j == col)
				{
					arrow(base.spriteBatch, vector + vector2);
				}
			}
		}
	}

	private void drawSkillPoints(int level, Vector2 pos)
	{
		if (level == 10)
		{
			skill10.Draw(base.spriteBatch, pos);
		}
		else
		{
			Text.Draw(base.spriteBatch, pos, level, pointColor);
		}
	}

	private void highlight(SpriteBatch spriteBatch, Vector2 pos)
	{
		spriteBatch.Draw(pixel, new Rectangle((int)pos.X - 34, (int)pos.Y - 34, 68, 68), selectedColor);
		spriteBatch.Draw(pixel, new Rectangle((int)pos.X + 46, (int)pos.Y - 18, 28, 36), selectedColor);
	}

	private void arrow(SpriteBatch spriteBatch, Vector2 pos)
	{
		if (!Disabled && DM.Player.SkillPoints > 0)
		{
			upA.Draw(spriteBatch, pos + arrowOffset + new Vector2(0f, 4f) * (float)Math.Sin(twoPiTimer));
		}
	}

	private void drawPoints()
	{
		Vector2 v = Offset + pointsOffset + new Vector2(-((Text.Width(pointText) + Text.Width(DM.Player.StatPoints) - 24) / 2), 0f);
		Text.Draw(base.spriteBatch, ref v, pointText, Color.Black);
		Text.Draw(base.spriteBatch, ref v, DM.Player.SkillPoints, Color.Black);
	}
}
