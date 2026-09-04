using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

internal class CPathStep
{
	public byte mdSpeed;

	public byte mdDir;

	public short mdDx;

	public short mdDy;

	public short mdCosinus;

	public short mdSinus;

	public short mdLength;

	public short mdPause;

	public string mdName;

	public void load(CFile file)
	{
		mdSpeed = file.readByte();
		mdDir = file.readByte();
		mdDx = file.readAShort();
		mdDy = file.readAShort();
		mdCosinus = file.readAShort();
		mdSinus = file.readAShort();
		mdLength = file.readAShort();
		mdPause = file.readAShort();
		string text = file.readAString();
		if (text.Length > 0)
		{
			mdName = text;
		}
	}
}
