using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

internal class Soldier : PowerUp
{
	public enum BulletState
	{
		READY,
		FIRED,
		BROKEN
	}

	private const int MAX_BULLET = 10;

	private const float BULLET_TIME = 100f;

	private const int BULLET_DAMAGE = 15;

	private const float BULLET_OFFSET = 5f;

	private const int WATERMELON_OFFSET = 1;

	private List<Body> m_BodyList = new List<Body>();

	private Sprite m_BulletTexture;

	private float FRICTION_FORCES;

	private Vector2 m_BulletImpulse;

	private float m_bulletTimer;

	private MercuryParticle m_SparksEffect;

	private bool m_bFire;

	private AudioClip m_BulletFlesh;

	private AudioClip m_BulletFloor;

	private Sprite m_Sprite;

	private AnimatedSprite m_SoldierSprite;

	public Soldier(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.LoadSprite("Soldier", GameState.GameAtlas.GAME);
		InitPowerUp(m_Sprite.Width, m_Sprite.Height, spriteBatch);
		m_BulletTexture = m_StateInstance.LoadSprite("watermelonBullet", GameState.GameAtlas.GAME);
		m_SoldierSprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soldier/SoldierFire.xml", GameState.GameAtlas.GAME, "PowerUp_SoldierFire");
		m_SparksEffect = new MercuryParticle(m_StateInstance, 0, 0, m_StateInstance.content.Load<ParticleEffect>("Fx/Particle/sparks"), "sparks", 1f, bUseBlending: true);
		m_SparksEffect.SetAutoTrigger(bAutoTrigger: false);
		m_StateInstance.AddParticle(m_SparksEffect);
		m_spriteBatch = spriteBatch;
		m_UseSound = new AudioClip("PowerUp_Watermelon");
		m_BulletFlesh = new AudioClip("Bullet_Flesh");
		m_BulletFloor = new AudioClip("Bullet_Floor");
		m_BulletImpulse = new Vector2(200f, 0f);
		for (int i = 0; i < 10; i++)
		{
			Body body = m_StateInstance.m_PhysicManager.CreateBody();
			body.BodyType = BodyType.Dynamic;
			body.IsBullet = true;
			body.UserData = BulletState.READY;
			PolygonShape polygonShape = new PolygonShape();
			polygonShape.SetAsBox((float)(m_BulletTexture.Width / 2) / 10f, (float)(m_BulletTexture.Height / 2) / 10f);
			Fixture fixture = body.CreateFixture(polygonShape);
			fixture.UserData = this;
			fixture.CollisionCategories = CollisionCategory.Cat7;
			fixture.CollidesWith = CollisionCategory.All;
			fixture.Friction = FRICTION_FORCES;
			fixture.OnCollision = (CollisionEventHandler)Delegate.Combine(fixture.OnCollision, new CollisionEventHandler(OnCollision));
			body.FixedRotation = true;
			body.IgnoreGravity = true;
			body.SleepingAllowed = false;
			body.Active = false;
			m_BodyList.Add(body);
		}
		m_bAvailable = true;
		m_sourceRectangle = new Rectangle(0, 0, m_BulletTexture.Width, m_BulletTexture.Height);
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 15000f;
		foreach (Body body in m_BodyList)
		{
			body.Position = m_MiddlePosition / 10f;
		}
		m_UseSound.Play();
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player == null)
		{
			return;
		}
		m_Effect = m_Player.m_SpriteEffect;
		UpdatePosition(gameTime, m_Player.GetPosition());
		m_MiddlePosition.Y--;
		m_SoldierSprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		if (m_bulletTimer > 100f)
		{
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_bFire = true;
				Vector2 middlePosition = m_MiddlePosition;
				middlePosition.Y += 5f;
				foreach (Body body in m_BodyList)
				{
					BulletState bulletState = (BulletState)body.UserData;
					Vector2 impulse = m_BulletImpulse;
					switch (bulletState)
					{
					case BulletState.READY:
						body.Active = true;
						if (m_Effect == SpriteEffects.None)
						{
							middlePosition.X += m_BulletTexture.Width * 2;
						}
						body.Position = middlePosition / 10f;
						if (m_Effect == SpriteEffects.None)
						{
							body.ApplyLinearImpulse(ref impulse);
						}
						else
						{
							impulse *= -1f;
							body.ApplyLinearImpulse(ref impulse);
						}
						body.UserData = BulletState.FIRED;
						goto end_IL_015f;
					case BulletState.BROKEN:
						body.Active = false;
						body.UserData = BulletState.READY;
						break;
					}
					continue;
					end_IL_015f:
					break;
				}
				m_bulletTimer = 0f;
			}
			else
			{
				m_bFire = false;
			}
		}
		m_bulletTimer += gameTime.ElapsedGameTime.Milliseconds;
		base.Update(gameTime);
	}

	public override void StopBonus()
	{
		base.StopBonus();
	}

	protected bool OnCollision(Fixture Fix1, Fixture Fix2, Contact contact)
	{
		if ((BulletState)Fix1.Body.UserData == BulletState.FIRED && m_Player != null)
		{
			if (Fix2.CollisionCategories != CollisionCategory.Cat1)
			{
				if (Fix2.CollisionCategories == CollisionCategory.Cat7)
				{
					return false;
				}
				m_BulletFloor.Play();
				Fix1.Body.UserData = BulletState.BROKEN;
				m_SparksEffect.Trigger(Fix1.Body.Position * 10f);
				return true;
			}
			Player player = (Player)Fix2.UserData;
			if (player != null)
			{
				if (player != m_Player && player.m_Tag == 0)
				{
					if (player.m_bSpecialEnable)
					{
						m_BulletFloor.Play();
						Fix1.Body.UserData = BulletState.BROKEN;
						m_SparksEffect.Trigger(Fix1.Body.Position * 10f);
						return true;
					}
					Fix1.Body.UserData = BulletState.BROKEN;
					player.m_life -= 15;
					if (player.m_life <= 0)
					{
						if (player.m_CurrentPowerUp != null && (object)player.m_CurrentPowerUp.GetType() == typeof(Heart))
						{
							player.m_life = 100;
							player.m_CurrentPowerUp.BONUS_DURATION = Heart.HEART_DIE_TIME;
						}
						else
						{
							player.m_Tag = 1;
							m_Player.IncreaseScore(1);
						}
					}
					m_BulletFlesh.Play();
					player.m_BleedingEmitter.Trigger(Fix1.Body.Position * 10f);
					return true;
				}
				return false;
			}
		}
		return false;
	}

	public override void DrawBonus()
	{
		if (m_Player != null && m_bFire)
		{
			m_SoldierSprite.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		}
		else
		{
			m_Sprite.Draw(m_MiddlePosition, Color.White, m_Effect, m_zorder);
		}
		base.DrawBonus();
		foreach (Body body in m_BodyList)
		{
			if ((BulletState)body.UserData == BulletState.FIRED)
			{
				if (Math.Abs(body.LinearVelocity.X) < 100f)
				{
					body.UserData = BulletState.BROKEN;
				}
				else
				{
					m_BulletTexture.Draw(body.Position * 10f, Color.White, SpriteEffects.None, m_zorder);
				}
			}
		}
	}
}
