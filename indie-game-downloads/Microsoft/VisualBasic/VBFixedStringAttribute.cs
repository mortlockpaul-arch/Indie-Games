using System;

namespace Microsoft.VisualBasic;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class VBFixedStringAttribute : Attribute
{
	public int Length { get; }

	public VBFixedStringAttribute(int Length)
	{
		if (Length < 1 || Length > 32767)
		{
			throw new ArgumentException(System.SR.Invalid_VBFixedString);
		}
		this.Length = Length;
	}
}
