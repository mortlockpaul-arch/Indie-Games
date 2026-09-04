using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

internal class CMoveBullet : CMove
{
	public bool MBul_Wait;

	public CObject MBul_ShootObject;

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		if (hoPtr.roc.rcSprite != null)
		{
			hoPtr.roc.rcSprite.setSpriteColFlag(0u);
		}
		if (hoPtr.ros != null)
		{
			hoPtr.ros.rsFlags &= -33;
			hoPtr.ros.obHide();
		}
		MBul_Wait = true;
		hoPtr.hoCalculX = 0;
		hoPtr.hoCalculY = 0;
		if (hoPtr.roa != null)
		{
			hoPtr.roa.init_Animation(1);
		}
		hoPtr.roc.rcSpeed = 0;
		hoPtr.roc.rcCheckCollides = true;
		hoPtr.roc.rcChanged = true;
	}

	public void init2(CObject parent)
	{
		hoPtr.roc.rcMaxSpeed = hoPtr.roc.rcSpeed;
		hoPtr.roc.rcMinSpeed = hoPtr.roc.rcSpeed;
		MBul_ShootObject = parent;
	}

	public override void move()
	{
		if (MBul_Wait)
		{
			if (MBul_ShootObject.roa != null && MBul_ShootObject.roa.raAnimOn == 6)
			{
				return;
			}
			startBullet();
		}
		if (hoPtr.roa != null)
		{
			hoPtr.roa.animate();
			if (CRun.bMoveChanged)
			{
				return;
			}
		}
		newMake_Move(hoPtr.roc.rcSpeed, hoPtr.roc.rcDir);
		if (!CRun.bMoveChanged)
		{
			if (hoPtr.hoX < -64 || hoPtr.hoX > hoPtr.hoAdRunHeader.rhLevelSx + 64 || hoPtr.hoY < -64 || hoPtr.hoY > hoPtr.hoAdRunHeader.rhLevelSy + 64)
			{
				hoPtr.hoCallRoutine = false;
				hoPtr.hoAdRunHeader.destroy_Add(hoPtr.hoNumber);
			}
			if (hoPtr.roc.rcCheckCollides)
			{
				hoPtr.roc.rcCheckCollides = false;
				hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
			}
		}
	}

	public void startBullet()
	{
		if (hoPtr.roc.rcSprite != null)
		{
			hoPtr.roc.rcSprite.setSpriteColFlag(1u);
		}
		if (hoPtr.ros != null)
		{
			hoPtr.ros.rsFlags |= 32;
			hoPtr.ros.obShow();
		}
		MBul_Wait = false;
		MBul_ShootObject = null;
	}

	public override void setXPosition(int x)
	{
		if (hoPtr.hoX != x)
		{
			hoPtr.hoX = x;
			hoPtr.rom.rmMoveFlag = true;
			hoPtr.roc.rcChanged = true;
			hoPtr.roc.rcCheckCollides = true;
		}
	}

	public override void setYPosition(int y)
	{
		if (hoPtr.hoY != y)
		{
			hoPtr.hoY = y;
			hoPtr.rom.rmMoveFlag = true;
			hoPtr.roc.rcChanged = true;
			hoPtr.roc.rcCheckCollides = true;
		}
	}
}
