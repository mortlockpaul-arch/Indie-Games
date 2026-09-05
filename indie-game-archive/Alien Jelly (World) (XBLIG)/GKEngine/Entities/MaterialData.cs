namespace GKEngine.Entities;

public class MaterialData
{
	public string part;

	public int index;

	public string param;

	public string value;

	public MaterialData()
	{
	}

	public MaterialData(string oPart, int xIndex, string xParam, string xValue)
	{
		part = oPart;
		index = xIndex;
		param = xParam;
		value = xValue;
	}
}
