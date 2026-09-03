namespace OluXNA;

internal class SoundPart
{
	public string cueName;

	public Beats beatPlay;

	public SoundPart(string _cueName, Beats _beatPlay)
	{
		cueName = _cueName;
		beatPlay = _beatPlay;
	}

	public bool isEqual(SoundPart other)
	{
		if (cueName.Equals(other.cueName))
		{
			return beatPlay == other.beatPlay;
		}
		return false;
	}
}
