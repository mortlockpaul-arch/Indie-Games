using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA.Sprites;

public class SpriteBatchEffect
{
	private int effect;

	private BlendState stateSub;

	private SpriteBatch batch;

	public GraphicsDevice GraphicsDevice;

	private Effect monoFx;

	private Effect invertFx;

	public SpriteBatchEffect(ContentManager content, GraphicsDevice device)
	{
		stateSub = new BlendState();
		stateSub.ColorSourceBlend = Blend.SourceAlpha;
		stateSub.AlphaSourceBlend = Blend.SourceAlpha;
		stateSub.ColorDestinationBlend = Blend.One;
		stateSub.AlphaDestinationBlend = Blend.One;
		stateSub.ColorBlendFunction = BlendFunction.ReverseSubtract;
		monoFx = content.Load<Effect>("mono");
		invertFx = content.Load<Effect>("invert");
		GraphicsDevice = device;
		batch = new SpriteBatch(device);
	}

	public void Begin()
	{
		effect = 0;
		batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
	}

	public void End()
	{
		batch.End();
	}

	public void SetEffect(int e)
	{
		if (e != effect)
		{
			effect = e;
			switch (effect)
			{
			case 9:
				batch.End();
				batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
				break;
			case 11:
				batch.End();
				batch.Begin(SpriteSortMode.Immediate, stateSub);
				break;
			case 2:
				batch.End();
				batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, invertFx);
				break;
			case 10:
				batch.End();
				batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, monoFx);
				break;
			default:
				batch.End();
				batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				effect = 0;
				break;
			}
		}
	}

	public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, int e, int effectParam)
	{
		if (e != effect)
		{
			SetEffect(e);
		}
		if (effect == 1)
		{
			float num = (float)((double)(128 - effectParam) / 128.0);
			color *= num;
		}
		batch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
	}

	public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, int e)
	{
		if (e != effect)
		{
			SetEffect(e);
		}
		batch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteEffects.None, 0f);
	}

	public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
	{
		SetEffect(0);
		batch.Draw(texture, destinationRectangle, sourceRectangle, color);
	}

	public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, int effect, int effectParam)
	{
		SetEffect(effect & 0xFFF);
		batch.Draw(texture, destinationRectangle, sourceRectangle, color);
	}

	public void DrawString(SpriteFont font, string s, Vector2 v, Color c)
	{
		SetEffect(0);
		batch.DrawString(font, s, v, c);
	}

	public void DrawString(SpriteFont font, string s, Vector2 v, Color c, int e, int effectParam)
	{
		if (e != effect)
		{
			SetEffect(e);
		}
		if (effect == 1)
		{
			float num = (float)((double)(128 - effectParam) / 128.0);
			c *= num;
		}
		batch.DrawString(font, s, v, c);
	}
}
