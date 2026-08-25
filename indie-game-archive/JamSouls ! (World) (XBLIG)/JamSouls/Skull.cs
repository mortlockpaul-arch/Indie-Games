using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal class Skull : PowerUp
{
	private const float ATTACK_DURATION = 2000f;

	private const float ATTACK_SPEED = 4f;

	private const float IMPULSE_X = 100f;

	private const float IMPULSE_Y = 50f;

	private const float MAX_SCALE = 1.8f;

	private const float IMPACT_X = 2000f;

	private const float IMPACT_Y = 1000f;

	private const float IMPACT_DURATION = 400f;

	private const int SKULL_SPEED = 10;

	private const int DAMAGE = 30;

	private AudioClip m_AieSound;

	private List<Sprite> m_SpriteList = new List<Sprite>();

	private SpriteEffects m_SpriteEffect;

	private List<Player> m_TargetList = new List<Player>();

	private float m_AttackTimer;

	private float m_Scale = 1f;

	private int m_SpriteNum;

	private bool m_bTrigger;

	public Skull(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_SpriteList.Add(StateInstance.LoadSprite("Powerup_Skull1", GameState.GameAtlas.GAME));
		m_SpriteList.Add(StateInstance.LoadSprite("Powerup_Skull2", GameState.GameAtlas.GAME));
		m_SpriteList.Add(StateInstance.LoadSprite("Powerup_Skull3", GameState.GameAtlas.GAME));
		InitPowerUp(m_SpriteList[0].Width, m_SpriteList[0].Height, spriteBatch);
		m_UseSound = new AudioClip("PowerUp_Skull");
		m_AieSound = new AudioClip("Bullet_Flesh");
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 15000f;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player == null)
		{
			return;
		}
		if (m_AttackTimer <= 0f)
		{
			m_SpriteNum = 2;
			if (!m_bTrigger)
			{
				if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Released)
				{
					m_bTrigger = true;
				}
			}
			else if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_AttackTimer = 2000f;
				m_SpriteEffect = m_Player.GetSpriteEffect();
				m_TargetList.Clear();
				for (int i = 0; i < m_StateInstance.m_Players.Count; i++)
				{
					if (m_Player.m_PlayerNum != m_StateInstance.m_Players[i].m_PlayerNum && !m_Player.m_bSpecialEnable && m_Player.m_Tag != 1)
					{
						m_TargetList.Add(m_StateInstance.m_Players[i]);
					}
				}
				m_UseSound.Play();
			}
			if (m_Player.GetSpriteEffect() == SpriteEffects.FlipHorizontally)
			{
				m_Effect = SpriteEffects.None;
			}
			else
			{
				m_Effect = SpriteEffects.FlipHorizontally;
			}
			UpdatePosition(gameTime, m_Player.GetPosition());
		}
		else
		{
			UpdatePosition(gameTime, m_Player.GetPosition());
			m_MiddlePosition = m_Player.GetPosition();
			if (m_SpriteEffect == SpriteEffects.FlipHorizontally)
			{
				m_MiddlePosition.X -= m_Size.X;
			}
			Vector2 impulse = new Vector2(2000f, -1000f);
			if (m_AttackTimer >= 1000f)
			{
				m_SpriteNum = 1;
				if (m_SpriteEffect == SpriteEffects.FlipHorizontally)
				{
					m_MiddlePosition.X -= MathHelper.Lerp(0f, 100f, 1f - (m_AttackTimer - 1000f) / 1000f);
					impulse.X = -2000f;
				}
				else
				{
					m_MiddlePosition.X += MathHelper.Lerp(0f, 100f, 1f - (m_AttackTimer - 1000f) / 1000f);
				}
				m_MiddlePosition.Y -= MathHelper.Lerp(0f, 50f, 1f - (m_AttackTimer - 1000f) / 1000f);
				m_Scale = MathHelper.Lerp(1f, 1.8f, 1f - (m_AttackTimer - 1000f) / 1000f);
				m_AttackTimer -= gameTime.ElapsedGameTime.Milliseconds * 10;
				Vector2 middlePosition = m_MiddlePosition;
				middlePosition.X += m_SpriteList[0].Width / 2;
				middlePosition.Y += m_SpriteList[0].Height / 2;
				for (int j = 0; j < m_TargetList.Count; j++)
				{
					Player player = m_TargetList[j];
					if (player.m_Tag == 1 || player.m_bSpecialEnable || !(Vector2.Distance(player.GetPosition(), middlePosition) < 70f))
					{
						continue;
					}
					player.Poke(impulse, 400f);
					player.m_BleedingEmitter.Trigger(player.GetPosition());
					player.m_life -= 30;
					player.SetAnimation(Player.AnimStates.JUMP);
					m_AieSound.Play();
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
					m_TargetList.RemoveAt(j);
					break;
				}
			}
			else
			{
				m_SpriteNum = 1;
				if (m_SpriteEffect == SpriteEffects.FlipHorizontally)
				{
					m_MiddlePosition.X -= MathHelper.Lerp(0f, 100f, m_AttackTimer / 1000f);
				}
				else
				{
					m_MiddlePosition.X += MathHelper.Lerp(0f, 100f, m_AttackTimer / 1000f);
				}
				m_MiddlePosition.Y -= MathHelper.Lerp(0f, 50f, m_AttackTimer / 1000f);
				m_Scale = MathHelper.Lerp(1f, 1.8f, m_AttackTimer / 1000f);
				m_AttackTimer -= gameTime.ElapsedGameTime.Milliseconds * 10;
				m_bTrigger = false;
			}
			if (m_SpriteEffect == SpriteEffects.FlipHorizontally)
			{
				m_Effect = SpriteEffects.None;
			}
			else
			{
				m_Effect = SpriteEffects.FlipHorizontally;
			}
		}
		base.Update(gameTime);
	}

	public override void StopBonus()
	{
		m_TargetList.Clear();
		m_Scale = 1f;
		m_bTrigger = false;
		m_SpriteNum = 0;
		base.StopBonus();
	}

	public override void DrawBonus()
	{
		m_SpriteList[m_SpriteNum].Draw(m_MiddlePosition, Color.White, m_Effect, m_zorder, 0f, m_Scale);
		base.DrawBonus();
	}

	public override void SpawnBonus(Vector2 Position, float zorder)
	{
		base.SpawnBonus(Position, zorder);
	}
}
