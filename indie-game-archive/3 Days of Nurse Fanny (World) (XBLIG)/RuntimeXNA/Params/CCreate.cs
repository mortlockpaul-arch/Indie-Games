using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public abstract class CCreate : CPosition
{
	public short cdpHFII;

	public short cdpOi;

	public CCreate()
	{
	}

	public abstract override void load(CRunApp app);
}
