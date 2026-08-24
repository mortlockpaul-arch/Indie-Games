using System.Collections.Generic;

namespace Game.Data;

public class DataPlayer
{
	public DataSettings settings;

	public List<DataLevelHeader> levels;

	public List<DataProgression> progression;

	public DataPlayer()
	{
	}

	public DataPlayer(DataSettings oSettings, List<DataLevelHeader> aLevels, List<DataProgression> aProgression)
	{
		settings = oSettings;
		levels = aLevels;
		progression = aProgression;
	}
}
