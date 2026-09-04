using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_EXTENSION : CParam
{
	public byte[] data;

	public override void load(CRunApp app)
	{
		short num = app.file.readAShort();
		app.file.skipBytes(4);
		if (num > 6)
		{
			data = new byte[num - 6];
			app.file.read(data);
		}
	}
}
