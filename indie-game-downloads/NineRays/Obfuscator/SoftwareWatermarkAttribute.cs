using System;

namespace NineRays.Obfuscator;

internal class SoftwareWatermarkAttribute : Attribute
{
	internal readonly string Watermark;

	internal SoftwareWatermarkAttribute(string P_0)
	{
		Watermark = P_0;
	}
}
