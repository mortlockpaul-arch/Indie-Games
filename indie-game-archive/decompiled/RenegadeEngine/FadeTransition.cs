using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine;

public class FadeTransition : TransitionTracker
{
	private Texture2D blankImage;

	public Color FadeColor = Color.Black;

	public Rectangle Area = new Rectangle(0, 0, Global.ScreenWidth, Global.ScreenHeight);

	public FadeTransition()
	{
		AssetManager.GetAsset(ImageKeys.pixel, ref blankImage);
		Global.ResolutionChanged += On_ResolutionChanged;
	}

	public FadeTransition(TimeSpan inTime, TimeSpan outTime)
		: base(inTime, outTime)
	{
		AssetManager.GetAsset(ImageKeys.pixel, ref blankImage);
	}

	public void Draw()
	{
		spriteBatch.Draw(blankImage, Area, FadeColor * base.Transition);
	}

	public void DrawBDE()
	{
		spriteBatch.Begin();
		spriteBatch.Draw(blankImage, Area, FadeColor * base.Transition);
		spriteBatch.End();
	}

	private void On_ResolutionChanged(object sender, EventArgs e)
	{
		Area.Width = Global.ScreenWidth;
		Area.Height = Global.ScreenHeight;
	}
}
