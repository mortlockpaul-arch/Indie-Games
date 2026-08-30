using System.Runtime.CompilerServices;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Games.Painter;

internal class ColorPalette
{
	[CompilerGenerated]
	private Rectangle _003CBounds_003Ek__BackingField;

	public PainterColor Color { get; private set; }

	public Rectangle Bounds
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CBounds_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CBounds_003Ek__BackingField = value;
		}
	}

	public float Scaling { get; set; }

	public float Animation { get; set; }

	public static bool IsLocked(PainterColor color)
	{
		if (Guide.IsTrialMode)
		{
			switch (color)
			{
			case PainterColor.Black:
				return true;
			case PainterColor.White:
				return true;
			case PainterColor.Green:
				return true;
			case PainterColor.Blue:
				return true;
			case PainterColor.Yellow:
				return true;
			}
		}
		return false;
	}

	public ColorPalette(Rectangle bounds, PainterColor color)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Color = color;
		Bounds = bounds;
		Scaling = 1f;
	}

	public bool IsSelected(Vector2 location)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Rectangle bounds = Bounds;
		return ((Rectangle)(ref bounds)).Contains(new Point((int)location.X, (int)location.Y));
	}

	public void Draw(SpriteBatch spriteBatch, float transition)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		Rectangle bounds = Bounds;
		float num = ((Rectangle)(ref bounds)).Left + Bounds.Width / 2;
		Rectangle bounds2 = Bounds;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(num, (float)(((Rectangle)(ref bounds2)).Top + Bounds.Height / 2));
		val.Y -= Animation;
		val.Y -= transition * 50f - 50f;
		spriteBatch.DrawAligned(Graphics.Blank, val, 0f, Scaling, Align.Center, new Color(PainterPlayScreen.GetColor(Color), transition));
		spriteBatch.DrawAligned(Graphics.Palette, val, 0f, Scaling, Align.Center, new Color(Color.Black, transition));
		if (IsLocked(Color))
		{
			spriteBatch.DrawAligned(Graphics.Locked, val + new Vector2(1f, 1f), 0f, Scaling, Align.Center, new Color(Color.White, transition));
		}
	}
}
