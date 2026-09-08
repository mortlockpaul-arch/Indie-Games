using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class Bomb : PowerUp
{
	private const float SHARE_DISTANCE = 66f;

	private const float EXPLOSION_DISTANCE = 200f;

	private AnimatedSprite m_Sprite;

	private Sprite m_StaticSprite;

	private AudioClip m_BipSound;

	private AudioClip m_BoomSound;

	private bool m_bExploded;

	private int elpasedSecond;

	public Bomb(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Bomb/PowerUp_Bomb.xml", GameState.GameAtlas.GAME, "BombFeu");
		m_StaticSprite = m_StateInstance.LoadSprite("Bomb", GameState.GameAtlas.GAME);
		InitPowerUp(m_Sprite.GetFrameWidth(), m_Sprite.GetFrameHeight(), spriteBatch);
		m_BipSound = new AudioClip("PowerUp_Bomb_Bip");
		m_BoomSound = new AudioClip("PowerUp_RootExplode");
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 6000f;
		m_bExploded = false;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		m_Sprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		if (m_Player == null)
		{
			return;
		}
		UpdatePosition(gameTime, m_Player.GetHeadPlot());
		if (m_Player.m_Tag == 0 || m_Player.m_Tag == 2)
		{
			foreach (Player player in m_StateInstance.m_Players)
			{
				if (player != m_Player && player.m_Tag == 0 && Vector2.Distance(player.GetPosition(), m_Player.GetPosition()) < 66f)
				{
					if (player.m_CurrentPowerUp != null)
					{
						m_Player.m_CurrentPowerUp = player.m_CurrentPowerUp;
					}
					else
					{
						m_Player.m_bUsePowerUp = false;
						m_Player.m_CurrentPowerUp = null;
					}
					if (player.m_CurrentPowerUp != null)
					{
						m_Player.m_bUsePowerUp = true;
						m_Player.m_CurrentPowerUp = player.m_CurrentPowerUp;
						player.m_CurrentPowerUp.m_Player = m_Player;
					}
					m_Player = null;
					player.m_CurrentPowerUp = this;
					player.m_bUsePowerUp = true;
					m_Player = player;
				}
			}
		}
		if (elpasedSecond != (int)(BONUS_DURATION / 1000f))
		{
			elpasedSecond = (int)(BONUS_DURATION / 1000f);
			m_BipSound.Play();
		}
		if (BONUS_DURATION < 150f && !m_bExploded)
		{
			m_BoomSound.Play();
			m_StateInstance.ExplodeBomb(m_MiddlePosition, Color.White, 1f);
			m_bExploded = true;
		}
		base.Update(gameTime);
	}

	public void Explode()
	{
		m_BoomSound.Play();
		m_StateInstance.ExplodeBomb(m_MiddlePosition, Color.White, 1f);
		m_bExploded = true;
		BONUS_DURATION = 140f;
	}

	public override void StopBonus()
	{
		if (BONUS_DURATION <= 0f)
		{
			foreach (Player player in m_StateInstance.m_Players)
			{
				if (player.m_Tag == 0 && Vector2.Distance(player.GetPosition(), m_MiddlePosition) <= 200f)
				{
					player.m_Tag = 1;
					player.DecreaseScore(1);
				}
			}
		}
		base.StopBonus();
	}

	public override void DrawBonus()
	{
		if (m_Player != null)
		{
			Vector2 position = m_MiddlePosition;
			position.Y -= 20f;
			m_StateInstance.ScreenManager.DrawText(m_StateInstance.ScreenManager.GoBoomBig, ref position, ((int)(BONUS_DURATION / 1000f)).ToString(), ScreenManager.TextOrigin.center_center, Color.White);
			m_Sprite.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		}
		else
		{
			m_StaticSprite.Draw(m_MiddlePosition, Color.White, m_Effect, 1f);
		}
		base.DrawBonus();
	}
}
