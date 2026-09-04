using RuntimeXNA.Animations;
using RuntimeXNA.Banks;
using RuntimeXNA.Frame;
using RuntimeXNA.Movements;
using RuntimeXNA.OI;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;
using RuntimeXNA.Values;

namespace RuntimeXNA.Objects;

public class CObject : IDrawing
{
	public const short HOF_DESTROYED = 1;

	public const short HOF_TRUEEVENT = 2;

	public const short HOF_REALSPRITE = 4;

	public const short HOF_FADEIN = 8;

	public const short HOF_FADEOUT = 16;

	public const short HOF_OWNERDRAW = 32;

	public const short HOF_NOCOLLISION = 8192;

	public const short HOF_FLOAT = 16384;

	public const short HOF_STRING = short.MinValue;

	public short hoNumber;

	public short hoNextSelected;

	public CRun hoAdRunHeader;

	public short hoHFII;

	public short hoOi;

	public short hoNumPrev;

	public short hoNumNext;

	public short hoType;

	public short hoCreationId;

	public CObjInfo hoOiList;

	public int hoEvents;

	public CArrayList hoPrevNoRepeat;

	public CArrayList hoBaseNoRepeat;

	public int hoMark1;

	public int hoMark2;

	public string hoMT_NodeName;

	public int hoEventNumber;

	public CObjectCommon hoCommon;

	public int hoCalculX;

	public int hoX;

	public int hoCalculY;

	public int hoY;

	public int hoImgXSpot;

	public int hoImgYSpot;

	public int hoImgWidth;

	public int hoImgHeight;

	public CRect hoRect = new CRect();

	public int hoOEFlags;

	public short hoFlags;

	public byte hoSelectedInOR;

	public int hoOffsetValue;

	public int hoLayer;

	public short hoLimitFlags;

	public short hoPreviousQuickDisplay;

	public short hoNextQuickDisplay;

	public int hoCurrentParam;

	public int hoIdentifier;

	public bool hoCallRoutine;

	public CRCom roc;

	public CRMvt rom;

	public CRAni roa;

	public CRVal rov;

	public CRSpr ros;

	public void setScale(float fScaleX, float fScaleY, bool bResample)
	{
		if (roc.rcScaleX != fScaleX || roc.rcScaleY != fScaleY)
		{
			roc.rcScaleX = fScaleX;
			roc.rcScaleY = fScaleY;
			roc.rcChanged = true;
			CImage imageInfoEx = hoAdRunHeader.rhApp.imageBank.getImageInfoEx(roc.rcImage, roc.rcAngle, roc.rcScaleX, roc.rcScaleY);
			hoImgWidth = imageInfoEx.width;
			hoImgHeight = imageInfoEx.height;
			hoImgXSpot = imageInfoEx.xSpot;
			hoImgYSpot = imageInfoEx.ySpot;
		}
	}

	public void shtCreate(PARAM_SHOOT p, int x, int y, int dir)
	{
		int num = hoLayer;
		int num2 = hoAdRunHeader.f_CreateObject(p.cdpHFII, p.cdpOi, x, y, dir, 3, num, -1);
		if (num2 < 0)
		{
			return;
		}
		CObject cObject = hoAdRunHeader.rhObjectList[num2];
		if (cObject.rom != null)
		{
			cObject.rom.initSimple(cObject, 13, bRestore: false);
			cObject.roc.rcDir = dir;
			cObject.roc.rcSpeed = p.shtSpeed;
			CMoveBullet cMoveBullet = (CMoveBullet)cObject.rom.rmMovement;
			cMoveBullet.init2(this);
			if (num != -1 && (cObject.hoOEFlags & 0x200) != 0)
			{
				CLayer cLayer = hoAdRunHeader.rhFrame.layers[num];
				if ((cLayer.dwOptions & 0x20010) != 16)
				{
					cObject.ros.obHide();
				}
			}
			hoAdRunHeader.rhEvtProg.evt_AddCurrentObject(cObject);
			if ((hoOEFlags & 0x20) != 0 && roa.anim_Exist(6))
			{
				roa.animation_Force(6);
				roa.animation_OneLoop();
			}
		}
		else
		{
			hoAdRunHeader.destroy_Add(cObject.hoNumber);
		}
	}

	public virtual void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
	}

	public virtual void handle()
	{
	}

	public virtual void modif()
	{
	}

	public virtual void display()
	{
	}

	public virtual void kill(bool bFast)
	{
	}

	public virtual void killBack()
	{
	}

	public virtual void getZoneInfos()
	{
	}

	public virtual void draw(SpriteBatchEffect batch)
	{
	}

	public virtual CMask getCollisionMask(int flags)
	{
		return null;
	}

	public virtual void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
	{
	}

	public virtual void drawableKill()
	{
	}

	public virtual CMask drawableGetMask(int flags)
	{
		return null;
	}
}
