using System;

namespace Microsoft.VisualBasic;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class VBFixedArrayAttribute : Attribute
{
	internal int FirstBound;

	internal int SecondBound;

	public int[] Bounds
	{
		get
		{
			if (SecondBound == -1)
			{
				return new int[1] { FirstBound };
			}
			return new int[2] { FirstBound, SecondBound };
		}
	}

	public int Length
	{
		get
		{
			checked
			{
				if (SecondBound == -1)
				{
					return FirstBound + 1;
				}
				return (FirstBound + 1) * (SecondBound + 1);
			}
		}
	}

	public VBFixedArrayAttribute(int UpperBound1)
	{
		if (UpperBound1 < 0)
		{
			throw new ArgumentException(System.SR.Invalid_VBFixedArray);
		}
		FirstBound = UpperBound1;
		SecondBound = -1;
	}

	public VBFixedArrayAttribute(int UpperBound1, int UpperBound2)
	{
		if (UpperBound1 < 0 || UpperBound2 < 0)
		{
			throw new ArgumentException(System.SR.Invalid_VBFixedArray);
		}
		FirstBound = UpperBound1;
		SecondBound = UpperBound2;
	}
}
