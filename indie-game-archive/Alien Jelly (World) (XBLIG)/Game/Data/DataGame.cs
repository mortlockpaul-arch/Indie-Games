using System.Collections.Generic;

namespace Game.Data;

public class DataGame
{
	public List<DataLevelGroup> groups;

	public List<DataLevelHeader> levels;

	public List<DataSky> skys;

	public DataGame()
	{
	}

	public DataGame(List<DataSky> aSkys, List<DataLevelHeader> aLevels)
	{
		skys = aSkys;
		levels = aLevels;
	}
}
