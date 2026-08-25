using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class BlackSugar : PowerUp
{
	private const float IMPULSE_X = 200f;

	private const float IMPULSE_Y = 60f;

	private const float EFRITE = 200f;

	private AudioClip m_SugarCollisionSound;

	private Sprite m_BlackSugar;

	private Body m_SugarBody;

	private Fixture m_SugarFixture;

	private Vector2 m_SugarImpulse = Vector2.Zero;

	private MercuryParticle m_SugarEffect;

	private AudioClip m_SugarPokeSound;

	private bool m_bIsOnGround;

	private float m_EfritTime;

	private float m_ThrowTime = 200f;

	public BlackSugar(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_BlackSugar = StateInstance.LoadSprite("PowerUp_BlackSugar", GameState.GameAtlas.GAME);
		m_SugarPokeSound = new AudioClip("SugarPoke");
		m_SugarCollisionSound = new AudioClip("Char_MaladieRebond");
		m_SugarEffect = new MercuryParticle(StateInstance, 0, 0, StateInstance.content.Load<ParticleEffect>("Fx/Particle/SugarPoke").DeepCopy(), "SugarEffect", m_zorder, bUseBlending: true);
		m_SugarEffect.m_bAutoTrigger = false;
		StateInstance.AddParticle(m_SugarEffect);
		m_SugarBody = m_StateInstance.m_PhysicManager.CreateBody();
		m_SugarBody.BodyType = BodyType.Dynamic;
		m_SugarBody.UserData = null;
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.SetAsBox((float)(m_BlackSugar.Width / 4) / 10f, (float)(m_BlackSugar.Height / 4) / 10f);
		m_SugarFixture = m_SugarBody.CreateFixture(polygonShape);
		m_SugarFixture.UserData = null;
		m_SugarFixture.CollisionCategories = CollisionCategory.Cat2;
		m_SugarFixture.CollidesWith = CollisionCategory.Cat1 | CollisionCategory.Cat2 | CollisionCategory.Cat3 | CollisionCategory.Cat5;
		m_SugarFixture.Friction = 0.1f;
		m_SugarBody.AngularDamping = 0.8f;
		Fixture sugarFixture = m_SugarFixture;
		sugarFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(sugarFixture.OnCollision, new CollisionEventHandler(OnCollision));
		Fixture sugarFixture2 = m_SugarFixture;
		sugarFixture2.OnSeparation = (SeparationEventHandler)Delegate.Combine(sugarFixture2.OnSeparation, new SeparationEventHandler(OnSeparation));
		m_SugarBody.FixedRotation = false;
		m_SugarBody.SleepingAllowed = true;
		m_SugarFixture.Restitution = 0.8f;
		m_SugarFixture.Body.Mass = 2f;
		m_SugarBody.Active = false;
		InitPowerUp(m_BlackSugar.Width, m_BlackSugar.Height, spriteBatch);
	}

	public override void InitBonus()
	{
		m_EfritTime = 0f;
		BONUS_DURATION = 4000f;
		m_ThrowTime = 200f;
		m_SugarImpulse = Vector2.Zero;
		m_SugarBody.Rotation = 0f;
		m_SugarBody.ResetDynamics();
		m_SugarBody.LinearVelocity = Vector2.Zero;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player == null)
		{
			return;
		}
		if (!m_SugarBody.Active)
		{
			UpdatePosition(gameTime, m_Player.GetPosition());
			m_SugarBody.Position = m_MiddlePosition / 10f;
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_SugarBody.Active = true;
				m_SugarPokeSound.Play();
				if (m_Player.GetSpriteEffect() == SpriteEffects.None)
				{
					m_SugarImpulse.X = 200f;
					m_MiddlePosition.X = m_Player.GetPosition().X + (float)(m_BlackSugar.Width / 2);
					m_SugarBody.Position = m_MiddlePosition / 10f;
				}
				else
				{
					m_SugarImpulse.X = -200f;
					m_MiddlePosition.X = m_Player.GetPosition().X - (float)(m_BlackSugar.Width / 2);
					m_SugarBody.Position = m_MiddlePosition / 10f;
				}
				if (InputManager.GetKeyState(m_Player.m_PlayerNum, 0) == ButtonState.Pressed)
				{
					m_SugarImpulse.Y -= 120f;
				}
				else
				{
					m_SugarImpulse.Y += 60f;
				}
				m_SugarPokeSound.Play();
				m_SugarBody.ApplyLinearImpulse(ref m_SugarImpulse);
			}
		}
		else
		{
			m_MiddlePosition = m_SugarBody.Position * 10f;
			if (m_EfritTime > 0f)
			{
				m_EfritTime -= gameTime.ElapsedGameTime.Milliseconds;
				m_SugarEffect.Trigger(m_MiddlePosition);
			}
			if (m_ThrowTime > 0f)
			{
				m_ThrowTime -= gameTime.ElapsedGameTime.Milliseconds;
			}
			BONUS_DURATION -= gameTime.ElapsedGameTime.Milliseconds;
			if (BONUS_DURATION <= 0f)
			{
				StopBonus();
			}
		}
		if (m_EffectTimer > 0f)
		{
			m_EffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public override void StopBonus()
	{
		m_SugarBody.Active = false;
		m_SugarEffect.Trigger(m_MiddlePosition);
		base.StopBonus();
	}

	public bool OnCollision(Fixture fix1, Fixture fix2, Contact contact)
	{
		if (fix2.CollisionCategories == CollisionCategory.Cat1)
		{
			Player player = (Player)fix2.UserData;
			if (player != null && (player != m_Player || m_ThrowTime <= 0f) && !player.m_bSpecialEnable && (player.m_Tag == 0 || player.m_Tag == 2))
			{
				player.m_Tag = 1;
				m_EfritTime = 400f;
				if (player == m_Player)
				{
					m_Player.DecreaseScore(1);
				}
				else
				{
					m_Player.IncreaseScore(1);
				}
			}
			return false;
		}
		if (!m_bIsOnGround && Math.Abs(m_SugarBody.LinearVelocity.Y) > 20f)
		{
			m_SugarCollisionSound.Play();
			m_SugarEffect.Trigger(m_MiddlePosition);
			m_EfritTime = 200f;
		}
		m_bIsOnGround = true;
		return true;
	}

	public void OnSeparation(Fixture self, Fixture other)
	{
		m_bIsOnGround = false;
	}

	public override void DrawBonus()
	{
		if (m_Player != null)
		{
			Vector2 origin = Vector2.Zero;
			if (m_SugarBody.Active)
			{
				origin = new Vector2(m_BlackSugar.Width / 2, m_BlackSugar.Height / 2);
			}
			m_BlackSugar.Draw(m_MiddlePosition, Color.White, SpriteEffects.None, m_zorder, m_SugarBody.Rotation, origin);
		}
		else
		{
			m_BlackSugar.Draw(m_MiddlePosition, Color.White, m_Effect, m_zorder, 0f, 1f);
		}
		base.DrawBonus();
	}
}
