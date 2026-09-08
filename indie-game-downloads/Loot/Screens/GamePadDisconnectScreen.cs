using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class GamePadDisconnectScreen : Screen
{
	private const string message = "Please reconnect your controller.";

	private Texture2D MessageBox;

	private Vector2 msgPos;

	private Rectangle boxLeft;

	private Rectangle boxMid;

	private Rectangle boxRight;

	private static readonly Rectangle boxLeftSrc = new Rectangle(0, 0, 64, 128);

	private static readonly Rectangle boxMidSrc = new Rectangle(32, 0, 64, 128);

	private static readonly Rectangle boxRightSrc = new Rectangle(64, 0, 64, 128);

	private static readonly Rectangle bgRect = new Rectangle(0, 310, 1280, 100);

	private static readonly Color bgColor = Color.Black;

	private static readonly Color msgColor = Color.White;

	public GamePadDisconnectScreen()
		: base(modal: true)
	{
		boxLeft = new Rectangle(640 - ("Please reconnect your controller.".Length - 1) * 24 / 2 - 64, 296, 64, 128);
		boxMid = new Rectangle(640 - ("Please reconnect your controller.".Length - 1) * 24 / 2, 296, "Please reconnect your controller.".Length * 24, 128);
		boxRight = new Rectangle(640 + ("Please reconnect your controller.".Length - 1) * 24 / 2, 296, 64, 128);
		msgPos = new Vector2(640 - ("Please reconnect your controller.".Length - 1) * 24 / 2, 360f);
	}

	public override void loadContent(ContentManager content)
	{
		MessageBox = content.Load<Texture2D>("Sprites\\UI\\MessageBox");
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.A) || MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			MC.ScreenManager.removeScreen(this);
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(MessageBox, boxLeft, boxLeftSrc, Color.White);
		base.spriteBatch.Draw(MessageBox, boxMid, boxMidSrc, Color.White);
		base.spriteBatch.Draw(MessageBox, boxRight, boxRightSrc, Color.White);
		Vector2 v = msgPos;
		Text.Draw(base.spriteBatch, ref v, "Please reconnect your controller.", msgColor);
		base.spriteBatch.End();
	}
}
