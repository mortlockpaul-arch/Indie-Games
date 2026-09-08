namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public struct ShaderInstance
{
	public ShaderId ShaderId;

	public ShaderParameter[] ShaderParameters;

	public ShaderInstance Clone()
	{
		ShaderInstance result = new ShaderInstance
		{
			ShaderId = ShaderId,
			ShaderParameters = new ShaderParameter[ShaderParameters.Length]
		};
		ShaderParameters.CopyTo(result.ShaderParameters, 0);
		return result;
	}
}
