namespace Microsoft.XboxLive.MathUtilities;

public struct Colorf
{
	public float blue;

	public float green;

	public float red;

	public float alpha;

	public int CompositeArgb
	{
		get
		{
			int num = (int)(red * 255f);
			int num2 = (int)(green * 255f);
			int num3 = (int)(blue * 255f);
			int num4 = (int)(alpha * 255f);
			return (num4 << 24) + (num << 16) + (num2 << 8) + num3;
		}
		set
		{
			alpha = (float)(int)(byte)(value >> 24) / 255f;
			red = (float)(int)(byte)(value >> 16) / 255f;
			green = (float)(int)(byte)(value >> 8) / 255f;
			blue = (float)(int)(byte)value / 255f;
		}
	}

	public Colorf(float red, float green, float blue, float alpha)
	{
		this.red = red;
		this.green = green;
		this.blue = blue;
		this.alpha = alpha;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Colorf))
		{
			return false;
		}
		return Equals((Colorf)obj);
	}

	public bool Equals(Colorf other)
	{
		return red == other.red && green == other.green && blue == other.blue && alpha == other.alpha;
	}

	public static bool operator ==(Colorf color1, Colorf color2)
	{
		return color1.Equals(color2);
	}

	public static bool operator !=(Colorf color1, Colorf color2)
	{
		return !color1.Equals(color2);
	}

	public override int GetHashCode()
	{
		return (int)(red + 255f * green + 65280f * blue + 16711680f * alpha);
	}
}
