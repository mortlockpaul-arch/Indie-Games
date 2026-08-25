using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class FireProut : PowerUp
{
	public const float IMPULSE_STRENGHT = -2800f;

	public const int BURN_ZONE_HEIGHT = 80;

	public const int BURN_ZONE_WIDTH = 140;

	public const float IMPULSE_DURATION = 500f;

	public MercuryParticle m_SpecialFx;

	public Sprite m_Sprite;

	public bool m_bImpulseApplied;

	public float m_ImpulseTimer;

	public bool m_bAllowImpulse = true;

	public Vector2 m_FartImpulse = new Vector2(0f, -2800f);

	public FireProut(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		ParticleEffect pe = StateInstance.content.Load<ParticleEffect>("Fx/Particle/FireProut");
		m_SpecialFx = new MercuryParticle(StateInstance, 0, 0, pe, "FireProut", 0f, bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		StateInstance.AddParticle(m_SpecialFx);
		m_UseSound = new AudioClip("PowerUp_PetoFlamme");
		m_Sprite = m_StateInstance.LoadSprite("PowerUp_Cassoulet", GameState.GameAtlas.GAME);
		m_spriteBatch = spriteBatch;
		m_bAvailable = true;
		InitPowerUp(m_Sprite.Width, m_Sprite.Height, m_spriteBatch);
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 10000f;
		m_bImpulseApplied = false;
		m_ImpulseTimer = 0f;
		m_SpecialFx.SetZ(m_Player.GetZ());
		m_FartImpulse.Y = (float)(m_Player.GetWidth() * m_Player.GetHeight()) * -2800f / 2275f;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player == null)
		{
			return;
		}
		UpdatePosition(gameTime, m_Player.GetPosition());
		BONUS_DURATION -= gameTime.ElapsedGameTime.Milliseconds;
		if (!m_bImpulseApplied)
		{
			if (m_bAllowImpulse && InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_Player.GetBody().LinearVelocity = new Vector2(m_Player.GetBody().LinearVelocity.X, 0f);
				m_Player.GetBody().ApplyLinearImpulse(ref m_FartImpulse);
				m_bImpulseApplied = true;
				m_bAllowImpulse = false;
				m_UseSound.Play();
			}
			else if (!m_bAllowImpulse)
			{
				m_bAllowImpulse = m_Player.m_bIsOnGround;
			}
		}
		else
		{
			if (m_ImpulseTimer < 500f)
			{
				m_SpecialFx.Update(gameTime);
				Vector2 location = new Vector2(m_Player.GetPosition().X, m_Player.GetPosition().Y + 30f);
				m_Player.SetAnimation(Player.AnimStates.JUMP);
				m_SpecialFx.Trigger(location);
				BurnPlayerAround();
			}
			else
			{
				m_bImpulseApplied = false;
				m_ImpulseTimer = 0f;
			}
			m_ImpulseTimer += gameTime.ElapsedGameTime.Milliseconds;
		}
		base.Update(gameTime);
	}

	public void BurnPlayerAround()
	{
		Vector2 position = m_Player.GetPosition();
		Rectangle rectangle = new Rectangle((int)(position.X - 70f), (int)position.Y, 140, 80);
		foreach (Player player in m_StateInstance.m_Players)
		{
			if (player != m_Player)
			{
				Vector2 position2 = player.GetPosition();
				if (rectangle.Contains((int)position2.X, (int)position2.Y))
				{
					player.Burn();
				}
			}
		}
	}

	public override void DrawBonus()
	{
		m_Sprite.Draw(m_MiddlePosition, Color.White, SpriteEffects.None, m_zorder);
		base.DrawBonus();
	}
}
