using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.MathUtilities;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct IndexedLine(int vertexIndex1, int vertexIndex2)
{
	[FieldOffset(0)]
	public int i1 = vertexIndex1;

	[FieldOffset(4)]
	public int i2 = vertexIndex2;

	public override int GetHashCode()
	{
		return i1 ^ i2;
	}

	public bool Equals(IndexedLine other)
	{
		if (i1 != other.i1)
		{
			return false;
		}
		if (i2 != other.i2)
		{
			return false;
		}
		return true;
	}
}
