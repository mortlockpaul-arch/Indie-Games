using RuntimeXNA.Params;

namespace RuntimeXNA.Events;

public abstract class CEvent
{
	public const byte EVFLAGS_REPEAT = 1;

	public const byte EVFLAGS_DONE = 2;

	public const byte EVFLAGS_DEFAULT = 4;

	public const byte EVFLAGS_DONEBEFOREFADEIN = 8;

	public const byte EVFLAGS_NOTDONEINSTART = 16;

	public const byte EVFLAGS_ALWAYS = 32;

	public const byte EVFLAGS_BAD = 64;

	public const byte EVFLAG2_NOT = 1;

	public int evtCode;

	public short evtOi;

	public short evtOiList;

	public byte evtFlags;

	public byte evtFlags2;

	public byte evtDefType;

	public byte evtNParams;

	public CParam[] evtParams;

	public static byte EVFLAGS_BADOBJECT = 128;

	public static readonly byte EVFLAGS_DEFAULTMASK = 61;

	public CEvent()
	{
	}
}
