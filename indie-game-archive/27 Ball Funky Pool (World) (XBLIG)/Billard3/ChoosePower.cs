using System;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class ChoosePower
{
	private const double MaxTime = 2.5;

	private const float MinRatio = 0.1f;

	public static Rectangle SpriteRect;

	public static Texture2D SpriteOverlay;

	private static RoundedRectangle overlay;

	private static double startTime = -1.0;

	private static float ratio;

	public static float Ratio => ratio;

	public static void Initialize(Point ScreenSize, ContentManager Content)
	{
		float num = (int)((float)ScreenSize.X * 0.2f);
		float num2 = (int)(num / 8f * 3f / 2f);
		float num3 = (float)ScreenSize.X * 0.55f;
		float num4 = (float)ScreenSize.Y * 0.5f - num * 0.5f;
		SpriteRect = new Rectangle((int)num3, (int)num4, (int)num2, (int)num);
		SpriteOverlay = Content.Load<Texture2D>("tex/gameInfo_ShootForce_Inside.v9");
		Rectangle spriteRect = SpriteRect;
		overlay = new RoundedRectangle(spriteRect);
		overlay.TexWidth = (int)(Statics.draw2D.ScreenSize.X / 120f);
		overlay.Color = GameMenus.ColorOutline;
		SpriteRect = Menus.ManagerV2.OverlayWithOffset(SpriteRect);
	}

	public static void Start(double startTimeSeconds)
	{
		startTime = startTimeSeconds;
	}

	public static void Update(GameTime gameTime)
	{
		if (startTime == -1.0 || GameState.Current != GameState.Type.CHOOSING_POWER)
		{
			return;
		}
		float num = (float)((gameTime.TotalGameTime.TotalSeconds - startTime) / 2.5);
		if (num < 0.5f)
		{
			ratio = Utils.PowerCurve(num * 2f, 1f);
		}
		else if (num < 1f)
		{
			float num2 = (num - 0.5f) * 2f;
			for (int i = 0; i < 0; i++)
			{
				num2 = (float)Math.Sqrt(num2);
			}
			ratio = 1f - num2;
			ratio = MathHelper.Max(0.1f, ratio);
		}
		ratio = Utils.clampRatio(MathHelper.Lerp(0f, 1f, ratio * 1.075f));
		ratio = Utils.PowerCurve(ratio, 2f);
		if (num >= 0.5f && ratio <= 0.1f)
		{
			GameState.ChangeWithTransition(GameState.Type.WATCHING_MOVE, gameTime);
		}
	}

	public static void Draw(SpriteBatch spriteBatch)
	{
		if (GameState.Current == GameState.Type.CHOOSING_POWER)
		{
			Rectangle spriteRect = SpriteRect;
			spriteRect.Height = (int)((float)spriteRect.Height * Ratio);
			spriteRect.Y += (int)((float)SpriteRect.Height * (1f - Ratio));
			spriteBatch.Draw(sourceRectangle: new Rectangle(0, (int)((float)SpriteOverlay.Height * (1f - Ratio)), SpriteOverlay.Width, (int)((float)SpriteOverlay.Height * Ratio)), texture: SpriteOverlay, destinationRectangle: spriteRect, color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 0.5f);
			overlay.Draw(spriteBatch);
			spriteBatch.Draw(GameMenus.Textures.MenuBG, overlay.Rect, null, Utils.ColorWithAlpha(GameMenus.ColorOverlay, 0.325f), 0f, Vector2.Zero, SpriteEffects.None, 0.4f);
		}
	}
}
