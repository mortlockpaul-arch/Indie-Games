using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TextDisplayCol
{
	public List<TextDisplay> tDisplay;

	public TextDisplayCol()
	{
		tDisplay = new List<TextDisplay>();
	}

	public void Draw(GameTime gametime)
	{
		if (tDisplay.Count <= 0)
		{
			return;
		}
		BaseGame.Get().SwitchEffectTechnique("TextLine");
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		if (tDisplay[0].DisplayProgress < tDisplay[0].TransitionLength)
		{
			BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue(tDisplay[0].DisplayProgress * 55f);
		}
		if (tDisplay[0].DisplayProgress > 1f - tDisplay[0].TransitionLength)
		{
			BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue((1f - tDisplay[0].DisplayProgress) * 55f);
		}
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (TextDisplay item in tDisplay)
		{
			item.BatchDraw(gametime);
		}
		BaseGame.Get().spriteBatch.End();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue(-15f);
	}
}
