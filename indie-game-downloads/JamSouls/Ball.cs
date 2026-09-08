using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class Ball
{
	private const float BALL_DIAMETER = 50f;

	private const float IMPULSE_TIMER = 200f;

	private const float BASKET_UP_IMPULSE = 50f;

	private const float BASKET_UP_EFFECT = 350f;

	private const float BASKET_FORWARD_EFFECT = 100f;

	private const float BASKET_FORWARD_IMPULSE = 130f;

	private const float BASKET_SHOOT_IMPULSE = 150f;

	private const float BASKET_SHOOT_EFFECT = 300f;

	private const float VOLLEY_UP_IMPULSE = 50f;

	private const float VOLLEY_UP_EFFECT = 300f;

	private const float VOLLEY_FORWARD_EFFECT = 100f;

	private const float VOLLEY_FORWARD_IMPULSE = 130f;

	private const float VOLLEY_SHOOT_IMPULSE = 160f;

	private const float VOLLEY_SHOOT_EFFECT = 280f;

	public Texture2D m_BallSprite;

	private GameState m_GameInstance;

	public Color m_BallColor;

	private Body m_BallBody;

	private Fixture m_BallFixture;

	public Player m_LastPlayer;

	private Rectangle m_Rectangle;

	private Vector2 m_UpImpulse;

	private Vector2 m_ForwardImpulse;

	private Vector2 m_ShootImpulse;

	private float m_BallImpulseTime;

	private AnimatedSprite m_Impact;

	private Vector2 m_ImpactPos = Vector2.Zero;

	private SpriteEffects m_ImpactEffect;

	private bool m_DrawBall;

	private bool m_MuteSound = true;

	private AudioClip m_ShootLow;

	private AudioClip m_ShootUp;

	private AudioClip m_ShootBig;

	private AudioClip m_GoalHit;

	private float UP_IMPULSE = 50f;

	private float UP_EFFECT = 300f;

	private float FORWARD_EFFECT = 100f;

	private float FORWARD_IMPULSE = 130f;

	private float SHOOT_IMPULSE = 300f;

	private float SHOOT_EFFECT = 160f;

	private float ANIM_OFFSET = 10f;

	public Ball(Vector2 Position, GameState gameinstance, Color color)
	{
		m_GameInstance = gameinstance;
		m_BallColor = color;
		m_BallSprite = m_GameInstance.content.Load<Texture2D>("Level/" + GameContext.SelectedLevel + "/Ball");
		m_BallBody = gameinstance.m_PhysicManager.CreateBody();
		m_BallBody.BodyType = BodyType.Dynamic;
		CircleShape shape = new CircleShape(2f);
		m_BallFixture = m_BallBody.CreateFixture(shape);
		m_BallFixture.UserData = this;
		m_BallFixture.CollisionCategories = CollisionCategory.Cat12;
		m_BallFixture.CollidesWith = CollisionCategory.Cat2 | CollisionCategory.Cat3 | CollisionCategory.Cat4 | CollisionCategory.Cat5 | CollisionCategory.Cat6 | CollisionCategory.Cat7 | CollisionCategory.Cat8 | CollisionCategory.Cat9 | CollisionCategory.Cat10 | CollisionCategory.Cat11 | CollisionCategory.Cat12 | CollisionCategory.Cat13 | CollisionCategory.Cat14 | CollisionCategory.Cat15 | CollisionCategory.Cat16 | CollisionCategory.Cat17 | CollisionCategory.Cat18 | CollisionCategory.Cat19 | CollisionCategory.Cat20 | CollisionCategory.Cat21 | CollisionCategory.Cat22 | CollisionCategory.Cat23 | CollisionCategory.Cat24 | CollisionCategory.Cat25 | CollisionCategory.Cat26 | CollisionCategory.Cat27 | CollisionCategory.Cat28 | CollisionCategory.Cat29 | CollisionCategory.Cat30 | CollisionCategory.Cat31;
		m_BallFixture.Friction = 0.6f;
		m_BallBody.AngularDamping = 1f;
		Fixture ballFixture = m_BallFixture;
		ballFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(ballFixture.OnCollision, new CollisionEventHandler(OnCollision));
		m_BallBody.FixedRotation = false;
		m_BallBody.SleepingAllowed = true;
		m_BallFixture.Restitution = 0.8f;
		m_BallFixture.Body.Mass = 4f;
		m_Rectangle = new Rectangle(0, 0, m_BallSprite.Width, m_BallSprite.Height);
		SetPosition(Position);
		if (GameContext.SelectedLevel.Contains("Guerre"))
		{
			UP_IMPULSE = 50f;
			UP_EFFECT = 350f;
			FORWARD_EFFECT = 100f;
			FORWARD_IMPULSE = 130f;
			SHOOT_IMPULSE = 150f;
			SHOOT_EFFECT = 300f;
		}
		else if (GameContext.SelectedLevel.Contains("Passion"))
		{
			UP_IMPULSE = 50f;
			UP_EFFECT = 300f;
			FORWARD_EFFECT = 100f;
			FORWARD_IMPULSE = 130f;
			SHOOT_IMPULSE = 160f;
			SHOOT_EFFECT = 280f;
		}
		m_UpImpulse = new Vector2(0f, 0f - UP_EFFECT);
		m_ForwardImpulse = new Vector2(0f, 0f - FORWARD_EFFECT);
		m_ShootImpulse = new Vector2(0f, 0f - SHOOT_EFFECT);
		m_Impact = m_GameInstance.LoadAnimatedSpriteFromXml("Fx/Ball/BallShot.xml", GameState.GameAtlas.GAME, "BallShot");
		m_Impact.m_bInfiniteLoop = false;
		m_Impact.m_TotalLoop = 1;
		m_ShootLow = new AudioClip("Foot_Passe");
		m_ShootUp = new AudioClip("Foot_Lobbe");
		m_ShootBig = new AudioClip("Foot_Tir");
		m_GoalHit = new AudioClip("Foot_Poteau");
	}

	public void ScoreGoal()
	{
		m_LastPlayer.m_Score++;
		m_Impact.SetLock(locked: false);
		m_MuteSound = true;
		m_Impact.Reset();
	}

	public bool IsEnable()
	{
		return m_BallBody.Active;
	}

	public void SetEnable(bool enable)
	{
		m_BallBody.Active = enable;
		m_DrawBall = enable;
		m_BallBody.ResetDynamics();
	}

	public void Draw()
	{
		if (m_DrawBall)
		{
			m_GameInstance.ScreenManager.SpriteBatch.Draw(m_BallSprite, GetPosition(), m_Rectangle, Color.White, m_BallBody.Rotation, new Vector2(m_BallSprite.Width / 2, m_BallSprite.Height / 2), 1f, SpriteEffects.None, GameContext.BALL_Z);
		}
		if (m_Impact.IsLocked())
		{
			if (m_ImpactEffect == SpriteEffects.None)
			{
				m_ImpactPos = m_LastPlayer.GetBottomRightPosition();
			}
			else
			{
				m_ImpactPos = m_LastPlayer.GetBottomLeftPosition();
			}
			m_ImpactPos.X -= m_Impact.GetFrameWidth() / 2;
			m_ImpactPos.Y -= (float)(m_Impact.GetFrameHeight() / 2) + ANIM_OFFSET;
			m_Impact.Draw(ref m_ImpactPos, m_ImpactEffect, Color.White, GameContext.BALL_Z);
		}
	}

	public void Update(GameTime gameTime)
	{
		if (m_Impact.IsLocked())
		{
			m_Impact.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		}
		if (m_BallImpulseTime <= 0f)
		{
			for (int i = 0; i < m_GameInstance.m_Players.Count; i++)
			{
				if (!(m_GameInstance.m_Players[i].m_KickTimer > 0f))
				{
					continue;
				}
				m_LastPlayer = m_GameInstance.m_Players[i];
				Vector2 position = m_LastPlayer.GetPosition();
				Vector2 position2 = GetPosition();
				if (!(Vector2.Distance(position, position2) <= 50f))
				{
					continue;
				}
				Vector2 impulse;
				switch (m_GameInstance.m_Players[i].m_Kick)
				{
				case Player.KickType.KICK_UP:
					impulse = m_UpImpulse;
					if (m_LastPlayer.GetSpriteEffect() == SpriteEffects.None)
					{
						impulse.X = UP_IMPULSE;
					}
					else
					{
						impulse.X = 0f - UP_IMPULSE;
					}
					m_ShootUp.Play();
					m_MuteSound = false;
					break;
				default:
					impulse = m_ForwardImpulse;
					if (m_LastPlayer.GetSpriteEffect() == SpriteEffects.None)
					{
						impulse.X = FORWARD_IMPULSE;
					}
					else
					{
						impulse.X = 0f - FORWARD_IMPULSE;
					}
					m_ShootLow.Play();
					m_MuteSound = false;
					break;
				case Player.KickType.KICK_HIGH:
					impulse = m_ShootImpulse;
					if (m_LastPlayer.GetSpriteEffect() == SpriteEffects.None)
					{
						impulse.X = SHOOT_IMPULSE;
					}
					else
					{
						impulse.X = 0f - SHOOT_IMPULSE;
					}
					m_ShootBig.Play();
					m_MuteSound = false;
					break;
				}
				m_Impact.Reset();
				Vector2 vector = position2;
				vector.X -= m_Impact.GetFrameWidth() / 2;
				vector.Y -= m_Impact.GetFrameHeight() / 2;
				m_ImpactEffect = m_LastPlayer.GetSpriteEffect();
				m_Impact.SetPosition(position2);
				m_Impact.SetLock(locked: true);
				m_BallBody.ResetDynamics();
				m_BallBody.LinearVelocity = Vector2.Zero;
				m_BallBody.ApplyLinearImpulse(ref impulse);
				m_BallImpulseTime = 200f;
			}
		}
		else
		{
			m_BallImpulseTime -= gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public void SetPosition(Vector2 pos)
	{
		m_BallBody.Position = pos / 10f;
	}

	public Vector2 GetPosition()
	{
		return m_BallBody.Position * 10f;
	}

	private bool OnCollision(Fixture Fix1, Fixture Fix2, Contact contact)
	{
		Vector2 localNormal = contact.Manifold.LocalNormal;
		localNormal.Normalize();
		if (localNormal.Y == 1f && Fix2.Body.Position.Y > m_BallBody.Position.Y)
		{
			localNormal.Y = -1f;
		}
		if (Fix2.CollisionCategories == CollisionCategory.Cat3)
		{
			if (localNormal.Y != -1f)
			{
				return false;
			}
			Fix2.GetAABB(out var _, 0);
			if (m_BallBody.Position.Y + 2.5f > Fix2.Body.Position.Y)
			{
				return false;
			}
		}
		else if (Fix2.CollisionCategories == CollisionCategory.Cat13 && !m_MuteSound && !m_GoalHit.IsPlaying())
		{
			m_GoalHit.Play();
		}
		return true;
	}
}
