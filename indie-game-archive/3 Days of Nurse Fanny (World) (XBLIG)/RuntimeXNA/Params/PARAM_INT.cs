using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_INT : CParam
{
	public int value_Renamed;

	public int value2;

	public override void load(CRunApp app)
	{
		value_Renamed = app.file.readAInt();
		value2 = 0;
	}
}
