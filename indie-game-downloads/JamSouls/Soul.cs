using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class Soul : Target
{
	public enum SoulAnim
	{
		MOVE,
		DUSK,
		FLY,
		COUNT
	}

	public const int MAX_SOUL = 6;

	public const float GRAB_DISTANCE = 40f;

	private const float STAND_TIME_VALUE = 2000f;

	private const int MIN_SCALE_VALUE = 6;

	public const float SPAWN_HEAVEN_TIMER = 1000f;

	private const float SIZE_X = 30f;

	private const float SIZE_Y = 30f;

	private const float FRICTION_FORCES = 0.1f;

	private const float RESTITUTION = 0.4f;

	private const float MAX_VELOCITY = 2f;

	private const int FLOATING_POINT = 40;

	private bool m_bReset;

	private float m_SpawnHeavenTimer;

	private GameState m_StateInstance;

	private Player m_Owner;

	public Body m_SoulBody;

	protected Fixture m_SoulFixture;

	private AnimatedSprite[] m_SoulAnim = new AnimatedSprite[3];

	private AnimatedSprite[] m_SoulAnimLight = new AnimatedSprite[3];

	private SoulAnim m_CurrentAnim;

	private Color m_SoulColor;

	private float m_TimeOffset;

	private Random m_Randomizer;

	private float m_ScaleValue = 1f;

	private float m_StandTimeValue;

	private float m_CurrentZ = 0.9f;

	private Vector2 m_BasePosition;

	public bool m_bSpawned;

	private float m_FloatingTimer;

	private AudioClip m_Grab;

	public Soul(GameState gamestate, int RandomSeedValue)
	{
		m_StateInstance = gamestate;
		m_SoulAnim[0] = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soul/JamsoulMove.xml", GameState.GameAtlas.GAME, "JamsoulMove");
		m_SoulAnim[1] = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soul/JamsoulDusk.xml", GameState.GameAtlas.GAME, "JamsoulDusk");
		m_SoulAnim[2] = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soul/JamsoulStand.xml", GameState.GameAtlas.GAME, "JamsoulStand");
		m_SoulAnimLight[0] = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soul/JamsoulMoveLight.xml", GameState.GameAtlas.GAME, "JamsoulMoveLight");
		m_SoulAnimLight[1] = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soul/JamsoulDuskLight.xml", GameState.GameAtlas.GAME, "JamsoulDuskLight");
		m_SoulAnimLight[2] = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Soul/JamsoulStandLight.xml", GameState.GameAtlas.GAME, "JamsoulStandLight");
		m_Grab = new AudioClip("PowerUp_Soul");
		m_Owner = null;
		m_SoulBody = m_StateInstance.m_PhysicManager.CreateBody();
		m_SoulBody.BodyType = BodyType.Dynamic;
		m_SoulBody.FixedRotation = true;
		m_SoulBody.SleepingAllowed = true;
		m_SoulBody.AngularDamping = 0f;
		m_SoulBody.LinearDamping = 0f;
		m_SoulBody.Active = false;
		m_SoulBody.Mass = 1f;
		m_TimeOffset = RandomSeedValue;
		m_Randomizer = new Random(RandomSeedValue);
		for (int i = 0; i < m_SoulAnim.Length; i++)
		{
			m_SoulAnim[i].m_Speed = RandomSeedValue;
			m_SoulAnimLight[i].m_Speed = RandomSeedValue;
		}
	}

	protected bool OnCollision(Fixture Fix1, Fixture Fix2, Contact contact)
	{
		if (Fix2.CollisionCategories == CollisionCategory.Cat8 || Fix2.CollisionCategories == CollisionCategory.Cat10)
		{
			m_bReset = true;
		}
		else if (Fix2.CollisionCategories == CollisionCategory.Cat7)
		{
			return false;
		}
		return true;
	}

	public void Spawn(int PokeX, int PokeY)
	{
		if (!m_SoulBody.Active)
		{
			m_FloatingTimer = 0f;
			m_CurrentZ = m_Owner.GetZ();
			m_ScaleValue = (float)m_Randomizer.Next(6, 10) / 10f;
			PolygonShape polygonShape = new PolygonShape();
			polygonShape.SetAsBox(30f * m_ScaleValue / 2f / 10f, 30f * m_ScaleValue / 2f / 10f);
			if (m_SoulFixture != null)
			{
				m_SoulBody.DestroyFixture(m_SoulFixture);
			}
			m_SoulFixture = m_SoulBody.CreateFixture(polygonShape);
			m_SoulFixture.UserData = this;
			m_SoulFixture.CollisionCategories = CollisionCategory.Cat4;
			m_SoulFixture.CollidesWith = CollisionCategory.Cat2 | CollisionCategory.Cat3 | CollisionCategory.Cat5 | CollisionCategory.Cat6 | CollisionCategory.Cat7 | CollisionCategory.Cat8 | CollisionCategory.Cat9 | CollisionCategory.Cat10 | CollisionCategory.Cat11 | CollisionCategory.Cat12 | CollisionCategory.Cat13 | CollisionCategory.Cat14 | CollisionCategory.Cat15 | CollisionCategory.Cat16 | CollisionCategory.Cat17 | CollisionCategory.Cat18 | CollisionCategory.Cat19 | CollisionCategory.Cat20 | CollisionCategory.Cat21 | CollisionCategory.Cat22 | CollisionCategory.Cat23 | CollisionCategory.Cat24 | CollisionCategory.Cat25 | CollisionCategory.Cat26 | CollisionCategory.Cat27 | CollisionCategory.Cat28 | CollisionCategory.Cat29 | CollisionCategory.Cat30 | CollisionCategory.Cat31;
			m_SoulFixture.Friction = 0.1f;
			m_SoulFixture.Restitution = 0.4f;
			m_SoulBody.Mass = 1f;
			Fixture soulFixture = m_SoulFixture;
			soulFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(soulFixture.OnCollision, new CollisionEventHandler(OnCollision));
			m_SoulBody.ResetDynamics();
			Vector2 bodyPosition = m_Owner.GetBodyPosition();
			m_BasePosition = bodyPosition * 10f;
			m_SoulBody.Position = bodyPosition;
			m_SoulBody.Active = true;
			Vector2 impulse = new Vector2(PokeX, PokeY);
			m_SoulBody.ApplyLinearImpulse(ref impulse);
			m_SoulColor = m_Owner.m_PlayerColor;
			m_Owner.m_SoulNumber--;
			m_Owner = null;
			m_bSpawned = true;
		}
	}

	public void Appear(float x, float y)
	{
		x += (float)(m_SoulAnim[2].GetFrameWidth() / 2);
		y += (float)(m_SoulAnim[2].GetFrameHeight() / 2);
		m_FloatingTimer = 1.1f;
		m_SoulBody.Position = new Vector2(x / 10f, y / 10f);
		m_SoulColor = Color.White;
		m_CurrentZ = 0.99f;
		m_CurrentAnim = SoulAnim.FLY;
		m_SoulBody.IgnoreGravity = true;
		m_bSpawned = true;
	}

	public void SpawnFromHeaven()
	{
		m_SpawnHeavenTimer = 1000f;
		m_SoulBody.Active = true;
		m_SoulColor.A = 0;
		m_bSpawned = true;
	}

	public override Vector2 GetPosition()
	{
		return m_SoulBody.Position * 10f;
	}

	public Vector2 GetBodyPosition()
	{
		return m_BasePosition;
	}

	public override Vector2 GetTopLeftPosition()
	{
		Vector2 basePosition = m_BasePosition;
		basePosition.X -= m_SoulAnim[0].GetFrameWidth() / 2;
		basePosition.Y -= m_SoulAnim[0].GetFrameHeight() / 2;
		return basePosition;
	}

	public override Vector2 GetBottomRightPosition()
	{
		Vector2 basePosition = m_BasePosition;
		basePosition.X += m_SoulAnim[0].GetFrameWidth() / 2;
		basePosition.Y += m_SoulAnim[0].GetFrameHeight() / 2;
		return basePosition;
	}

	public void Reset()
	{
		m_SoulBody.LinearVelocity = Vector2.Zero;
		m_SoulBody.Active = false;
		m_Owner = null;
		m_bSpawned = false;
	}

	public void Update(GameTime gametime)
	{
		if (!m_SoulBody.Active)
		{
			return;
		}
		if (m_SpawnHeavenTimer > 0f)
		{
			m_SpawnHeavenTimer -= gametime.ElapsedGameTime.Milliseconds;
			if (m_SpawnHeavenTimer <= 0f)
			{
				m_SoulColor.A = byte.MaxValue;
				m_SoulAnim[(int)m_CurrentAnim].UpdateFrame(gametime.ElapsedGameTime.Milliseconds);
				m_SoulAnimLight[(int)m_CurrentAnim].UpdateFrame(gametime.ElapsedGameTime.Milliseconds);
			}
			return;
		}
		if (m_CurrentAnim == SoulAnim.FLY)
		{
			if (m_FloatingTimer <= 1f)
			{
				m_SoulBody.Position = Vector2.Lerp(m_BasePosition, m_BasePosition + new Vector2(0f, -40f), m_FloatingTimer) / 10f;
				m_FloatingTimer += (float)gametime.ElapsedGameTime.Milliseconds / 1000f;
				if (m_FloatingTimer > 1f)
				{
					m_SoulBody.LinearVelocity = Vector2.Zero;
				}
			}
			Vector2 linearVelocity = m_SoulBody.LinearVelocity;
			if (Math.Abs(m_SoulBody.LinearVelocity.X) > 2f)
			{
				linearVelocity.X = 6f;
			}
			if (Math.Abs(m_SoulBody.LinearVelocity.Y) > 2f)
			{
				linearVelocity.Y = 6f;
			}
			m_SoulBody.LinearVelocity = linearVelocity;
		}
		else if (m_SoulBody.ContactList != null)
		{
			if (m_SoulBody.LinearVelocity.X < 4f && m_SoulBody.LinearVelocity.X > -4f)
			{
				m_StandTimeValue += gametime.ElapsedGameTime.Milliseconds;
				m_CurrentAnim = SoulAnim.DUSK;
				if (m_CurrentAnim != SoulAnim.FLY && m_StandTimeValue >= 2000f)
				{
					m_BasePosition = m_SoulBody.Position * 10f;
					m_CurrentAnim = SoulAnim.FLY;
					m_SoulBody.IgnoreGravity = true;
				}
			}
			else
			{
				m_StandTimeValue = 0f;
			}
		}
		else
		{
			m_CurrentAnim = SoulAnim.MOVE;
		}
		m_SoulAnim[(int)m_CurrentAnim].UpdateFrame(gametime.ElapsedGameTime.Milliseconds);
		m_SoulAnimLight[(int)m_CurrentAnim].UpdateFrame(gametime.ElapsedGameTime.Milliseconds);
		for (int i = 0; i < m_StateInstance.m_Players.Count; i++)
		{
			Player player = m_StateInstance.m_Players[i];
			if (Vector2.Distance(m_SoulBody.Position * 10f, player.GetPosition()) <= 40f && player.m_Tag == 0 && player.m_SbireDef == PlayerConfig.SBIRE_DEF.NONE)
			{
				m_SoulBody.IgnoreGravity = false;
				m_SoulBody.Active = false;
				m_CurrentAnim = SoulAnim.DUSK;
				m_FloatingTimer = 0f;
				SetOwner(player);
				player.m_SoulNumber++;
				m_Grab.Play();
			}
		}
		if (m_bReset)
		{
			m_bReset = false;
			Reset();
		}
	}

	public void SetOwner(Player p)
	{
		m_CurrentZ = p.GetZ();
		m_Owner = p;
		if (p != null)
		{
			m_bSpawned = false;
		}
	}

	public Player GetOwner()
	{
		return m_Owner;
	}

	public void Draw()
	{
		if (m_SoulBody.Active)
		{
			Vector2 Position = m_SoulBody.Position * 10f;
			Position.X -= (float)m_SoulAnim[(int)m_CurrentAnim].GetFrameWidth() * m_ScaleValue;
			Position.Y -= (float)m_SoulAnim[(int)m_CurrentAnim].GetFrameHeight() * m_ScaleValue;
			m_SoulAnim[(int)m_CurrentAnim].Draw(ref Position, SpriteEffects.None, m_SoulColor, m_ScaleValue, m_CurrentZ);
			m_SoulAnimLight[(int)m_CurrentAnim].Draw(ref Position, SpriteEffects.None, Color.White, m_ScaleValue, m_CurrentZ + 0.1f);
		}
	}
}
