using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class WhiteSugar
{
	private const int LIFE_BONUS = 20;

	private GameState m_StateInstance;

	private SpriteBatch m_SpriteBatch;

	private Rectangle m_sourceRectangle;

	private AudioClip m_UseSound;

	private Texture2D m_Sprite;

	public Vector2 m_Position;

	private float m_GrabRadius;

	private bool m_bSpawn;

	public WhiteSugar(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.content.Load<Texture2D>("PowerUp/Sugar/PowerUp_WhiteSugar");
		m_SpriteBatch = spriteBatch;
		m_UseSound = new AudioClip("PowerUp_SucreBoost");
		m_GrabRadius = m_Sprite.Width;
		m_sourceRectangle = new Rectangle(0, 0, m_Sprite.Width, m_Sprite.Height);
	}

	public void Spawn(Vector2 pos)
	{
		m_Position = pos;
		m_bSpawn = true;
	}

	public bool IsSpawned()
	{
		return m_bSpawn;
	}

	public void StopBonus()
	{
		m_bSpawn = false;
	}

	public bool Update()
	{
		if (m_bSpawn)
		{
			for (int i = 0; i < m_StateInstance.m_Players.Count; i++)
			{
				if (Vector2.Distance(m_StateInstance.m_Players[i].GetPosition(), m_Position) < m_GrabRadius)
				{
					m_StateInstance.m_Players[i].m_life += 20;
					m_UseSound.Play();
					StopBonus();
					return true;
				}
			}
		}
		return false;
	}

	public void DrawBonus()
	{
		if (m_bSpawn)
		{
			m_SpriteBatch.Draw(m_Sprite, m_Position, m_sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
		}
	}
}
