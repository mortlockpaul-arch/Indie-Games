using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogMenu : Dialog
{
	public delegate void MenuDialogShowDelegate(DialogMenu oDialog);

	private const int SELECTION_TIME = 150;

	private const float SELECTION_DEADZONE = 0.5f;

	public List<DialogMenuOption> options;

	public List<DialogMenuButtonLable> lables;

	public List<SpriteString> strings;

	public List<Sprite> sprites;

	public int selectedIndex;

	public MenuDialogShowDelegate show;

	protected Sprite barSelect;

	protected Sprite arrowUp;

	protected Sprite arrowDown;

	protected int inputTimeVerticle;

	protected int inputTimeHorizontal;

	protected int optionsHeightDefault = 26;

	protected int optionsHeightSelected = 55;

	protected int optionsOffsetTop;

	protected int optionsHeight = 250;

	protected int optionsBarOffset = 5;

	public bool optionsRender = true;

	protected int lablesOffsetTop = 270;

	protected int lablesSpacing = 10;

	public DialogMenu(DialogManager oManager, List<DialogMenuOption> aOptions, List<DialogMenuButtonLable> aLables)
		: base(oManager, null, null, null, null)
	{
		options = aOptions;
		lables = aLables;
		strings = new List<SpriteString>();
		sprites = new List<Sprite>();
		Init();
	}

	public override void Load()
	{
		base.Load();
		barSelect = new Sprite(manager.spriteManager);
		barSelect.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Bar");
		barSelect.axisType = Sprite.AxisType.Center;
		sprites.Add(barSelect);
		arrowUp = new Sprite(manager.spriteManager);
		arrowUp.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Arrow_Up");
		arrowUp.axisType = Sprite.AxisType.Center;
		sprites.Add(arrowUp);
		arrowDown = new Sprite(manager.spriteManager);
		arrowDown.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Menu Dialogs/Common/Arrow_Down");
		arrowDown.axisType = Sprite.AxisType.Center;
		sprites.Add(arrowDown);
		Lables_Load();
		Options_Load();
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
		Lables_Dispose();
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
		Lables_Hide();
		Options_Hide();
	}

	public override void Show()
	{
		base.Show();
		selectedIndex = 0;
		int? num = manager.data as int?;
		if (num.HasValue)
		{
			if (options.Count > num.Value)
			{
				selectedIndex = num.Value;
			}
			manager.data = null;
		}
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
		Lables_Show();
		Options_Show();
	}

	public virtual void SetAlpha(byte xAlpha)
	{
		manager.spriteManager.Tint_SetAll(xAlpha);
	}

	protected void Lables_Load()
	{
		for (int i = 0; i < lables.Count; i++)
		{
			lables[i].Load(manager);
		}
	}

	protected void Lables_SetColors(Color oTitle, Color oShadow)
	{
		for (int i = 0; i < lables.Count; i++)
		{
			lables[i].SetColor(oTitle, oShadow);
		}
	}

	public void Lables_Dispose()
	{
		for (int i = 0; i < lables.Count; i++)
		{
			lables[i].Dispose();
		}
		lables.Clear();
	}

	protected void Lables_Show()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = 0;
		int num2 = 0;
		int num3 = width;
		int num4 = height;
		if (DataManager.local != null)
		{
			num = DataManager.local.settings.screen.X;
			num2 = DataManager.local.settings.screen.Y;
			num3 = DataManager.local.settings.screen.Width;
			num4 = DataManager.local.settings.screen.Height;
		}
		float num5 = 0f;
		float num6 = 0f;
		for (int i = 0; i < lables.Count; i++)
		{
			num5 += lables[i].GetWidth() + (float)lablesSpacing;
		}
		num5 -= (float)lablesSpacing;
		for (int i = 0; i < lables.Count; i++)
		{
			lables[i].position.X = (float)num + (float)num3 * 0.5f - num5 * 0.5f + num6;
			lables[i].position.Y = (float)num2 + (float)num4 * 0.5f + (float)lablesOffsetTop;
			lables[i].Refresh();
			lables[i].visible = true;
			num6 += lables[i].GetWidth() + (float)lablesSpacing;
		}
	}

	protected void Lables_Hide()
	{
		for (int i = 0; i < lables.Count; i++)
		{
			lables[i].visible = false;
		}
	}

	protected void Lables_Execute(DialogMenuButtonLable.Button oButton)
	{
		for (int i = 0; i < lables.Count; i++)
		{
			if (lables[i].button == oButton && lables[i].action != null)
			{
				if (lables[i].actionImmediate)
				{
					paused = true;
					lables[i].action(this);
				}
				else
				{
					manager.Dialog_Out((DialogManager.ExitEvent)(lables[i].button + 1), lables[i].action);
				}
				break;
			}
		}
	}

	public void Lables_Refresh()
	{
		Lables_Load();
		Lables_Show();
	}

	protected virtual void Options_Load()
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].Load(manager);
		}
	}

	public virtual void Options_Show()
	{
		Options_Hide();
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = 0;
		int num2 = 0;
		int num3 = width;
		int num4 = height;
		if (DataManager.local != null)
		{
			num = DataManager.local.settings.screen.X;
			num2 = DataManager.local.settings.screen.Y;
			num3 = DataManager.local.settings.screen.Width;
			num4 = DataManager.local.settings.screen.Height;
		}
		float num5 = (float)num2 + (float)num4 * 0.5f + (float)optionsOffsetTop;
		uint num6 = 1 + (uint)Math.Floor((float)((optionsHeight - optionsHeightSelected) / optionsHeightDefault));
		optionsHeight = (int)((num6 - 1) * optionsHeightDefault + optionsHeightSelected);
		float num7 = ((options.Count >= num6) ? num5 : (num5 + (float)(optionsHeight - ((options.Count - 1) * optionsHeightDefault + optionsHeightSelected)) * 0.5f));
		if (selectedIndex > num6 - 1)
		{
			num7 -= (float)((selectedIndex - (num6 - 1)) * optionsHeightDefault);
		}
		float num8 = num7;
		for (int i = 0; i < options.Count; i++)
		{
			float num9 = ((i == selectedIndex) ? optionsHeightSelected : optionsHeightDefault);
			num8 += num9 * 0.5f;
			options[i].SetState(i == selectedIndex);
			options[i].X = (float)num + (float)num3 * 0.5f;
			options[i].Y = num8;
			num8 += num9 * 0.5f;
			options[i].visible = options[i].Y - num9 * 0.5f >= num5 && options[i].Y + num9 * 0.5f <= num5 + (float)optionsHeight;
			if (options[i].show != null)
			{
				options[i].show(this, options[i]);
			}
		}
		if (options.Count > 0)
		{
			barSelect.visible = true;
			barSelect.position.X = options[selectedIndex].X;
			barSelect.position.Y = options[selectedIndex].Y + (float)optionsBarOffset;
		}
		else
		{
			barSelect.visible = false;
		}
		arrowUp.position.X = (float)num + (float)num3 * 0.5f;
		arrowUp.position.Y = num5 - 7f;
		arrowDown.position.X = (float)num + (float)num3 * 0.5f;
		arrowDown.position.Y = num5 + (float)optionsHeight + 10f;
		arrowUp.visible = selectedIndex >= num6;
		arrowDown.visible = options.Count > num6 && selectedIndex < options.Count - 1;
		if (!optionsRender)
		{
			Options_Hide();
		}
	}

	protected void Options_Hide()
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].visible = false;
		}
	}

	protected virtual void Options_OffsetSelect(int xDir)
	{
		if (Options_ActiveCount() <= 1 || !optionsRender)
		{
			return;
		}
		bool flag = false;
		int num = 0;
		int num2 = selectedIndex;
		while (!flag && num <= options.Count)
		{
			num2 += xDir;
			num2 %= options.Count;
			num2 = ((num2 < 0) ? (options.Count + num2) : num2);
			if (!options[num2].deactivated)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			selectedIndex = num2;
			manager.audio.EventCues_Trigger("Sound_Over_0");
			Options_Show();
		}
	}

	protected void Options_Execute()
	{
		if (options.Count > 0 && !options[selectedIndex].deactivated)
		{
			manager.audio.EventCues_Trigger("Sound_Click_0");
			if (options[selectedIndex].autoCloseDialog)
			{
				manager.Dialog_Out(DialogManager.ExitEvent.A, options[selectedIndex].action);
			}
			else if (options[selectedIndex].action != null)
			{
				options[selectedIndex].action(this);
			}
		}
	}

	public virtual void Options_Set(List<DialogMenuOption> aOptions)
	{
		Options_Dispose();
		options = aOptions;
		Options_Load();
	}

	public void Options_Dispose()
	{
		for (int i = 0; i < options.Count; i++)
		{
			options[i].Dispose();
		}
		options.Clear();
	}

	protected virtual void Options_HorizontalSelect(int xDir)
	{
		if (options.Count > 0 && options[selectedIndex].hasHorizontal)
		{
			manager.audio.EventCues_Trigger("Sound_Click_0");
			object obj = options[selectedIndex].data;
			options[selectedIndex].data = xDir;
			options[selectedIndex].action(this);
			options[selectedIndex].data = obj;
		}
	}

	public void Options_Refresh()
	{
		Options_Load();
		if (selectedIndex >= options.Count)
		{
			selectedIndex = Math.Max(options.Count - 1, 0);
		}
		Options_Show();
	}

	private int Options_ActiveCount()
	{
		int num = 0;
		for (int i = 0; i < options.Count; i++)
		{
			num += ((!options[i].deactivated) ? 1 : 0);
		}
		return num;
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (paused)
		{
			return;
		}
		if (UniversalInput.inputEntities["DialogA"].active && UniversalInput.inputEntities["DialogA"].pressed)
		{
			Options_Execute();
		}
		else if (UniversalInput.inputEntities["DialogB"].active && UniversalInput.inputEntities["DialogB"].pressed)
		{
			Lables_Execute(DialogMenuButtonLable.Button.B);
		}
		else if (UniversalInput.inputEntities["DialogX"].active && UniversalInput.inputEntities["DialogX"].pressed)
		{
			Lables_Execute(DialogMenuButtonLable.Button.X);
		}
		else if (UniversalInput.inputEntities["DialogY"].active && UniversalInput.inputEntities["DialogY"].pressed)
		{
			Lables_Execute(DialogMenuButtonLable.Button.Y);
		}
		if (UniversalInput.inputEntities["DialogStick"].value2D.Y > 0.5f || UniversalInput.inputEntities["DialogStick"].value2D.Y < -0.5f || UniversalInput.inputEntities["DialogStickRight"].value2D.Y > 0.5f || UniversalInput.inputEntities["DialogStickRight"].value2D.Y < -0.5f)
		{
			int xDir = (Math.Sign(UniversalInput.inputEntities["DialogStick"].value2D.Y) + Math.Sign(UniversalInput.inputEntities["DialogStickRight"].value2D.Y)) * -1;
			if (inputTimeVerticle == 0)
			{
				Options_OffsetSelect(xDir);
			}
			inputTimeVerticle += oGameTime.ElapsedGameTime.Milliseconds;
			if (inputTimeVerticle >= 150)
			{
				inputTimeVerticle = 0;
			}
		}
		else if (inputTimeVerticle > 0)
		{
			inputTimeVerticle = 0;
		}
		if (UniversalInput.inputEntities["DialogStick"].value2D.X > 0.5f || UniversalInput.inputEntities["DialogStick"].value2D.X < -0.5f)
		{
			int xDir2 = Math.Sign(UniversalInput.inputEntities["DialogStick"].value2D.X);
			if (inputTimeHorizontal == 0)
			{
				Options_HorizontalSelect(xDir2);
			}
			inputTimeHorizontal += oGameTime.ElapsedGameTime.Milliseconds;
			if (inputTimeHorizontal >= 150)
			{
				inputTimeHorizontal = 0;
			}
		}
		else if (inputTimeHorizontal > 0)
		{
			inputTimeHorizontal = 0;
		}
	}

	public override void Event_In_Start()
	{
		manager.audio.EventCues_Trigger("Menu In");
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
		manager.audio.EventCues_Trigger("Menu Out");
		SetAlpha(byte.MaxValue);
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		SetAlpha((byte)(255f * (1f - xRatio)));
	}

	public override void Event_Out_Done()
	{
		SetAlpha(0);
	}
}
