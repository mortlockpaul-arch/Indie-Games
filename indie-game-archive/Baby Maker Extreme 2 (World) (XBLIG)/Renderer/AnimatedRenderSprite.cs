using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Renderer;

public class AnimatedRenderSprite
{
	private List<SpriteInstance> m_lImages;

	private List<float> m_lTimes;

	private float m_fTimer;

	private float m_fLastTimer;

	private bool m_bRepeats;

	private bool m_bMirror;

	public float Depth
	{
		get
		{
			return m_lImages[0].Depth;
		}
		set
		{
			foreach (SpriteInstance lImage in m_lImages)
			{
				lImage.Depth = value;
			}
		}
	}

	public Color Color
	{
		get
		{
			return m_lImages[0].Color;
		}
		set
		{
			foreach (SpriteInstance lImage in m_lImages)
			{
				lImage.Color = value;
			}
		}
	}

	public float Alpha
	{
		get
		{
			return m_lImages[0].Alpha;
		}
		set
		{
			foreach (SpriteInstance lImage in m_lImages)
			{
				lImage.Alpha = value;
			}
		}
	}

	public bool AnimationComplete => m_lTimes.Last() < m_fTimer;

	public float Timer
	{
		get
		{
			return m_fTimer;
		}
		set
		{
			m_fTimer = value;
		}
	}

	public float TotalAnimationTime => m_lTimes.Last();

	public Vector2 Position
	{
		get
		{
			return m_lImages[0].Position;
		}
		set
		{
			foreach (SpriteInstance lImage in m_lImages)
			{
				lImage.Position = value;
			}
		}
	}

	public float Rotation
	{
		get
		{
			return m_lImages[0].Rotation;
		}
		set
		{
			foreach (SpriteInstance lImage in m_lImages)
			{
				lImage.Rotation = value;
			}
		}
	}

	public float WidthScale
	{
		get
		{
			return m_lImages[0].WidthScale;
		}
		set
		{
			foreach (SpriteInstance lImage in m_lImages)
			{
				lImage.WidthScale = value;
			}
		}
	}

	public AnimatedRenderSprite(List<SpriteInstance> imgs, List<float> times, bool repeats)
	{
		Initialize(imgs, times, repeats);
	}

	public AnimatedRenderSprite(string imgName, float timeBetween, bool repeats, Rectangle startPos, int horizCount, int verticalCount, float depth)
	{
		m_bMirror = false;
		List<SpriteInstance> list = new List<SpriteInstance>();
		List<float> list2 = new List<float>();
		SpritePage page = TextureContainer.GetPage(imgName);
		float num = 0f;
		for (int i = 0; i < verticalCount; i++)
		{
			for (int j = 0; j < horizCount; j++)
			{
				list.Add(new SpriteInstance(new SpriteImage(page, new Rectangle(startPos.X + startPos.Width * j, startPos.Y + startPos.Height * i, startPos.Width, startPos.Height)), default(Vector2), depth));
				num += timeBetween;
				list2.Add(num);
			}
		}
		Initialize(list, list2, repeats);
	}

	private void Initialize(List<SpriteInstance> imgs, List<float> times, bool repeats)
	{
		m_lImages = imgs;
		m_lTimes = times;
		m_fTimer = 0f;
		m_fLastTimer = 0f;
		m_bRepeats = repeats;
	}

	public SpriteInstance GetRenderSprite()
	{
		return m_lImages[RenderSpriteIndex()];
	}

	private int RenderSpriteIndex()
	{
		for (int i = 0; i < m_lImages.Count; i++)
		{
			if (m_lTimes[i] > m_fTimer)
			{
				return i;
			}
		}
		return m_lImages.Count - 1;
	}

	public void Update(TimeTracker gameTime)
	{
		m_fLastTimer = m_fTimer;
		m_fTimer += gameTime.FractionOfSecond;
		if (m_bRepeats && AnimationComplete)
		{
			m_fLastTimer = 0f;
			m_fTimer -= m_lTimes.Last();
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		GetRenderSprite().Draw(gameTime);
	}
}
