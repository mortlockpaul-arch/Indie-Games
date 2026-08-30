namespace Platformer.Asset_Classes;

internal class AssetDefinition
{
	public readonly string textureName;

	public readonly string typeSymbol;

	public readonly int defID;

	public readonly int frameCount;

	public AssetDefinition(int defID, string type, string textureName, int fc)
	{
		typeSymbol = type;
		this.textureName = textureName;
		this.defID = defID;
		frameCount = fc;
	}
}
