namespace Game.Data;

public class DataLevelHeader
{
	public string name;

	public string author;

	public uint index;

	public uint type;

	public bool edit;

	public bool passed;

	public int group = -1;

	public DataLevelHeader()
	{
	}

	public DataLevelHeader(string xName, string xAuthor, uint xIndex, uint xType, bool xEdit, bool xPassed, int xGroup)
	{
		name = xName;
		author = xAuthor;
		index = xIndex;
		type = xType;
		edit = xEdit;
		passed = xPassed;
		group = xGroup;
	}
}
