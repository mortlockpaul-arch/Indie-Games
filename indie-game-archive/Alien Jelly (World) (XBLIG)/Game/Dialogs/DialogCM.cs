using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogCM : Dialog
{
	public delegate void DialogLoadindDelegate();

	private const int TIME = 1000;

	private static readonly Range BUZZ_SCALE = new Range(1f, 1.5f);

	private Texture2D textureSprites;

	private Sprite spriteTitle;

	private Sprite spriteCM_0;

	private Sprite spriteCM_1;

	private Sprite spriteCM_2;

	private Sprite spriteBackground;

	public DialogLoadindDelegate __opened;

	private bool animationActive;

	private float animationTime;

	private int animationIndex;

	private int animationBuzzIndex;

	private List<List<float>> animationBuzz = new List<List<float>>();

	private List<Sprite> animationBuzzAsset = new List<Sprite>();

	public DialogCM(DialogManager oManager)
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
		textureSprites = GameEngine.Content.Load<Texture2D>("Content/UI/Intro/CM Logo");
		spriteTitle = new Sprite(manager.spriteManager, new Rectangle(1, 148, 271, 53));
		spriteTitle.texture = textureSprites;
		spriteCM_0 = new Sprite(manager.spriteManager, new Rectangle(1, 1, 222, 146));
		spriteCM_0.texture = textureSprites;
		spriteCM_1 = new Sprite(manager.spriteManager, new Rectangle(224, 1, 179, 145));
		spriteCM_1.texture = textureSprites;
		spriteCM_2 = new Sprite(manager.spriteManager, new Rectangle(224, 1, 179, 145));
		spriteCM_2.texture = textureSprites;
	}

	public override void Init()
	{
		Load();
		Range range = new Range(50f, 70f);
		Range range2 = new Range(70f, 90f);
		Range range3 = new Range(3f, 4f);
		for (int i = 0; i < 3; i++)
		{
			int num = (int)Math.Round(range3.random);
			List<float> list = new List<float>();
			for (int j = 0; j < num; j++)
			{
				list.Add(range.random);
				list.Add(range2.random);
			}
			animationBuzz.Add(list);
		}
		animationBuzz[animationBuzz.Count - 1][animationBuzz[animationBuzz.Count - 1].Count - 1] = 3000f;
		animationBuzzAsset.Add(spriteCM_0);
		animationBuzzAsset.Add(spriteCM_1);
		animationBuzzAsset.Add(spriteCM_2);
		base.Init();
	}

	public override void Dispose()
	{
		base.Dispose();
		textureSprites = null;
		spriteTitle.Dispose();
		spriteCM_0.Dispose();
		spriteCM_1.Dispose();
		spriteCM_2.Dispose();
		spriteBackground.Dispose();
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		if (!animationActive)
		{
			return;
		}
		animationTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
		if (!(animationTime >= animationBuzz[animationIndex][animationBuzzIndex]))
		{
			return;
		}
		animationTime = 0f;
		animationBuzzIndex++;
		if (animationBuzzIndex >= animationBuzz[animationIndex].Count)
		{
			animationBuzzIndex = 0;
			animationIndex++;
			if (animationIndex >= animationBuzz.Count)
			{
				animationActive = false;
				__opened();
				__opened = null;
			}
			return;
		}
		animationBuzzAsset[animationIndex].visible = !animationBuzzAsset[animationIndex].visible;
		if (animationBuzzAsset[animationIndex].visible)
		{
			float num = ((float)animationBuzzIndex + 1f) / (float)animationBuzz[animationIndex].Count;
			animationBuzzAsset[animationIndex].tint = new Color(num, num, num);
			((IntroScene)manager.scene).audio.EventCues_Trigger("CM Buzz");
		}
		if (animationIndex >= animationBuzz.Count - 1 && animationBuzzIndex >= animationBuzz[animationIndex].Count - 1)
		{
			((IntroScene)manager.scene).audio.EventCues_Trigger("Collective Mass");
		}
	}

	public override void Hide()
	{
		base.Hide();
		spriteTitle.visible = false;
		spriteBackground.visible = false;
		spriteCM_0.visible = false;
		spriteCM_1.visible = false;
		spriteCM_2.visible = false;
	}

	public override void Show()
	{
		base.Show();
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int num = (int)((float)width * 0.5f);
		int num2 = (int)((float)height * 0.5f);
		spriteBackground.position.X = 0f;
		spriteBackground.position.Y = 0f;
		spriteBackground.scale = new Vector2(width, height);
		spriteBackground.visible = true;
		spriteTitle.position.X = num - 163;
		spriteTitle.position.Y = num2 + 53;
		spriteCM_0.position.X = num - 189;
		spriteCM_0.position.Y = num2 - 106;
		spriteCM_1.position.X = num - 68;
		spriteCM_1.position.Y = num2 - 106;
		spriteCM_2.position.X = num + 10;
		spriteCM_2.position.Y = num2 - 106;
		spriteTitle.visible = true;
		spriteCM_0.visible = false;
		spriteCM_1.visible = false;
		spriteCM_2.visible = false;
	}

	private void Buzz_Start()
	{
		animationIndex = 0;
		animationBuzzIndex = 0;
		animationTime = 0f;
		animationActive = true;
	}

	public override void Event_In_Lerp(float xRatio)
	{
		manager.spriteManager.Tint_SetAll((byte)(255f * xRatio));
	}

	public override void Event_In_Done()
	{
		((IntroScene)GameEngine.scene).Sprites_HideOverlay();
		Buzz_Start();
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		if (xRatio > 0.5f)
		{
			xRatio = 1f - (xRatio - 0.5f) / 0.5f;
			spriteTitle.Tint_SetAll(0);
			spriteCM_0.Tint_SetAll(0);
			spriteCM_1.Tint_SetAll(0);
			spriteCM_2.Tint_SetAll(0);
			spriteBackground.Tint_SetAll((byte)(255f * xRatio));
		}
		else
		{
			xRatio = 1f - xRatio * 2f;
			spriteTitle.Tint_SetAll((byte)(255f * xRatio));
			spriteCM_0.Tint_SetAll((byte)(255f * xRatio));
			spriteCM_1.Tint_SetAll((byte)(255f * xRatio));
			spriteCM_2.Tint_SetAll((byte)(255f * xRatio));
		}
	}
}
