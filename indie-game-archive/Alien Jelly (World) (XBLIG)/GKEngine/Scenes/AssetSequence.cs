namespace GKEngine.Scenes;

public struct AssetSequence
{
	public string name;

	public string path;

	public AssetType type;

	public int start;

	public int end;

	public int digits;

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
