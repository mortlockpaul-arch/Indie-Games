using GKEngine;
using GKEngine.Entities;
using Game.Atoms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Build.UI;

public class BuildUIStatus
{
	public static Color COLOR_TEXT = new Color(249, 180, 6, 255);

	public static Color COLOR_SHADOW = new Color(21, 1, 16, 255);

	public BuildUI ui;

	public uint state = 9999u;

	private Sprite spriteBackground;

	private SpriteString textTitle;

	private SpriteString textTitleShadow;

	private string _text = "";

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
			spriteBackground.visible = value;
			textTitle.visible = value;
			textTitleShadow.visible = value;
		}
	}

	public BuildUIStatus(BuildUI oUI)
	{
		ui = oUI;
		Load();
	}

	private void Load()
	{
		spriteBackground = new Sprite(ui.spriteManager);
		spriteBackground.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Status_Background");
		textTitleShadow = new SpriteString(ui.spriteManager, ui.fontKA_25, text, 0f);
		textTitleShadow.align = SpriteString.Align.Center;
		textTitleShadow.color = COLOR_SHADOW;
		textTitle = new SpriteString(ui.spriteManager, ui.fontKA_25, text, 0f);
		textTitle.align = SpriteString.Align.Center;
		textTitle.color = COLOR_TEXT;
	}

	public virtual void Update(GameTime elapsed)
	{
		uint num = GetState();
		if (num != state)
		{
			SetState(num);
		}
	}

	public void Dispose()
	{
		spriteBackground.Dispose();
		textTitle.Dispose();
		textTitleShadow.Dispose();
	}

	public void RenderText()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = (int)((float)width * 0.1f);
		int num2 = (int)((float)height * 0.1f);
		int num3 = (int)((float)width * 0.8f);
		textTitle.Set(_text, num, num2 + 27 - 5, num3, SpriteString.Align.Center);
		textTitleShadow.Set(_text, num, num2 + 27 - 5 + 3, num3, SpriteString.Align.Center);
		visible = _text.Length > 0;
	}

	public void Render()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = (int)((float)height * 0.1f);
		spriteBackground.position.X = 0f;
		spriteBackground.position.Y = num + 27;
		spriteBackground.scale.X = (float)width / spriteBackground.size.X;
		RenderText();
	}

	public virtual uint GetState()
	{
		uint result = 0u;
		if (ui.universe.mode == BuildUniverse.Modes.Edit)
		{
			result = ((ui.universe.atoms.over != null && ui.universe.atoms.over is AtomMarker) ? 3u : ((ui.universe.atoms.over == null || (ui.universe.atoms.over.properties.Length <= 0 && !(ui.universe.atoms.over is AtomSwitch))) ? 1u : 4u));
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Add)
		{
			result = 2u;
		}
		return result;
	}

	public virtual void SetState(uint xState)
	{
		state = xState;
		visible = state != 0;
		switch (state)
		{
		case 1u:
			text = "";
			break;
		case 2u:
			text = ui.universe.painter.selected.title + " (" + ui.universe.painter.selected.cost + " / " + ui.universe.atoms.BuildUnits_Left() + ")";
			break;
		case 3u:
			text = AtomMarker.PROPERTIES[0].options[ui.universe.atoms.over.properties[0]] + " Marker";
			break;
		case 4u:
			text = "This item has properties. Use the right shoulder / bumper button to set.";
			break;
		}
	}
}
