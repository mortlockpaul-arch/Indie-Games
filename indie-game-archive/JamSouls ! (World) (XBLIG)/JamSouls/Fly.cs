using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class Fly : PowerUp
{
	private const float AERO_FORCE = -4000f;

	private const float FIRST_IMPULSE = -1000f;

	private const float FLY_TIME = 2000f;

	private AnimatedSprite m_Sprite;

	private float m_HeadTimer;

	private float m_flyTime = 2000f;

	private bool m_bStartImpulse = true;

	private float m_InputTimer;

	private AudioClip m_FlySfx;

	public Fly(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Fly/PowerUp_Fly.xml", GameState.GameAtlas.GAME, "PowerUp_Fly");
		m_FlySfx = new AudioClip("PowerUp_Mouche");
		InitPowerUp(m_Sprite.GetFrameWidth(), m_Sprite.GetFrameHeight(), spriteBatch);
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 20000f;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		m_Sprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		if (m_Player == null)
		{
			return;
		}
		m_Effect = m_Player.m_SpriteEffect;
		bool flag = InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed;
		if (flag && m_flyTime <= 0f)
		{
			m_FlySfx.Play();
			m_flyTime = 2000f;
		}
		else
		{
			m_flyTime -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (flag && m_Player.m_CurrentAnim == Player.AnimStates.FALL)
		{
			if (m_bStartImpulse)
			{
				Vector2 impulse = new Vector2(0f, -1000f);
				m_bStartImpulse = false;
				if (m_InputTimer < 0f)
				{
					m_Player.GetBody().ApplyLinearImpulse(ref impulse);
					m_InputTimer = 1000f;
				}
				m_HeadTimer = 0f;
				m_Position = m_MiddlePosition;
				m_PositionToFollow.Clear();
			}
			else
			{
				Vector2 headPlot = m_Player.GetHeadPlot();
				headPlot.X -= m_Sprite.GetFrameWidth() / 2;
				headPlot.Y -= m_Sprite.GetFrameHeight() / 2;
				if (m_HeadTimer <= 1000f)
				{
					m_MiddlePosition = Vector2.Lerp(m_Position, headPlot, m_HeadTimer / 1000f);
					m_HeadTimer += gameTime.ElapsedGameTime.Milliseconds * 10;
				}
				else
				{
					m_MiddlePosition = headPlot;
					m_Position = headPlot;
				}
				m_Player.GetBody().ApplyForce(new Vector2(0f, -4000f));
			}
		}
		else
		{
			m_bStartImpulse = true;
			UpdatePosition(gameTime, m_Player.GetHeadPlot());
		}
		base.Update(gameTime);
	}

	public override void DrawBonus()
	{
		m_Sprite.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		base.DrawBonus();
	}
}
