using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.MathUtilities;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct IndexedTriangle(int vertexIndex1, int vertexIndex2, int vertexIndex3)
{
	[FieldOffset(0)]
	public int i1 = vertexIndex1;

	[FieldOffset(4)]
	public int i2 = vertexIndex2;

	[FieldOffset(8)]
	public int i3 = vertexIndex3;

	public override int GetHashCode()
	{
		return i1 ^ i2 ^ i3;
	}

	public bool Equals(IndexedTriangle other)
	{
		if (i1 != other.i1)
		{
			return false;
		}
		if (i2 != other.i2)
		{
			return false;
		}
		if (i3 != other.i3)
		{
			return false;
		}
		return true;
	}
}
