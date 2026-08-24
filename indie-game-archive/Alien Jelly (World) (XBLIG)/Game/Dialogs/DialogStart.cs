using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogStart : Dialog
{
	public delegate void DialogCloseDelegate();

	private const int PULSE_TIME = 2000;

	public DialogCloseDelegate __completed;

	private Sprite spriteLogo;

	private Sprite spriteButton;

	private bool pulse;

	private float pulseTime;

	private bool lookingForIndex;

	public DialogStart(DialogManager oManager)
		: base(oManager, null, null, null, null)
	{
		postIndex = 1;
		timeIn = 500f;
		timeOut = 500f;
		Init();
	}

	public override void Load()
	{
		base.Load();
		spriteLogo = new Sprite(manager.spriteManager);
		spriteLogo.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Start/Logo");
		spriteButton = new Sprite(manager.spriteManager);
		spriteButton.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Start/PressStart");
	}

	public override void Init()
	{
		Load();
		base.Init();
	}

	public override void Dispose()
	{
		base.Dispose();
		spriteLogo.Dispose();
		spriteButton.Dispose();
	}

	public override void Hide()
	{
		base.Hide();
		spriteLogo.visible = false;
		spriteButton.visible = false;
	}

	public override void Show()
	{
		base.Show();
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		spriteLogo.position.X = ((float)width - spriteLogo.size.X) * 0.5f;
		spriteLogo.position.Y = ((float)height - spriteLogo.size.Y) * 0.5f;
		spriteLogo.visible = true;
		spriteButton.position.X = ((float)width - spriteButton.size.X) * 0.5f;
		spriteButton.position.Y = spriteLogo.position.Y + 380f;
		spriteButton.visible = true;
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		if (pulse)
		{
			pulseTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (pulseTime >= 2000f)
			{
				pulseTime %= 2000f;
			}
			float num = pulseTime / 2000f;
			float num2 = (float)(Math.Cos((double)num * Math.PI * 2.0) + 1.0) * 0.5f;
			spriteButton.Tint_SetAll((byte)(127f + num2 * 128f));
		}
	}

	public override void Input_Update(GameTime oGameTime)
	{
		base.Input_Update(oGameTime);
		if (!lookingForIndex)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (UniversalInput.GamePadButtonDowned(GamePadButton.Start, i))
			{
				lookingForIndex = false;
				UniversalInput.GamePadSetPrimaryIndex(i);
				manager.audio.EventCues_Trigger("Sound_Click_0");
				break;
			}
		}
		if (!lookingForIndex && __completed != null)
		{
			__completed();
		}
	}

	public override void Event_In_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}

	public override void Event_In_Done()
	{
		pulse = true;
		lookingForIndex = true;
	}

	public override void Event_Out_Start()
	{
		pulse = true;
		base.Event_Out_Start();
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * (1f - xRatio)));
	}
}
