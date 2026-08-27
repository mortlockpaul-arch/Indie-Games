using Microsoft.Xna.Framework;

namespace EGEngine;

public class CoverPointRequestCls
{
	public bool moveCloser;

	public bool restrictDistance;

	public float curDisSqr;

	public int curSearchIndex;

	public int curResultIndex;

	public Vector3 coverPosition;

	public Vector3 coverDirection;

	public Vector3 targetPosition;

	public Vector3 targetDirection;

	public BaseData requestOwner;
}
