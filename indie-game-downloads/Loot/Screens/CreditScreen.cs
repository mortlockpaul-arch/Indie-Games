using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class CreditScreen : Screen
{
	private struct Blood
	{
		public int type;

		public int x;

		public float height;

		public int slowHeight;

		public float speed;
	}

	private List<Blood> drips;

	private static Texture2D bloodDrip;

	private float dy = 760f;

	private float scrollSpeed = 30f;

	private string[] credits = new string[25]
	{
		"Cursed Loot", "", "", "Designer, Programmer, Writer,", "Composer, Artist, & Animator", "", "Mike Muir", "Eyehook Games LLC", "www.eyehookgames.com", "",
		"Special thanks to everyone", "who playtested Cursed Loot!", "", "Extra special thanks to BillJ", "for your feedback & testing!", "", "", "", "", "",
		"", "", "Last, but not least...", "", "Thank you for playing Cursed Loot!"
	};

	private Vector2 shadowOffset = new Vector2(2f, 2f);

	public CreditScreen()
		: base(modal: true)
	{
		drips = new List<Blood>();
		int num = 0;
		Blood item = default(Blood);
		while (true)
		{
			num += DM.Random.Next(128) + 32;
			if (num <= 1280)
			{
				item.type = DM.Random.Next(3);
				item.x = num;
				item.height = -DM.Random.Next(128) - 64;
				item.slowHeight = 180 + DM.Random.Next(540);
				item.speed = 15 + DM.Random.Next(30);
				drips.Add(item);
				continue;
			}
			break;
		}
	}

	public static void Load(ContentManager content)
	{
		bloodDrip = content.Load<Texture2D>("Sprites\\UI\\BloodDrip");
	}

	public override void transitionOn()
	{
		BgMusic.GameOver();
	}

	private void removeScreen()
	{
		MC.ScreenManager.removeScreen(this);
		MC.ScreenManager.addScreen(new MainMenu());
		MC.ScreenManager.addScreen(new FadeInScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, null));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.A) || MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			MC.ScreenManager.addScreen(new FadeOutScreen(TimeSpan.FromMilliseconds(500.0), MC.ScreenManager.BackgroundColor, removeScreen));
			return;
		}
		if ((double)dy > (double)(-(credits.Length * 40) + 400))
		{
			dy -= (float)gameTime.ElapsedGameTime.TotalSeconds * scrollSpeed;
		}
		for (int i = 0; i < drips.Count; i++)
		{
			Blood value = drips[i];
			if (!((double)value.speed > 0.0))
			{
				continue;
			}
			if ((double)value.height >= (double)value.slowHeight)
			{
				value.speed -= (float)(25.0 * gameTime.ElapsedGameTime.TotalSeconds);
				if ((double)value.speed <= 0.0)
				{
					value.speed = 0f;
					continue;
				}
			}
			value.height += drips[i].speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			drips[i] = value;
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		for (int i = 0; i < drips.Count; i++)
		{
			drawBlood(base.spriteBatch, drips[i]);
		}
		Vector2 vector = new Vector2(640f, dy);
		Vector2 zero = Vector2.Zero;
		for (int j = 0; j < credits.Length; j++)
		{
			Vector2 vector2 = vector + zero;
			Text.DrawCentered(base.spriteBatch, vector2 + shadowOffset, credits[j], Color.Black);
			Text.DrawCentered(base.spriteBatch, vector2, credits[j], Color.White);
			zero += new Vector2(0f, 40f);
		}
		base.spriteBatch.End();
	}

	private void drawBlood(SpriteBatch spriteBatch, Blood b)
	{
		spriteBatch.Draw(bloodDrip, new Rectangle(b.x - 32, 0, 64, (int)b.height), new Rectangle(64 * b.type, 0, 64, 64), Color.White);
		spriteBatch.Draw(bloodDrip, new Rectangle(b.x - 32, (int)b.height, 64, 64), new Rectangle(64 * b.type, 64, 64, 64), Color.White);
	}
}
