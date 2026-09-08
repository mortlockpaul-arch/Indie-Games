using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class Splash
{
	public struct SplashData
	{
		public bool isFreeSlot;

		public Sprite Splashtex;

		public Color SplashColor;

		public Vector2 SplashPosition;

		public float SplashTimer;
	}

	public const float SPLASH_DURATION = 1000f;

	public const int MAX_SPLASH_DATA = 10;

	public const int SPLASH_NUMBER = 11;

	public Sprite[] m_Splash = new Sprite[11];

	public SplashData[] m_Splashdata = new SplashData[10];

	public SpriteBatch m_Batch;

	public Random m_RandSplash;

	public Splash(GameState currentState, SpriteBatch batch)
	{
		for (int i = 0; i < 11; i++)
		{
			m_Splash[i] = currentState.LoadSprite("Spash_" + (i + 1), GameState.GameAtlas.GAME);
		}
		for (int j = 0; j < 10; j++)
		{
			m_Splashdata[j].isFreeSlot = true;
		}
		m_Batch = batch;
		m_RandSplash = new Random();
	}

	public void SpawnSplash(Vector2 center, Color color, bool callOnce)
	{
		SplashData splashData = default(SplashData);
		splashData.isFreeSlot = false;
		splashData.Splashtex = m_Splash[m_RandSplash.Next(11)];
		splashData.SplashColor = color;
		splashData.SplashPosition.X = center.X - (float)(splashData.Splashtex.Width / 2);
		splashData.SplashPosition.Y = center.Y - (float)(splashData.Splashtex.Height / 2);
		splashData.SplashTimer = 0f;
		for (int i = 0; i < 10; i++)
		{
			if (m_Splashdata[i].isFreeSlot)
			{
				m_Splashdata[i] = splashData;
				if (callOnce)
				{
					break;
				}
				SpawnSplash(center, color, callOnce: true);
			}
		}
	}

	public void Update(GameTime gametime)
	{
		for (int i = 0; i < 10; i++)
		{
			if (!m_Splashdata[i].isFreeSlot)
			{
				m_Splashdata[i].SplashTimer += gametime.ElapsedGameTime.Milliseconds;
			}
		}
	}

	public void Draw()
	{
		m_Batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
		for (int i = 0; i < 10; i++)
		{
			if (m_Splashdata[i].isFreeSlot)
			{
				continue;
			}
			if (m_Splashdata[i].SplashTimer < 1000f)
			{
				if (m_Splashdata[i].SplashColor.A > 0 && m_Splashdata[i].SplashTimer > 500f)
				{
					m_Splashdata[i].SplashColor.A = (byte)(255f - m_Splashdata[i].SplashTimer * 255f / 500f);
				}
				m_Splashdata[i].Splashtex.Draw(m_Splashdata[i].SplashPosition, m_Splashdata[i].SplashColor);
			}
			else
			{
				m_Splashdata[i].isFreeSlot = true;
				m_Splashdata[i].SplashTimer = 0f;
				m_Splashdata[i].Splashtex = null;
			}
		}
		m_Batch.End();
	}
}
