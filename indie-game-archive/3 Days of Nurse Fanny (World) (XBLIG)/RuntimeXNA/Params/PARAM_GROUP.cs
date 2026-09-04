using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_GROUP : CParam
{
	public const short GRPFLAGS_INACTIVE = 1;

	public const short GRPFLAGS_CLOSED = 2;

	public const short GRPFLAGS_PARENTINACTIVE = 4;

	public const short GRPFLAGS_GROUPINACTIVE = 8;

	public const short GRPFLAGS_GLOBAL = 16;

	public short grpFlags;

	public short grpId;

	public override void load(CRunApp app)
	{
		grpFlags = app.file.readAShort();
		grpId = app.file.readAShort();
	}
}
