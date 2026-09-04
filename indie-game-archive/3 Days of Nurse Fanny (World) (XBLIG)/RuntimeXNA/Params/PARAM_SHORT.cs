using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_SHORT : CParam
{
	public short value;

	public override void load(CRunApp app)
	{
		value = app.file.readAShort();
	}
}
