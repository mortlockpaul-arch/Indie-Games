using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TextDisplay2Col
{
	public List<TextDisplay2> tDisplay;

	public TextDisplay2Col()
	{
		tDisplay = new List<TextDisplay2>();
	}

	public void Draw(GameTime gametime)
	{
		if (tDisplay.Count <= 0)
		{
			return;
		}
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		foreach (TextDisplay2 item in tDisplay)
		{
			item.BatchDraw(gametime);
		}
		BaseGame.Get().spriteBatch.End();
	}
}
