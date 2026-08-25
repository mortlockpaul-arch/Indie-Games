using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;

namespace JamSouls;

public class Flag : ScenaricEntitie
{
	public const float FLAG_RETURN_LATENCY = 200f;

	public const float FIREWORKS_DURATION = 800f;

	public AnimatedSprite m_Pot;

	public AnimatedSprite m_Jam;

	private AnimatedSprite m_EmptyPot;

	private GameState m_GameInstance;

	public Color FlagColor;

	private Body m_FlagBody;

	private Fixture m_FlagFixture;

	public Player m_Owner;

	private Vector2 m_StartPosition;

	private bool m_bHasBeenReturned = true;

	private MercuryParticle m_Fireworks;

	private MercuryParticle m_Halo;

	private float m_FireworksTimer = 800f;

	private Vector2 m_EmptyPotCenter;

	private AudioClip m_FlagWinSound;

	private AudioClip m_FlagGrabSound;

	public float m_flagReturnTimer;

	public Flag(Vector2 Position, GameState gameinstance, Color color)
	{
		m_GameInstance = gameinstance;
		FlagColor = color;
		m_Fireworks = new MercuryParticle(m_GameInstance, 0, 0, m_GameInstance.content.Load<ParticleEffect>("Fx/Particle/Fireworks"), "FireworksFx", 0f, bUseBlending: true);
		m_GameInstance.AddParticle(m_Fireworks);
		m_Fireworks.SetAutoTrigger(bAutoTrigger: false);
		m_Halo = new MercuryParticle(m_GameInstance, 0, 0, m_GameInstance.content.Load<ParticleEffect>("Fx/Particle/TeamHalo").DeepCopy(), "HaloFx", 0f, bUseBlending: true);
		m_GameInstance.AddParticle(m_Halo);
		m_Halo.SetAutoTrigger(bAutoTrigger: false);
		m_Halo.SetParticleColor(color, Vector3.Zero);
		m_FlagWinSound = new AudioClip("Flag_Win");
		m_FlagGrabSound = new AudioClip("Flag_Grabbed");
		InitEntity();
		TypeId = SCENARIC.TYPE_FLAG;
		if (color == PlayerConfig.BLUE_TEAM_COLOR)
		{
			m_Pot = m_GameInstance.LoadAnimatedSpriteFromXml("Scenaric/Pot.xml", "Scenaric/Pot_Blue");
			m_Jam = m_GameInstance.LoadAnimatedSpriteFromXml("Scenaric/Jam.xml", GameState.GameAtlas.GAME, "Jam_Blue");
			m_EmptyPot = m_GameInstance.LoadAnimatedSpriteFromXml("Scenaric/Pot_Empty.xml", "Scenaric/Pot_Blue_Empty");
		}
		else
		{
			m_Pot = m_GameInstance.LoadAnimatedSpriteFromXml("Scenaric/Pot.xml", "Scenaric/Pot_Red");
			m_Jam = m_GameInstance.LoadAnimatedSpriteFromXml("Scenaric/Jam.xml", GameState.GameAtlas.GAME, "Jam_Red");
			m_EmptyPot = m_GameInstance.LoadAnimatedSpriteFromXml("Scenaric/Pot_Empty.xml", "Scenaric/Pot_Red_Empty");
		}
		m_FlagBody = gameinstance.m_PhysicManager.CreateBody();
		m_FlagBody.BodyType = BodyType.Dynamic;
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.SetAsBox((float)(m_Jam.GetFrameWidth() / 2) / 10f, (float)(m_Jam.GetFrameHeight() / 2) / 10f);
		m_FlagFixture = m_FlagBody.CreateFixture(polygonShape);
		m_FlagFixture.UserData = this;
		m_FlagFixture.CollisionCategories = CollisionCategory.Cat6;
		m_FlagFixture.CollidesWith = CollisionCategory.All;
		m_FlagFixture.Friction = 0.1f;
		Fixture flagFixture = m_FlagFixture;
		flagFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(flagFixture.OnCollision, new CollisionEventHandler(OnCollision));
		m_FlagBody.FixedRotation = true;
		m_FlagBody.SleepingAllowed = true;
		m_FlagFixture.Restitution = 0.3f;
		Position.X += m_Jam.GetFrameWidth() / 2;
		m_StartPosition = Position;
		m_EmptyPotCenter = Position;
		m_EmptyPotCenter.X += m_EmptyPot.GetFrameWidth() / 2;
		m_EmptyPotCenter.Y += m_EmptyPot.GetFrameHeight() / 2;
		m_EmptyPot.SetPosition(new Vector2(Position.X - (float)(m_EmptyPot.GetFrameWidth() / 2), Position.Y - (float)(m_EmptyPot.GetFrameHeight() / 2)));
		m_Pot.SetPosition(new Vector2(Position.X - (float)(m_EmptyPot.GetFrameWidth() / 2), Position.Y - (float)(m_EmptyPot.GetFrameHeight() / 2)));
		SetPosition(Position);
		m_zOrder = GameContext.PLAYER_Z - 1E-05f;
	}

	public bool IsAtStartPosition()
	{
		return m_bHasBeenReturned;
	}

	public override void Draw()
	{
		if (m_bHasBeenReturned)
		{
			m_Pot.DrawFixed(m_SpriteEffect, Color.White, m_zOrder);
			return;
		}
		m_EmptyPot.DrawFixed(m_SpriteEffect, Color.White, m_zOrder);
		Vector2 Position = GetPosition();
		Position.X -= m_Jam.GetFrameWidth() / 2;
		Position.Y -= m_Jam.GetFrameHeight() / 2;
		m_Jam.Draw(ref Position, SpriteEffects.None, Color.White, m_zOrder);
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Owner == null)
		{
			m_FlagBody.Active = true;
			if (m_bHasBeenReturned)
			{
				SetPosition(m_StartPosition);
			}
			else
			{
				m_Halo.Trigger(GetPosition());
			}
		}
		else
		{
			m_Halo.Trigger(GetPosition());
			if (m_Owner.m_Tag == 1 || m_Owner.m_bSpecialEnable)
			{
				m_Owner = null;
				m_FlagBody.Active = true;
				m_flagReturnTimer = 0f;
			}
			else
			{
				m_FlagBody.Active = false;
				m_FlagBody.Position = m_Owner.GetHeadPlot() / 10f;
				m_zOrder = m_Owner.GetZ() + 0.0001f;
			}
		}
		if (m_FireworksTimer < 800f)
		{
			m_Fireworks.Trigger(GetPosition());
			m_FireworksTimer += gameTime.ElapsedGameTime.Milliseconds;
		}
		m_Jam.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		m_Pot.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		m_EmptyPot.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		m_flagReturnTimer += gameTime.ElapsedGameTime.Milliseconds;
	}

	public override void SetPosition(Vector2 pos)
	{
		m_FlagBody.Position = pos / 10f;
	}

	public override Vector2 GetPosition()
	{
		return m_FlagBody.Position * 10f;
	}

	public override Vector2 GetTopLeftPosition()
	{
		Vector2 position = GetPosition();
		position.X -= m_Jam.GetFrameWidth() / 2;
		position.Y -= m_Jam.GetFrameHeight() / 2;
		return position;
	}

	public override Vector2 GetBottomRightPosition()
	{
		Vector2 position = GetPosition();
		position.X += m_Jam.GetFrameWidth() / 2;
		position.Y += m_Jam.GetFrameHeight() / 2;
		return position;
	}

	protected bool OnCollision(Fixture Fix1, Fixture Fix2, Contact contact)
	{
		if (Fix2.UserData != null && m_Owner == null && m_flagReturnTimer > 200f)
		{
			if ((object)Fix2.UserData.GetType() == typeof(PlayerHuman) || (object)Fix2.UserData.GetType() == typeof(PlayerBot))
			{
				m_Owner = (Player)Fix2.UserData;
				if (m_Owner.m_Team == FlagColor && !m_Owner.m_bSpecialEnable)
				{
					Flag flag = ((!(FlagColor == PlayerConfig.BLUE_TEAM_COLOR)) ? m_GameInstance.m_BlueFlag : m_GameInstance.m_RedFlag);
					if (flag.m_Owner == m_Owner && m_bHasBeenReturned)
					{
						if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM)
						{
							m_Owner.m_Score++;
						}
						flag.m_flagReturnTimer = 0f;
						flag.m_Owner = null;
						flag.m_bHasBeenReturned = true;
						m_FireworksTimer = 0f;
						m_FlagWinSound.Play();
					}
					m_bHasBeenReturned = true;
					m_Owner = null;
				}
				else
				{
					if (m_Owner.m_bSpecialEnable)
					{
						m_Owner = null;
						return false;
					}
					m_FlagGrabSound.Play();
					m_bHasBeenReturned = false;
				}
				return false;
			}
		}
		else if (Fix2.CollisionCategories == CollisionCategory.Cat8 || Fix2.CollisionCategories == CollisionCategory.Cat10)
		{
			m_bHasBeenReturned = true;
			m_Owner = null;
			return false;
		}
		return true;
	}
}
