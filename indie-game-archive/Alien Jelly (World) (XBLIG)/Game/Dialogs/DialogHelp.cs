using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogHelp : Dialog
{
	public delegate void HelpDialogDelegate(DialogHelp oDialog);

	private const int SELECTION_TIME = 150;

	private const float SELECTION_DEADZONE = 0.5f;

	protected int index;

	protected float selectionTime;

	protected Sprite spriteBack;

	protected Sprite spriteNext;

	protected Sprite spriteScreen;

	protected Texture2D[] screens;

	protected string[] screenTextures;

	public HelpDialogDelegate show;

	public HelpDialogDelegate close;

	public DialogHelp(DialogManager oManager, string[] aTextures)
		: base(oManager, null, null, null, null)
	{
		screenTextures = aTextures;
		Init();
	}

	public override void Load()
	{
		base.Load();
		spriteScreen = new Sprite(manager.spriteManager);
		screens = new Texture2D[screenTextures.Length];
		for (int i = 0; i < screenTextures.Length; i++)
		{
			screens[i] = GameEngine.Content.Load<Texture2D>(screenTextures[i]);
		}
		spriteBack = new Sprite(manager.spriteManager);
		spriteBack.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Help/Back");
		spriteNext = new Sprite(manager.spriteManager);
		spriteNext.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Help/Next");
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
		spriteBack.Dispose();
		spriteNext.Dispose();
		spriteScreen.Dispose();
		screenTextures = null;
		screens = null;
	}

	public override void Hide()
	{
		base.Hide();
		spriteBack.visible = false;
		spriteNext.visible = false;
		spriteScreen.visible = false;
	}

	public override void Show()
	{
		base.Show();
		index = 0;
		if (show != null)
		{
			show(this);
		}
		Render();
	}

	public virtual void Render()
	{
		_ = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		_ = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int x = DataManager.local.settings.screen.X;
		int y = DataManager.local.settings.screen.Y;
		int width = DataManager.local.settings.screen.Width;
		int height = DataManager.local.settings.screen.Height;
		spriteScreen.texture = screens[index];
		spriteScreen.position.X = (float)x + (float)(width - spriteScreen.texture.Width) * 0.5f;
		spriteScreen.position.Y = (float)y + (float)(height - spriteScreen.texture.Height) * 0.5f;
		spriteScreen.visible = true;
		spriteBack.position.X = (float)x + (float)width * 0.5f - 455f;
		spriteBack.position.Y = (float)y + (float)(height - spriteBack.texture.Height) * 0.5f;
		spriteNext.position.X = (float)x + (float)width * 0.5f + 381f;
		spriteNext.position.Y = (float)y + (float)(height - spriteBack.texture.Height) * 0.5f;
		spriteBack.visible = index > 0;
		spriteNext.visible = index < screens.Length - 1;
	}

	private void Scroll(int xDir)
	{
		index += xDir;
		index = (int)MathHelper.Clamp(index, 0f, screens.Length - 1);
		Render();
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (paused)
		{
			return;
		}
		if (!UniversalInput.inputEntities["DialogA"].active || !UniversalInput.inputEntities["DialogA"].pressed)
		{
			if (UniversalInput.inputEntities["DialogB"].active && UniversalInput.inputEntities["DialogB"].pressed)
			{
				manager.Dialog_Out(DialogManager.ExitEvent.None, delegate
				{
					if (close != null)
					{
						close(this);
					}
				});
			}
			else if ((!UniversalInput.inputEntities["DialogX"].active || !UniversalInput.inputEntities["DialogX"].pressed) && (!UniversalInput.inputEntities["DialogY"].active || !UniversalInput.inputEntities["DialogY"].pressed) && UniversalInput.inputEntities["DialogStart"].active)
			{
				_ = UniversalInput.inputEntities["DialogStart"].pressed;
			}
		}
		if (UniversalInput.inputEntities["DialogStick"].value2D.X > 0.5f || UniversalInput.inputEntities["DialogStick"].value2D.X < -0.5f)
		{
			int xDir = Math.Sign(UniversalInput.inputEntities["DialogStick"].value2D.X);
			if (selectionTime == 0f)
			{
				Scroll(xDir);
			}
			selectionTime += oGameTime.ElapsedGameTime.Milliseconds;
			if (selectionTime >= 150f)
			{
				selectionTime = 0f;
			}
		}
		else if (selectionTime > 0f)
		{
			selectionTime = 0f;
		}
	}

	public override void Event_In_Start()
	{
		manager.spriteManager.Tint_SetAll(0);
	}

	public override void Event_In_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}

	public override void Event_In_Done()
	{
		manager.spriteManager.Tint_SetAll(byte.MaxValue);
	}

	public override void Event_Out_Start()
	{
		manager.spriteManager.Tint_SetAll(byte.MaxValue);
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		xRatio = 1f - xRatio;
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}

	public override void Event_Out_Done()
	{
		manager.spriteManager.Tint_SetAll(0);
	}
}
