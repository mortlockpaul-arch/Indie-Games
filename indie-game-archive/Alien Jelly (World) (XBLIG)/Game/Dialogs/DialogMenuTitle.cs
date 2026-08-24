using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using Game.Data;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogMenuTitle : DialogMenu
{
	private Sprite logo;

	private Sprite vingette;

	private Sprite background;

	public DialogMenuTitle(DialogManager oManager, List<DialogMenuOption> aOptions, List<DialogMenuButtonLable> aLables)
		: base(oManager, aOptions, aLables)
	{
		postIndex = 1;
		timeIn = 500f;
	}

	public override void Load()
	{
		vingette = new Sprite(manager.spriteManager);
		vingette.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Vingette");
		background = new Sprite(manager.spriteManager);
		background.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Title/Background");
		logo = new Sprite(manager.spriteManager);
		logo.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Title/Logo");
		base.Load();
		sprites.Add(logo);
		sprites.Add(background);
		sprites.Add(vingette);
	}

	public override void Show()
	{
		base.Show();
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int x = DataManager.local.settings.screen.X;
		int y = DataManager.local.settings.screen.Y;
		int width2 = DataManager.local.settings.screen.Width;
		int height2 = DataManager.local.settings.screen.Height;
		vingette.scale.X = (float)width / vingette.size.X;
		vingette.scale.Y = (float)height / vingette.size.Y;
		logo.position.X = (float)x + (float)width2 * 0.5f - 182f;
		logo.position.Y = (float)y + (float)height2 * 0.5f - 304f;
		background.position.X = (float)x + (float)width2 * 0.5f - 437f;
		background.position.Y = (float)y + (float)height2 * 0.5f - 263f;
	}
}
