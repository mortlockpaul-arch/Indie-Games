using GKEngine;
using GKEngine.Entities;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Build.UI;

public class BuildUIButton
{
	public static Color COLOR_SHADOW = new Color(21, 1, 16, 255);

	public static float TEXT_WIDTH = 150f;

	public BuildUI ui;

	public Vector2 offset;

	public Color textColor;

	public string iconPath;

	public SpriteFont font;

	public SpriteString.Align align;

	public Vector2 textOffset;

	public uint state = 9999u;

	protected Sprite spriteIcon;

	protected SpriteString textTitle;

	protected SpriteString textTitleShadow;

	protected string _text = "";

	public string text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			RenderText();
		}
	}

	public bool visible
	{
		set
		{
			spriteIcon.visible = value;
			textTitle.visible = value;
			textTitleShadow.visible = value;
		}
	}

	public BuildUIButton(BuildUI oUI, Vector2 oOffset, Color oTextColor, string xIconPath, SpriteFont oFont, SpriteString.Align oAlign, Vector2 oTextOffset)
	{
		ui = oUI;
		offset = oOffset;
		textColor = oTextColor;
		iconPath = xIconPath;
		font = oFont;
		align = oAlign;
		textOffset = oTextOffset;
		Load();
	}

	protected virtual void Load()
	{
		spriteIcon = new Sprite(ui.spriteManager);
		spriteIcon.texture = GameEngine.SceneContent.Load<Texture2D>(iconPath);
		textTitleShadow = new SpriteString(ui.spriteManager, font, text, 0f);
		textTitleShadow.align = align;
		textTitleShadow.color = COLOR_SHADOW;
		textTitleShadow.lineHeight = 16f;
		textTitle = new SpriteString(ui.spriteManager, font, text, 0f);
		textTitle.align = align;
		textTitle.color = textColor;
		textTitle.lineHeight = 16f;
	}

	public virtual void Update(GameTime elapsed)
	{
		uint num = GetState();
		if (num != state)
		{
			SetState(num);
		}
	}

	public virtual void Dispose()
	{
		spriteIcon.Dispose();
		textTitle.Dispose();
		textTitleShadow.Dispose();
	}

	public virtual void RenderText()
	{
		if (align == SpriteString.Align.Right)
		{
			textTitle.Set(_text, spriteIcon.position.X + textOffset.X - TEXT_WIDTH, spriteIcon.position.Y + textOffset.Y, TEXT_WIDTH, align);
		}
		else
		{
			textTitle.Set(_text, spriteIcon.position.X + textOffset.X, spriteIcon.position.Y + textOffset.Y, TEXT_WIDTH, align);
		}
		textTitleShadow.Set(_text, textTitle.position.X, textTitle.position.Y + 3f, TEXT_WIDTH, align);
	}

	public virtual void Render()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = (int)((float)width * 0.1f);
		int num2 = (int)((float)height * 0.1f);
		int num3 = (int)((float)width * 0.8f);
		int num4 = (int)((float)height * 0.8f);
		float num5 = (float)num + (float)num3 * 0.5f;
		float num6 = num2 + num4;
		spriteIcon.position.X = num5 + offset.X;
		spriteIcon.position.Y = num6 + offset.Y;
		RenderText();
	}

	public virtual uint GetState()
	{
		return 0u;
	}

	public virtual void SetState(uint xState)
	{
		state = xState;
		if (DataManager.local.settings.showBuildHelpBar)
		{
			if (state == 0)
			{
				spriteIcon.tint = Color.White * 0.25f;
				textTitle.visible = false;
				textTitleShadow.visible = false;
			}
			else
			{
				spriteIcon.tint = Color.White;
				visible = true;
			}
		}
		else
		{
			visible = false;
		}
	}
}
