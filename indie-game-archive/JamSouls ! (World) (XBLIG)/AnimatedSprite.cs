using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class AnimatedSprite
{
	public Texture2D m_Texture;

	public int m_TotalFrames;

	public int m_FrameWidth;

	public int m_FrameHeight;

	public float m_Speed;

	public int m_CurrentFrame;

	public float m_TimeFrames;

	public Vector2 m_FixedPos;

	public int m_TotalLoop;

	public int m_CurrentLoop;

	public bool m_bInfiniteLoop;

	private bool m_AnimLocked;

	private int m_OffsetX;

	private int m_OffsetY;

	private SpriteBatch m_Batch;

	public AnimatedSprite(SpriteBatch LocalBatch, Texture2D Sprite, int FrameCount, int Width, int Height, float Speed, int Loop, int AtlasOffsetX, int AtlasOffsetY)
	{
		m_OffsetX = AtlasOffsetX;
		m_OffsetY = AtlasOffsetY;
		InitAnim(LocalBatch, Sprite, FrameCount, Width, Height, Speed, Loop);
	}

	public AnimatedSprite(SpriteBatch LocalBatch, Texture2D Sprite, int FrameCount, int Width, int Height, float Speed, int Loop)
	{
		InitAnim(LocalBatch, Sprite, FrameCount, Width, Height, Speed, Loop);
	}

	public int GetOffsetY()
	{
		return m_OffsetY;
	}

	public int GetOffsetX()
	{
		return m_OffsetX;
	}

	private void InitAnim(SpriteBatch LocalBatch, Texture2D Sprite, int FrameCount, int Width, int Height, float Speed, int Loop)
	{
		m_Batch = LocalBatch;
		m_Texture = Sprite;
		m_TotalFrames = FrameCount;
		m_FrameWidth = Width;
		m_FrameHeight = Height;
		m_TotalLoop = Loop;
		m_CurrentLoop = m_TotalLoop;
		m_Speed = Speed;
		m_CurrentFrame = 0;
		m_TimeFrames = 0f;
		m_FixedPos.X = 0f;
		m_FixedPos.Y = 0f;
		if (m_TotalLoop == 0)
		{
			m_bInfiniteLoop = true;
		}
		else
		{
			m_bInfiniteLoop = false;
		}
	}

	public void SetPosition(Vector2 pos)
	{
		m_FixedPos = pos;
	}

	public void Draw(ref Vector2 Position, SpriteEffects spe, Color c, float zdepth)
	{
		Rectangle value = new Rectangle(m_OffsetX + m_CurrentFrame * m_FrameWidth, m_OffsetY, m_FrameWidth, m_FrameHeight);
		m_Batch.Draw(m_Texture, Position, value, c, 0f, Vector2.Zero, 1f, spe, zdepth);
	}

	public void Draw(ref Vector2 Position, float Rotation, Vector2 m_Origin, SpriteEffects speffect, Color c, float zdepth)
	{
		Rectangle value = new Rectangle(m_OffsetX + m_CurrentFrame * m_FrameWidth, m_OffsetY, m_FrameWidth, m_FrameHeight);
		m_Batch.Draw(m_Texture, Position, value, c, Rotation, m_Origin, 1f, speffect, zdepth);
	}

	public void Draw(ref Vector2 Position, SpriteEffects spe, Color c, float scale, float zdepth)
	{
		Rectangle value = new Rectangle(m_OffsetX + m_CurrentFrame * m_FrameWidth, m_OffsetY, m_FrameWidth, m_FrameHeight);
		m_Batch.Draw(m_Texture, Position, value, c, 0f, new Vector2(-m_FrameWidth / 2, -m_FrameHeight / 2), scale, spe, zdepth);
	}

	public void DrawFixed(SpriteEffects spe, Color color, float zDepth)
	{
		Rectangle value = new Rectangle(m_OffsetX + m_CurrentFrame * m_FrameWidth, m_OffsetY, m_FrameWidth, m_FrameHeight);
		m_Batch.Draw(m_Texture, m_FixedPos, value, color, 0f, Vector2.Zero, 1f, spe, zDepth);
	}

	public void DrawFixed(SpriteEffects spe, Color color, float scale, float zDepth)
	{
		Rectangle value = new Rectangle(m_OffsetX + m_CurrentFrame * m_FrameWidth, m_OffsetY, m_FrameWidth, m_FrameHeight);
		m_Batch.Draw(m_Texture, m_FixedPos, value, color, 0f, Vector2.Zero, scale, spe, zDepth);
	}

	public void UpdateFrame(float elapsed)
	{
		m_TimeFrames += elapsed;
		if (m_TimeFrames > m_Speed)
		{
			m_CurrentFrame++;
			if (m_CurrentFrame == m_TotalFrames)
			{
				m_CurrentFrame--;
				if (!m_bInfiniteLoop)
				{
					m_CurrentLoop--;
				}
				if (m_CurrentLoop > 0 || m_bInfiniteLoop)
				{
					m_CurrentFrame = 0;
				}
			}
			m_TimeFrames = 0f;
		}
		if (IsAnimFinished())
		{
			m_AnimLocked = false;
		}
	}

	public void SetLock(bool locked)
	{
		m_AnimLocked = locked;
	}

	public bool IsLocked()
	{
		return m_AnimLocked;
	}

	public void Reset()
	{
		m_CurrentLoop = m_TotalLoop;
		m_CurrentFrame = 0;
	}

	public int GetFrameWidth()
	{
		return m_FrameWidth;
	}

	public int GetFrameHeight()
	{
		return m_FrameHeight;
	}

	public bool IsAnimFinished()
	{
		if (m_CurrentLoop <= 0)
		{
			return !m_bInfiniteLoop;
		}
		return false;
	}
}
