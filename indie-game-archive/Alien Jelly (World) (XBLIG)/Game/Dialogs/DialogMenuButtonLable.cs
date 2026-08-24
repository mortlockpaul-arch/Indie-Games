using GKEngine;
using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogMenuButtonLable
{
	public enum Button
	{
		A,
		B,
		X,
		Y
	}

	private const int TEXT_SPACING_X = -4;

	private static string[] BUTTON_LETTERS = new string[4] { "A", "B", "X", "Y" };

	public static Color COLOR_TITLE = new Color(93, 87, 163);

	public static Color COLOR_TITLE_SHADOW = new Color(21, 1, 16);

	private string title;

	public Dialog.DialogDelegate action;

	public Button button;

	public bool actionImmediate;

	private SpriteString spriteTitle;

	private SpriteString spriteTitleShadow;

	private Sprite spriteButton;

	public Vector2 position = default(Vector2);

	public bool visible
	{
		set
		{
			spriteTitle.visible = value;
			spriteTitleShadow.visible = value;
			spriteButton.visible = value;
		}
	}

	public byte alpha
	{
		set
		{
			spriteTitle.color.A = value;
			spriteTitleShadow.color.A = value;
			spriteButton.tint.A = value;
		}
	}

	public DialogMenuButtonLable(string xTitle, Button oButton, Dialog.DialogDelegate oAction)
	{
		title = xTitle;
		button = oButton;
		action = oAction;
		actionImmediate = false;
	}

	public DialogMenuButtonLable(string xTitle, Button oButton, Dialog.DialogDelegate oAction, bool pImmediate)
	{
		title = xTitle;
		button = oButton;
		action = oAction;
		actionImmediate = pImmediate;
	}

	public void Load(DialogManager oManager)
	{
		spriteTitleShadow = new SpriteString(oManager.spriteManager, oManager.fontKA_20, title, 0f);
		spriteTitle = new SpriteString(oManager.spriteManager, oManager.fontKA_20, title, 0f);
		spriteTitle.color = COLOR_TITLE;
		spriteTitleShadow.color = COLOR_TITLE_SHADOW;
		spriteButton = new Sprite(oManager.spriteManager);
		spriteButton.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Button" + BUTTON_LETTERS[(int)button]);
	}

	public void SetColor(Color oTitle, Color oShadow)
	{
		spriteTitle.color = oTitle;
		spriteTitleShadow.color = oShadow;
	}

	public void Dispose()
	{
		spriteButton.manager.Remove(spriteButton);
		spriteTitle.Dispose();
		spriteTitleShadow.Dispose();
		spriteButton.Dispose();
		spriteTitleShadow = null;
		spriteTitle = null;
		spriteButton = null;
		action = null;
	}

	public void Refresh()
	{
		_ = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		spriteButton.position.X = position.X;
		spriteButton.position.Y = position.Y;
		spriteTitle.X = position.X + spriteButton.size.X + -4f;
		spriteTitle.Y = position.Y + 6f;
		spriteTitleShadow.X = spriteTitle.X;
		spriteTitleShadow.Y = spriteTitle.Y + 3f;
	}

	public float GetWidth()
	{
		return spriteButton.size.X + -4f + spriteTitle.width;
	}
}
