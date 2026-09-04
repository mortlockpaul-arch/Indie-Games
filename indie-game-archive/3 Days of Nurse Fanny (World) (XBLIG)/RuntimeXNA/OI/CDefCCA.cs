using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

internal class CDefCCA : CDefObject
{
	public int odCx;

	public int odCy;

	public short odVersion;

	public short odNStartFrame;

	public int odOptions;

	public string odName;

	public override void load(CFile file)
	{
		file.skipBytes(4);
		odCx = file.readAInt();
		odCy = file.readAInt();
		odVersion = file.readAShort();
		odNStartFrame = file.readAShort();
		odOptions = file.readAInt();
		file.skipBytes(8);
		odName = file.readAString();
	}
}
