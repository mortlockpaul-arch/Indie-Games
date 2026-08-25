using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class Seed : PowerUp
{
	private enum CloneState
	{
		IDLE,
		PLANTED,
		WAIT_DETONATOR,
		EXPLODE
	}

	private const float EXPLOSION_DISTANCE = 100f;

	private const float EXPLODE_TIMER = 300f;

	private AnimatedSprite m_Sprite;

	private Sprite m_StaticSprite;

	private Vector2 m_PlantedPos;

	private Vector2 m_LastPlayerPos;

	private float m_ExplodeTimer;

	private CloneState m_CloneState;

	private AudioClip m_RootExplode;

	private AudioClip m_RootSpawn;

	public Seed(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Seed/PowerUp_SeedAnim.xml", GameState.GameAtlas.GAME, "PowerUp_SeedAnim");
		m_StaticSprite = m_StateInstance.LoadSprite("Seed", GameState.GameAtlas.GAME);
		InitPowerUp(m_StaticSprite.Width, m_StaticSprite.Height, spriteBatch);
		m_RootExplode = new AudioClip("PowerUp_RootExplode");
		m_RootSpawn = new AudioClip("PowerUp_Seed");
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 15000f;
		m_CloneState = CloneState.IDLE;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player == null)
		{
			return;
		}
		switch (m_CloneState)
		{
		case CloneState.IDLE:
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed && m_Player.m_bIsOnGround)
			{
				m_Sprite.Reset();
				m_Effect = m_Player.m_SpriteEffect;
				m_PlantedPos = m_Player.GetPosition();
				m_LastPlayerPos = m_Player.GetPosition();
				m_PlantedPos.X -= m_Sprite.GetFrameWidth() / 2;
				m_PlantedPos.Y -= m_Sprite.GetFrameHeight() / 2;
				m_CloneState = CloneState.PLANTED;
				m_RootSpawn.Play();
			}
			break;
		case CloneState.PLANTED:
			if (m_Sprite.m_CurrentFrame >= m_Sprite.m_TotalFrames - 1)
			{
				m_CloneState = CloneState.WAIT_DETONATOR;
				m_PlantedPos = m_LastPlayerPos;
				m_PlantedPos.X -= m_Player.m_PlayerSprite[0].GetFrameWidth() / 2 + m_Player.m_OffsetX;
				m_PlantedPos.Y -= m_Player.m_PlayerSprite[0].GetFrameHeight() / 2 + m_Player.m_OffsetY;
			}
			m_Sprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			break;
		case CloneState.WAIT_DETONATOR:
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_RootExplode.Play();
				m_StateInstance.ExplodeSeed(m_LastPlayerPos, new Color(200, 255, 200, 255));
				m_ExplodeTimer = 300f;
				m_CloneState = CloneState.EXPLODE;
			}
			break;
		case CloneState.EXPLODE:
			m_ExplodeTimer -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_ExplodeTimer <= 0f)
			{
				m_CloneState = CloneState.IDLE;
			}
			foreach (Player player in m_StateInstance.m_Players)
			{
				if (player.m_Tag == 0 && Vector2.Distance(m_PlantedPos, player.GetPosition()) <= 100f)
				{
					player.m_Tag = 1;
					player.DecreaseScore(1);
				}
			}
			break;
		}
		if (m_Player != null)
		{
			UpdatePosition(gameTime, m_Player.GetPosition());
			base.Update(gameTime);
		}
	}

	public override void StopBonus()
	{
		base.StopBonus();
	}

	public override Vector2 GetNodePosition()
	{
		if (m_CloneState == CloneState.WAIT_DETONATOR)
		{
			return m_PlantedPos;
		}
		return m_MiddlePosition;
	}

	public override void DrawBonus()
	{
		if (m_Player != null)
		{
			if (m_CloneState == CloneState.PLANTED)
			{
				m_Sprite.Draw(ref m_PlantedPos, m_Effect, Color.White, m_zorder);
			}
			else if (m_CloneState == CloneState.WAIT_DETONATOR)
			{
				new Rectangle(0, 0, m_Player.m_PlayerSprite[0].GetFrameWidth(), m_Player.m_PlayerSprite[0].GetFrameHeight());
				m_Player.m_PlayerSprite[0].Draw(ref m_PlantedPos, m_Effect, Color.White, m_zorder + 0.0001f);
			}
		}
		m_StaticSprite.Draw(m_MiddlePosition, Color.White, m_Effect, m_zorder);
		base.DrawBonus();
	}
}
