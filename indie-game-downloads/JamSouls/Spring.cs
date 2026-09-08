using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class Spring : ScenaricEntitie
{
	private const float SPRING_MIN_DAMPING = -1250f;

	private const float SPRING_DAMPING = -2500f;

	protected const int COLLISION_OFFSET = 10;

	public Body m_SpringBody;

	protected Fixture m_SpringFixture;

	protected Vector2 m_Position = Vector2.Zero;

	protected bool m_Animate;

	protected Vector2 m_Impulse = Vector2.Zero;

	private AnimatedSprite m_SpringAnim;

	private AudioClip m_Rebond;

	public Spring(GameState gamestate, AnimatedSprite sprite, Vector2 Position, float zorder)
	{
		m_Position = Position;
		m_SpringAnim = sprite;
		m_zOrder = zorder;
		m_Position = Position;
		m_SpringBody = gamestate.m_PhysicManager.CreateBody();
		Position.X += m_SpringAnim.GetFrameWidth() / 2;
		Position.Y += m_SpringAnim.GetFrameHeight() / 2 + 10;
		m_SpringBody.Position = Position / 10f;
		m_SpringBody.BodyType = BodyType.Static;
		m_SpringBody.FixedRotation = true;
		m_SpringBody.SleepingAllowed = true;
		m_SpringBody.AngularDamping = 0f;
		m_SpringBody.LinearDamping = 0f;
		m_SpringBody.Active = true;
		m_SpringBody.IgnoreGravity = false;
		m_SpringBody.Mass = 10f;
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.SetAsBox((float)(m_SpringAnim.GetFrameWidth() / 4) / 10f, (float)(m_SpringAnim.GetFrameHeight() / 5) / 10f);
		m_SpringFixture = m_SpringBody.CreateFixture(polygonShape);
		m_SpringFixture.UserData = this;
		m_SpringFixture.CollisionCategories = CollisionCategory.Cat9;
		m_SpringFixture.CollidesWith = CollisionCategory.All;
		Fixture springFixture = m_SpringFixture;
		springFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(springFixture.OnCollision, new CollisionEventHandler(OnCollision));
		m_SpringAnim.m_bInfiniteLoop = false;
		m_SpringAnim.m_TotalLoop = 1;
		m_Rebond = new AudioClip("Ressort");
	}

	protected bool OnCollision(Fixture Fix1, Fixture Fix2, Contact contact)
	{
		Vector2 localNormal = contact.Manifold.LocalNormal;
		if (Fix2.CollisionCategories == CollisionCategory.Cat1 && (localNormal.Y == -1f || localNormal.Y >= (float)Math.PI * 113f / 355f))
		{
			Player player = (Player)Fix2.UserData;
			if (Fix2.Body.LinearVelocity.Y > 10f)
			{
				Fix2.Body.LinearVelocity = new Vector2(Fix2.Body.LinearVelocity.X, 0f);
				if (InputManager.GetKeyState(player.m_PlayerNum, 4) == ButtonState.Pressed)
				{
					m_Impulse.Y = -2500f;
				}
				else
				{
					m_Impulse.Y = -1250f;
				}
				m_Rebond.Play();
				if (player.m_SbireDef != PlayerConfig.SBIRE_DEF.NONE)
				{
					m_Impulse.Y /= 2.5f;
				}
				if (player.m_bIsMorphing)
				{
					m_Impulse.Y /= 4f;
				}
				player.m_PlayerSprite[2].Reset();
				player.SetAnimation(Player.AnimStates.JUMP);
				Fix2.Body.ApplyLinearImpulse(ref m_Impulse);
				m_Impulse = Vector2.Zero;
				m_Animate = true;
				return true;
			}
			return false;
		}
		return false;
	}

	public override Vector2 GetPosition()
	{
		return m_SpringBody.Position * 10f;
	}

	public override void Update(GameTime gametime)
	{
		if (m_Animate)
		{
			m_SpringAnim.UpdateFrame(gametime.ElapsedGameTime.Milliseconds);
			if (m_SpringAnim.m_CurrentLoop < 0)
			{
				m_Animate = false;
				m_SpringAnim.m_TotalLoop = 1;
				m_SpringAnim.m_CurrentFrame = 0;
				m_SpringAnim.m_CurrentLoop = 0;
			}
		}
	}

	public override void Draw()
	{
		m_SpringAnim.Draw(ref m_Position, SpriteEffects.None, Color.White, m_zOrder);
	}
}
