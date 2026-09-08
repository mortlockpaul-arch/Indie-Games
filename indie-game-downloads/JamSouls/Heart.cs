using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

internal class Heart : PowerUp
{
	private const float POKE_TIMER = 200f;

	private const float BALOON_OFFSET = 100f;

	private const int ROPE_ITEM = 4;

	public static float HEART_DIE_TIME = 200f;

	public Vector2 m_FragDamper = new Vector2(0f, -1280f);

	private AnimatedSprite m_Sprite;

	private AnimatedSprite m_SpritePoc;

	private AudioClip m_HeartExplosion;

	public Heart(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Heart/Heart_Anim.xml", GameState.GameAtlas.GAME, "PowerUp_Heart");
		m_SpritePoc = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Heart/Heart_Poc.xml", GameState.GameAtlas.GAME, "PowerUp_HeartPoc");
		InitPowerUp(m_Sprite.GetFrameWidth(), m_Sprite.GetFrameHeight(), spriteBatch);
		m_HeartExplosion = new AudioClip("PowerUp_HeartExplode");
	}

	public override void InitBonus()
	{
		BONUS_DURATION = 15000f;
		m_SpritePoc.Reset();
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player != null)
		{
			m_Effect = m_Player.m_SpriteEffect;
			Vector2 position = m_Player.GetPosition();
			position.Y -= 100f;
			UpdatePosition(gameTime, position);
			Vector2 middlePosition = m_MiddlePosition;
			middlePosition.X += m_Sprite.GetFrameWidth() / 2;
			middlePosition.Y += m_Sprite.GetFrameHeight() / 2;
			if (BONUS_DURATION <= HEART_DIE_TIME)
			{
				if (BONUS_DURATION == HEART_DIE_TIME)
				{
					m_HeartExplosion.Play();
				}
				BONUS_DURATION -= gameTime.ElapsedGameTime.Milliseconds;
				m_Player.SetAnimation(Player.AnimStates.HALF_EXPLODE, bForcePlay: true);
				if (BONUS_DURATION <= 0f)
				{
					StopBonus();
				}
				m_SpritePoc.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			}
		}
		if (m_EffectTimer > 0f)
		{
			m_EffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		m_Sprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
	}

	public override void StopBonus()
	{
		m_Player.m_life = 100;
		BONUS_DURATION = 15000f;
		base.StopBonus();
	}

	public override void DrawBonus()
	{
		if (BONUS_DURATION <= HEART_DIE_TIME)
		{
			m_SpritePoc.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		}
		else
		{
			m_Sprite.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		}
		base.DrawBonus();
	}
}
