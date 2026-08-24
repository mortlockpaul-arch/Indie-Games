using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FiftyGames.LightBikes;

internal class GridBlock
{
	private Texture2D m_Sprite;

	private byte alpha = byte.MaxValue;

	private bool set;

	private bool firstPass;

	public Color LastBackingColor = Color.White;

	public Color backingColor = Color.White;

	public Color blockColor = Color.White;

	public GridBlock(Texture2D inSprite)
	{
		m_Sprite = inSprite;
	}

	public void Update()
	{
		if (set && firstPass)
		{
			firstPass = false;
			alpha = 128;
		}
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 gridPosition, int inPixelGap, int x, int y, bool clearMode)
	{
		if (set)
		{
			spriteBatch.Draw(m_Sprite, new Vector2(gridPosition.X + (float)(x * inPixelGap), gridPosition.Y + (float)(y * inPixelGap)), null, blockColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
	}

	public void DrawBackingOnly(SpriteBatch spriteBatch, Vector2 gridPosition, int inPixelGap, int x, int y, bool clearMode)
	{
		Color color = backingColor;
		color.A = alpha;
		spriteBatch.Draw(m_Sprite, new Vector2(gridPosition.X + (float)(x * inPixelGap), gridPosition.Y + (float)(y * inPixelGap)), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
	}

	public void setColor(Color inColor)
	{
		blockColor = inColor;
	}

	public bool getSet()
	{
		return set;
	}

	public Color getColor()
	{
		return blockColor;
	}

	public void setBackingColor(byte R, byte G, byte B)
	{
		backingColor = new Color(R, G, B);
	}

	public void setElement(Color inColor)
	{
		blockColor = inColor;
		alpha = byte.MaxValue;
		set = true;
		firstPass = true;
	}
}
