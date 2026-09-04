using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

internal class CMoveMouse : CMove
{
	public int MM_DXMouse;

	public int MM_DYMouse;

	public int MM_FXMouse;

	public int MM_FYMouse;

	public int MM_Stopped;

	public int MM_OldSpeed;

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefMouse cMoveDefMouse = (CMoveDefMouse)mvPtr;
		hoPtr.roc.rcPlayer = cMoveDefMouse.mvControl;
		MM_DXMouse = cMoveDefMouse.mmDx + hoPtr.hoX;
		MM_DYMouse = cMoveDefMouse.mmDy + hoPtr.hoY;
		MM_FXMouse = cMoveDefMouse.mmFx + hoPtr.hoX;
		MM_FYMouse = cMoveDefMouse.mmFy + hoPtr.hoY;
		hoPtr.roc.rcSpeed = 0;
		MM_OldSpeed = 0;
		MM_Stopped = 0;
		hoPtr.roc.rcMinSpeed = 0;
		hoPtr.roc.rcMaxSpeed = 100;
		rmOpt = cMoveDefMouse.mvOpt;
		moveAtStart(mvPtr);
		hoPtr.roc.rcChanged = true;
	}

	public override void move()
	{
		int num = hoPtr.hoX;
		int num2 = hoPtr.hoY;
		if (rmStopSpeed == 0 && hoPtr.hoAdRunHeader.rh2InputMask[hoPtr.roc.rcPlayer - 1] != 0)
		{
			num = hoPtr.hoX + hoPtr.hoAdRunHeader.rh2MouseX;
			num2 = hoPtr.hoY + hoPtr.hoAdRunHeader.rh2MouseY;
			if (num < MM_DXMouse)
			{
				num = MM_DXMouse;
			}
			if (num > MM_FXMouse)
			{
				num = MM_FXMouse;
			}
			if (num2 < MM_DYMouse)
			{
				num2 = MM_DYMouse;
			}
			if (num2 > MM_FYMouse)
			{
				num2 = MM_FYMouse;
			}
			int num3 = num - hoPtr.hoX;
			int num4 = num2 - hoPtr.hoY;
			int num5 = 0;
			if (num3 < 0)
			{
				num3 = -num3;
				num5 |= 1;
			}
			if (num4 < 0)
			{
				num4 = -num4;
				num5 |= 2;
			}
			int num6 = num3 + num4 << 2;
			if (num6 > 250)
			{
				num6 = 250;
			}
			hoPtr.roc.rcSpeed = num6;
			if (num6 != 0)
			{
				num3 <<= 8;
				if (num4 == 0)
				{
					num4 = 1;
				}
				num3 /= num4;
				int i;
				for (i = 0; num3 < CMove.CosSurSin32[i]; i += 2)
				{
				}
				int num7 = CMove.CosSurSin32[i + 1];
				if ((num5 & 2) != 0)
				{
					num7 = -num7 + 32;
					num7 &= 0x1F;
				}
				if ((num5 & 1) != 0)
				{
					num7 -= 8;
					num7 &= 0x1F;
					num7 = -num7;
					num7 &= 0x1F;
					num7 += 8;
					num7 &= 0x1F;
				}
				hoPtr.roc.rcDir = num7;
			}
		}
		if (hoPtr.roc.rcSpeed != 0)
		{
			MM_Stopped = 0;
			MM_OldSpeed = hoPtr.roc.rcSpeed;
		}
		MM_Stopped++;
		if (MM_Stopped > 10)
		{
			MM_OldSpeed = 0;
		}
		hoPtr.roc.rcSpeed = MM_OldSpeed;
		if (hoPtr.roa != null)
		{
			hoPtr.roa.animate();
		}
		if (!CRun.bMoveChanged)
		{
			hoPtr.hoX = num;
			hoPtr.hoY = num2;
			hoPtr.roc.rcChanged = true;
			hoPtr.hoAdRunHeader.rh3CollisionCount++;
			rmCollisionCount = hoPtr.hoAdRunHeader.rh3CollisionCount;
			hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
		}
	}

	public override void stop()
	{
		if (rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount)
		{
			mv_Approach((rmOpt & 1) != 0);
		}
		hoPtr.roc.rcSpeed = 0;
	}

	public override void start()
	{
		rmStopSpeed = 0;
		hoPtr.rom.rmMoveFlag = true;
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
