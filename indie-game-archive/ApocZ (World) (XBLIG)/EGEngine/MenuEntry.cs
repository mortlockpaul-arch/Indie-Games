using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class MenuEntry : EventArgs
{
	public MenuEntryType entryType;

	public MenuEntryAttribute entryAttribute;

	public MenuTextJustify entryTextJustify;

	public bool isAvailable = true;

	public bool isSelected;

	public bool isCursorOver;

	public float scale = 1f;

	public float textHeight;

	public string text;

	public string strikeOutText;

	public Color diffuse;

	public Color shadow;

	public Color diffuseSelected;

	public Vector2 position;

	public Vector2 size;

	public Texture2D icon;

	public Texture2D iconSelected;

	public Texture2D iconHiLight;

	public Vector2 textOffset = new Vector2(0f, 0f);

	private byte alphaValue;

	private float timer;

	private float blinkTimer;

	private float blinkdirection = 1f;

	private Vector4 transitionDiffuse = Vector4.Zero;

	private Vector4 transitionShadow = Vector4.Zero;

	private Rectangle buttonRec = Rectangle.Empty;

	private static float windowScale = 1f;

	private static Vector2 shadowOffset = new Vector2(2f, 2f);

	public static SpriteFont gameFont;

	public static Color MenuColor = Color.LightGray;

	public event EventHandler<MenuEntry> Selected;

	public void TrySelected()
	{
		if (Selected != null)
		{
			Selected(this, this);
		}
	}

	public virtual MenuEntry Set(string eText, MenuTextJustify tj, Vector2 pos, EventHandler<MenuEntry> handler, ContentManager cntMgr)
	{
		entryTextJustify = tj;
		return Set(MenuEntryType.Text, (MenuEntryAttribute)5, eText, pos, null, Color.LightGray, "menus\\button01", "menus\\button02", "menus\\button03", handler, cntMgr);
	}

	public virtual MenuEntry Set(string eText, Vector2 pos, Color clr, EventHandler<MenuEntry> handler, ContentManager cntMgr)
	{
		entryTextJustify = MenuTextJustify.Left;
		return Set(MenuEntryType.Text, (MenuEntryAttribute)5, eText, pos, null, clr, "menus\\button01", "menus\\button02", "menus\\button03", handler, cntMgr);
	}

	public virtual MenuEntry Set(MenuEntryType type, MenuEntryAttribute attribute, string eText, Vector2 pos, Vector2? sz, string button1, string button2, string button3, EventHandler<MenuEntry> handler, ContentManager cntMgr)
	{
		return Set(type, attribute, eText, pos, sz, Color.White, button1, button2, button3, handler, cntMgr);
	}

	public virtual MenuEntry Set(MenuEntryType type, MenuEntryAttribute attribute, string eText, Vector2 pos, Vector2? sz, Color clr, string button1, string button2, string button3, EventHandler<MenuEntry> handler, ContentManager cntMgr)
	{
		return Set(type, attribute, eText, pos, sz, clr, availability: true, button1, button2, button3, handler, cntMgr);
	}

	public virtual MenuEntry Set(MenuEntryType type, MenuEntryAttribute attribute, string eText, Vector2 pos, Vector2? sz, Color clr, bool availability, string button1, string button2, string button3, EventHandler<MenuEntry> handler, ContentManager cntMgr)
	{
		isAvailable = availability;
		isSelected = false;
		entryType = type;
		entryAttribute = attribute;
		scale = 0.9f;
		text = eText;
		diffuse = MenuColor;
		shadow = Color.Black;
		position = pos;
		size = Menu.defaultFont.MeasureString(text);
		if (entryTextJustify == MenuTextJustify.Center)
		{
			position.X -= size.X * 0.5f;
		}
		else if (entryTextJustify == MenuTextJustify.Right)
		{
			position.X -= size.X;
		}
		if (button1 != null)
		{
			TextureBase.GetTexture2DByName(cntMgr, button1, out icon);
		}
		if (button2 != null)
		{
			TextureBase.GetTexture2DByName(cntMgr, button2, out iconSelected);
		}
		if (button3 != null)
		{
			TextureBase.GetTexture2DByName(cntMgr, button3, out iconHiLight);
		}
		Selected += handler;
		Build();
		return this;
	}

	public virtual void Build()
	{
		gameFont = Menu.defaultFont;
		buttonRec.X = (int)position.X;
		buttonRec.Y = (int)position.Y;
		buttonRec.Width = (int)size.X;
		buttonRec.Height = (int)size.Y;
		textOffset = Vector2.Zero;
		textHeight = gameFont.MeasureString(text).Y * 0.9f;
		transitionDiffuse.X = (int)diffuse.R;
		transitionDiffuse.Y = (int)diffuse.G;
		transitionDiffuse.Z = (int)diffuse.B;
		transitionDiffuse.W = (int)diffuse.A;
		transitionShadow.X = (int)shadow.R;
		transitionShadow.Y = (int)shadow.G;
		transitionShadow.Z = (int)shadow.B;
		transitionShadow.W = (int)shadow.A;
	}

	public virtual void Update(float eTime, float alpha)
	{
		if ((entryAttribute & MenuEntryAttribute.Blinking) > (MenuEntryAttribute)0)
		{
			blinkTimer += eTime * 2f * blinkdirection;
			if (blinkTimer > 1f)
			{
				blinkTimer = 1f;
				blinkdirection = -1f;
			}
			if (blinkTimer < 0f)
			{
				blinkTimer = 0f;
				blinkdirection = 1f;
			}
			alpha *= blinkTimer;
		}
		diffuse.R = (byte)(transitionDiffuse.X * alpha);
		diffuse.G = (byte)(transitionDiffuse.Y * alpha);
		diffuse.B = (byte)(transitionDiffuse.Z * alpha);
		diffuse.A = (byte)(transitionDiffuse.W * alpha);
		shadow.R = (byte)(transitionShadow.X * alpha);
		shadow.G = (byte)(transitionShadow.Y * alpha);
		shadow.B = (byte)(transitionShadow.Z * alpha);
		shadow.A = (byte)(transitionShadow.W * alpha);
		float num = (float)(int)diffuse.R * 1.74f * alpha;
		float num2 = (float)(int)diffuse.G * 1.74f * alpha;
		float num3 = (float)(int)diffuse.B * 1.74f * alpha;
		diffuseSelected.R = (byte)((num < 256f) ? num : 255f);
		diffuseSelected.G = (byte)((num2 < 256f) ? num2 : 255f);
		diffuseSelected.B = (byte)((num3 < 256f) ? num3 : 255f);
		diffuseSelected.A = (byte)(255f * alpha);
		switch (entryType)
		{
		}
	}

	public virtual void Draw()
	{
		Draw(hasFocus: true);
	}

	public virtual void Draw(bool hasFocus)
	{
		Menu.spriteBatch.Begin();
		switch (entryType)
		{
		case MenuEntryType.Text:
			DrawText(hasFocus);
			break;
		case MenuEntryType.Button:
			DrawButton(hasFocus);
			break;
		case MenuEntryType.ButtonWithText:
			DrawButton(hasFocus);
			DrawText(hasFocus);
			break;
		}
		Menu.spriteBatch.End();
	}

	private void DrawText(bool hasFocus)
	{
		float g = ((hasFocus && isSelected && isAvailable) ? 1.12f : 1f);
		if ((entryAttribute & MenuEntryAttribute.Shadow) > (MenuEntryAttribute)0)
		{
			Menu.spriteBatch.DrawString(gameFont, text, position + shadowOffset, shadow, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		}
		if (hasFocus && isSelected && !isAvailable)
		{
			Menu.spriteBatch.DrawString(gameFont, text, position, Color.LightGray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		}
		else if (hasFocus && isSelected)
		{
			Menu.spriteBatch.DrawString(gameFont, text, position, diffuseSelected, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		}
		else if (PlayerBase.ApocalypseZ_Hack)
		{
			Color d = diffuse;
			d.R = (byte)((d.R - 160 >= 0) ? ((uint)(d.R - 160)) : 0u);
			d.G = (byte)((d.G - 160 >= 0) ? ((uint)(d.G - 160)) : 0u);
			d.B = (byte)((d.B - 160 >= 0) ? ((uint)(d.B - 160)) : 0u);
			d.A = (byte)((d.A - 160 >= 0) ? ((uint)(d.A - 160)) : 0u);
			Menu.spriteBatch.DrawString(gameFont, text, position, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		}
		else
		{
			Menu.spriteBatch.DrawString(gameFont, text, position, diffuse, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		}
	}

	private void DrawButton(bool hasFocus)
	{
		if (hasFocus && isSelected)
		{
			Menu.spriteBatch.Draw(iconSelected, buttonRec, diffuse);
		}
		if (hasFocus && isCursorOver)
		{
			Menu.spriteBatch.Draw(iconHiLight, buttonRec, diffuse);
		}
		else
		{
			Menu.spriteBatch.Draw(icon, buttonRec, diffuse);
		}
	}

	private void UpdateBlinkTimer(float eTime)
	{
		timer += eTime * 2f;
		if (timer < 1f)
		{
			alphaValue = (byte)(timer * 255f);
		}
		else if (timer > 3f && timer < 4f)
		{
			alphaValue = (byte)((4f - timer) * 255f);
		}
		else if (timer >= 4f)
		{
			alphaValue = 0;
			timer = 0f;
		}
	}
}
