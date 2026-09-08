using System;
using Microsoft.Xna.Framework;

namespace JamSouls;

public class BattleMode : ScenaricEntitie
{
	private GameState m_GameInstance;

	private Random Randomize;

	public bool m_BattleMode;

	private Vector2 m_BtOffset = new Vector2(0f, -100f);

	private Vector2 m_BtPos = Vector2.Zero;

	private bool m_bActive;

	public Sprite m_Splash;

	public AnimatedSprite m_Flux_Lr;

	public AnimatedSprite m_Flux_rL;

	public AnimatedSprite m_BattleBt;

	public AudioClip m_JamsoulAppearCenter;

	public AudioClip m_JamsoulAppearChar;

	public AudioClip m_JamRun;

	public BattleMode(GameState gameInstance)
	{
		m_GameInstance = gameInstance;
		Randomize = new Random();
		m_Splash = gameInstance.LoadSprite("Splat", GameState.GameAtlas.GAME);
		m_Flux_Lr = gameInstance.LoadAnimatedSpriteFromXml("BattleScreen/Flux.xml", GameState.GameAtlas.GAME, "Flux_lr");
		m_Flux_rL = gameInstance.LoadAnimatedSpriteFromXml("BattleScreen/Flux.xml", GameState.GameAtlas.GAME, "Flux_rl");
		m_BattleBt = gameInstance.LoadAnimatedSpriteFromXml("BattleScreen/BattleBt.xml", GameState.GameAtlas.GAME, "BattleBt");
		m_JamsoulAppearCenter = new AudioClip("JamSoul_PowerUp_Center");
		m_JamsoulAppearChar = new AudioClip("Jamsoul_PowerUp_Char");
		m_JamRun = new AudioClip("JamSoul_PowerUp_Run");
	}

	public void StartBattle()
	{
		m_bActive = true;
	}

	public override void Update(GameTime gametime)
	{
		if (m_bActive)
		{
			m_GameInstance.ScreenManager.AddScreen(new BattleScreen(m_GameInstance), PlayerIndex.One);
			m_BattleMode = true;
			m_bActive = false;
		}
	}

	public void Stop()
	{
		m_BattleMode = false;
		m_bActive = false;
	}

	public override void Draw()
	{
	}
}
