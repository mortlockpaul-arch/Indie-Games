using System;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Movements;

internal class CMovePath : CMove
{
	public int MT_Speed;

	public int MT_Sinus;

	public int MT_Cosinus;

	public int MT_Longueur;

	public int MT_XOrigin;

	public int MT_YOrigin;

	public int MT_XDest;

	public int MT_YDest;

	public int MT_MoveNumber;

	public bool MT_Direction;

	public CMoveDefPath MT_Movement;

	public int MT_Calculs;

	public int MT_XStart;

	public int MT_YStart;

	public int MT_Pause;

	public string MT_GotoNode;

	private bool MT_FlagBranch;

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefPath cMoveDefPath = (CMoveDefPath)mvPtr;
		MT_XStart = hoPtr.hoX;
		MT_YStart = hoPtr.hoY;
		MT_Direction = false;
		MT_Pause = 0;
		hoPtr.hoMark1 = 0;
		MT_Movement = cMoveDefPath;
		hoPtr.roc.rcMinSpeed = cMoveDefPath.mtMinSpeed;
		hoPtr.roc.rcMaxSpeed = cMoveDefPath.mtMaxSpeed;
		MT_Calculs = 0;
		MT_GotoNode = null;
		mtGoAvant(0);
		moveAtStart(mvPtr);
		hoPtr.roc.rcSpeed = MT_Speed;
		hoPtr.roc.rcChanged = true;
		if (MT_Movement.steps.Length == 0)
		{
			stop();
		}
	}

	public override void move()
	{
		hoPtr.hoMark1 = 0;
		hoPtr.roc.rcAnim = 1;
		if (hoPtr.roa != null)
		{
			hoPtr.roa.animate();
		}
		if (CRun.bMoveChanged)
		{
			return;
		}
		if (MT_Speed == 0)
		{
			int mT_Pause = MT_Pause;
			if (mT_Pause == 0)
			{
				hoPtr.roc.rcSpeed = 0;
				hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
				return;
			}
			mT_Pause -= hoPtr.hoAdRunHeader.rhTimerDelta;
			if (mT_Pause > 0)
			{
				MT_Pause = mT_Pause;
				hoPtr.roc.rcSpeed = 0;
				hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
				return;
			}
			MT_Pause = 0;
			MT_Speed = rmStopSpeed & 0x7FFF;
			rmStopSpeed = 0;
			hoPtr.roc.rcSpeed = MT_Speed;
		}
		int num = (((hoPtr.hoAdRunHeader.rhFrame.leFlags & 0x8000) == 0) ? 256 : ((int)(256.0 * hoPtr.hoAdRunHeader.rh4MvtTimerCoef)));
		hoPtr.hoAdRunHeader.rhMT_VBLCount = (short)num;
		bool flag;
		do
		{
			flag = false;
			hoPtr.hoAdRunHeader.rhMT_VBLStep = (short)num;
			num *= MT_Speed;
			num <<= 5;
			if (num <= 524288)
			{
				hoPtr.hoAdRunHeader.rhMT_MoveStep = num;
			}
			else
			{
				num = 16384;
				num /= MT_Speed;
				hoPtr.hoAdRunHeader.rhMT_VBLStep = (short)num;
				hoPtr.hoAdRunHeader.rhMT_MoveStep = 524288;
			}
			MT_FlagBranch = false;
			if (mtMove(hoPtr.hoAdRunHeader.rhMT_MoveStep) && !MT_FlagBranch)
			{
				flag = true;
				continue;
			}
			if (hoPtr.hoAdRunHeader.rhMT_VBLCount == hoPtr.hoAdRunHeader.rhMT_VBLStep)
			{
				flag = true;
				continue;
			}
			if (hoPtr.hoAdRunHeader.rhMT_VBLCount > hoPtr.hoAdRunHeader.rhMT_VBLStep)
			{
				hoPtr.hoAdRunHeader.rhMT_VBLCount -= hoPtr.hoAdRunHeader.rhMT_VBLStep;
				num = hoPtr.hoAdRunHeader.rhMT_VBLCount;
				continue;
			}
			num = hoPtr.hoAdRunHeader.rhMT_VBLCount * MT_Speed;
			num <<= 5;
			mtMove(num);
			flag = true;
		}
		while (!flag);
	}

	private bool mtMove(int step)
	{
		step += MT_Calculs;
		int num = (step >> 16) & 0xFFFF;
		if (num < MT_Longueur)
		{
			MT_Calculs = step;
			int hoX = num * MT_Cosinus / 16384 + MT_XOrigin;
			int hoY = num * MT_Sinus / 16384 + MT_YOrigin;
			hoPtr.hoX = hoX;
			hoPtr.hoY = hoY;
			hoPtr.roc.rcChanged = true;
			hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
			return hoPtr.rom.rmMoveFlag;
		}
		num -= MT_Longueur;
		step = (num << 16) | (step & 0xFFFF);
		if (MT_Speed != 0)
		{
			step /= MT_Speed;
		}
		step >>= 5;
		hoPtr.hoAdRunHeader.rhMT_VBLCount += (short)(step & 0xFFFF);
		hoPtr.hoX = MT_XDest;
		hoPtr.hoY = MT_YDest;
		hoPtr.roc.rcChanged = true;
		hoPtr.hoAdRunHeader.newHandle_Collisions(hoPtr);
		if (hoPtr.rom.rmMoveFlag)
		{
			return true;
		}
		hoPtr.hoMark1 = hoPtr.hoAdRunHeader.rhLoopCount;
		hoPtr.hoMT_NodeName = null;
		int mT_MoveNumber = MT_MoveNumber;
		MT_Calculs = 0;
		if (!MT_Direction)
		{
			mT_MoveNumber++;
			if (mT_MoveNumber < MT_Movement.mtNumber)
			{
				hoPtr.hoMT_NodeName = MT_Movement.steps[mT_MoveNumber].mdName;
				if (MT_GotoNode != null && MT_Movement.steps[mT_MoveNumber].mdName != null && string.Compare(MT_GotoNode, MT_Movement.steps[mT_MoveNumber].mdName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					MT_MoveNumber = mT_MoveNumber;
					mtMessages();
					return mtTheEnd();
				}
				mtGoAvant(mT_MoveNumber);
				mtMessages();
				return hoPtr.rom.rmMoveFlag;
			}
			hoPtr.hoMark2 = hoPtr.hoAdRunHeader.rhLoopCount;
			MT_MoveNumber = mT_MoveNumber;
			if (MT_Direction)
			{
				mtMessages();
				return hoPtr.rom.rmMoveFlag;
			}
			if (MT_Movement.mtReverse != 0)
			{
				MT_Direction = true;
				mT_MoveNumber--;
				hoPtr.hoMT_NodeName = MT_Movement.steps[mT_MoveNumber].mdName;
				mtGoArriere(mT_MoveNumber);
				mtMessages();
				return hoPtr.rom.rmMoveFlag;
			}
			mtReposAtEnd();
			if (MT_Movement.mtLoop == 0)
			{
				mtTheEnd();
				mtMessages();
				return hoPtr.rom.rmMoveFlag;
			}
			mT_MoveNumber = 0;
			mtGoAvant(mT_MoveNumber);
			mtMessages();
			return hoPtr.rom.rmMoveFlag;
		}
		if (MT_GotoNode != null && MT_Movement.steps[mT_MoveNumber].mdName != null && string.Compare(MT_GotoNode, MT_Movement.steps[mT_MoveNumber].mdName, StringComparison.OrdinalIgnoreCase) == 0)
		{
			mtMessages();
			return mtTheEnd();
		}
		hoPtr.hoMT_NodeName = MT_Movement.steps[mT_MoveNumber].mdName;
		MT_Pause = MT_Movement.steps[mT_MoveNumber].mdPause;
		mT_MoveNumber--;
		if (mT_MoveNumber >= 0)
		{
			mtGoArriere(mT_MoveNumber);
			mtMessages();
			return hoPtr.rom.rmMoveFlag;
		}
		mtReposAtEnd();
		if (!MT_Direction)
		{
			mtMessages();
			return hoPtr.rom.rmMoveFlag;
		}
		if (MT_Movement.mtLoop == 0)
		{
			mtTheEnd();
			mtMessages();
			return hoPtr.rom.rmMoveFlag;
		}
		mT_MoveNumber = 0;
		MT_Direction = false;
		mtGoAvant(mT_MoveNumber);
		mtMessages();
		return hoPtr.rom.rmMoveFlag;
	}

	private void mtGoAvant(int number)
	{
		if (number >= MT_Movement.steps.Length)
		{
			stop();
			return;
		}
		MT_Direction = false;
		MT_MoveNumber = number;
		MT_Pause = MT_Movement.steps[number].mdPause;
		MT_Cosinus = MT_Movement.steps[number].mdCosinus;
		MT_Sinus = MT_Movement.steps[number].mdSinus;
		MT_XOrigin = hoPtr.hoX;
		MT_YOrigin = hoPtr.hoY;
		MT_XDest = hoPtr.hoX + MT_Movement.steps[number].mdDx;
		MT_YDest = hoPtr.hoY + MT_Movement.steps[number].mdDy;
		hoPtr.roc.rcDir = MT_Movement.steps[number].mdDir;
		mtBranche();
	}

	private void mtGoArriere(int number)
	{
		if (number >= MT_Movement.steps.Length)
		{
			stop();
			return;
		}
		MT_Direction = true;
		MT_MoveNumber = number;
		MT_Cosinus = -MT_Movement.steps[number].mdCosinus;
		MT_Sinus = -MT_Movement.steps[number].mdSinus;
		MT_XOrigin = hoPtr.hoX;
		MT_YOrigin = hoPtr.hoY;
		MT_XDest = hoPtr.hoX - MT_Movement.steps[number].mdDx;
		MT_YDest = hoPtr.hoY - MT_Movement.steps[number].mdDy;
		int mdDir = MT_Movement.steps[number].mdDir;
		mdDir += 16;
		mdDir &= 0x1F;
		hoPtr.roc.rcDir = mdDir;
		mtBranche();
	}

	private void mtBranche()
	{
		MT_Longueur = MT_Movement.steps[MT_MoveNumber].mdLength;
		int num = MT_Movement.steps[MT_MoveNumber].mdSpeed;
		int mT_Pause = MT_Pause;
		if (mT_Pause != 0)
		{
			MT_Pause = mT_Pause * 20;
			num = (rmStopSpeed = num | 0x8000);
		}
		if (rmStopSpeed != 0)
		{
			num = 0;
		}
		if (num != MT_Speed || num != 0)
		{
			MT_Speed = num;
			hoPtr.rom.rmMoveFlag = true;
			MT_FlagBranch = true;
		}
		hoPtr.roc.rcSpeed = MT_Speed;
	}

	private void mtMessages()
	{
		if (hoPtr.hoMark1 == hoPtr.hoAdRunHeader.rhLoopCount)
		{
			hoPtr.hoAdRunHeader.rhEvtProg.rhCurParam0 = 0;
			hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, -1310720 | (hoPtr.hoType & 0xFFFF));
			hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, -2293760 | (hoPtr.hoType & 0xFFFF));
		}
		if (hoPtr.hoMark2 == hoPtr.hoAdRunHeader.rhLoopCount)
		{
			hoPtr.hoAdRunHeader.rhEvtProg.rhCurParam0 = 0;
			hoPtr.hoAdRunHeader.rhEvtProg.handle_Event(hoPtr, -1376256 | (hoPtr.hoType & 0xFFFF));
		}
	}

	private bool mtTheEnd()
	{
		MT_Speed = 0;
		rmStopSpeed = 0;
		hoPtr.rom.rmMoveFlag = true;
		MT_FlagBranch = false;
		return true;
	}

	private void mtReposAtEnd()
	{
		if (MT_Movement.mtRepos != 0)
		{
			hoPtr.hoX = MT_XStart;
			hoPtr.hoY = MT_YStart;
			hoPtr.roc.rcChanged = true;
		}
	}

	public void mtBranchNode(string pName)
	{
		for (int i = 0; i < MT_Movement.mtNumber; i++)
		{
			if (MT_Movement.steps[i].mdName != null && string.Compare(pName, MT_Movement.steps[i].mdName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (!MT_Direction)
				{
					mtGoAvant(i);
					hoPtr.hoMark1 = hoPtr.hoAdRunHeader.rhLoopCount;
					hoPtr.hoMT_NodeName = MT_Movement.steps[i].mdName;
					hoPtr.hoMark2 = 0;
					mtMessages();
				}
				else if (i > 0)
				{
					i--;
					mtGoArriere(i);
					hoPtr.hoMark1 = hoPtr.hoAdRunHeader.rhLoopCount;
					hoPtr.hoMT_NodeName = MT_Movement.steps[i].mdName;
					hoPtr.hoMark2 = 0;
					mtMessages();
				}
				hoPtr.rom.rmMoveFlag = true;
				break;
			}
		}
	}

	private void freeMTNode()
	{
		MT_GotoNode = null;
	}

	public void mtGotoNode(string pName)
	{
		for (int i = 0; i < MT_Movement.mtNumber; i++)
		{
			if (MT_Movement.steps[i].mdName == null || string.Compare(pName, MT_Movement.steps[i].mdName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				continue;
			}
			if (i == MT_MoveNumber && MT_Calculs == 0)
			{
				break;
			}
			freeMTNode();
			MT_GotoNode = pName;
			if (!MT_Direction)
			{
				if (i > MT_MoveNumber)
				{
					if (MT_Speed == 0)
					{
						if ((rmStopSpeed & 0x8000) != 0)
						{
							start();
						}
						else
						{
							mtGoAvant(MT_MoveNumber);
						}
					}
				}
				else if (MT_Speed != 0)
				{
					reverse();
				}
				else if ((rmStopSpeed & 0x8000) != 0)
				{
					start();
					reverse();
				}
				else
				{
					mtGoArriere(MT_MoveNumber - 1);
				}
			}
			else if (i <= MT_MoveNumber)
			{
				if (MT_Speed == 0)
				{
					if ((rmStopSpeed & 0x8000) != 0)
					{
						start();
					}
					else
					{
						mtGoArriere(MT_MoveNumber - 1);
					}
				}
			}
			else if (MT_Speed != 0)
			{
				reverse();
			}
			else if ((rmStopSpeed & 0x8000) != 0)
			{
				start();
				reverse();
			}
			else
			{
				mtGoAvant(MT_MoveNumber);
			}
			break;
		}
	}

	public override void stop()
	{
		if (rmStopSpeed == 0)
		{
			rmStopSpeed = MT_Speed | 0x8000;
		}
		MT_Speed = 0;
		hoPtr.rom.rmMoveFlag = true;
	}

	public override void start()
	{
		if ((rmStopSpeed & 0x8000) != 0)
		{
			MT_Speed = rmStopSpeed & 0x7FFF;
			MT_Pause = 0;
			rmStopSpeed = 0;
			hoPtr.rom.rmMoveFlag = true;
		}
	}

	public override void reverse()
	{
		if (rmStopSpeed != 0)
		{
			return;
		}
		hoPtr.rom.rmMoveFlag = true;
		int mT_MoveNumber = MT_MoveNumber;
		if (MT_Calculs == 0)
		{
			MT_Direction = !MT_Direction;
			if (MT_Direction)
			{
				if (mT_MoveNumber == 0)
				{
					MT_Direction = !MT_Direction;
					return;
				}
				mT_MoveNumber--;
				mtGoArriere(mT_MoveNumber);
			}
			else
			{
				mtGoAvant(mT_MoveNumber);
			}
			return;
		}
		MT_Direction = !MT_Direction;
		MT_Cosinus = -MT_Cosinus;
		MT_Sinus = -MT_Sinus;
		int mT_XOrigin = MT_XOrigin;
		int mT_XDest = MT_XDest;
		MT_XOrigin = mT_XDest;
		MT_XDest = mT_XOrigin;
		mT_XOrigin = MT_YOrigin;
		mT_XDest = MT_YDest;
		MT_YOrigin = mT_XDest;
		MT_YDest = mT_XOrigin;
		hoPtr.roc.rcDir += 16;
		hoPtr.roc.rcDir &= 31;
		int num = (MT_Calculs >> 16) & 0xFFFF;
		num = MT_Longueur - num;
		MT_Calculs = (num << 16) | (MT_Calculs & 0xFFFF);
	}

	public override void setXPosition(int x)
	{
		int hoX = hoPtr.hoX;
		hoPtr.hoX = x;
		hoX -= MT_XOrigin;
		x -= hoX;
		hoX = MT_XDest - MT_XOrigin + x;
		MT_XDest = hoX;
		hoX = MT_XOrigin;
		MT_XOrigin = x;
		hoX -= x;
		MT_XStart -= hoX;
		hoPtr.rom.rmMoveFlag = true;
		hoPtr.roc.rcChanged = true;
		hoPtr.roc.rcCheckCollides = true;
	}

	public override void setYPosition(int y)
	{
		int hoY = hoPtr.hoY;
		hoPtr.hoY = y;
		hoY -= MT_YOrigin;
		y -= hoY;
		hoY = MT_YDest - MT_YOrigin + y;
		MT_YDest = hoY;
		hoY = MT_YOrigin;
		MT_YOrigin = y;
		hoY -= y;
		MT_YStart -= hoY;
		hoPtr.rom.rmMoveFlag = true;
		hoPtr.roc.rcChanged = true;
		hoPtr.roc.rcCheckCollides = true;
	}

	public override void setSpeed(int speed)
	{
		if (speed < 0)
		{
			speed = 0;
		}
		if (speed > 250)
		{
			speed = 250;
		}
		MT_Speed = speed;
		hoPtr.roc.rcSpeed = speed;
		hoPtr.rom.rmMoveFlag = true;
	}

	public override void setMaxSpeed(int speed)
	{
		setSpeed(speed);
	}
}
