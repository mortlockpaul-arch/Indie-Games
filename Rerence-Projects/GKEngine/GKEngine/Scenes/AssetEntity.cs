namespace GKEngine.Scenes;

public struct AssetEntity(string xName, string xPath, AssetType xType)
{
	public string name = xName;

	public string path = xPath;

	public AssetType type = xType;

	public int Type
	{
		get
		{
			return (int)type;
		}
		set
		{
			type = (AssetType)value;
		}
	}
}
