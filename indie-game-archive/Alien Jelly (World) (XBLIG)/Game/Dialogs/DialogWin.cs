using System.Collections.Generic;
using GKEngine;
using GKEngine.Animation;
using GKEngine.Entities;
using GKEngine.Input;
using Game.Data;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogWin : Dialog
{
	private static float[] ANIMATION_TIME = new float[10] { 100f, 400f, 400f, 600f, 400f, 200f, 200f, 200f, 200f, 200f };

	private Sprite title;

	private Sprite background;

	private Sprite[] stars = new Sprite[5];

	private SpriteString stringDeaths;

	private SpriteString stringJems;

	private SpriteString stringRewinds;

	private SpriteString stringScore;

	private List<Sprite> sprites = new List<Sprite>();

	private List<SpriteString> strings = new List<SpriteString>();

	private int safeX;

	private int safeY;

	private int safeWidth;

	private int safeHeight;

	private int anchorX;

	private int anchorY;

	public string deaths = "";

	public string jems = "";

	public string rewinds = "";

	public string score = "";

	public uint rating;

	private Range animBackground;

	private Range animTitle;

	private bool animating;

	private uint animState;

	private float animTime;

	public DialogWin(DialogManager oManager)
		: base(oManager, null, null, null, null)
	{
		Init();
	}

	public override void Init()
	{
		timeIn = 500f;
		timeOut = 500f;
		postIndex = 1;
		Load();
		base.Init();
	}

	public override void Load()
	{
		base.Load();
		background = new Sprite(manager.spriteManager);
		background.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Dialogs/Win/Background");
		sprites.Add(background);
		title = new Sprite(manager.spriteManager);
		title.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Dialogs/Win/Title");
		sprites.Add(title);
		stars = new Sprite[5];
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i] = new Sprite(manager.spriteManager);
			stars[i].texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Dialogs/Win/Brain");
			sprites.Add(stars[i]);
		}
		stringDeaths = new SpriteString(manager.spriteManager, manager.fontKA_40, "", 300f);
		stringDeaths.color = new Color(177, 226, 10, 255);
		stringJems = new SpriteString(manager.spriteManager, manager.fontKA_40, "", 300f);
		stringJems.color = new Color(177, 226, 10, 255);
		stringRewinds = new SpriteString(manager.spriteManager, manager.fontKA_40, "", 300f);
		stringRewinds.color = new Color(177, 226, 10, 255);
		stringScore = new SpriteString(manager.spriteManager, manager.fontKA_60, "", 300f);
		stringScore.color = new Color(177, 226, 10, 255);
		strings.Add(stringDeaths);
		strings.Add(stringJems);
		strings.Add(stringRewinds);
		strings.Add(stringScore);
	}

	public override void Dispose()
	{
		base.Dispose();
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
		stars = null;
	}

	public override void Show()
	{
		safeX = DataManager.local.settings.screen.X;
		safeY = DataManager.local.settings.screen.Y;
		safeWidth = DataManager.local.settings.screen.Width;
		safeHeight = DataManager.local.settings.screen.Height;
		anchorX = safeX + (int)((float)safeWidth * 0.5f);
		anchorY = safeY + (int)((float)safeHeight * 0.5f);
		base.Show();
		for (int i = 0; i < strings.Count; i++)
		{
			strings[i].visible = true;
		}
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].visible = true;
		}
		Render();
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
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		if (!animating)
		{
			return;
		}
		animTime += oGameTime.ElapsedGameTime.Milliseconds;
		if (animTime >= ANIMATION_TIME[animState])
		{
			animState++;
			if (animState >= ANIMATION_TIME.Length)
			{
				Anim_Complete();
				return;
			}
			animTime = 0f;
			Anim_SetState();
		}
	}

	private void Opened()
	{
		Anim_Start();
	}

	public void Render()
	{
		stringDeaths.visible = false;
		stringJems.visible = false;
		stringRewinds.visible = false;
		stringScore.visible = false;
		background.visible = true;
		title.visible = true;
		background.position.X = anchorX - 381;
		background.position.Y = anchorY - 319;
		title.position.X = anchorX - 230;
		title.position.Y = anchorY - 348;
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].position.X = anchorX - 234 + i * 90;
			stars[i].position.Y = anchorY + 111;
			stars[i].visible = false;
		}
		stringDeaths.Set(deaths, anchorX + 14, anchorY - 156, 300f, SpriteString.Align.Left);
		stringJems.Set(jems, anchorX + 14, anchorY - 110, 300f, SpriteString.Align.Left);
		stringRewinds.Set(rewinds, anchorX + 14, anchorY - 63, 300f, SpriteString.Align.Left);
		stringScore.Set(score, anchorX - 3, anchorY + 8, 300f, SpriteString.Align.Left);
	}

	public void SetText()
	{
		stringDeaths.SetText(deaths);
		stringJems.SetText(jems);
		stringRewinds.SetText(rewinds);
		stringScore.SetText(score);
	}

	public void SetAlpha(byte xAlpha)
	{
		manager.spriteManager.Tint_SetAll(xAlpha);
	}

	private void Anim_Start()
	{
		animTime = 0f;
		animState = 0u;
		Anim_SetState();
		animating = true;
	}

	private void Anim_SetState()
	{
		switch (animState)
		{
		case 1u:
			stringDeaths.visible = true;
			(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Squish");
			return;
		case 2u:
			stringJems.visible = true;
			(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Squish");
			return;
		case 3u:
			stringRewinds.visible = true;
			(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Squish");
			return;
		case 4u:
			stringScore.visible = true;
			(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Squish");
			return;
		case 0u:
			return;
		}
		uint num = animState - 5;
		if (num >= rating)
		{
			Anim_Complete();
			return;
		}
		stars[num].visible = true;
		(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Collect");
	}

	private void Anim_Complete()
	{
		animating = false;
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (!paused && UniversalInput.inputEntities["DialogA"].active && UniversalInput.inputEntities["DialogA"].pressed)
		{
			(manager.scene as PlayScene).audio.EventCues_Trigger("Sound_Click_0");
			manager.Dialog_Out(DialogManager.ExitEvent.None, delegate
			{
				manager.Show("WonMenu");
			});
		}
	}

	public override void Event_In_Start()
	{
		base.Event_In_Start();
		SetAlpha(0);
		animBackground = new Range((float)safeX - background.size.X, anchorX - 381);
		animTitle = new Range(safeX + safeWidth, anchorX - 230);
		background.position.X = animBackground.Lerp(0f);
		title.position.X = animTitle.Lerp(0f);
	}

	public override void Event_In_Lerp(float xRatio)
	{
		base.Event_In_Lerp(xRatio);
		SetAlpha((byte)(255f * xRatio));
		float xRatio2 = Tween.EaseIn(xRatio);
		background.position.X = animBackground.Lerp(xRatio2);
		title.position.X = animTitle.Lerp(xRatio2);
	}

	public override void Event_In_Done()
	{
		base.Event_In_Done();
		SetAlpha(byte.MaxValue);
		background.position.X = animBackground.Lerp(1f);
		title.position.X = animTitle.Lerp(1f);
		Opened();
	}

	public override void Event_Out_Start()
	{
		base.Event_Out_Start();
		SetAlpha(byte.MaxValue);
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].visible = false;
		}
		animBackground = new Range(anchorX - 381, safeX + safeWidth);
		animTitle = new Range(anchorX - 230, (float)safeX - title.size.X);
		background.position.X = animBackground.Lerp(0f);
		title.position.X = animTitle.Lerp(0f);
	}

	public override void Event_Out_Lerp(float xRatio)
	{
		base.Event_Out_Lerp(xRatio);
		SetAlpha((byte)(255f * (1f - xRatio)));
		float xRatio2 = Tween.EaseIn(xRatio);
		background.position.X = animBackground.Lerp(xRatio2);
		title.position.X = animTitle.Lerp(xRatio2);
	}

	public override void Event_Out_Done()
	{
		base.Event_Out_Done();
		SetAlpha(0);
		background.position.X = animBackground.Lerp(1f);
		title.position.X = animTitle.Lerp(1f);
	}
}
