using System;
using Loot.Core;
using Loot.PC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public static class Tips
{
	private enum Mode
	{
		Hide,
		FadeIn,
		Show,
		FadeOut
	}

	private const string rt = "Press \u0096\u0097 to use a Health Potion";

	private const string lb = "Press \u0090\u0091 to assign Stat Points";

	private const string rb = "Press \u0092\u0093 to assign Skill Points";

	private const string lamp = "Refill your lamp with oil";

	private const string tip1 = "Use \u0088\u0089 to Move & Attack";

	private const string tip4 = "Press \u008c\u008d to open your Inventory";

	private const string tip5 = "Zoom In/Out with \u008a\u008b";

	private const string tip6 = "You can disable Tips in Options";

	private const string tip7 = "No more secret door tips!";

	private const string tip2gambler = "Press \u0080\u0081 to Poison enemies";

	private const string tip2berserk = "Press \u0082\u0083 for Frenzy";

	private const string tip2shaman = "Press \u0084\u0085 to Freeze enemies";

	private const string tip2tinker = "Press \u0086\u0087 to summon your Orb";

	private const string tip2Btinker = "Press \u0086\u0087 to detonate your Orb";

	private const string tip3gambler = "Chain Poison: \u0080\u0081 (Wait 1s) \u0080\u0081";

	private const string tip3berserk = "Chain Frenzy: \u0082\u0083+\u0082\u0083+\u0082\u0083";

	private const string tip3shaman = "Chain Freeze: \u0084\u0085 (Wait 1s) \u0084\u0085";

	private const string tip3tinker = "Chain Orb: \u0086\u0087 (Wait 1s) \u0086\u0087";

	private static int curTip = 0;

	private static string curTipText;

	private static Mode curMode = Mode.Hide;

	private static TimeSpan timer = TimeSpan.Zero;

	private static TimeSpan showDuration = TimeSpan.FromSeconds(5.0);

	private static TimeSpan hideDuration = TimeSpan.FromSeconds(15.0);

	private static TimeSpan fadeDuration = TimeSpan.FromSeconds(1.0);

	private static TimeSpan tipCheckDuration = TimeSpan.FromMilliseconds(500.0);

	private static Texture2D hintBg;

	private static Rectangle leftSrc = new Rectangle(0, 0, 64, 64);

	private static Rectangle midSrc = new Rectangle(96, 0, 64, 64);

	private static Rectangle rightSrc = new Rectangle(160, 0, 64, 64);

	public static void Reset()
	{
		curTip = 0;
		curTipText = null;
		curMode = Mode.Hide;
		timer = TimeSpan.Zero;
	}

	public static void LoadContent(ContentManager content)
	{
		hintBg = content.Load<Texture2D>("Sprites\\HUD\\Hint");
	}

	public static void Update(GameTime gameTime)
	{
		if (!Profile.Preferences.Tips)
		{
			return;
		}
		timer -= gameTime.ElapsedGameTime;
		if (!(timer <= TimeSpan.Zero))
		{
			return;
		}
		switch (curMode)
		{
		case Mode.Hide:
			curTipText = getTipText();
			if (curTipText != null)
			{
				curMode = Mode.FadeIn;
				timer = fadeDuration;
			}
			else
			{
				timer = tipCheckDuration;
			}
			break;
		case Mode.FadeIn:
			curMode = Mode.Show;
			timer = showDuration;
			break;
		case Mode.Show:
			curMode = Mode.FadeOut;
			timer = fadeDuration;
			break;
		default:
			curMode = Mode.Hide;
			timer = hideDuration;
			break;
		}
	}

	public static void Draw(SpriteBatch spriteBatch, Vector2 pos)
	{
		if (Profile.Preferences.Tips && curMode != Mode.Hide && curTipText != null)
		{
			Draw(spriteBatch, pos, curTipText);
		}
	}

	private static string getTipText()
	{
		if (curTip < DM.Player.Depth)
		{
			curTip = DM.Player.Depth;
		}
		if (curTip == DM.Player.Depth)
		{
			switch (curTip)
			{
			case 1:
				curTip++;
				return "Use \u0088\u0089 to Move & Attack";
			case 2:
				curTip++;
				return tip2();
			case 3:
				curTip++;
				return tip3();
			case 4:
				curTip++;
				return "Press \u008c\u008d to open your Inventory";
			case 5:
				curTip++;
				return "Zoom In/Out with \u008a\u008b";
			case 6:
				curTip++;
				return "You can disable Tips in Options";
			case 7:
				curTip++;
				return "No more secret door tips!";
			}
		}
		if (DM.Player.HP <= DM.Player.MaxHP / 2 && DM.Player.HealthPotions > 0)
		{
			return "Press \u0096\u0097 to use a Health Potion";
		}
		if ((double)DM.Player.Lantern.Fuel <= 0.25)
		{
			return "Refill your lamp with oil";
		}
		if (DM.Player.StatPoints > 0)
		{
			return "Press \u0090\u0091 to assign Stat Points";
		}
		return (DM.Player.SkillPoints > 0) ? "Press \u0092\u0093 to assign Skill Points" : null;
	}

	private static string tip2()
	{
		switch (DM.Player.ClassType)
		{
		case PlayerClassType.Berserker:
			return "Press \u0082\u0083 for Frenzy";
		case PlayerClassType.Shaman:
			return "Press \u0084\u0085 to Freeze enemies";
		case PlayerClassType.Tinkerer:
			if (DM.Player.Orb != null)
			{
				return "Press \u0086\u0087 to detonate your Orb";
			}
			curTip--;
			return "Press \u0086\u0087 to summon your Orb";
		case PlayerClassType.Gambler:
			return "Press \u0080\u0081 to Poison enemies";
		case PlayerClassType.Goblin:
			return "Regen heals you over time.";
		case PlayerClassType.Peasant:
			return "Luck gives you more loot.";
		default:
			throw new Exception("Unknown class type: " + DM.Player.ClassType);
		}
	}

	public static string tip3()
	{
		return DM.Player.ClassType switch
		{
			PlayerClassType.Berserker => "Chain Frenzy: \u0082\u0083+\u0082\u0083+\u0082\u0083", 
			PlayerClassType.Shaman => "Chain Freeze: \u0084\u0085 (Wait 1s) \u0084\u0085", 
			PlayerClassType.Tinkerer => "Chain Orb: \u0086\u0087 (Wait 1s) \u0086\u0087", 
			PlayerClassType.Gambler => "Chain Poison: \u0080\u0081 (Wait 1s) \u0080\u0081", 
			PlayerClassType.Goblin => "If you get hit, regen pauses.", 
			PlayerClassType.Peasant => "Luck helps with encounters.", 
			_ => throw new Exception("Unknown class type: " + DM.Player.ClassType), 
		};
	}

	private static void Draw(SpriteBatch spriteBatch, Vector2 pos, string hint)
	{
		Color white = Color.White;
		Color black = Color.Black;
		switch (curMode)
		{
		case Mode.FadeIn:
		{
			float num2 = (float)(1.0 - timer.TotalSeconds / fadeDuration.TotalSeconds);
			white *= num2;
			black *= num2;
			break;
		}
		case Mode.FadeOut:
		{
			float num = (float)(timer.TotalSeconds / fadeDuration.TotalSeconds);
			white *= num;
			black *= num;
			break;
		}
		}
		int width = Text.Width(hint);
		Rectangle destinationRectangle = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
		Rectangle destinationRectangle2 = new Rectangle(destinationRectangle.Right, destinationRectangle.Top, width, 64);
		Rectangle destinationRectangle3 = new Rectangle(destinationRectangle2.Right, destinationRectangle.Top, 64, 64);
		Vector2 v = pos + new Vector2(84f, 30f);
		spriteBatch.Draw(hintBg, destinationRectangle, leftSrc, white);
		spriteBatch.Draw(hintBg, destinationRectangle2, midSrc, white);
		spriteBatch.Draw(hintBg, destinationRectangle3, rightSrc, white);
		Text.Draw(spriteBatch, v, hint, black);
	}
}
