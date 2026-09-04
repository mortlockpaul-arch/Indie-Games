using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

internal class CDefCounter : CDefObject
{
	public int ctInit;

	public int ctMini;

	public int ctMaxi;

	public override void load(CFile file)
	{
		file.skipBytes(2);
		ctInit = file.readAInt();
		ctMini = file.readAInt();
		ctMaxi = file.readAInt();
	}
}
