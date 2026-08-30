using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class CurveRelationship
{
	public delegate float RelationShipCB(float f);

	private RelationShipCB cb;

	private float[] sampleValues;

	private int count;

	public readonly float MaximumIndex;

	private Texture2D tex;

	private UIBoxColorBG box;

	private readonly Color drawColor;

	public readonly Rectangle DestRect;

	public CurveRelationship(RelationShipCB CB, float precision, Rectangle drawLocation, Color drawColor, Color bgCol)
		: this(CB, precision, willBeDrawn: true, drawLocation, drawColor, bgCol)
	{
	}

	public CurveRelationship(RelationShipCB CB, float precision)
		: this(CB, precision, willBeDrawn: false, Rectangle.Empty, Color.White, Color.White)
	{
	}

	private CurveRelationship(RelationShipCB CB, float precision, bool willBeDrawn, Rectangle drawLocation, Color drawColor, Color bgCol)
	{
		cb = CB;
		if (precision <= 0f || precision >= 1f)
		{
			throw new Exception("precision must be in range [ 0 -> 1 ]");
		}
		count = (int)(1f / precision);
		sampleValues = new float[count + 1];
		float num = -1f;
		float maximumIndex = -1f;
		for (int i = 0; i < count + 1; i++)
		{
			float num2 = (float)i / (float)count;
			sampleValues[i] = Evaluate(num2);
			if (sampleValues[i] > num)
			{
				num = sampleValues[i];
				maximumIndex = num2;
			}
		}
		MaximumIndex = maximumIndex;
		if (!willBeDrawn)
		{
			return;
		}
		box = new UIBoxColorBG(drawLocation, 0.03f, bgCol);
		this.drawColor = drawColor;
		DestRect = box.InsideRect;
		tex = new Texture2D(MaximinusGame.Draw2D.Device, DestRect.Width, DestRect.Height);
		Color[] array = new Color[tex.Width * tex.Height];
		for (int j = 0; j < tex.Width; j++)
		{
			for (int k = 0; k < tex.Height; k++)
			{
				if (Math.Abs(Evaluate((float)j / (float)tex.Width) - (float)k / (float)tex.Height) < precision * 1f)
				{
					array[(tex.Height - 1 - k) * tex.Width + j] = drawColor;
				}
			}
		}
		tex.SetData(array);
	}

	public virtual float Evaluate(float x)
	{
		return cb(x);
	}

	public void Draw()
	{
		Draw(useCurrentValue: false, Vector2.Zero);
	}

	public void Draw(Vector2 currentValue)
	{
		Draw(useCurrentValue: true, currentValue);
	}

	private void Draw(bool useCurrentValue, Vector2 currentValue)
	{
		if (box == null)
		{
			throw new Exception("not correctly initialized");
		}
		box.Draw();
		MaximinusGame.Draw2D.SpriteBatch.Draw(tex, DestRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.01f);
		if (useCurrentValue)
		{
			Rectangle destinationRectangle = new Rectangle((int)((float)DestRect.X + (float)DestRect.Width * currentValue.X), DestRect.Y, 1, DestRect.Height);
			MaximinusGame.Draw2D.SpriteBatch.Draw(MaximinusGame.Draw2D.BlankTex, destinationRectangle, null, Color.White);
		}
	}
}
