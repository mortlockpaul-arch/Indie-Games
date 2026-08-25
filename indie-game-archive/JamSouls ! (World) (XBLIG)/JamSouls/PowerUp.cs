using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public abstract class PowerUp
{
	private const float MOVE_LATENCY = 150f;

	private const int OFFSET_Y = 2;

	private const float EFFECT_GRAB_TIME = 250f;

	public GameState m_StateInstance;

	public Player m_Player;

	protected bool m_bAvailable;

	public Vector2 m_Position;

	public Vector2 m_MiddlePosition;

	public SpriteBatch m_spriteBatch;

	public float m_GrabRadius;

	public AudioClip m_UseSound;

	public SpriteEffects m_Effect;

	public float m_zorder = 1f;

	public bool m_IsMoving = true;

	public Vector2 m_Size = Vector2.Zero;

	public float BONUS_DURATION = 5000f;

	public Rectangle m_sourceRectangle;

	public List<Vector2> m_PositionToFollow = new List<Vector2>();

	private MercuryParticle m_GrabParticle;

	private Vector2 m_TargetPosition;

	protected float m_MoveTimer;

	private bool m_bOffsetUp;

	protected float m_EffectTimer;

	public void InitPowerUp(int x, int y, SpriteBatch spritebatch)
	{
		m_spriteBatch = spritebatch;
		m_Size.X = x;
		m_Size.Y = y;
		m_sourceRectangle = new Rectangle(0, 0, x, y);
		m_GrabRadius = x;
		m_bAvailable = true;
		ParticleEffect particleEffect = m_StateInstance.content.Load<ParticleEffect>("Fx/Particle/BonusGrab");
		m_GrabParticle = new MercuryParticle(m_StateInstance, 0, 0, particleEffect.DeepCopy(), "BonusGrab", 1f, bUseBlending: true);
		m_GrabParticle.m_bAutoTrigger = false;
		m_StateInstance.AddParticle(m_GrabParticle);
	}

	public virtual void InitBonus()
	{
		m_Player.m_UsedPowerUp++;
		m_zorder = GameContext.POWERUP_Z;
		InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: false);
	}

	public virtual void StopBonus()
	{
		m_Player.m_GameStateInstance.m_BonusSpawnEffect.Trigger(m_MiddlePosition);
		m_Player.m_bUsePowerUp = false;
		m_Player.m_CurrentPowerUp = null;
		m_bAvailable = true;
		m_Player = null;
		m_PositionToFollow.Clear();
		m_MoveTimer = 0f;
	}

	public virtual void Update(GameTime gameTime)
	{
		BONUS_DURATION -= gameTime.ElapsedGameTime.Milliseconds;
		if (BONUS_DURATION <= 0f)
		{
			StopBonus();
		}
		if (m_EffectTimer > 0f)
		{
			m_EffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public void UpdatePosition(GameTime gameTime, Vector2 playerPos)
	{
		m_TargetPosition = playerPos;
		if (m_IsMoving)
		{
			if (m_MoveTimer > 150f)
			{
				if (m_PositionToFollow.Count > 0)
				{
					m_PositionToFollow.RemoveAt(0);
				}
				m_Position = m_MiddlePosition;
				if (m_bOffsetUp)
				{
					m_TargetPosition.Y += 2f;
					m_bOffsetUp = false;
				}
				else
				{
					m_TargetPosition.Y -= 2f;
					m_bOffsetUp = true;
				}
				if (m_Player.GetSpriteEffect() == SpriteEffects.FlipHorizontally)
				{
					m_TargetPosition.X -= m_Size.X;
				}
				m_PositionToFollow.Add(m_TargetPosition);
				m_MoveTimer = 0f;
			}
			else if (m_PositionToFollow.Count > 0)
			{
				m_MiddlePosition = Vector2.Lerp(m_Position, m_PositionToFollow[0], m_MoveTimer / 150f);
			}
		}
		m_MoveTimer += gameTime.ElapsedGameTime.Milliseconds;
	}

	public virtual bool IsGrabbedByPlayer(Player player)
	{
		if (InputManager.GetKeyState(player.m_PlayerNum, 5) == ButtonState.Pressed && player.m_bAllowPowerUp && Vector2.Distance(player.GetPosition(), m_Position) < m_GrabRadius)
		{
			if (!player.m_bSpecialEnable && player.m_Tag == 0 && !player.m_bIsMorphing)
			{
				m_bAvailable = false;
				m_Player = player;
				m_TargetPosition = player.GetPosition();
				player.SetCurrentPowerUp(this);
				m_EffectTimer = 250f;
				return true;
			}
			return false;
		}
		return false;
	}

	public bool IsAvailable()
	{
		return m_bAvailable;
	}

	public bool HasOwner()
	{
		return m_Player != null;
	}

	public virtual void SpawnBonus(Vector2 Position, float zorder)
	{
		m_zorder = zorder;
		m_Position = Position;
		m_MiddlePosition = new Vector2(m_Position.X - m_Size.X / 2f, m_Position.Y - m_Size.Y / 2f);
	}

	public virtual Vector2 GetNodePosition()
	{
		return m_MiddlePosition;
	}

	public virtual void DrawBonus()
	{
		if (m_Player == null)
		{
			Vector2 Position = m_MiddlePosition;
			Position.Y -= 80f;
			m_StateInstance.m_btSprite[1].Draw(ref Position, SpriteEffects.None, Color.White, 0.7f, 1f);
		}
		else if (m_EffectTimer > 0f)
		{
			m_GrabParticle.Trigger(m_Player.GetPosition());
		}
	}
}
