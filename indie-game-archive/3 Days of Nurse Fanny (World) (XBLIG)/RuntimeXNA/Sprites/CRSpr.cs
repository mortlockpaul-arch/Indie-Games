using RuntimeXNA.Frame;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Sprites;

public class CRSpr
{
	public const short RSFLAG_HIDDEN = 1;

	public const short RSFLAG_INACTIVE = 2;

	public const short RSFLAG_SLEEPING = 4;

	public const short RSFLAG_SCALE_RESAMPLE = 8;

	public const short RSFLAG_ROTATE_ANTIA = 16;

	public const short RSFLAG_VISIBLE = 32;

	public const short SPRTYPE_TRUESPRITE = 0;

	public const short SPRTYPE_OWNERDRAW = 1;

	public const short SPRTYPE_QUICKDISPLAY = 2;

	public CObject hoPtr;

	public CSpriteGen spriteGen;

	public int rsFlash;

	public int rsFlashCpt;

	public short rsLayer;

	public int rsZOrder;

	public uint rsCreaFlags;

	public int rsBackColor;

	public int rsEffect;

	public int rsEffectParam;

	public short rsFlags;

	public uint rsFadeCreaFlags;

	public short rsSpriteType;

	public long startFade;

	public void init1(CObject ho, CObjectCommon ocPtr, CCreateObjectInfo cobPtr)
	{
		hoPtr = ho;
		spriteGen = ho.hoAdRunHeader.rhApp.spriteGen;
		rsLayer = (short)cobPtr.cobLayer;
		rsZOrder = cobPtr.cobZOrder;
		rsCreaFlags = 1u;
		if ((hoPtr.hoLimitFlags & 0x100) == 0)
		{
			rsCreaFlags &= 4294967294u;
		}
		rsBackColor = 0;
		if ((hoPtr.hoOEFlags & 4) == 0 || (hoPtr.hoOiList.oilOCFlags2 & 1) != 0)
		{
			hoPtr.hoOEFlags &= -5;
			rsCreaFlags |= 512u;
			if ((hoPtr.hoOiList.oilOCFlags2 & 2) != 0)
			{
				rsBackColor = hoPtr.hoOiList.oilBackColor;
				rsCreaFlags |= 1024u;
			}
		}
		if ((hoPtr.hoOEFlags & 0x400) != 0)
		{
			rsCreaFlags |= 16384u;
		}
		if ((hoPtr.hoOiList.oilOCFlags2 & 4) != 0)
		{
			rsCreaFlags |= 256u;
		}
		if ((cobPtr.cobFlags & 2) != 0)
		{
			rsCreaFlags |= 128u;
			rsFlags = 1;
			if (hoPtr.hoType == 3)
			{
				hoPtr.hoFlags |= 8192;
			}
		}
		else
		{
			rsFlags |= 32;
		}
		rsEffect = hoPtr.hoOiList.oilInkEffect;
		rsEffectParam = hoPtr.hoOiList.oilEffectParam;
		if (hoPtr.roc.rcMovementType == 0)
		{
			rsFlags |= 2;
			rsCreaFlags |= 8u;
		}
		rsFadeCreaFlags = (ushort)rsCreaFlags;
	}

	public void init2(bool bTransition)
	{
		createSprite(null, bTransition);
	}

	public void displayRoutine()
	{
		switch (rsSpriteType)
		{
		case 0:
			if (hoPtr.roc.rcSprite != null)
			{
				spriteGen.modifSpriteEx(hoPtr.roc.rcSprite, hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX, hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY, hoPtr.roc.rcImage, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, (hoPtr.ros.rsFlags & 8) != 0, hoPtr.roc.rcAngle, (hoPtr.ros.rsFlags & 0x10) != 0);
			}
			break;
		case 1:
			if (hoPtr.roc.rcSprite != null)
			{
				spriteGen.activeSprite(hoPtr.roc.rcSprite, 1, null);
			}
			break;
		case 2:
			break;
		}
	}

	public void handle()
	{
		CRun hoAdRunHeader = hoPtr.hoAdRunHeader;
		if ((rsFlags & 4) == 0)
		{
			if ((hoPtr.hoFlags & 8) != 0)
			{
				performFadeIn();
				return;
			}
			if ((hoPtr.hoFlags & 0x10) != 0)
			{
				performFadeOut();
				return;
			}
			if (rsFlash != 0)
			{
				rsFlashCpt -= hoAdRunHeader.rhTimerDelta;
				if (rsFlashCpt < 0)
				{
					rsFlashCpt = rsFlash;
					if ((rsFlags & 0x20) == 0)
					{
						rsFlags |= 32;
						obShow();
					}
					else
					{
						rsFlags &= -33;
						obHide();
					}
				}
			}
			if (hoPtr.rom != null)
			{
				hoPtr.rom.move();
			}
			if (hoPtr.roc.rcPlayer != 0 || (hoPtr.hoOEFlags & 0x4000) != 0)
			{
				return;
			}
			int num = hoPtr.hoX - hoPtr.hoImgXSpot;
			int num2 = hoPtr.hoY - hoPtr.hoImgYSpot;
			int num3 = num + hoPtr.hoImgWidth;
			int num4 = num2 + hoPtr.hoImgHeight;
			if (num3 >= hoAdRunHeader.rh3XMinimum && num <= hoAdRunHeader.rh3XMaximum && num4 >= hoAdRunHeader.rh3YMinimum && num2 <= hoAdRunHeader.rh3YMaximum)
			{
				return;
			}
			if (num3 >= hoAdRunHeader.rh3XMinimumKill && num <= hoAdRunHeader.rh3XMaximumKill && num4 >= hoAdRunHeader.rh3YMinimumKill && num2 <= hoAdRunHeader.rh3YMaximumKill)
			{
				rsFlags |= 4;
				if (hoPtr.roc.rcSprite != null)
				{
					rsZOrder = hoPtr.roc.rcSprite.sprZOrder;
					hoPtr.hoAdRunHeader.rhApp.spriteGen.delSpriteFast(hoPtr.roc.rcSprite);
					hoPtr.roc.rcSprite = null;
				}
				else
				{
					hoPtr.killBack();
				}
			}
			else if ((hoPtr.hoOEFlags & 0x2000) == 0)
			{
				hoAdRunHeader.destroy_Add(hoPtr.hoNumber);
			}
		}
		else
		{
			int num5 = hoPtr.hoX - hoPtr.hoImgXSpot;
			int num6 = hoPtr.hoY - hoPtr.hoImgYSpot;
			int num7 = num5 + hoPtr.hoImgWidth;
			int num8 = num6 + hoPtr.hoImgHeight;
			if (num7 >= hoAdRunHeader.rh3XMinimum && num5 <= hoAdRunHeader.rh3XMaximum && num8 >= hoAdRunHeader.rh3YMinimum && num6 <= hoAdRunHeader.rh3YMaximum)
			{
				rsFlags &= -5;
				init2(bTransition: false);
			}
		}
	}

	public void modifRoutine()
	{
		switch (rsSpriteType)
		{
		case 0:
			if (hoPtr.roc.rcSprite != null)
			{
				spriteGen.modifSpriteEx(hoPtr.roc.rcSprite, hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX, hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY, hoPtr.roc.rcImage, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, (hoPtr.ros.rsFlags & 8) != 0, hoPtr.roc.rcAngle, (hoPtr.ros.rsFlags & 0x10) != 0);
			}
			break;
		case 1:
			objGetZoneInfos();
			if (hoPtr.roc.rcSprite != null)
			{
				spriteGen.modifOwnerDrawSprite(hoPtr.roc.rcSprite, hoPtr.hoRect.left, hoPtr.hoRect.top, hoPtr.hoRect.right, hoPtr.hoRect.bottom);
			}
			break;
		case 2:
			objGetZoneInfos();
			break;
		}
	}

	public bool createSprite(CSprite pSprBefore, bool bTransition)
	{
		if ((hoPtr.hoOEFlags & 0x20) != 0)
		{
			CSprite cSprite = spriteGen.addSprite(hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX, hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY, hoPtr.roc.rcImage, rsLayer, rsZOrder, rsBackColor, rsCreaFlags | 0x20000000, hoPtr);
			if (cSprite != null)
			{
				hoPtr.roc.rcSprite = cSprite;
				hoPtr.hoFlags |= 4;
				spriteGen.modifSpriteEffect(cSprite, rsEffect, rsEffectParam);
				if (pSprBefore != null)
				{
					spriteGen.moveSpriteBefore(cSprite, pSprBefore);
				}
				rsSpriteType = 0;
				if (bTransition && hoPtr.hoCommon.ocFadeInLength != 0)
				{
					hoPtr.hoFlags |= 8;
					spriteGen.modifSpriteEffect(cSprite, 1, 128);
					hoPtr.hoFlags |= 8192;
					cSprite.setSpriteColFlag(0u);
					startFade = hoPtr.hoAdRunHeader.rhTimer;
				}
			}
			return true;
		}
		if ((hoPtr.hoOEFlags & 0x1000) == 0 || ((hoPtr.hoOEFlags & 0x1000) != 0 && rsLayer != 0))
		{
			rsCreaFlags |= 8200u;
			if ((rsCreaFlags & 0x100) == 0)
			{
				rsCreaFlags |= 8388608u;
			}
			rsFlags |= 2;
			hoPtr.hoFlags |= 32;
			hoPtr.hoRect.left = hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX - hoPtr.hoImgXSpot;
			hoPtr.hoRect.top = hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY - hoPtr.hoImgYSpot;
			hoPtr.hoRect.right = hoPtr.hoRect.left + hoPtr.hoImgWidth;
			hoPtr.hoRect.bottom = hoPtr.hoRect.top + hoPtr.hoImgHeight;
			CSprite cSprite2 = spriteGen.addOwnerDrawSprite(hoPtr.hoRect.left, hoPtr.hoRect.top, hoPtr.hoRect.right, hoPtr.hoRect.bottom, rsLayer, rsZOrder, rsBackColor, rsCreaFlags, hoPtr, hoPtr);
			if (cSprite2 == null)
			{
				return false;
			}
			hoPtr.roc.rcSprite = cSprite2;
			if (pSprBefore != null)
			{
				spriteGen.moveSpriteBefore(cSprite2, pSprBefore);
			}
			rsSpriteType = 1;
			return true;
		}
		hoPtr.hoAdRunHeader.add_QuickDisplay(hoPtr);
		rsSpriteType = 2;
		return true;
	}

	public void performFadeIn()
	{
		long num = hoPtr.hoAdRunHeader.rhTimer - startFade;
		if (num >= hoPtr.hoCommon.ocFadeInLength)
		{
			spriteGen.modifSpriteEffect(hoPtr.roc.rcSprite, 1, rsEffectParam);
			hoPtr.hoFlags &= -9;
			hoPtr.hoFlags &= -8193;
			hoPtr.roc.rcSprite.setSpriteColFlag(rsCreaFlags & 1);
		}
		else
		{
			int effectParam = (int)(128.0 - (double)(128 - rsEffectParam) * (double)num / (double)hoPtr.hoCommon.ocFadeInLength);
			spriteGen.modifSpriteEffect(hoPtr.roc.rcSprite, 1, effectParam);
		}
	}

	public bool initFadeOut()
	{
		if (hoPtr.hoCommon.ocFadeOutLength != 0 && hoPtr.roc.rcSprite != null)
		{
			hoPtr.hoFlags |= 16;
			hoPtr.hoFlags |= 8192;
			hoPtr.roc.rcSprite.setSpriteColFlag(0u);
			startFade = hoPtr.hoAdRunHeader.rhTimer;
			return true;
		}
		return false;
	}

	public void performFadeOut()
	{
		long num = hoPtr.hoAdRunHeader.rhTimer - startFade;
		if (num >= hoPtr.hoCommon.ocFadeOutLength)
		{
			spriteGen.modifSpriteEffect(hoPtr.roc.rcSprite, 1, 128);
			hoPtr.hoCallRoutine = false;
			hoPtr.hoAdRunHeader.destroy_Add(hoPtr.hoNumber);
		}
		else
		{
			int effectParam = (int)((double)rsEffectParam + (double)num / (double)hoPtr.hoCommon.ocFadeOutLength * (double)(128 - rsEffectParam));
			spriteGen.modifSpriteEffect(hoPtr.roc.rcSprite, 1, effectParam);
		}
	}

	public bool kill(bool fast)
	{
		bool result = false;
		if (hoPtr.roc.rcSprite != null)
		{
			rsZOrder = hoPtr.roc.rcSprite.sprZOrder;
			if (!fast)
			{
				result = (hoPtr.roc.rcSprite.sprFlags & 0x2000) != 0;
				spriteGen.delSprite(hoPtr.roc.rcSprite);
			}
			else
			{
				spriteGen.delSpriteFast(hoPtr.roc.rcSprite);
			}
			hoPtr.roc.rcSprite = null;
		}
		else if ((hoPtr.hoOEFlags & 0x1000) != 0)
		{
			hoPtr.hoAdRunHeader.remove_QuickDisplay(hoPtr);
		}
		return result;
	}

	public void objGetZoneInfos()
	{
		hoPtr.getZoneInfos();
		hoPtr.hoRect.left = hoPtr.hoX - hoPtr.hoAdRunHeader.rhWindowX - hoPtr.hoImgXSpot;
		hoPtr.hoRect.right = hoPtr.hoRect.left + hoPtr.hoImgWidth;
		hoPtr.hoRect.top = hoPtr.hoY - hoPtr.hoAdRunHeader.rhWindowY - hoPtr.hoImgYSpot;
		hoPtr.hoRect.bottom = hoPtr.hoRect.top + hoPtr.hoImgHeight;
	}

	public void obHide()
	{
		if ((rsFlags & 1) == 0)
		{
			rsFlags |= 1;
			rsCreaFlags |= 128u;
			rsFadeCreaFlags |= 128u;
			hoPtr.roc.rcChanged = true;
			if (hoPtr.roc.rcSprite != null)
			{
				spriteGen.showSprite(hoPtr.roc.rcSprite, showFlag: false);
			}
		}
	}

	public void obShow()
	{
		if ((rsFlags & 1) == 0)
		{
			return;
		}
		CLayer cLayer = hoPtr.hoAdRunHeader.rhFrame.layers[hoPtr.hoLayer];
		if ((cLayer.dwOptions & 0x20010) == 16)
		{
			rsCreaFlags &= 4294967167u;
			rsFadeCreaFlags &= 4294967167u;
			rsFlags &= -2;
			hoPtr.hoFlags &= -8193;
			hoPtr.roc.rcChanged = true;
			if (hoPtr.roc.rcSprite != null)
			{
				hoPtr.hoAdRunHeader.rhApp.spriteGen.showSprite(hoPtr.roc.rcSprite, showFlag: true);
			}
		}
	}

	public void modifSpriteEffect(int effect, int effectParam)
	{
		rsEffect &= -4096;
		rsEffect |= effect;
		rsEffectParam = effectParam;
		hoPtr.roc.rcChanged = true;
		if (hoPtr.roc.rcSprite != null)
		{
			spriteGen.modifSpriteEffect(hoPtr.roc.rcSprite, rsEffect, rsEffectParam);
		}
	}
}
