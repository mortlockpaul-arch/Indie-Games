using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_STRING : CParam
{
	public string pString;

	public override void load(CRunApp app)
	{
		pString = app.file.readAString();
	}
}
