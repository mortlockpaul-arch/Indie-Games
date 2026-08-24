using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogMenuPlay : DialogMenu
{
	private const float DESC_SCALE = 0.75f;

	private static Color COLOR_TITLE = new Color(74, 69, 127);

	private static Color COLOR_TITLE_SHADOW = new Color(21, 1, 16);

	private static Color COLOR_DESC_SHADOW = new Color(32, 21, 45);

	private string _title = "";

	private string _desc = "";

	private Sprite logo;

	private Sprite vingette;

	private Sprite background;

	private SpriteString stringTitle;

	private SpriteString stringTitleShadow;

	private SpriteString stringDesc;

	private SpriteString stringDescShadow;

	public string title
	{
		get
		{
			return _title;
		}
		set
		{
			_title = value;
			stringTitle.SetText(value);
			stringTitleShadow.SetText(value);
		}
	}

	public string desc
	{
		get
		{
			return _desc;
		}
		set
		{
			_desc = value;
			stringDesc.SetText(value);
			stringDescShadow.SetText(value);
		}
	}

	public DialogMenuPlay(DialogManager oManager, string xTitle, string xDesc, List<DialogMenuOption> aOptions, List<DialogMenuButtonLable> aLables)
		: base(oManager, aOptions, aLables)
	{
		_title = xTitle;
		_desc = xDesc;
		postIndex = 1;
		optionsOffsetTop = -30;
		optionsHeight = 230;
		lablesOffsetTop = 212;
	}

	public override void Load()
	{
		vingette = new Sprite(manager.spriteManager);
		vingette.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Vingette");
		background = new Sprite(manager.spriteManager);
		background.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Play/Background");
		logo = new Sprite(manager.spriteManager);
		logo.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Play/Logo");
		base.Load();
		stringTitleShadow = new SpriteString(manager.spriteManager, manager.fontKA_40, title, 600f);
		stringTitle = new SpriteString(manager.spriteManager, manager.fontKA_40, title, 600f);
		stringDescShadow = new SpriteString(manager.spriteManager, manager.fontKH_15, desc, 600f);
		stringDesc = new SpriteString(manager.spriteManager, manager.fontKH_15, desc, 600f);
		stringDesc.lineHeight = 20f;
		stringDescShadow.lineHeight = stringDesc.lineHeight;
		stringTitle.align = SpriteString.Align.Center;
		stringTitleShadow.align = SpriteString.Align.Center;
		stringDesc.align = SpriteString.Align.Center;
		stringDescShadow.align = SpriteString.Align.Center;
		stringTitle.color = COLOR_TITLE;
		stringTitleShadow.color = COLOR_TITLE_SHADOW;
		stringDesc.color = COLOR_TITLE;
		stringDescShadow.color = COLOR_DESC_SHADOW;
		sprites.Add(logo);
		sprites.Add(background);
		sprites.Add(vingette);
		strings.Add(stringTitle);
		strings.Add(stringTitleShadow);
		strings.Add(stringDesc);
		strings.Add(stringDescShadow);
	}

	public override void Show()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int x = DataManager.local.settings.screen.X;
		int y = DataManager.local.settings.screen.Y;
		int width2 = DataManager.local.settings.screen.Width;
		int height2 = DataManager.local.settings.screen.Height;
		vingette.scale.X = (float)width / vingette.size.X;
		vingette.scale.Y = (float)height / vingette.size.Y;
		logo.position.X = (float)x + (float)width2 * 0.5f - 172f;
		logo.position.Y = (float)y + (float)height2 * 0.5f - 331f;
		background.position.X = (float)x + ((float)width2 - background.size.X) * 0.5f;
		background.position.Y = (float)y + (float)height2 * 0.5f - 331f;
		stringTitle.SetText(title);
		stringTitle.X = (float)x + ((float)width2 - stringTitle.length) * 0.5f;
		stringTitle.Y = (float)y + (float)height2 * 0.5f - 196f;
		stringTitle.SetPositions();
		stringTitleShadow.SetText(title);
		stringTitleShadow.X = (float)x + ((float)width2 - stringTitle.length) * 0.5f;
		stringTitleShadow.Y = (float)y + (float)height2 * 0.5f - 196f + 4f;
		stringTitleShadow.SetPositions();
		stringDesc.Set(desc, (float)x + ((float)width2 - stringDesc.length) * 0.5f, (float)y + (float)height2 * 0.5f - 130f, 600f, SpriteString.Align.Center);
		stringDesc.SetPositions();
		stringDescShadow.Set(desc, stringDesc.X, stringDesc.Y + 3f, 600f, SpriteString.Align.Center);
		stringDescShadow.SetPositions();
		base.Show();
	}
}
