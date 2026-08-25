using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine;

public static class TransitionEffects
{
	private static SpriteBatch spriteBatch;

	private static Texture2D blankImage;

	public static Color FadeColor = Color.Black;

	public static void InitializeScreenFadingEffect()
	{
		spriteBatch = EngineManager.GetSpriteBatch;
		AssetManager.GetAsset(ImageKeys.pixel, ref blankImage);
	}

	public static void FadeScreenSpriteBatch(Rectangle area, float alpha)
	{
		spriteBatch.Begin();
		spriteBatch.Draw(blankImage, area, FadeColor * alpha);
		spriteBatch.End();
	}

	public static void FadeScreen(Rectangle area, float alpha)
	{
		spriteBatch.Draw(blankImage, area, FadeColor * alpha);
	}
}
