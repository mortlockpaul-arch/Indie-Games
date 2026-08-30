using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86;

public class SpriteTools
{
	public static SpriteBatch sprite;

	public static void BeginOpaque()
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
	}

	public static void BeginAlpha()
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
	}

	public static void BeginAlphaPoint()
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
	}

	public static void BeginAdditivePoint()
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, null);
	}

	public static void BeginAdditive()
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive);
	}

	public static void BeginAlpha(Effect effect)
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, effect);
	}

	public static void BeginAdditive(Effect effect)
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, effect);
	}

	public static void BeginOpaque(Effect effect)
	{
		sprite.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, effect);
	}

	public static void End()
	{
		sprite.End();
	}
}
