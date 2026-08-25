using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class WaveFx
{
	private const float WAVE_TIMER = 2000f;

	private Texture2D m_Sprite;

	private SpriteBatch m_Batch;

	private Vector2 m_Position;

	private Effect m_Shader;

	private float currentTime;

	private bool bReverse;

	private EffectParameter waveParam;

	public WaveFx(SpriteBatch LocalBatch, Texture2D Sprite, int x, int y, Effect shader)
	{
		m_Batch = LocalBatch;
		m_Sprite = Sprite;
		m_Position = new Vector2(x, y);
		m_Shader = shader;
		m_Shader.Parameters["WavePeriod"].SetValue(5);
		m_Shader.Parameters["WaveAmplitude"].SetValue(0.2f);
		waveParam = m_Shader.Parameters["XOffset"];
	}

	public void Update(GameTime gameTime)
	{
		if (!bReverse)
		{
			currentTime += (float)gameTime.ElapsedGameTime.Milliseconds / 2000f;
			if (currentTime >= 1f)
			{
				currentTime = 1f;
				bReverse = true;
			}
		}
		else
		{
			currentTime += (float)gameTime.ElapsedGameTime.Milliseconds / 2000f;
			if (currentTime <= 0f)
			{
				currentTime = 0f;
				bReverse = false;
			}
		}
		waveParam.SetValue(MathHelper.Lerp(0f, 1f, currentTime));
	}

	public void Draw()
	{
		m_Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, m_Shader);
		m_Batch.Draw(m_Sprite, m_Position, Color.White);
		m_Batch.End();
	}
}
