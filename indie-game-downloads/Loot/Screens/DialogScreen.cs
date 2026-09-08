using System;
using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class DialogScreen : Screen
{
	private const int columnWidth = 864;

	private const int scrollWidth = 1024;

	private const int titleHeight = 72;

	private const int titleSpacer = -12;

	private const int bodyPad = 64;

	private const int bodySpacer = -20;

	private const int optionBorder = 12;

	private const int optionInset = 20;

	private const int optionPad = 20;

	private const int optionSpacer = 0;

	private string title;

	private FormattedText text;

	protected DialogOption[] options;

	protected DialogCallback defaultCallback;

	protected int selected;

	private int top;

	private int left;

	private int height;

	private int optionHeight;

	private int optionWidth;

	private static Texture2D pixel;

	private static Sprite arrow;

	private static StretchyTexture titleTexture;

	private static StretchyTexture bodyTexture;

	private static StretchyTexture optionTexture;

	private static StretchyTexture optionBgTexture;

	private float arrowTimer;

	private Color titleTextColor = new Color(151, 100, 0);

	private Color titleTextShadowColor = new Color(255, 255, 0);

	private Color optionGray = new Color(192, 192, 192);

	protected virtual int CenterX => 640;

	public DialogScreen(string title, string text, DialogCallback defaultCallback, params DialogOption[] options)
		: base(modal: true)
	{
		if (title != null)
		{
			this.title = title;
			height += 60;
		}
		if (text != null)
		{
			this.text = new FormattedText(text, 864);
			height += this.text.Height + 64 - 20;
		}
		this.defaultCallback = defaultCallback;
		optionWidth = 0;
		if (options != null && options.Length != 0)
		{
			for (int i = 0; i < options.Length; i++)
			{
				options[i].Format(864);
				if (options[i].FormattedText.Width > optionWidth)
				{
					optionWidth = options[i].FormattedText.Width;
				}
				optionHeight += options[i].FormattedText.Height + 20;
			}
			optionHeight = optionHeight;
			height += optionHeight + 24;
			this.options = options;
		}
		top = 360 - height / 2;
		left = CenterX - 512;
		selected = 0;
	}

	public static void Load(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
		arrow = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\Arrow"));
		titleTexture = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\Title"), new Rectangle(64, 20, 128, 40));
		bodyTexture = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\Body"), new Rectangle(64, 64, 128, 128));
		optionTexture = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\Option"), new Rectangle(16, 16, 224, 32));
		optionBgTexture = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\MenuBox"), new Rectangle(16, 16, 96, 96));
	}

	public override void update(GameTime gameTime)
	{
		arrowTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds * Math.PI);
		if ((double)arrowTimer > Math.PI * 2.0)
		{
			arrowTimer -= (float)Math.PI * 2f;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(this);
			if (options != null && options[selected].Callback != null)
			{
				options[selected].Callback();
			}
			else if (defaultCallback != null)
			{
				defaultCallback();
			}
		}
		else
		{
			if (options == null)
			{
				return;
			}
			if (MC.GamePadManager.isNewDirDown())
			{
				PlaySound.MenuMove();
				selected++;
				if (selected >= options.Length)
				{
					selected = 0;
				}
				arrowTimer = 0f;
			}
			else if (MC.GamePadManager.isNewDirUp())
			{
				PlaySound.MenuMove();
				selected--;
				if (selected < 0)
				{
					selected = options.Length - 1;
				}
				arrowTimer = 0f;
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, MC.ScreenManager.BackgroundColor);
		drawOptions();
		drawBody();
		drawTitle();
		base.spriteBatch.End();
	}

	private void drawTitle()
	{
		if (title != null)
		{
			int num = Text.Width(title) + 128;
			titleTexture.Draw(base.spriteBatch, new Rectangle(CenterX - num / 2, top, num, 72));
			Text.DrawCentered(base.spriteBatch, new Vector2(CenterX + 2, top + 30), title, titleTextShadowColor);
			Text.DrawCentered(base.spriteBatch, new Vector2(CenterX, top + 28), title, titleTextColor);
		}
	}

	private void drawBody()
	{
		if (text != null)
		{
			int num = 0;
			if (title != null)
			{
				num = 60;
			}
			bodyTexture.Draw(base.spriteBatch, new Rectangle(left, top + num, 1024, text.Height + 64));
			Text.Draw(base.spriteBatch, new Vector2(CenterX - text.MaxWidth / 2 + 12, top + num + 40), text, Color.Black);
		}
	}

	private void drawOptions()
	{
		if (options == null)
		{
			return;
		}
		int num = 0;
		if (title != null)
		{
			num = 60;
		}
		if (text != null)
		{
			num += text.Height + 64 - 20;
		}
		int num2 = num + 12;
		int num3 = CenterX - optionWidth / 2 + 12;
		int num4 = num3 - 12 - 20;
		int num5 = optionWidth + 40;
		optionBgTexture.Draw(base.spriteBatch, new Rectangle(num4 - 14, top + num2 - 14, num5 + 28, optionHeight + 28));
		for (int i = 0; i < options.Length; i++)
		{
			Color color = ((selected == i) ? Color.White : optionGray);
			FormattedText formattedText = options[i].FormattedText;
			optionTexture.Draw(base.spriteBatch, new Rectangle(num4, top + num2, num5, formattedText.Height + 20), color);
			Vector2 vector = new Vector2(num3, top + num2 + 30);
			Text.Draw(base.spriteBatch, vector, formattedText, Color.Black);
			num2 += formattedText.Height + 20;
			if (selected == i)
			{
				Vector2 position = vector + new Vector2(-50f, 0f) + new Vector2(8f, 0f) * (float)Math.Sin(arrowTimer);
				arrow.Draw(base.spriteBatch, position);
			}
		}
	}
}
