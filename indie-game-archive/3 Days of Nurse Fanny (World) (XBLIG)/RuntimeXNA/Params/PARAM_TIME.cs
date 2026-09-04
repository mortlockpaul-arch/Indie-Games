using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_TIME : CParam
{
	public int timer;

	public int loops;

	public override void load(CRunApp app)
	{
		timer = app.file.readAInt();
		loops = app.file.readAInt();
	}
}
