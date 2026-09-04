using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Movements;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Params;

public abstract class CPosition : CParam
{
	public const short CPF_DIRECTION = 1;

	public const short CPF_ACTION = 2;

	public const short CPF_INITIALDIR = 4;

	public const short CPF_DEFAULTDIR = 8;

	public short posOINUMParent;

	public short posFlags;

	public short posX;

	public short posY;

	public short posSlope;

	public short posAngle;

	public int posDir;

	public short posTypeParent;

	public short posOiList;

	public short posLayer;

	public CPosition()
	{
	}

	public virtual bool read_Position(CRun rhPtr, int getDir, CPositionInfo pInfo)
	{
		pInfo.layer = -1;
		if (posOINUMParent == -1)
		{
			if (getDir != 0)
			{
				pInfo.dir = -1;
				if ((posFlags & 8) == 0)
				{
					pInfo.dir = rhPtr.get_Direction(posDir);
				}
			}
			pInfo.x = posX;
			pInfo.y = posY;
			int num = posLayer;
			if (num > rhPtr.rhFrame.nLayers - 1)
			{
				num = rhPtr.rhFrame.nLayers - 1;
			}
			pInfo.layer = num;
			pInfo.bRepeat = false;
		}
		else
		{
			rhPtr.rhEvtProg.rh2EnablePick = false;
			CObject cObject = rhPtr.rhEvtProg.get_CurrentObjects(posOiList);
			pInfo.bRepeat = rhPtr.rhEvtProg.repeatFlag;
			if (cObject == null)
			{
				return false;
			}
			pInfo.x = cObject.hoX;
			pInfo.y = cObject.hoY;
			pInfo.layer = cObject.hoLayer;
			if ((posFlags & 2) != 0 && (cObject.hoOEFlags & 0x20) != 0 && cObject.roc.rcImage != 0)
			{
				CImage imageInfoEx = rhPtr.rhApp.imageBank.getImageInfoEx(cObject.roc.rcImage, cObject.roc.rcAngle, cObject.roc.rcScaleX, cObject.roc.rcScaleY);
				pInfo.x += imageInfoEx.xAP - imageInfoEx.xSpot;
				pInfo.y += imageInfoEx.yAP - imageInfoEx.ySpot;
			}
			if ((posFlags & 1) != 0)
			{
				int angle = (posAngle + cObject.roc.rcDir) & 0x1F;
				int deltaX = CMove.getDeltaX(posSlope, angle);
				int deltaY = CMove.getDeltaY(posSlope, angle);
				pInfo.x += deltaX;
				pInfo.y += deltaY;
			}
			else
			{
				pInfo.x += posX;
				pInfo.y += posY;
			}
			if ((getDir & 1) != 0)
			{
				if ((posFlags & 8) != 0)
				{
					pInfo.dir = -1;
				}
				else if ((posFlags & 4) != 0)
				{
					pInfo.dir = cObject.roc.rcDir;
				}
				else
				{
					pInfo.dir = rhPtr.get_Direction(posDir);
				}
			}
		}
		if ((getDir & 2) != 0)
		{
			if (pInfo.x < rhPtr.rh3XMinimumKill || pInfo.x > rhPtr.rh3XMaximumKill)
			{
				return false;
			}
			if (pInfo.y < rhPtr.rh3YMinimumKill || pInfo.y > rhPtr.rh3YMaximumKill)
			{
				return false;
			}
		}
		return true;
	}

	public abstract override void load(CRunApp app);
}
