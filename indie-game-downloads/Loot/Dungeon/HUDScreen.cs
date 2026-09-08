using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Statuses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public class HUDScreen : Screen
{
	private const string depthText = "DEPTH:";

	private Sprite depthBg;

	private Sprite potionRT;

	private Texture2D Health;

	private Texture2D XP;

	private Sprite Regen;

	private Sprite LevelUp;

	private Sprite LevelUpLB;

	private Sprite LevelUpRB;

	private Sprite BrokenChain;

	private static readonly Rectangle hpEmptyLeft = new Rectangle(2, 0, 16, 32);

	private static readonly Rectangle hpEmptyMid = new Rectangle(26, 0, 8, 32);

	private static readonly Rectangle hpEmptyRight = new Rectangle(42, 0, 16, 32);

	private static readonly Rectangle hpLeft = new Rectangle(2, 32, 16, 32);

	private static readonly Rectangle hpMid = new Rectangle(26, 32, 8, 32);

	private static readonly Rectangle hpRight = new Rectangle(42, 32, 16, 32);

	private static readonly Rectangle hpEnd = new Rectangle(62, 32, 12, 32);

	private static readonly Rectangle hpPoisonLeft = new Rectangle(2, 64, 16, 32);

	private static readonly Rectangle hpPoisonMid = new Rectangle(26, 64, 8, 32);

	private static readonly Rectangle hpPoisonRight = new Rectangle(42, 64, 16, 32);

	private static readonly Rectangle hpPoisonEnd = new Rectangle(62, 64, 12, 32);

	private static readonly Rectangle xpLeft = new Rectangle(2, 0, 4, 32);

	private static readonly Rectangle xpFull = new Rectangle(12, 0, 4, 32);

	private static readonly Rectangle xpEmpty = new Rectangle(24, 0, 4, 32);

	private static readonly Rectangle xpRight = new Rectangle(34, 0, 4, 32);

	private static readonly Color hudColor = Color.White;

	private static readonly Vector2 hintPos = new Vector2(136f, 72f);

	private Vector2 aPos = new Vector2(1070f, 190f);

	private Vector2 bPos = new Vector2(1114f, 146f);

	private Vector2 xPos = new Vector2(1026f, 146f);

	private Vector2 yPos = new Vector2(1070f, 102f);

	private Color chainButtonColor = new Color(255, 255, 255) * 0.5f;

	private int hpX = 136;

	private int hpY = 594;

	private int hpWidth = 1008;

	private int xpX = 136;

	private int xpY = 622;

	private int xpWidth = 1008;

	private static readonly Vector2 LevelUpPos = new Vector2(640f, 618f);

	public HUDScreen()
		: base(modal: false)
	{
	}

	public override void loadContent(ContentManager content)
	{
		depthBg = new StillSprite(content.Load<Texture2D>("Sprites\\HUD\\Depth"));
		potionRT = new StillSprite(content.Load<Texture2D>("Sprites\\HUD\\PotionRT"));
		Health = content.Load<Texture2D>("Sprites\\HUD\\Health");
		Regen = new AnimatedSprite(content.Load<Texture2D>("Sprites\\HUD\\Regen"), new Rectangle(0, 0, 32, 32), Vector2.Zero, 8, TimeSpan.FromMilliseconds(50.0));
		XP = content.Load<Texture2D>("Sprites\\HUD\\XP");
		LevelUp = new StillSprite(content.Load<Texture2D>("Sprites\\HUD\\LevelUpButton"));
		LevelUpLB = new StillSprite(content.Load<Texture2D>("Sprites\\HUD\\LevelUpButtonLB"));
		LevelUpRB = new StillSprite(content.Load<Texture2D>("Sprites\\HUD\\LevelUpButtonRB"));
		BrokenChain = new StillSprite(content.Load<Texture2D>("Sprites\\HUD\\BrokenChain"));
	}

	public override void update(GameTime gameTime)
	{
		if (DM.Player.SkillSet.Regen.Active)
		{
			Regen.Update(gameTime);
		}
		Tips.Update(gameTime);
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		drawHP();
		drawXP();
		drawLevelUp();
		drawSkillButtons();
		drawDepth();
		drawHealthPotions();
		drawTip();
		base.spriteBatch.End();
	}

	private void drawTip()
	{
		Tips.Draw(base.spriteBatch, hintPos);
	}

	private void drawDepth()
	{
		Vector2 vector = new Vector2(262f, 574f);
		int num = (Text.Width("DEPTH:") + Text.Width(DM.Player.Depth)) / 2 - 12;
		Vector2 v = vector + new Vector2(-num, 2f);
		depthBg.Draw(base.spriteBatch, vector);
		Text.Draw(base.spriteBatch, ref v, "DEPTH:", Color.Black);
		Text.Draw(base.spriteBatch, ref v, DM.Player.Depth, Color.Black);
	}

	private void drawHealthPotions()
	{
		Vector2 vector = new Vector2(1032f, 560f);
		int healthPotions = DM.Player.HealthPotions;
		int num = Text.Width(healthPotions) + 24 + 8;
		Vector2 v = vector + new Vector2(-num, 16f);
		potionRT.Draw(base.spriteBatch, vector);
		Text.Draw(base.spriteBatch, ref v, healthPotions, Color.Black);
		Text.Draw(base.spriteBatch, ref v, '*', Color.Black);
	}

	private void drawSkillButtons()
	{
		if (DM.Player.SkillSet.Poison.Active && !DM.Player.SkillSet.Poison.CanChain)
		{
			float scale = (float)(3.0 * (double)DM.Player.SkillSet.Poison.ChainTimer + 1.0);
			ButtonSprite.A.Draw(base.spriteBatch, aPos, chainButtonColor, scale);
		}
		if (DM.Player.SkillSet.Frenzy.Active && !DM.Player.SkillSet.Frenzy.CanChain)
		{
			float scale2 = (float)(3.0 * (double)DM.Player.SkillSet.Frenzy.ChainTimer + 1.0);
			ButtonSprite.B.Draw(base.spriteBatch, bPos, chainButtonColor, scale2);
		}
		if (DM.Player.SkillSet.Freeze.Active && !DM.Player.SkillSet.Freeze.CanChain)
		{
			float scale3 = (float)(3.0 * (double)DM.Player.SkillSet.Freeze.ChainTimer + 1.0);
			ButtonSprite.X.Draw(base.spriteBatch, xPos, chainButtonColor, scale3);
		}
		if (DM.Player.SkillSet.Orb.Active && !DM.Player.SkillSet.Orb.CanChain)
		{
			float scale4 = (float)(3.0 * (double)DM.Player.SkillSet.Orb.ChainTimer + 1.0);
			ButtonSprite.Y.Draw(base.spriteBatch, yPos, chainButtonColor, scale4);
		}
		Sprite sprite;
		if (DM.Player.SkillSet.Poison.CanCast || DM.Player.SkillSet.Poison.CanChain)
		{
			sprite = ButtonSprite.A;
		}
		else if ((double)DM.Player.SkillSet.Poison.CoolDown > 0.0)
		{
			int num = (int)MathHelper.Clamp((float)((1.0 - (double)DM.Player.SkillSet.Poison.CoolDown) * 16.0), 0f, ButtonSprite.ATimer.Length - 1);
			sprite = ButtonSprite.ATimer[num];
		}
		else
		{
			sprite = ButtonSprite.AGray;
		}
		Sprite sprite2;
		if (DM.Player.SkillSet.Frenzy.CanCast || DM.Player.SkillSet.Frenzy.CanChain)
		{
			sprite2 = ButtonSprite.B;
		}
		else if ((double)DM.Player.SkillSet.Frenzy.CoolDown > 0.0)
		{
			int num2 = (int)MathHelper.Clamp((float)((1.0 - (double)DM.Player.SkillSet.Frenzy.CoolDown) * 16.0), 0f, ButtonSprite.BTimer.Length - 1);
			sprite2 = ButtonSprite.BTimer[num2];
		}
		else
		{
			sprite2 = ButtonSprite.BGray;
		}
		Sprite sprite3;
		if (DM.Player.SkillSet.Freeze.CanCast || DM.Player.SkillSet.Freeze.CanChain)
		{
			sprite3 = ButtonSprite.X;
		}
		else if ((double)DM.Player.SkillSet.Freeze.CoolDown > 0.0)
		{
			int num3 = (int)MathHelper.Clamp((float)((1.0 - (double)DM.Player.SkillSet.Freeze.CoolDown) * 16.0), 0f, ButtonSprite.XTimer.Length - 1);
			sprite3 = ButtonSprite.XTimer[num3];
		}
		else
		{
			sprite3 = ButtonSprite.XGray;
		}
		Sprite sprite4;
		if (DM.Player.SkillSet.Orb.CanCast || DM.Player.SkillSet.Orb.CanChain)
		{
			sprite4 = ButtonSprite.Y;
		}
		else if ((double)DM.Player.SkillSet.Orb.CoolDown > 0.0)
		{
			int num4 = (int)MathHelper.Clamp((float)((1.0 - (double)DM.Player.SkillSet.Orb.CoolDown) * 16.0), 0f, ButtonSprite.YTimer.Length - 1);
			sprite4 = ButtonSprite.YTimer[num4];
		}
		else
		{
			sprite4 = ButtonSprite.YGray;
		}
		sprite.Draw(base.spriteBatch, aPos);
		sprite2.Draw(base.spriteBatch, bPos);
		sprite3.Draw(base.spriteBatch, xPos);
		sprite4.Draw(base.spriteBatch, yPos);
		if (DM.Player.SkillSet.Poison.ChainBroken)
		{
			BrokenChain.Draw(base.spriteBatch, aPos);
		}
		if (DM.Player.SkillSet.Frenzy.ChainBroken)
		{
			BrokenChain.Draw(base.spriteBatch, bPos);
		}
		if (DM.Player.SkillSet.Freeze.ChainBroken)
		{
			BrokenChain.Draw(base.spriteBatch, xPos);
		}
		if (DM.Player.SkillSet.Orb.ChainBroken)
		{
			BrokenChain.Draw(base.spriteBatch, yPos);
		}
	}

	private void drawHP()
	{
		float num = (float)DM.Player.HP / (float)DM.Player.MaxHP;
		Rectangle destinationRectangle = new Rectangle(hpX, hpY, 16, 32);
		Rectangle destinationRectangle2 = new Rectangle(hpX + 16, hpY, hpWidth - 32, 32);
		Rectangle destinationRectangle3 = new Rectangle(hpX + 16, hpY, (int)((double)num * (double)(hpWidth - 32)), 32);
		Rectangle destinationRectangle4 = new Rectangle(hpX + hpWidth - 16, hpY, 16, 32);
		Vector2 position = new Vector2(hpX + 16 + (int)((double)num * (double)(hpWidth - 32)), hpY);
		base.spriteBatch.Draw(Health, destinationRectangle, hpEmptyLeft, Color.White);
		base.spriteBatch.Draw(Health, destinationRectangle2, hpEmptyMid, Color.White);
		base.spriteBatch.Draw(Health, destinationRectangle4, hpEmptyRight, Color.White);
		bool flag = DM.Player.Status.Is<StatusPoison>();
		if (DM.Player.HP > 0)
		{
			base.spriteBatch.Draw(Health, destinationRectangle, flag ? hpPoisonLeft : hpLeft, Color.White);
		}
		base.spriteBatch.Draw(Health, destinationRectangle3, flag ? hpPoisonMid : hpMid, Color.White);
		if (DM.Player.HP == DM.Player.MaxHP)
		{
			base.spriteBatch.Draw(Health, destinationRectangle4, flag ? hpPoisonRight : hpRight, Color.White);
			return;
		}
		base.spriteBatch.Draw(Health, destinationRectangle4, hpEmptyRight, Color.White);
		if (DM.Player.SkillSet.Regen.Active)
		{
			Regen.Draw(base.spriteBatch, position);
		}
		else if (DM.Player.HP > 0)
		{
			base.spriteBatch.Draw(Health, position, flag ? hpPoisonEnd : hpEnd, Color.White);
		}
	}

	private void drawXP()
	{
		float num = (float)(DM.Player.XP - DM.Player.PrevXP) / (float)(DM.Player.NextXP - DM.Player.PrevXP);
		Rectangle destinationRectangle = new Rectangle(xpX, xpY, 4, 32);
		Rectangle destinationRectangle2 = new Rectangle(xpX + 4, xpY, xpWidth - 8, 32);
		Rectangle destinationRectangle3 = new Rectangle(xpX + 4, xpY, (int)((double)num * (double)(xpWidth - 8)), 32);
		Rectangle destinationRectangle4 = new Rectangle(xpX + xpWidth - 4, xpY, 4, 32);
		base.spriteBatch.Draw(XP, destinationRectangle, xpLeft, Color.White);
		base.spriteBatch.Draw(XP, destinationRectangle2, xpEmpty, Color.White);
		base.spriteBatch.Draw(XP, destinationRectangle3, xpFull, Color.White);
		base.spriteBatch.Draw(XP, destinationRectangle4, xpRight, Color.White);
	}

	private void drawLevelUp()
	{
		if (DM.Player.SkillPoints > 0 || DM.Player.StatPoints > 0)
		{
			LevelUp.Draw(base.spriteBatch, LevelUpPos);
		}
		if (DM.Player.StatPoints > 0)
		{
			LevelUpLB.Draw(base.spriteBatch, LevelUpPos);
		}
		if (DM.Player.SkillPoints > 0)
		{
			LevelUpRB.Draw(base.spriteBatch, LevelUpPos);
		}
	}
}
