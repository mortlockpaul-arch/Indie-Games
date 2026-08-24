using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogScreen : Dialog
{
	public delegate void DialogCloseDelegate();

	private const float MOVE_SPEED = 0.03f;

	private InputEntity inputA;

	private InputEntity inputLS;

	private InputEntity inputRS;

	private bool inputsActive;

	public DialogCloseDelegate __completed;

	private Sprite spriteBackground;

	private Sprite spriteLeft;

	private Sprite spriteRight;

	private Sprite spriteMessage;

	private Vector2 moveLeft = default(Vector2);

	private Vector2 moveRight = default(Vector2);

	public DialogScreen(DialogManager oManager)
		: base(oManager, null, null, null, null)
	{
		timeIn = 500f;
		timeOut = 500f;
		Init();
	}

	public override void Load()
	{
		base.Load();
		spriteLeft = new Sprite(manager.spriteManager);
		spriteLeft.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Screen/Left");
		spriteRight = new Sprite(manager.spriteManager);
		spriteRight.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Screen/Right");
		spriteMessage = new Sprite(manager.spriteManager);
		spriteMessage.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Screen/Message");
	}

	public override void Init()
	{
		Load();
		base.Init();
	}

	public override void Dispose()
	{
		base.Dispose();
		spriteLeft.Dispose();
		spriteRight.Dispose();
		spriteMessage.Dispose();
	}

	public override void Hide()
	{
		base.Hide();
		spriteLeft.visible = false;
		spriteRight.visible = false;
		spriteMessage.visible = false;
	}

	public override void Show()
	{
		base.Show();
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		Rectangle titleSafeArea = GameEngine.Graphics.GraphicsDevice.Viewport.TitleSafeArea;
		spriteLeft.position.X = ((DataManager.local.settings.screen.Width <= 0) ? titleSafeArea.X : DataManager.local.settings.screen.X);
		spriteLeft.position.Y = ((DataManager.local.settings.screen.Width <= 0) ? titleSafeArea.Y : DataManager.local.settings.screen.Y);
		spriteLeft.visible = true;
		spriteRight.position.X = ((DataManager.local.settings.screen.Width <= 0) ? ((float)titleSafeArea.Right - spriteRight.size.X) : ((float)DataManager.local.settings.screen.Right - spriteRight.size.X));
		spriteRight.position.Y = ((DataManager.local.settings.screen.Width <= 0) ? ((float)titleSafeArea.Bottom - spriteRight.size.Y) : ((float)DataManager.local.settings.screen.Bottom - spriteRight.size.Y));
		spriteRight.visible = true;
		spriteMessage.position.X = ((float)width - spriteMessage.size.X) * 0.5f;
		spriteMessage.position.Y = ((float)height - spriteMessage.size.Y) * 0.5f;
		spriteMessage.visible = true;
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		float num = (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
		spriteLeft.position.X = MathHelper.Clamp(spriteLeft.position.X + moveLeft.X * num * 0.03f, 0f, GameEngine.Graphics.GraphicsDevice.Viewport.TitleSafeArea.Left);
		spriteLeft.position.Y = MathHelper.Clamp(spriteLeft.position.Y + moveLeft.Y * num * 0.03f, 0f, GameEngine.Graphics.GraphicsDevice.Viewport.TitleSafeArea.Top);
		spriteRight.position.X = MathHelper.Clamp(spriteRight.position.X + moveRight.X * num * 0.03f, (float)GameEngine.Graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - spriteRight.size.X, (float)GameEngine.Graphics.GraphicsDevice.Viewport.Width - spriteRight.size.X);
		spriteRight.position.Y = MathHelper.Clamp(spriteRight.position.Y + moveRight.Y * num * 0.03f, (float)GameEngine.Graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - spriteRight.size.Y, (float)GameEngine.Graphics.GraphicsDevice.Viewport.Height - spriteRight.size.Y);
	}

	private void Start()
	{
		Input_Set();
	}

	private void Halt()
	{
		Input_Clear();
		moveLeft.X = 0f;
		moveLeft.Y = 0f;
		moveRight.X = 0f;
		moveRight.Y = 0f;
		DataManager.local.settings.screen.X = (int)spriteLeft.position.X;
		DataManager.local.settings.screen.Y = (int)spriteLeft.position.Y;
		DataManager.local.settings.screen.Width = (int)(spriteRight.position.X + spriteRight.size.X - spriteLeft.position.X);
		DataManager.local.settings.screen.Height = (int)(spriteRight.position.Y + spriteRight.size.Y - spriteLeft.position.Y);
		DataManager.local.settings.resolution.X = GameEngine.Graphics.GraphicsDevice.DisplayMode.Width;
		DataManager.local.settings.resolution.Y = GameEngine.Graphics.GraphicsDevice.DisplayMode.Height;
		DataManager.PlayerData_Save(delegate
		{
			if (__completed != null)
			{
				__completed();
			}
		}, manager.Message_Saving_Show, manager.Message_Saving_Hide);
	}

	public override void Input_Update(GameTime oGameTime)
	{
		base.Input_Update(oGameTime);
		if (inputsActive)
		{
			if ((double)inputLS.value2D.X > 0.3 || (double)inputLS.value2D.X < -0.3 || (double)inputLS.value2D.Y > 0.3 || (double)inputLS.value2D.Y < -0.3)
			{
				moveLeft.X = inputLS.value2D.X;
				moveLeft.Y = inputLS.value2D.Y * -1f;
			}
			else
			{
				moveLeft.X = 0f;
				moveLeft.Y = 0f;
			}
			if ((double)inputRS.value2D.X > 0.3 || (double)inputRS.value2D.X < -0.3 || (double)inputRS.value2D.Y > 0.3 || (double)inputRS.value2D.Y < -0.3)
			{
				moveRight.X = inputRS.value2D.X;
				moveRight.Y = inputRS.value2D.Y * -1f;
			}
			else
			{
				moveRight.X = 0f;
				moveRight.Y = 0f;
			}
			if (inputA.pressed)
			{
				Halt();
			}
		}
	}

	private void Input_Set()
	{
		inputA = new InputEntity(InputEntity.Type.Button, "ButtonA", InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputA);
		inputA.Add(new InputButton(GamePadButton.A));
		inputA.active = true;
		inputLS = new InputEntity(InputEntity.Type.Analog2D, "LeftStick", InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputLS);
		inputLS.Add(new InputAnalog2D(GamePadAnalog2D.Left));
		inputLS.active = true;
		inputRS = new InputEntity(InputEntity.Type.Analog2D, "RightStick", InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputRS);
		inputRS.Add(new InputAnalog2D(GamePadAnalog2D.Right));
		inputRS.active = true;
		inputsActive = true;
	}

	private void Input_Clear()
	{
		inputsActive = false;
		UniversalInput.InputEntity_Remove(inputA);
		UniversalInput.InputEntity_Remove(inputLS);
		UniversalInput.InputEntity_Remove(inputRS);
		inputA = null;
		inputLS = null;
		inputRS = null;
	}

	public override void Event_In_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}

	public override void Event_In_Done()
	{
		Start();
	}

	public override void Event_Out_Start()
	{
		base.Event_Out_Start();
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * (1f - xRatio)));
	}
}
