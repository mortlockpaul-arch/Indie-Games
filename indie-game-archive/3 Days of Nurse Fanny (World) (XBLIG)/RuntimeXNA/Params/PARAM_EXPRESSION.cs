using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_EXPRESSION : CParamExpression
{
	public override void load(CRunApp app)
	{
		comparaison = app.file.readAShort();
		load(app.file);
	}
}
