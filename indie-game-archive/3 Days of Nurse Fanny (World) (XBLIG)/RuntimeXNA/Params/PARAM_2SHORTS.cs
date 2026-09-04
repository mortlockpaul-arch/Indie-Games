using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_2SHORTS : CParam
{
	public short value1;

	public short value2;

	public override void load(CRunApp app)
	{
		value1 = app.file.readAShort();
		value2 = app.file.readAShort();
	}
}
