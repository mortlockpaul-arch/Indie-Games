using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class Cloud : PowerUp
{
	private const float FLASH_DURATION = 100f;

	private AnimatedSprite m_Bolt;

	private AnimatedSprite m_Cloud;

	private bool m_bStarted;

	private float m_FlashTimer;

	private AudioClip m_ThunderSound;

	public Cloud(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Bolt = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Cloud/PowerUp_Bolt.xml", GameState.GameAtlas.GAME, "PowerUp_Bolt");
		m_Cloud = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Cloud/PowerUp_CloudAnim.xml", GameState.GameAtlas.GAME, "CloudAnim");
		m_Bolt.m_TotalLoop = 1;
		m_Bolt.m_bInfiniteLoop = false;
		m_ThunderSound = new AudioClip("PowerUp_Cloud");
		InitPowerUp(m_Cloud.GetFrameWidth(), m_Cloud.GetFrameHeight(), spriteBatch);
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 15000f;
		m_FlashTimer = 0f;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		m_Cloud.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		if (m_Player == null)
		{
			return;
		}
		m_Effect = m_Player.m_SpriteEffect;
		if (!m_bStarted)
		{
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_bStarted = true;
				m_ThunderSound.Play();
			}
			UpdatePosition(gameTime, m_Player.GetPosition());
			base.Update(gameTime);
		}
		else if (m_FlashTimer <= 0f)
		{
			m_Bolt.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			m_MiddlePosition = m_Player.GetHeadPlot();
			if (m_Bolt.m_CurrentFrame >= m_Bolt.m_TotalFrames - 1)
			{
				m_FlashTimer = 100f;
				foreach (Player player in m_StateInstance.m_Players)
				{
					if (player != m_Player && (player.m_Tag == 0 || player.m_Tag == 2) && !player.m_bIsDucked && player.m_SbireDef == PlayerConfig.SBIRE_DEF.NONE)
					{
						player.Morph(2f);
					}
				}
			}
		}
		if (m_FlashTimer > 0f)
		{
			m_FlashTimer -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_FlashTimer <= 0f)
			{
				StopBonus();
			}
		}
	}

	public override void StopBonus()
	{
		m_bStarted = false;
		m_Bolt.Reset();
		base.StopBonus();
	}

	public override void DrawBonus()
	{
		if (m_FlashTimer > 0f)
		{
			m_spriteBatch.Draw(m_StateInstance.ScreenManager.blankTexture, new Rectangle(0, 0, 1280, 720), new Rectangle(0, 0, 4, 4), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
		}
		if (m_Player != null)
		{
			Vector2 Position = Vector2.Zero;
			Position.X = 640 - m_Bolt.GetFrameWidth() * 4;
			Position.Y -= m_Bolt.GetFrameHeight() * 4 / 2;
			if (m_bStarted)
			{
				m_Bolt.Draw(ref Position, SpriteEffects.None, Color.White, 4f, m_zorder);
			}
		}
		m_Cloud.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		base.DrawBonus();
	}
}
