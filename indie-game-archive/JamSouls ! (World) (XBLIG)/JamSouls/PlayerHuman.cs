using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class PlayerHuman : Player
{
	private ButtonState m_lastToggle;

	public PlayerHuman(GameState GameStateInstance, int CharIdx, PlayerIndex nIndex, string name, PlayerConfig.SBIRE_DEF sbiredef)
	{
		InitPlayer(GameStateInstance, CharIdx, nIndex, name, sbiredef);
		m_bIsPlayerBot = false;
	}

	public override void Update(GameTime gameTime)
	{
		if (m_AnimLatency > 0f)
		{
			m_AnimLatency -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (m_KickTimer > 0f)
		{
			m_KickTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		base.Update(gameTime);
	}

	public override void ManageInput()
	{
		if (!m_bSpecialEnable)
		{
			if (InputManager.GetKeyState(m_PlayerNum, 10) == ButtonState.Pressed)
			{
				if (m_bIsMorphing)
				{
					SetWalkSpeed(6f, m_WalkAnimationSpeed / 2f);
				}
				else
				{
					SetWalkSpeed(40f, m_WalkAnimationSpeed / 2f);
				}
			}
			else if (m_Speed != 25f)
			{
				if (m_bIsMorphing)
				{
					SetWalkSpeed(4f, m_WalkAnimationSpeed);
				}
				else
				{
					SetWalkSpeed(25f, m_WalkAnimationSpeed);
				}
			}
		}
		if (InputManager.GetKeyState(m_PlayerNum, 1) == ButtonState.Pressed)
		{
			if (m_bIsOnGround)
			{
				SetAnimation(AnimStates.WALK);
			}
			m_PlayerBody.ApplyForce(new Vector2(0f - m_Speed, 0f));
			m_bLeftRelease = false;
		}
		else
		{
			m_bLeftRelease = true;
		}
		if (InputManager.GetKeyState(m_PlayerNum, 3) == ButtonState.Pressed)
		{
			if (m_bIsOnGround)
			{
				SetAnimation(AnimStates.WALK);
			}
			m_PlayerBody.ApplyForce(new Vector2(m_Speed, 0f));
			m_bRightRelease = false;
		}
		else
		{
			m_bRightRelease = true;
		}
		if (InputManager.GetKeyState(m_PlayerNum, 4) == ButtonState.Pressed)
		{
			if (!m_bLockJump)
			{
				ProcessJump();
				m_bJumpRelease = false;
			}
		}
		else
		{
			m_bJumpRelease = true;
			if (!m_bIsOnGround && !m_bDampingEnable)
			{
				m_bLockJump = true;
			}
		}
		if (GameContext.GameMode == GAME_MODE.JAM_BALL)
		{
			if (InputManager.GetKeyState(m_PlayerNum, 5) == ButtonState.Pressed || InputManager.GetKeyState(m_PlayerNum, 6) == ButtonState.Pressed || InputManager.GetKeyState(m_PlayerNum, 7) == ButtonState.Pressed)
			{
				if (InputManager.GetKeyState(m_PlayerNum, 7) == ButtonState.Pressed)
				{
					m_Kick = KickType.KICK_UP;
				}
				else if (InputManager.GetKeyState(m_PlayerNum, 6) == ButtonState.Pressed)
				{
					m_Kick = KickType.KICK_HIGH;
				}
				else
				{
					m_Kick = KickType.KICK_LOW;
				}
				if (m_AnimLatency <= 0f)
				{
					m_PlayerSprite[13].Reset();
					SetAnimation(AnimStates.KICK);
					m_AnimLatency = 400f;
					m_KickTimer = 160f;
					if (m_bIsOnGround && InputManager.GetKeyState(m_PlayerNum, 4) != ButtonState.Pressed)
					{
						m_PlayerBody.ApplyLinearImpulse(ref m_KickFlip);
					}
				}
			}
			if (m_CurrentAnim == AnimStates.KICK && !m_bIsOnGround && m_KickTimer <= 0f)
			{
				SetAnimation(AnimStates.JUMP);
			}
		}
		if (m_bLeftRelease && m_bRightRelease && m_KickTimer <= 0f && m_bIsOnGround)
		{
			SetAnimation(AnimStates.STAND);
		}
		if (InputManager.GetKeyState(m_PlayerNum, 2) == ButtonState.Pressed && !m_bDampingEnable)
		{
			if (m_bLeftRelease && m_bRightRelease && m_bIsOnGround)
			{
				SetAnimation(AnimStates.DUCK);
				m_bIsDucked = true;
			}
			else
			{
				m_bIsDucked = false;
			}
		}
		else
		{
			m_bIsDucked = false;
		}
		if (InputManager.GetKeyState(m_PlayerNum, 12) == ButtonState.Pressed && m_lastToggle == ButtonState.Released)
		{
			m_bToggleName = !m_bToggleName;
		}
		m_lastToggle = InputManager.GetKeyState(m_PlayerNum, 12);
	}
}
