using GKEngine;
using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogLoading : Dialog
{
	public delegate void DialogLoadindDelegate();

	private Sprite spriteLoading;

	private Sprite spriteBackground;

	public DialogLoadindDelegate __opened;

	private int oopCount;

	public DialogLoading(DialogManager oManager)
		: base(oManager, null, null, null, null)
	{
		timeIn = 500f;
		timeOut = 1000f;
		Init();
	}

	public override void Load()
	{
		base.Load();
		spriteBackground = new Sprite(manager.spriteManager);
		spriteBackground.texture = new Texture2D(GameMain.instance.GraphicsDevice, 1, 1);
		spriteBackground.texture.SetData(new Color[1]
		{
			new Color(0, 0, 0, 255)
		});
		spriteLoading = new Sprite(manager.spriteManager);
		spriteLoading.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Loading");
	}

	public override void Init()
	{
		Load();
		base.Init();
	}

	public override void Dispose()
	{
		base.Dispose();
		spriteLoading.Dispose();
		spriteBackground.Dispose();
	}

	public override void Hide()
	{
		base.Hide();
		spriteLoading.visible = false;
		spriteBackground.visible = false;
	}

	public override void Show()
	{
		base.Show();
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		spriteLoading.position.X = ((float)width - spriteLoading.size.X) * 0.5f;
		spriteLoading.position.Y = ((float)height - spriteLoading.size.Y) * 0.5f;
		spriteLoading.visible = true;
		spriteBackground.position.X = 0f;
		spriteBackground.position.Y = 0f;
		spriteBackground.scale = new Vector2(width, height);
		spriteBackground.visible = true;
	}

	public bool OOP(GameTime oGameTime)
	{
		if (oopCount >= 2)
		{
			__opened();
			__opened = null;
			return true;
		}
		oopCount++;
		return false;
	}

	public override void Event_In_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}

	public override void Event_In_Done()
	{
		if (__opened != null)
		{
			oopCount = 0;
			GameEngine.instance.updateStack.Add(OOP);
		}
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		xRatio = 1f - xRatio;
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}
}
