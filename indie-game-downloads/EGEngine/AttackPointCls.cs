using Microsoft.Xna.Framework;

namespace EGEngine;

public class AttackPointCls
{
	public const int MaxCoverPositions = 2;

	public string Name;

	public AttackPositionType NodeType;

	public Vector3 Position;

	public bool OccupiedFlag;

	public CoverPointCls[] CoverPositions;

	public AttackPointCls()
	{
		CoverPositions = new CoverPointCls[2];
		for (int i = 0; i < 2; i++)
		{
			CoverPositions[i].IsValid = false;
		}
	}
}
