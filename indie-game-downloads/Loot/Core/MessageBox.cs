using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Core;

public class MessageBox
{
	public delegate void Callback();

	public class MessageBoxScreen : Screen
	{
		private MessageBox box;

		public MessageBoxScreen(MessageBox box)
			: base(modal: true)
		{
			this.box = box;
		}

		public override void update(GameTime gameTime)
		{
			if (box.acceptCallback != null && MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				PlaySound.MenuClick();
				box.acceptCallback();
				MC.ScreenManager.removeScreen(this);
			}
			else if (box.cancelCallback != null && MC.GamePadManager.isNewButtonDown(Buttons.B))
			{
				PlaySound.MenuCancel();
				box.cancelCallback();
				MC.ScreenManager.removeScreen(this);
			}
			else if (box.acceptCallback == null && box.cancelCallback == null && (MC.GamePadManager.isNewButtonDown(Buttons.A) || MC.GamePadManager.isNewButtonDown(Buttons.B)))
			{
				PlaySound.MenuClick();
				MC.ScreenManager.removeScreen(this);
			}
		}

		public override void draw(GameTime gameTime)
		{
			base.spriteBatch.Begin();
			Pixel.Draw(base.spriteBatch, base.viewportRect, Color.Black * 0.25f);
			box.Draw(base.spriteBatch);
			base.spriteBatch.End();
		}
	}

	private string title;

	private FormattedText body;

	private int titleWidth;

	private Vector2 textPos;

	private Rectangle boxRect;

	private Callback acceptCallback;

	private Callback cancelCallback;

	private static Sprite buttonBg;

	private static Texture2D boxTitle;

	private static StretchyTexture box;

	private string accept = "\u0080\u0081Accept";

	private string cancel = "\u0082\u0083Cancel";

	private static Vector2 shadowOffset = new Vector2(2f, 2f);

	private static Color titleTextColor = new Color(151, 100, 0);

	private static Color titleTextShadowColor = new Color(255, 255, 0);

	public MessageBox(string title, FormattedText body, Callback accept, Callback cancel)
	{
		this.title = title;
		this.body = body;
		titleWidth = Text.Width(title) + 40;
		textPos = new Vector2(640 - body.Width / 2 + 12, 360 - body.Height / 2 + 20);
		boxRect = new Rectangle(640 - body.Width / 2 - 40, 360 - body.Height / 2 - 48, body.Width + 80, body.Height + 96);
		acceptCallback = accept;
		cancelCallback = cancel;
	}

	public static void Display(string title, FormattedText body)
	{
		MC.ScreenManager.addScreen(new MessageBoxScreen(new MessageBox(title, body, null, null)));
	}

	public static void Display(string title, string body)
	{
		MC.ScreenManager.addScreen(new MessageBoxScreen(new MessageBox(title, new FormattedText(body, body.Length * 24), null, null)));
	}

	public static void Display(string title, FormattedText body, Callback accept)
	{
		MC.ScreenManager.addScreen(new MessageBoxScreen(new MessageBox(title, body, accept, null)));
	}

	public static void Display(string title, string body, Callback accept)
	{
		MC.ScreenManager.addScreen(new MessageBoxScreen(new MessageBox(title, new FormattedText(body, body.Length * 24), accept, null)));
	}

	public static void Display(string title, FormattedText body, Callback accept, Callback cancel)
	{
		MC.ScreenManager.addScreen(new MessageBoxScreen(new MessageBox(title, body, accept, cancel)));
	}

	public static void Display(string title, string body, Callback accept, Callback cancel)
	{
		MC.ScreenManager.addScreen(new MessageBoxScreen(new MessageBox(title, new FormattedText(body, body.Length * 24), accept, cancel)));
	}

	public static void LoadContent(ContentManager content)
	{
		boxTitle = content.Load<Texture2D>("Sprites\\UI\\MenuBoxTitle");
		box = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\MenuBox"), new Rectangle(16, 16, 96, 96));
		buttonBg = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\ButtonBg"));
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		box.Draw(spriteBatch, boxRect);
		Text.Draw(spriteBatch, textPos, body, Color.Black);
		int num = (boxRect.Left + boxRect.Right) / 2;
		int num2 = 640 - titleWidth / 2 - 128;
		int y = boxRect.Top - 76;
		spriteBatch.Draw(boxTitle, new Rectangle(num2, y, 128, 128), new Rectangle(0, 0, 128, 128), Color.White);
		spriteBatch.Draw(boxTitle, new Rectangle(num2 + 128, y, titleWidth, 128), new Rectangle(192, 0, 128, 128), Color.White);
		spriteBatch.Draw(boxTitle, new Rectangle(num2 + 128 + titleWidth, y, 128, 128), new Rectangle(384, 0, 128, 128), Color.White);
		Text.DrawCentered(spriteBatch, new Vector2(num, boxRect.Top - 12) + shadowOffset, title, titleTextShadowColor);
		Text.DrawCentered(spriteBatch, new Vector2(num, boxRect.Top - 12), title, titleTextColor);
		Vector2 vector = Vector2.Zero;
		Vector2 vector2 = Vector2.Zero;
		if (acceptCallback != null && cancelCallback != null)
		{
			vector = new Vector2(num - 148, boxRect.Bottom + 8);
			vector2 = new Vector2(num + 148, boxRect.Bottom + 8);
		}
		else if (acceptCallback != null && cancelCallback == null)
		{
			vector = new Vector2(num, boxRect.Bottom + 8);
		}
		else if (acceptCallback == null && cancelCallback != null)
		{
			vector2 = new Vector2(num, boxRect.Bottom + 8);
		}
		if (acceptCallback != null)
		{
			buttonBg.Draw(spriteBatch, vector);
			Text.DrawCentered(spriteBatch, vector, accept, Color.Black);
		}
		if (cancelCallback != null)
		{
			buttonBg.Draw(spriteBatch, vector2);
			Text.DrawCentered(spriteBatch, vector2, cancel, Color.Black);
		}
	}
}
