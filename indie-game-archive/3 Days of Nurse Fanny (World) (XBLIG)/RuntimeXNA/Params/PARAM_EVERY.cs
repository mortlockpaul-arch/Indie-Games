using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_EVERY : CParam
{
	public int delay;

	public int compteur;

	public override void load(CRunApp app)
	{
		delay = app.file.readAInt();
		compteur = app.file.readAInt();
	}
}
