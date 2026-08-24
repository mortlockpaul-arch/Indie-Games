using System;
using GKEngine;
using GKEngine.Entities;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Play;

public class PlayTitles
{
	private static float[] TIME = new float[2] { 3000f, 1000f };

	private static Color[] STRING_COLORS = new Color[4]
	{
		new Color(67, 126, 59),
		new Color(146, 214, 20),
		new Color(22, 2, 13),
		new Color(255, 255, 255)
	};

	public PlayUniverse universe;

	public SpriteManager spriteManager;

	public SpriteFont fontKA_40;

	public SpriteFont fontKA_60;

	private Sprite spriteStripe;

	private SpriteString stringChapter;

	private SpriteString stringChapterShadow;

	private SpriteString stringTitle;

	private SpriteString stringTitleShadow;

	private SpriteString[] strings;

	private float time;

	private int stage;

	private bool active;

	public PlayTitles(PlayUniverse pUniverse)
	{
		universe = pUniverse;
		Init();
	}

	private void Init()
	{
		spriteManager = new SpriteManager(universe.scene, universe.scene.RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		spriteManager.effect = null;
		Load();
	}

	private void Load()
	{
		spriteManager.Load();
		fontKA_40 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_40");
		fontKA_60 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_60");
		spriteStripe = new Sprite(spriteManager);
		spriteStripe.texture = GameEngine.instance.GetSolidColorTexture(new Color(22, 2, 13, 148));
		stringChapterShadow = new SpriteString(spriteManager, fontKA_40, "", GameEngine.Graphics.GraphicsDevice.Viewport.Width);
		stringChapterShadow.color = STRING_COLORS[0];
		stringChapter = new SpriteString(spriteManager, fontKA_40, "", GameEngine.Graphics.GraphicsDevice.Viewport.Width);
		stringChapter.color = STRING_COLORS[1];
		stringTitleShadow = new SpriteString(spriteManager, fontKA_60, "", GameEngine.Graphics.GraphicsDevice.Viewport.Width);
		stringTitleShadow.color = STRING_COLORS[2];
		stringTitle = new SpriteString(spriteManager, fontKA_60, "", GameEngine.Graphics.GraphicsDevice.Viewport.Width);
		stringTitle.color = STRING_COLORS[3];
		strings = new SpriteString[4] { stringChapterShadow, stringChapter, stringTitleShadow, stringTitle };
	}

	public void Update(GameTime elapsed)
	{
		if (!active)
		{
			return;
		}
		time += (float)elapsed.ElapsedGameTime.TotalMilliseconds;
		if (time >= TIME[stage])
		{
			time = 0f;
			if (stage >= 1)
			{
				Lerp(0f);
				HideSprites();
				active = false;
			}
			else
			{
				stage++;
			}
		}
		if (stage == 1)
		{
			Lerp(time / TIME[stage]);
		}
	}

	public void Dispose()
	{
		fontKA_40 = null;
		fontKA_60 = null;
		spriteManager.Dispose();
		spriteStripe.Dispose();
		for (int i = 0; i < strings.Length; i++)
		{
			strings[i].Dispose();
			strings[i] = null;
		}
		strings = null;
		stringChapterShadow = null;
		stringChapter = null;
		stringTitleShadow = null;
		stringTitle = null;
		spriteStripe = null;
	}

	private void HideSprites()
	{
		spriteStripe.visible = false;
		for (int i = 0; i < strings.Length; i++)
		{
			strings[i].visible = false;
		}
	}

	private void SetAlpha(float xValue)
	{
		ref Color tint = ref spriteStripe.tint;
		ref Color tint2 = ref spriteStripe.tint;
		ref Color tint3 = ref spriteStripe.tint;
		byte b = (spriteStripe.tint.A = (byte)(xValue * 255f));
		byte b3 = (tint3.B = b);
		byte r = (tint2.G = b3);
		tint.R = r;
		for (int i = 0; i < strings.Length; i++)
		{
			strings[i].color.R = (byte)((float)(int)STRING_COLORS[i].R * xValue);
			strings[i].color.G = (byte)((float)(int)STRING_COLORS[i].G * xValue);
			strings[i].color.B = (byte)((float)(int)STRING_COLORS[i].B * xValue);
			strings[i].color.A = (byte)((float)(int)STRING_COLORS[i].A * xValue);
		}
	}

	private void Render()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		_ = DataManager.local.settings.screen.X;
		_ = DataManager.local.settings.screen.Y;
		_ = DataManager.local.settings.screen.Width;
		_ = DataManager.local.settings.screen.Height;
		HideSprites();
		spriteStripe.visible = true;
		spriteStripe.scale.X = (float)width / spriteStripe.size.X;
		spriteStripe.scale.Y = 200f;
		spriteStripe.position.X = 0f;
		spriteStripe.position.Y = ((float)height - spriteStripe.scale.Y) * 0.5f;
		for (int i = 0; i < strings.Length; i++)
		{
			strings[i].visible = true;
		}
	}

	private void RenderText(string xChapter, string xTitle)
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		stringChapter.Set(xChapter, 0f, spriteStripe.position.Y + 25f, width, SpriteString.Align.Center);
		stringChapterShadow.Set(xChapter, 0f, spriteStripe.position.Y + 25f + 3f, width, SpriteString.Align.Center);
		stringTitle.Set(xTitle, 0f, spriteStripe.position.Y + 65f, width, SpriteString.Align.Center);
		stringTitleShadow.Set(xTitle, 0f, spriteStripe.position.Y + 65f + 3f, width, SpriteString.Align.Center);
	}

	public void Start(string xChapter, string xTitle)
	{
		time = 0f;
		stage = 0;
		Render();
		RenderText(xChapter, xTitle);
		active = true;
	}

	private void Lerp(float xValue)
	{
		float alpha = (float)Math.Sin(Math.PI / 2.0 * (double)(1f - xValue));
		SetAlpha(alpha);
	}

	public void Event_Resize()
	{
		Render();
	}
}
