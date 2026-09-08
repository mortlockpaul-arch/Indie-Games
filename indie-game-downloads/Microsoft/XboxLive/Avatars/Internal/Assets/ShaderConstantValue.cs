using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public struct ShaderConstantValue
{
	public float f0;

	public float f1;

	public float f2;

	public float f3;

	public void SetValue(Vector4 value)
	{
		f0 = value.X;
		f1 = value.Y;
		f2 = value.Z;
		f3 = value.W;
	}
}
