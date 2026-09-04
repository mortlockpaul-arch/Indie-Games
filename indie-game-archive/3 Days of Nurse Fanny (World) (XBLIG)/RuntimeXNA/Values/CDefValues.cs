using RuntimeXNA.Services;

namespace RuntimeXNA.Values;

public class CDefValues
{
	public short nValues;

	public int[] values;

	public void load(CFile file)
	{
		nValues = file.readAShort();
		values = new int[nValues];
		for (int i = 0; i < nValues; i++)
		{
			values[i] = file.readAInt();
		}
	}
}
