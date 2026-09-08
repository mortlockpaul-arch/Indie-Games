using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public sealed class Utilities
{
	public static Vector4 Vector4FromInt(int color)
	{
		return new Vector4
		{
			W = (float)((color >> 24) & 0xFF) / 255f,
			X = (float)((color >> 16) & 0xFF) / 255f,
			Y = (float)((color >> 8) & 0xFF) / 255f,
			Z = (float)(color & 0xFF) / 255f
		};
	}

	public static Vector4 ColorbToVector4(Colorb color)
	{
		return new Vector4
		{
			W = (float)(int)color.alpha * 0.003921569f,
			X = (float)(int)color.red * 0.003921569f,
			Y = (float)(int)color.green * 0.003921569f,
			Z = (float)(int)color.blue * 0.003921569f
		};
	}

	public static Colorb ColorbFromVector4(Vector4 color)
	{
		return new Colorb
		{
			alpha = (byte)(color.W * 255f),
			red = (byte)(color.X * 255f),
			green = (byte)(color.Y * 255f),
			blue = (byte)(color.Z * 255f)
		};
	}

	public static Colorb ColorFromUint(uint color)
	{
		return new Colorb
		{
			alpha = (byte)((color >> 24) & 0xFF),
			red = (byte)((color >> 16) & 0xFF),
			green = (byte)((color >> 8) & 0xFF),
			blue = (byte)(color & 0xFF)
		};
	}
}
