using System;
using GKEngine;
using GKEngine.Animation;
using GKEngine.Entities;
using GKEngine.Utils;
using Game.Data;
using Game.QBits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Play.Players.UI;

public class PlayerUI
{
	private static Range SCORE_SCALE = new Range(2.2f, 1f);

	private static float SCORE_TIME = 150f;

	private static int SCORE_COUNT = 50;

	public PlayerManager manager;

	public SpriteManager spriteManager;

	public SpriteFont fontKA_40;

	public SpriteFont fontKA_25;

	private Sprite spriteVingette;

	private Sprite spriteScore;

	public SpriteString stringScore;

	private SpriteString stringScoreShadow;

	private Sprite spriteRewind;

	private Sprite spriteRewindMessage;

	private Sprite spriteRewindMessageAuto;

	private Sprite spriteRewindMessageStopping;

	private Sprite spriteRewindBar;

	public SpriteString stringRewindSpeed;

	public SpriteString stringRewindTime;

	public Sprite spriteYToContinue;

	private int _score;

	private int _scoreTo;

	private bool scoreActive;

	private float scoreTime;

	public PlayerScoreManager scoreItems;

	public Texture2D[] playerActiveIcons = new Texture2D[4];

	public Texture2D[] playerNonActiveIcons = new Texture2D[4];

	public Sprite[] spritePlayers = new Sprite[4];

	private QBit qbit;

	private float rewindTimeTotal;

	private float rewindTimeFirst;

	public int score
	{
		get
		{
			return _scoreTo;
		}
		set
		{
			_scoreTo = value;
			_score = ((_scoreTo <= _score) ? _scoreTo : _score);
			Score_Start();
		}
	}

	public PlayerUI(PlayerManager oManager)
	{
		manager = oManager;
		Init();
	}

	private void Init()
	{
		spriteManager = new SpriteManager(manager.scene, manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		spriteManager.effect = null;
		Load();
		scoreItems = new PlayerScoreManager(this);
		Render();
		score = 0;
	}

	private void Load()
	{
		spriteManager.Load();
		fontKA_40 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_40");
		fontKA_25 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_25");
		spriteVingette = new Sprite(spriteManager);
		spriteVingette.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Build/Vingette");
		spriteVingette.tint = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		spriteScore = new Sprite(spriteManager);
		spriteScore.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Play_Score");
		stringScoreShadow = new SpriteString(spriteManager, fontKA_40, "", 0f);
		stringScoreShadow.color = new Color(171, 6, 45, 255);
		stringScore = new SpriteString(spriteManager, fontKA_40, "", 0f);
		stringScore.color = new Color(255, 255, 255, 255);
		for (int i = 0; i < 4; i++)
		{
			playerActiveIcons[i] = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Icons/Player_QBit_" + i);
			playerNonActiveIcons[i] = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Icons/Player_P" + i);
		}
		for (int i = 0; i < 4; i++)
		{
			spritePlayers[i] = new Sprite(spriteManager);
			spritePlayers[i].texture = playerNonActiveIcons[i];
		}
		spriteRewind = new Sprite(spriteManager);
		spriteRewind.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Play_Rewind");
		spriteRewindMessage = new Sprite(spriteManager);
		spriteRewindMessage.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Play_Rewind_Message");
		spriteRewindMessageAuto = new Sprite(spriteManager);
		spriteRewindMessageAuto.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Play_Rewind_Message_Auto");
		spriteRewindMessageStopping = new Sprite(spriteManager);
		spriteRewindMessageStopping.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Play_Rewind_Message_Stopping");
		spriteRewindBar = new Sprite(spriteManager);
		spriteRewindBar.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/Play_Rewind_Bar");
		stringRewindSpeed = new SpriteString(spriteManager, fontKA_25, "", 0f);
		stringRewindSpeed.color = new Color(255, 255, 255, 255);
		stringRewindTime = new SpriteString(spriteManager, fontKA_25, "", 0f);
		stringRewindTime.color = new Color(255, 255, 255, 255);
		spriteYToContinue = new Sprite(spriteManager);
		spriteYToContinue.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Play/YToContinue");
	}

	public void Update(GameTime elapsed)
	{
		Score_Update(elapsed.ElapsedGameTime.Milliseconds);
		scoreItems.Update(elapsed);
	}

	public void Dispose()
	{
		fontKA_40 = null;
		fontKA_25 = null;
		scoreItems.Dispose();
		spriteManager.Dispose();
		spriteRewind.Dispose();
		spriteRewindMessage.Dispose();
		spriteRewindMessageAuto.Dispose();
		spriteRewindMessageStopping.Dispose();
		spriteRewindBar.Dispose();
		spriteVingette.Dispose();
		spriteScore.Dispose();
		spriteYToContinue.Dispose();
		stringScore.Dispose();
		stringScoreShadow.Dispose();
		stringRewindSpeed.Dispose();
		stringRewindTime.Dispose();
	}

	public void HideSprites()
	{
		spriteVingette.visible = false;
		spriteScore.visible = false;
		spriteRewind.visible = false;
		spriteRewindMessageAuto.visible = false;
		spriteRewindMessage.visible = false;
		spriteRewindMessageStopping.visible = false;
		spriteRewindBar.visible = false;
		spriteYToContinue.visible = false;
		stringScore.visible = false;
		stringScoreShadow.visible = false;
		stringRewindTime.visible = false;
		stringRewindSpeed.visible = false;
	}

	public void Render()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		int x = DataManager.local.settings.screen.X;
		int y = DataManager.local.settings.screen.Y;
		int width2 = DataManager.local.settings.screen.Width;
		int height2 = DataManager.local.settings.screen.Height;
		HideSprites();
		spriteVingette.visible = true;
		spriteVingette.scale.X = (float)width / spriteVingette.size.X;
		spriteVingette.scale.Y = (float)height / spriteVingette.size.Y;
		spriteScore.visible = true;
		spriteScore.position.X = x - 80;
		spriteScore.position.Y = y + 2 - 70;
		stringScore.visible = true;
		stringScoreShadow.visible = true;
		spriteRewind.position.X = ((float)width - spriteRewind.size.X) * 0.5f;
		spriteRewind.position.Y = ((float)height - spriteRewind.size.Y) * 0.5f;
		spriteRewindMessageAuto.position.X = spriteRewind.position.X + 80f;
		spriteRewindMessageAuto.position.Y = spriteRewind.position.Y + 162f;
		spriteRewindMessage.position.X = spriteRewind.position.X + 71f;
		spriteRewindMessage.position.Y = spriteRewind.position.Y + 195f;
		spriteRewindMessageStopping.position.X = spriteRewind.position.X + 152f;
		spriteRewindMessageStopping.position.Y = spriteRewind.position.Y + 200f;
		spriteRewindBar.position.X = spriteRewind.position.X + 49f;
		spriteRewindBar.position.Y = spriteRewind.position.Y + 259f;
		spriteYToContinue.position.X = ((float)width - spriteYToContinue.size.X) * 0.5f;
		spriteYToContinue.position.Y = (float)(x + height2) - spriteYToContinue.size.Y - 50f;
		for (int i = 0; i < 4; i++)
		{
			spritePlayers[i].position.X = x + width2 - 220 + i * 50;
			spritePlayers[i].position.Y = y;
		}
	}

	public void RenderText()
	{
		int x = DataManager.local.settings.screen.X;
		int y = DataManager.local.settings.screen.Y;
		stringScore.Set(MathUtils.Commas(_score, 3u), x + 61, y - 8, 300f, SpriteString.Align.Left);
		stringScoreShadow.Set(stringScore.text, x + 61, y - 8 + 2, 300f, SpriteString.Align.Left);
	}

	public void Resolve()
	{
		qbit = null;
		for (int i = 0; i < manager.players.Length; i++)
		{
			if (manager.players[i].qbit != null)
			{
				qbit = manager.players[i].qbit;
				spritePlayers[i].texture = playerActiveIcons[(int)manager.players[i].qbit.type];
			}
			else
			{
				spritePlayers[i].texture = playerNonActiveIcons[i];
			}
		}
	}

	private void Score_Start()
	{
		_score += Math.Min(SCORE_COUNT, _scoreTo - _score);
		RenderText();
		scoreTime = 0f;
		Score_Lerp(0f);
		scoreActive = true;
	}

	private void Score_Update(float elapsed)
	{
		if (!scoreActive)
		{
			return;
		}
		scoreTime += elapsed;
		if (scoreTime >= SCORE_TIME)
		{
			scoreActive = false;
			Score_Lerp(1f);
			if (_scoreTo > _score)
			{
				Score_Start();
			}
		}
		else
		{
			Score_Lerp(scoreTime / SCORE_TIME);
		}
	}

	private void Score_Lerp(float xRatio)
	{
		float num = SCORE_SCALE.Lerp(Tween.EaseIn(xRatio));
		stringScore.scale.X = num;
		stringScore.scale.Y = num;
		stringScoreShadow.scale.X = num;
		stringScoreShadow.scale.Y = num;
	}

	public void RewindShow(bool xAuto, float pTime, float pTimeFirst)
	{
		rewindTimeTotal = pTime;
		rewindTimeFirst = pTimeFirst;
		spriteRewind.visible = true;
		spriteRewindMessageAuto.visible = xAuto;
		spriteRewindMessage.visible = !xAuto;
		spriteRewindBar.visible = true;
		stringRewindTime.visible = true;
		stringRewindSpeed.visible = true;
		RewindUpdate(pTime);
	}

	public void RewindStopping()
	{
		spriteRewindMessageAuto.visible = false;
		spriteRewindMessage.visible = false;
		spriteRewindMessageStopping.visible = true;
	}

	public void RewindHide()
	{
		spriteRewind.visible = false;
		spriteRewindMessage.visible = false;
		spriteRewindMessageAuto.visible = false;
		spriteRewindMessageStopping.visible = false;
		spriteRewindBar.visible = false;
		stringRewindTime.visible = false;
		stringRewindSpeed.visible = false;
	}

	public void RewindUpdate(float pTime)
	{
		float num = (pTime - rewindTimeFirst) / (rewindTimeTotal - rewindTimeFirst);
		if (num > 1f)
		{
			Console.Write((object)num);
		}
		spriteRewindBar.scale.X = num;
		stringRewindTime.Set(MathUtils.FormatTimeHHMMSS(pTime), spriteRewind.position.X + 52f, spriteRewind.position.Y + 281f, 200f, SpriteString.Align.Left);
	}

	public void RewindSetSpeed(float pSpeed)
	{
		stringRewindSpeed.Set(pSpeed + "X", spriteRewind.position.X + 360f, spriteRewind.position.Y + 281f, 100f, SpriteString.Align.Right);
	}

	public void YToContinue_Show()
	{
		spriteYToContinue.visible = true;
	}

	public void YToContinue_Hide()
	{
		spriteYToContinue.visible = false;
	}

	public void Event_Resize()
	{
		Render();
	}
}
