using RuntimeXNA.OI;

namespace RuntimeXNA.RunLoop;

public class CObjInfo
{
	public const short OILIMITFLAGS_BORDERS = 15;

	public const short OILIMITFLAGS_BACKDROPS = 16;

	public const short OILIMITFLAGS_ONCOLLIDE = 128;

	public const short OILIMITFLAGS_QUICKCOL = 256;

	public const short OILIMITFLAGS_QUICKBACK = 512;

	public const short OILIMITFLAGS_QUICKBORDER = 1024;

	public const short OILIMITFLAGS_QUICKSPR = 2048;

	public const short OILIMITFLAGS_QUICKEXT = 4096;

	public const short OILIMITFLAGS_ALL = -1;

	public short oilOi;

	public short oilListSelected;

	public short oilType;

	public short oilObject;

	public int oilEvents;

	public byte oilWrap;

	public bool oilNextFlag;

	public int oilNObjects;

	public int oilActionCount;

	public int oilActionLoopCount;

	public int oilCurrentRoutine;

	public int oilCurrentOi;

	public int oilNext;

	public int oilEventCount;

	public int oilNumOfSelected;

	public int oilOEFlags;

	public short oilLimitFlags;

	public int oilLimitList;

	public short oilOIFlags;

	public short oilOCFlags2;

	public int oilInkEffect;

	public int oilEffectParam;

	public short oilHFII;

	public int oilBackColor;

	public short[] oilQualifiers = new short[8];

	public string oilName;

	public int oilEventCountOR;

	public short[] oilColList;

	public int oilColCount;

	public void copyData(COI oiPtr)
	{
		oilOi = oiPtr.oiHandle;
		oilType = oiPtr.oiType;
		oilOIFlags = oiPtr.oiFlags;
		CObjectCommon cObjectCommon = (CObjectCommon)oiPtr.oiOC;
		oilOCFlags2 = cObjectCommon.ocFlags2;
		oilInkEffect = oiPtr.oiInkEffect;
		oilEffectParam = oiPtr.oiInkEffectParam;
		oilOEFlags = cObjectCommon.ocOEFlags;
		oilBackColor = cObjectCommon.ocBackColor;
		oilEventCount = 0;
		oilObject = -1;
		oilLimitFlags = -1;
		if (oiPtr.oiName != null)
		{
			oilName = oiPtr.oiName;
		}
		for (int i = 0; i < 8; i++)
		{
			oilQualifiers[i] = cObjectCommon.ocQualifiers[i];
		}
	}
}
