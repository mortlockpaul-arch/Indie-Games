using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

public class COI
{
	public const short OILF_OCLOADED = 1;

	public const short OILF_ELTLOADED = 2;

	public const short OILF_TOLOAD = 4;

	public const short OILF_TODELETE = 8;

	public const short OILF_CURFRAME = 16;

	public const short OILF_TORELOAD = 32;

	public const short OILF_IGNORELOADONCALL = 64;

	public const short OIF_LOADONCALL = 1;

	public const short OIF_DISCARDABLE = 2;

	public const short OIF_GLOBAL = 4;

	public const short NUMBEROF_SYSTEMTYPES = 7;

	public const short OBJ_PLAYER = -7;

	public const short OBJ_KEYBOARD = -6;

	public const short OBJ_CREATE = -5;

	public const short OBJ_TIMER = -4;

	public const short OBJ_GAME = -3;

	public const short OBJ_SPEAKER = -2;

	public const short OBJ_SYSTEM = -1;

	public const short OBJ_BOX = 0;

	public const short OBJ_BKD = 1;

	public const short OBJ_SPR = 2;

	public const short OBJ_TEXT = 3;

	public const short OBJ_QUEST = 4;

	public const short OBJ_SCORE = 5;

	public const short OBJ_LIVES = 6;

	public const short OBJ_COUNTER = 7;

	public const short OBJ_RTF = 8;

	public const short OBJ_CCA = 9;

	public const short NB_SYSOBJ = 10;

	public const short OBJ_LAST = 10;

	public const int KPX_BASE = 32;

	public const ushort OIFLAG_QUALIFIER = 32768;

	public short oiHandle;

	public short oiType;

	public short oiFlags;

	public int oiInkEffect;

	public int oiInkEffectParam;

	public string oiName;

	public COC oiOC;

	public int oiFileOffset;

	public int oiLoadFlags;

	public short oiLoadCount;

	public short oiCount;

	public void loadHeader(CFile file)
	{
		oiHandle = file.readAShort();
		oiType = file.readAShort();
		oiFlags = file.readAShort();
		file.skipBytes(2);
		oiInkEffect = file.readAInt();
		oiInkEffectParam = file.readAInt();
	}

	public void load(CFile file, CRunApp app)
	{
		file.seek(oiFileOffset);
		switch (oiType)
		{
		case 0:
			oiOC = new COCQBackdrop(app);
			break;
		case 1:
			oiOC = new COCBackground();
			break;
		default:
			oiOC = new CObjectCommon();
			break;
		}
		oiOC.load(file, oiType);
		oiOC.oi = this;
		oiLoadFlags = 0;
	}

	public void unLoad()
	{
		oiOC = null;
	}

	public void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		oiOC.enumElements(enumImages, enumFonts);
	}
}
