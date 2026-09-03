using System;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PComboPath : IPath
{
	public IPath first;

	public IPath second;

	public Vector3 curLocation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return first.curLocation() + second.curLocation();
	}

	public float advance()
	{
		return Math.Max(first.advance(), second.advance());
	}

	public void reset()
	{
		first.reset();
		second.reset();
	}

	public PComboPath(IPath _first, IPath _second)
	{
		first = _first;
		second = _second;
	}

	public PComboPath(PComboPath other)
	{
		first = other.first.copy();
		second = other.second.copy();
	}

	public float maxSpeed()
	{
		return first.maxSpeed() + second.maxSpeed();
	}

	public IPath copy()
	{
		return new PComboPath(this);
	}

	public Vector3 dir()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return first.dir() + second.dir();
	}
}
