using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

[StructLayout(LayoutKind.Explicit)]
public struct ShaderParameterData
{
	[FieldOffset(0)]
	public TextureInstance Texture;

	[FieldOffset(0)]
	public ShaderConstantValue Constant;

	[FieldOffset(0)]
	public ShaderConstantInt ConstantInt;
}
