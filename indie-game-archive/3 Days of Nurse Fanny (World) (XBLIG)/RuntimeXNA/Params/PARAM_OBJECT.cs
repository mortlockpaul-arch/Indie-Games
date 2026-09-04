using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_OBJECT : CParam
{
	public short oiList;

	public short oi;

	public short type;

	public override void load(CRunApp app)
	{
		oiList = app.file.readAShort();
		oi = app.file.readAShort();
		type = app.file.readAShort();
	}
}
