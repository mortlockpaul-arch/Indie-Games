using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Actions;
using RuntimeXNA.Banks;
using RuntimeXNA.Conditions;
using RuntimeXNA.Expressions;
using RuntimeXNA.Extensions;
using RuntimeXNA.Movements;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

public class CExtension : CObject, IDrawing
{
	public CRunExtension ext;

	private bool noHandle;

	public int privateData;

	public int objectCount;

	public int objectNumber;

	public CExtension(int type, CRun rhPtr)
	{
		ext = rhPtr.rhApp.extLoader.loadRunObject(type);
	}

	public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		ext.init(this);
		CFile cFile = null;
		if (ocPtr.ocExtension != null)
		{
			cFile = new CFile(ocPtr.ocExtension);
			cFile.setUnicode(hoAdRunHeader.rhApp.bUnicode);
		}
		privateData = ocPtr.ocPrivate;
		ext.createRunObject(cFile, cob, ocPtr.ocVersion);
	}

	public override void handle()
	{
		if ((hoOEFlags & 0x200) != 0)
		{
			ros.handle();
		}
		else if ((hoOEFlags & 0x30) == 16 || (hoOEFlags & 0x30) == 48)
		{
			rom.move();
		}
		else if ((hoOEFlags & 0x30) == 32)
		{
			roa.animate();
		}
		int num = 0;
		if (!noHandle)
		{
			num = ext.handleRunObject();
		}
		if ((num & 2) != 0)
		{
			noHandle = true;
		}
		if (roc != null && roc.rcChanged)
		{
			num |= 1;
			roc.rcChanged = false;
		}
		if ((num & 1) != 0)
		{
			modif();
		}
	}

	public override void modif()
	{
		if (ros != null)
		{
			ros.modifRoutine();
		}
		else if ((hoOEFlags & 2) != 0)
		{
			hoAdRunHeader.redrawLevel(2);
		}
		else
		{
			ext.displayRunObject(null);
		}
	}

	public override void display()
	{
	}

	public override void kill(bool bFast)
	{
		ext.destroyRunObject(bFast);
	}

	public override void getZoneInfos()
	{
		ext.getZoneInfos();
		hoRect.left = hoX - hoAdRunHeader.rhWindowX - hoImgXSpot;
		hoRect.right = hoRect.left + hoImgWidth;
		hoRect.top = hoY - hoAdRunHeader.rhWindowY - hoImgYSpot;
		hoRect.bottom = hoRect.top + hoImgHeight;
	}

	public override CMask getCollisionMask(int flags)
	{
		return ext.getRunObjectCollisionMask(flags);
	}

	public override void draw(SpriteBatchEffect batch)
	{
		Texture2D runObjectSurface = ext.getRunObjectSurface();
		if (runObjectSurface != null)
		{
			batch.Draw(runObjectSurface, new Rectangle
			{
				X = hoRect.left + hoAdRunHeader.rhApp.xOffset,
				Y = hoRect.top + hoAdRunHeader.rhApp.yOffset,
				Width = hoRect.right - hoRect.left,
				Height = hoRect.bottom - hoRect.top
			}, null, Color.White);
		}
		else
		{
			ext.displayRunObject(batch);
		}
	}

	public override void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
	{
		draw(batch);
	}

	public override void drawableKill()
	{
	}

	public override CMask drawableGetMask(int flags)
	{
		return ext.getRunObjectCollisionMask(flags);
	}

	public virtual bool condition(int num, CCndExtension cnd)
	{
		return ext.condition(num, cnd);
	}

	public virtual void action(int num, CActExtension act)
	{
		ext.action(num, act);
	}

	public virtual CValue expression(int num)
	{
		return ext.expression(num);
	}

	public int getX()
	{
		return hoX;
	}

	public int getY()
	{
		return hoY;
	}

	public int getWidth()
	{
		return hoImgWidth;
	}

	public int getHeight()
	{
		return hoImgHeight;
	}

	public void setX(int x)
	{
		if (rom != null)
		{
			rom.rmMovement.setXPosition(x);
			return;
		}
		hoX = x;
		if (roc != null)
		{
			roc.rcChanged = true;
			roc.rcCheckCollides = true;
		}
	}

	public void setY(int y)
	{
		if (rom != null)
		{
			rom.rmMovement.setYPosition(y);
			return;
		}
		hoY = y;
		if (roc != null)
		{
			roc.rcChanged = true;
			roc.rcCheckCollides = true;
		}
	}

	public void setWidth(int width)
	{
		hoImgWidth = width;
		hoRect.right = hoRect.left + width;
	}

	public void setHeight(int height)
	{
		hoImgHeight = height;
		hoRect.bottom = hoRect.top + height;
	}

	public virtual void loadImageList(short[] list)
	{
		hoAdRunHeader.rhApp.imageBank.loadImageList(list);
	}

	public virtual CImage getImage(short handle)
	{
		return hoAdRunHeader.rhApp.imageBank.getImageFromHandle(handle);
	}

	public virtual void reHandle()
	{
		noHandle = false;
	}

	public virtual void generateEvent(int code, int param)
	{
		if (hoAdRunHeader.rh2PauseCompteur == 0)
		{
			int rhCurParam = hoAdRunHeader.rhEvtProg.rhCurParam0;
			hoAdRunHeader.rhEvtProg.rhCurParam0 = param;
			code = -(code + 80 + 1) << 16;
			code |= hoType & 0xFFFF;
			hoAdRunHeader.rhEvtProg.handle_Event(this, code);
			hoAdRunHeader.rhEvtProg.rhCurParam0 = rhCurParam;
		}
	}

	public virtual void pushEvent(int code, int param)
	{
		if (hoAdRunHeader.rh2PauseCompteur == 0)
		{
			code = -(code + 80 + 1) << 16;
			code |= hoType & 0xFFFF;
			hoAdRunHeader.rhEvtProg.push_Event(1, code, param, this, hoOi);
		}
	}

	public virtual void pause()
	{
		hoAdRunHeader.pause();
	}

	public virtual void resume()
	{
		hoAdRunHeader.resume();
	}

	public virtual void redisplay()
	{
		hoAdRunHeader.ohRedrawLevel(bRedrawTotalColMask: true);
	}

	public virtual void redraw()
	{
		modif();
		if ((hoOEFlags & 0x230) != 0)
		{
			roc.rcChanged = true;
		}
	}

	public virtual void destroy()
	{
		hoAdRunHeader.destroy_Add(hoNumber);
	}

	public virtual void setPosition(int x, int y)
	{
		if (rom != null)
		{
			rom.rmMovement.setXPosition(x);
			rom.rmMovement.setYPosition(y);
			return;
		}
		hoX = x;
		hoY = y;
		if (roc != null)
		{
			roc.rcChanged = true;
			roc.rcCheckCollides = true;
		}
	}

	public virtual void addBackdrop(CImage img, int x, int y, int dwEffect, int dwEffectParam, int typeObst, int nLayer)
	{
	}

	public int getEventCount()
	{
		return hoAdRunHeader.rh4EventCount;
	}

	public CValue getExpParam()
	{
		hoAdRunHeader.rh4CurToken++;
		return hoAdRunHeader.getExpression();
	}

	public int getEventParam()
	{
		return hoAdRunHeader.rhEvtProg.rhCurParam0;
	}

	public virtual double callMovement(CObject hoPtr, int action, double param)
	{
		if ((hoPtr.hoOEFlags & 0x10) != 0 && hoPtr.roc.rcMovementType == 14)
		{
			CMoveExtension cMoveExtension = (CMoveExtension)hoPtr.rom.rmMovement;
			return cMoveExtension.callMovement(action, param);
		}
		return 0.0;
	}

	public virtual CValue callExpression(CObject hoPtr, int action, int param)
	{
		CExtension cExtension = (CExtension)hoPtr;
		cExtension.privateData = param;
		return cExtension.expression(action);
	}

	public virtual CObject getObjectFromFixed(int fixed_Renamed)
	{
		int i = 0;
		for (int j = 0; j < hoAdRunHeader.rhNObjects; j++)
		{
			for (; hoAdRunHeader.rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = hoAdRunHeader.rhObjectList[i];
			i++;
			int num = (cObject.hoCreationId << 16) | (cObject.hoNumber & 0xFFFF);
			if (num == fixed_Renamed)
			{
				return cObject;
			}
		}
		return null;
	}

	public CObject getFirstObject()
	{
		objectCount = 0;
		objectNumber = 0;
		return getNextObject();
	}

	public CObject getNextObject()
	{
		if (objectNumber < hoAdRunHeader.rhNObjects)
		{
			while (hoAdRunHeader.rhObjectList[objectCount] == null)
			{
				objectCount++;
			}
			CObject result = hoAdRunHeader.rhObjectList[objectCount];
			objectNumber++;
			objectCount++;
			return result;
		}
		return null;
	}
}
