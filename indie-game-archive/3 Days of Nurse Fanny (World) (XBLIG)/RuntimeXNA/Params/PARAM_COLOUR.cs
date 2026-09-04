using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_COLOUR : CParam
{
	public int color;

	public override void load(CRunApp app)
	{
		color = app.file.readAColor();
	}
}
