using RuntimeXNA.Actions;
using RuntimeXNA.Application;
using RuntimeXNA.Conditions;

namespace RuntimeXNA.Events;

public class CEventGroup
{
	public const ushort EVGFLAGS_ONCE = 1;

	public const ushort EVGFLAGS_NOTALWAYS = 2;

	public const ushort EVGFLAGS_REPEAT = 4;

	public const ushort EVGFLAGS_NOMORE = 8;

	public const ushort EVGFLAGS_SHUFFLE = 16;

	public const ushort EVGFLAGS_EDITORMARK = 32;

	public const ushort EVGFLAGS_UNDOMARK = 64;

	public const ushort EVGFLAGS_COMPLEXGROUP = 128;

	public const ushort EVGFLAGS_BREAKPOINT = 256;

	public const ushort EVGFLAGS_ALWAYSCLEAN = 512;

	public const ushort EVGFLAGS_ORINGROUP = 1024;

	public const ushort EVGFLAGS_STOPINGROUP = 2048;

	public const ushort EVGFLAGS_ORLOGICAL = 4096;

	public const ushort EVGFLAGS_GROUPED = 8192;

	public const ushort EVGFLAGS_INACTIVE = 16384;

	public byte evgNCond;

	public byte evgNAct;

	public ushort evgFlags;

	public ushort evgInhibit;

	public short evgInhibitCpt;

	public ushort evgIdentifier;

	public CEvent[] evgEvents;

	public static ushort EVGFLAGS_NOGOOD = 32768;

	public static readonly ushort EVGFLAGS_LIMITED = 30;

	public static readonly ushort EVGFLAGS_DEFAULTMASK = 8448;

	public static CEventGroup create(CRunApp app)
	{
		int filePointer = app.file.getFilePointer();
		short num = app.file.readAShort();
		CEventGroup cEventGroup = new CEventGroup();
		cEventGroup.evgNCond = app.file.readByte();
		cEventGroup.evgNAct = app.file.readByte();
		cEventGroup.evgFlags = (ushort)app.file.readAShort();
		cEventGroup.evgInhibit = (ushort)app.file.readAShort();
		cEventGroup.evgInhibitCpt = app.file.readAShort();
		cEventGroup.evgIdentifier = (ushort)app.file.readAShort();
		app.file.skipBytes(2);
		cEventGroup.evgEvents = new CEvent[cEventGroup.evgNCond + cEventGroup.evgNAct];
		int num2 = 0;
		for (int i = 0; i < cEventGroup.evgNCond; i++)
		{
			cEventGroup.evgEvents[num2++] = CCnd.create(app);
		}
		for (int i = 0; i < cEventGroup.evgNAct; i++)
		{
			cEventGroup.evgEvents[num2++] = CAct.create(app);
		}
		app.file.seek(filePointer - num);
		return cEventGroup;
	}
}
