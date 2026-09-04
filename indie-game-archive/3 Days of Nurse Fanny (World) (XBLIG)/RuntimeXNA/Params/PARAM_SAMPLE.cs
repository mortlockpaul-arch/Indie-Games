using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_SAMPLE : CParam
{
	public short sndHandle;

	public short sndFlags;

	public override void load(CRunApp app)
	{
		sndHandle = app.file.readAShort();
		sndFlags = app.file.readAShort();
	}
}
