using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using Game.Atoms;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogIconMenu : Dialog
{
	public delegate void IconMenuDialogShowDelegate(DialogIconMenu oDialog);

	private const int SELECTION_TIME = 300;

	private const float SELECTION_DEADZONE = 0.2f;

	private const int ICONS_SPACING = 110;

	private const int ICONS_SELECT_OFFSET = 3;

	private const string SPRITE_PATH = "Content/UI/Dialogs/Icons/";

	private static Color TEXT_COLOR_TITLE = new Color(221, 18, 73);

	private static Color TEXT_COLOR_TITLE_SHADOW = new Color(172, 6, 45);

	private static Color TEXT_COLOR_NAME = new Color(146, 214, 20);

	private static Color TEXT_COLOR_NAME_SHADOW = new Color(51, 96, 30);

	private static Color TEXT_COLOR_DESC = new Color(147, 143, 139);

	private static Color TEXT_COLOR_DESC_SHADOW = new Color(25, 24, 23);

	private static Color TEXT_COLOR_BUTTON = new Color(147, 143, 139);

	private static Color TEXT_COLOR_BUTTON_SHADOW = new Color(21, 1, 16);

	public List<DialogIconMenuOption> options;

	public List<SpriteString> strings;

	public List<Sprite> sprites;

	public int currentIndex;

	private Sprite currentMarker;

	private Sprite selectedMarker;

	private Sprite vingette;

	private Sprite background;

	private Sprite buttonB;

	private Sprite buttonA;

	public IconMenuDialogShowDelegate show;

	public DialogDelegate exit;

	public SpriteString stringTitle;

	public SpriteString stringTitleShadow;

	public SpriteString stringPartName;

	public SpriteString stringPartNameShadow;

	public SpriteString stringPartDesc;

	public SpriteString stringPartDescShadow;

	public SpriteString stringSelect;

	public SpriteString stringSelectShadow;

	public SpriteString stringBack;

	public SpriteString stringBackShadow;

	private int gridX;

	private int gridY;

	private int selectionTime;

	public DialogIconMenu(DialogManager oManager, int xGridX, int xGridY, List<DialogIconMenuOption> aOptions)
		: base(oManager, null, null, null, null)
	{
		options = aOptions;
		gridX = xGridX;
		gridY = xGridY;
		strings = new List<SpriteString>();
		sprites = new List<Sprite>();
		Init();
	}

	public override void Load()
	{
		base.Load();
		vingette = new Sprite(manager.spriteManager);
		vingette.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Vingette");
		sprites.Add(vingette);
		background = new Sprite(manager.spriteManager);
		background.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Icons/Background");
		sprites.Add(background);
		stringTitleShadow = new SpriteString(manager.spriteManager, manager.fontKA_40, "-", 0f);
		stringTitleShadow.color = TEXT_COLOR_TITLE_SHADOW;
		strings.Add(stringTitleShadow);
		stringTitle = new SpriteString(manager.spriteManager, manager.fontKA_40, "-", 0f);
		stringTitle.color = TEXT_COLOR_TITLE;
		strings.Add(stringTitle);
		stringPartNameShadow = new SpriteString(manager.spriteManager, manager.fontKA_25, "-", 0f);
		stringPartNameShadow.color = TEXT_COLOR_NAME_SHADOW;
		strings.Add(stringPartNameShadow);
		stringPartName = new SpriteString(manager.spriteManager, manager.fontKA_25, "-", 0f);
		stringPartName.color = TEXT_COLOR_NAME;
		strings.Add(stringPartName);
		stringPartDescShadow = new SpriteString(manager.spriteManager, manager.fontKH_15, "-", 680f);
		stringPartDescShadow.align = SpriteString.Align.Center;
		stringPartDescShadow.color = TEXT_COLOR_DESC_SHADOW;
		stringPartDescShadow.lineHeight = 18f;
		strings.Add(stringPartDescShadow);
		stringPartDesc = new SpriteString(manager.spriteManager, manager.fontKH_15, "-", 680f);
		stringPartDesc.align = SpriteString.Align.Center;
		stringPartDesc.color = TEXT_COLOR_DESC;
		stringPartDesc.lineHeight = 18f;
		strings.Add(stringPartDesc);
		stringSelectShadow = new SpriteString(manager.spriteManager, manager.fontKA_20, "SELECT", 0f);
		stringSelectShadow.color = TEXT_COLOR_BUTTON_SHADOW;
		strings.Add(stringSelectShadow);
		stringSelect = new SpriteString(manager.spriteManager, manager.fontKA_20, "SELECT", 0f);
		stringSelect.color = TEXT_COLOR_BUTTON;
		strings.Add(stringSelect);
		stringBackShadow = new SpriteString(manager.spriteManager, manager.fontKA_20, "BACK", 0f);
		stringBackShadow.color = TEXT_COLOR_BUTTON_SHADOW;
		strings.Add(stringBackShadow);
		stringBack = new SpriteString(manager.spriteManager, manager.fontKA_20, "BACK", 0f);
		stringBack.color = TEXT_COLOR_BUTTON;
		strings.Add(stringBack);
		buttonA = new Sprite(manager.spriteManager);
		buttonA.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu dialogs/Common/ButtonA");
		sprites.Add(buttonA);
		buttonB = new Sprite(manager.spriteManager);
		buttonB.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu dialogs/Common/ButtonB");
		sprites.Add(buttonB);
		selectedMarker = new Sprite(manager.spriteManager, "Content/UI/Dialogs/Icons/Selected");
		selectedMarker.Load();
		sprites.Add(selectedMarker);
		Options_Set(options);
		currentMarker = new Sprite(manager.spriteManager, "Content/UI/Dialogs/Icons/Select");
		currentMarker.Load();
		sprites.Add(currentMarker);
	}

	public override void Init()
	{
		Load();
		base.Init();
	}

	public override void Dispose()
	{
		base.Dispose();
		show = null;
		for (int i = 0; i < strings.Count; i++)
		{
			strings[i].Dispose();
		}
		strings.Clear();
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].Dispose();
		}
		sprites.Clear();
		Options_Dispose();
	}

	public override void Hide()
	{
		base.Hide();
		for (int i = 0; i < strings.Count; i++)
		{
			strings[i].visible = false;
		}
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].visible = false;
		}
		Options_Hide();
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
		currentIndex = 0;
		if (show != null)
		{
			show(this);
		}
		for (int i = 0; i < strings.Count; i++)
		{
			strings[i].visible = true;
		}
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].visible = true;
		}
		vingette.scale.X = (float)width / vingette.size.X;
		vingette.scale.Y = (float)height / vingette.size.Y;
		background.position.X = ((float)width - background.size.X) * 0.5f;
		background.position.Y = 0f;
		background.scale.Y = (float)height / background.size.Y;
		stringTitle.X = (float)x + ((float)width2 - stringTitle.width) * 0.5f;
		stringTitle.Y = (float)y + (float)height2 * 0.5f - 310f;
		stringTitleShadow.X = stringTitle.X;
		stringTitleShadow.Y = stringTitle.Y + 4f;
		stringPartName.X = (float)x + ((float)width2 - stringPartName.width) * 0.5f;
		stringPartName.Y = (float)y + (float)height2 * 0.5f - 250f;
		stringPartNameShadow.X = stringPartName.X;
		stringPartNameShadow.Y = stringPartName.Y + 3f;
		stringPartDesc.X = (float)x + ((float)width2 - stringPartDesc.width) * 0.5f;
		stringPartDesc.Y = (float)y + (float)height2 * 0.5f - 210f;
		stringPartDescShadow.X = stringPartDesc.X;
		stringPartDescShadow.Y = stringPartDesc.Y + 2f;
		buttonA.position.X = (float)x + (float)width2 * 0.5f - 50f;
		buttonA.position.Y = (float)y + (float)height2 * 0.5f - 154f;
		buttonB.position.X = (float)x + (float)width2 * 0.5f;
		buttonB.position.Y = (float)y + (float)height2 * 0.5f - 154f;
		stringSelect.X = (float)x + (float)width2 * 0.5f - stringSelect.width - 45f;
		stringSelect.Y = (float)y + (float)height2 * 0.5f - 148f;
		stringSelectShadow.X = stringSelect.X;
		stringSelectShadow.Y = stringSelect.Y + 3f;
		stringBack.X = (float)x + (float)width2 * 0.5f + 45f;
		stringBack.Y = (float)y + (float)height2 * 0.5f - 148f;
		stringBackShadow.X = stringBack.X;
		stringBackShadow.Y = stringBack.Y + 3f;
		Options_Show((int)((float)y + (float)height2 * 0.5f - 100f));
	}

	public virtual void SetAlpha(byte xAlpha)
	{
		manager.spriteManager.Tint_SetAll(xAlpha);
	}

	public void Text_SetTitle(string xTitle)
	{
		int x = DataManager.local.settings.screen.X;
		int y = DataManager.local.settings.screen.Y;
		int width = DataManager.local.settings.screen.Width;
		int height = DataManager.local.settings.screen.Height;
		stringTitle.SetText(xTitle);
		stringTitleShadow.SetText(xTitle);
		stringTitle.X = (float)x + ((float)width - stringTitle.width) * 0.5f;
		stringTitle.Y = (float)y + (float)height * 0.5f - 310f;
		stringTitleShadow.X = stringTitle.X;
		stringTitleShadow.Y = stringTitle.Y + 4f;
	}

	public void Text_SetNameDesc(string xName, string xDesc)
	{
		int x = DataManager.local.settings.screen.X;
		_ = DataManager.local.settings.screen.Y;
		int width = DataManager.local.settings.screen.Width;
		_ = DataManager.local.settings.screen.Height;
		stringPartName.SetText(xName);
		stringPartNameShadow.SetText(xName);
		stringPartDesc.SetText(xDesc);
		stringPartDescShadow.SetText(xDesc);
		stringPartName.X = (float)x + ((float)width - stringPartName.width) * 0.5f;
		stringPartNameShadow.X = stringPartName.X;
		stringPartDesc.X = (float)x + (float)(width - 680) * 0.5f;
		stringPartDescShadow.X = stringPartDesc.X;
	}

	protected void Options_SetAlpha(byte xAlpha)
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].sprite.tint.A = xAlpha;
		}
	}

	protected void Options_Load()
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].Load(manager);
		}
	}

	protected void Options_Show(int xY)
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		_ = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		_ = DataManager.local.settings.screen.X;
		_ = DataManager.local.settings.screen.Y;
		_ = DataManager.local.settings.screen.Width;
		_ = DataManager.local.settings.screen.Height;
		int num = (width - gridX * 110) / 2;
		selectedMarker.visible = false;
		for (int i = 0; i < options.Count; i++)
		{
			options[i].sprite.position.X = i % gridX * 110 + num;
			options[i].sprite.position.Y = (int)Math.Floor((double)i / (double)gridX) * 110 + xY;
			Options_SetState(options[i], currentIndex == i);
			options[i].sprite.visible = true;
			options[i].sprite.tint.A = byte.MaxValue;
			if (options[i].selected)
			{
				selectedMarker.visible = true;
				selectedMarker.position.X = options[i].sprite.position.X - 3f;
				selectedMarker.position.Y = options[i].sprite.position.Y - 3f;
			}
		}
		currentMarker.visible = true;
		Options_Refresh();
	}

	private void Options_SetState(DialogIconMenuOption oOption, bool xValue)
	{
		oOption.SetState(xValue);
		if (xValue)
		{
			currentMarker.position.X = oOption.sprite.position.X - 3f;
			currentMarker.position.Y = oOption.sprite.position.Y - 3f;
		}
	}

	protected void Options_Hide()
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].sprite.visible = false;
		}
	}

	protected void Options_Refresh()
	{
		for (int i = 0; i < options.Count; i++)
		{
			Options_SetState(options[i], i == currentIndex);
		}
		Text_SetNameDesc((options[currentIndex].data as AtomDefinition).title + " (" + (options[currentIndex].data as AtomDefinition).cost + ")", (options[currentIndex].data as AtomDefinition).desc);
	}

	protected void Options_OffsetSelect(int xDirX, int xDirY)
	{
		int num = xDirX + xDirY * gridX;
		if (num + currentIndex >= 0 && num + currentIndex < options.Count)
		{
			currentIndex += num;
		}
		Options_Refresh();
	}

	protected void Options_Execute()
	{
		manager.Dialog_Out(DialogManager.ExitEvent.A, options[currentIndex].action);
	}

	public void Options_Set(List<DialogIconMenuOption> aOptions)
	{
		Options_Dispose();
		options = aOptions;
		Options_Load();
	}

	protected void Options_Dispose()
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].Dispose();
			options[i] = null;
		}
		options.Clear();
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (UniversalInput.inputEntities["DialogA"].active && UniversalInput.inputEntities["DialogA"].pressed)
		{
			Options_Execute();
		}
		else if (UniversalInput.inputEntities["DialogB"].active && UniversalInput.inputEntities["DialogB"].pressed)
		{
			manager.Dialog_Out(DialogManager.ExitEvent.None, exit);
		}
		else if (UniversalInput.inputEntities["DialogStart"].active && UniversalInput.inputEntities["DialogStart"].pressed)
		{
			manager.Dialog_Out(DialogManager.ExitEvent.None, null);
		}
		if (Math.Abs(UniversalInput.inputEntities["DialogStick"].value2D.X) > 0.2f || Math.Abs(UniversalInput.inputEntities["DialogStick"].value2D.Y) > 0.2f)
		{
			int xDirX = 0;
			int xDirY = 0;
			if (Math.Abs(UniversalInput.inputEntities["DialogStick"].value2D.X) > Math.Abs(UniversalInput.inputEntities["DialogStick"].value2D.Y))
			{
				xDirX = Math.Sign(UniversalInput.inputEntities["DialogStick"].value2D.X);
			}
			else
			{
				xDirY = Math.Sign(UniversalInput.inputEntities["DialogStick"].value2D.Y) * -1;
			}
			if (selectionTime == 0)
			{
				Options_OffsetSelect(xDirX, xDirY);
			}
			selectionTime += oGameTime.ElapsedGameTime.Milliseconds;
			if (selectionTime >= 300)
			{
				selectionTime = 0;
			}
		}
		else if (selectionTime > 0)
		{
			selectionTime = 0;
		}
	}

	public override void Event_In_Start()
	{
		SetAlpha(0);
	}

	public override void Event_In_Lerp(float xRatio)
	{
		SetAlpha((byte)(255f * xRatio));
	}

	public override void Event_In_Done()
	{
		SetAlpha(byte.MaxValue);
	}

	public override void Event_Out_Start()
	{
		SetAlpha(byte.MaxValue);
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		xRatio = 1f - xRatio;
		SetAlpha((byte)(255f * xRatio));
	}

	public override void Event_Out_Done()
	{
		SetAlpha(0);
	}
}
