namespace Microsoft.XboxLive.MathUtilities;

public struct Vector4b(int packedVector)
{
	public byte X = (byte)(packedVector >> 24);

	public byte Y = (byte)(packedVector >> 16);

	public byte Z = (byte)(packedVector >> 8);

	public byte W = (byte)packedVector;
}
