#define DEBUG
using System;
using System.Diagnostics;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public struct ComponentColors
{
	public Vector4 CustomColor0;

	public Vector4 CustomColor1;

	public Vector4 CustomColor2;

	public ComponentColors(Colorb[] colors)
	{
		Debug.Assert(colors != null);
		switch (colors.Length)
		{
		case 0:
			CustomColor2 = (CustomColor1 = (CustomColor0 = Utilities.Vector4FromInt(0)));
			break;
		case 1:
			CustomColor2 = (CustomColor1 = (CustomColor0 = Utilities.Vector4FromInt(colors[0].CompositeArgb)));
			break;
		case 3:
			CustomColor0 = Utilities.Vector4FromInt(colors[0].CompositeArgb);
			CustomColor1 = Utilities.Vector4FromInt(colors[1].CompositeArgb);
			CustomColor2 = Utilities.Vector4FromInt(colors[2].CompositeArgb);
			break;
		default:
			CustomColor2 = (CustomColor1 = (CustomColor0 = Utilities.Vector4FromInt(0)));
			throw new ArgumentException("Invalid length of the array. Must be one of 0, 1 or 3.");
		}
	}

	public ComponentColors(byte R, byte G, byte B, byte A)
	{
		Vector4 customColor = new Vector4((float)(int)R / 255f, (float)(int)G / 255f, (float)(int)B / 255f, (float)(int)A / 255f);
		CustomColor2 = (CustomColor1 = (CustomColor0 = customColor));
	}

	public Colorb[] ToColorArray()
	{
		return new Colorb[3]
		{
			Utilities.ColorbFromVector4(CustomColor0),
			Utilities.ColorbFromVector4(CustomColor1),
			Utilities.ColorbFromVector4(CustomColor2)
		};
	}
}
