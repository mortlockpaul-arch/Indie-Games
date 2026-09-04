using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_EFFECT : CParam
{
	public string pEffect;

	public override void load(CRunApp app)
	{
		pEffect = app.file.readAString();
	}
}
