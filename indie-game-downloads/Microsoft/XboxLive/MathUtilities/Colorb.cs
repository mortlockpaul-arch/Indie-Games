namespace Microsoft.XboxLive.MathUtilities;

public struct Colorb
{
	public byte blue;

	public byte green;

	public byte red;

	public byte alpha;

	public int CompositeArgb
	{
		get
		{
			return (alpha << 24) + (red << 16) + (green << 8) + blue;
		}
		set
		{
			alpha = (byte)(value >> 24);
			red = (byte)(value >> 16);
			green = (byte)(value >> 8);
			blue = (byte)value;
		}
	}

	public int CompositeRgb => (red << 16) + (green << 8) + blue;

	public Colorb(byte red, byte green, byte blue)
	{
		this.red = red;
		this.green = green;
		this.blue = blue;
		alpha = byte.MaxValue;
	}

	public Colorb(byte red, byte green, byte blue, byte alpha)
	{
		this.red = red;
		this.green = green;
		this.blue = blue;
		this.alpha = alpha;
	}

	public Colorb(int colorArgb8)
	{
		alpha = (byte)(colorArgb8 >> 24);
		red = (byte)(colorArgb8 >> 16);
		green = (byte)(colorArgb8 >> 8);
		blue = (byte)colorArgb8;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Colorb))
		{
			return false;
		}
		return Equals((Colorb)obj);
	}

	public bool Equals(Colorb other)
	{
		return CompositeArgb == other.CompositeArgb;
	}

	public static bool operator ==(Colorb color1, Colorb color2)
	{
		return color1.Equals(color2);
	}

	public static bool operator !=(Colorb color1, Colorb color2)
	{
		return !color1.Equals(color2);
	}

	public override int GetHashCode()
	{
		return CompositeArgb;
	}
}
