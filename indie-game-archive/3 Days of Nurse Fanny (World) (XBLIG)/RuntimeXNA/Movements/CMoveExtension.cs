using RuntimeXNA.Objects;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

public class CMoveExtension : CMove
{
	public CRunMvtExtension movement;

	public double callParam;

	public CMoveExtension(CRunMvtExtension m)
	{
		movement = m;
	}

	public override void init(CObject ho, CMoveDef mvPtr)
	{
		hoPtr = ho;
		CMoveDefExtension cMoveDefExtension = (CMoveDefExtension)mvPtr;
		CFile cFile = new CFile(cMoveDefExtension.data);
		cFile.setUnicode(ho.hoAdRunHeader.rhApp.bUnicode);
		movement.initialize(cFile);
		hoPtr.roc.rcCheckCollides = true;
		hoPtr.roc.rcChanged = true;
	}

	public override void kill()
	{
		movement.kill();
	}

	public override void move()
	{
		if (movement.move())
		{
			hoPtr.roc.rcChanged = true;
		}
	}

	public override void stop()
	{
		movement.stop(rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount);
	}

	public override void start()
	{
		movement.start();
	}

	public override void bounce()
	{
		movement.bounce(rmCollisionCount == hoPtr.hoAdRunHeader.rh3CollisionCount);
	}

	public override void reverse()
	{
		movement.reverse();
	}

	public virtual double callMovement(int function, double param)
	{
		callParam = param;
		return movement.actionEntry(function);
	}

	public override void setSpeed(int value)
	{
		movement.setSpeed(value);
	}

	public override void setMaxSpeed(int value)
	{
		movement.setMaxSpeed(value);
	}

	public override void setXPosition(int value)
	{
		movement.setXPosition(value);
		hoPtr.roc.rcChanged = true;
		hoPtr.roc.rcCheckCollides = true;
	}

	public override void setYPosition(int value)
	{
		movement.setYPosition(value);
		hoPtr.roc.rcChanged = true;
		hoPtr.roc.rcCheckCollides = true;
	}

	public override void setDir(int value)
	{
		movement.setDir(value);
		hoPtr.roc.rcChanged = true;
		hoPtr.roc.rcCheckCollides = true;
	}
}
