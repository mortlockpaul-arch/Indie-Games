using RuntimeXNA.Services;

namespace RuntimeXNA.Values;

public class CDefStrings
{
	public short nStrings;

	public string[] strings;

	public void load(CFile file)
	{
		nStrings = file.readAShort();
		strings = new string[nStrings];
		for (int i = 0; i < nStrings; i++)
		{
			strings[i] = file.readAString();
		}
	}
}
