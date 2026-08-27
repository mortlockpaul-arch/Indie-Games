using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class CoverPoints
{
	public const float CoverDirectionAngle = 0.7f;

	public const float COVERPOINT_SEARCH_DISQR = 10240000f;

	private const int MaxCoverPointRequest = 16;

	private const string AttackPositionStr = "attack_position";

	private const string CoverPositionStr = "cover_position";

	public static List<AttackPointCls> AttackPositions = new List<AttackPointCls>();

	private static CoverPointRequestCls[] CoverPointRequest = new CoverPointRequestCls[16];

	private static int CurrentCoverPointRequest = 0;

	private static int RayCastTestCount = 0;

	private static Vector3 vecToAP = Vector3.Zero;

	private static IntersectSegmentParams segParams;

	public static void LoadContent(Model m)
	{
		Matrix[] array = new Matrix[m.Bones.Count];
		m.CopyAbsoluteBoneTransformsTo(array);
		_ = Vector3.Zero;
		AttackPositions.Clear();
		foreach (ModelBone bone in m.Bones)
		{
			if (!bone.Name.Contains("attack_position"))
			{
				continue;
			}
			AttackPointCls attackPointCls = new AttackPointCls();
			attackPointCls.OccupiedFlag = false;
			attackPointCls.NodeType = AttackPositionType.Undeclared;
			attackPointCls.Position = array[bone.Index].Translation;
			if (bone.Name.Contains("stand"))
			{
				attackPointCls.NodeType = AttackPositionType.Stand;
			}
			else if (bone.Name.Contains("crouch"))
			{
				attackPointCls.NodeType = AttackPositionType.Crouch;
			}
			int num = 0;
			foreach (ModelBone child in bone.Children)
			{
				CoverPointCls coverPointCls = default(CoverPointCls);
				coverPointCls.IsValid = true;
				coverPointCls.Position = array[child.Index].Translation;
				coverPointCls.Direction = array[child.Children[0].Index].Translation - coverPointCls.Position;
				coverPointCls.Direction.Normalize();
				attackPointCls.CoverPositions[num] = coverPointCls;
				num++;
			}
			AttackPositions.Add(attackPointCls);
		}
		for (int i = 0; i < 16; i++)
		{
			CoverPointRequest[i] = new CoverPointRequestCls();
			CoverPointRequest[i].curDisSqr = 4000000f;
			CoverPointRequest[i].moveCloser = false;
			CoverPointRequest[i].restrictDistance = false;
			CoverPointRequest[i].curSearchIndex = 0;
			CoverPointRequest[i].curResultIndex = 0;
			CoverPointRequest[i].requestOwner = null;
		}
	}

	public static void Update(GameTime gameTime)
	{
		for (int i = 0; i < 16; i++)
		{
			if (CoverPointRequest[CurrentCoverPointRequest].requestOwner != null)
			{
				if (GetAttackPosition(CoverPointRequest[CurrentCoverPointRequest]))
				{
					CoverPointRequest[CurrentCoverPointRequest].requestOwner.CoverPositionAquired_CallBack(CoverPointRequest[CurrentCoverPointRequest]);
					CoverPointRequest[CurrentCoverPointRequest].requestOwner = null;
					CurrentCoverPointRequest = ((CurrentCoverPointRequest + 1 < 16) ? (CurrentCoverPointRequest + 1) : 0);
				}
				break;
			}
			CurrentCoverPointRequest = ((CurrentCoverPointRequest + 1 < 16) ? (CurrentCoverPointRequest + 1) : 0);
		}
	}

	public virtual void Draw()
	{
	}

	public static bool RequestCoverPosition(BaseData e, bool mv, bool rd)
	{
		for (int i = 0; i < 16; i++)
		{
			if (CoverPointRequest[i].requestOwner == null)
			{
				CoverPointRequest[i].curDisSqr = 10240000f;
				CoverPointRequest[i].moveCloser = mv;
				CoverPointRequest[i].restrictDistance = rd;
				CoverPointRequest[i].curSearchIndex = 0;
				CoverPointRequest[i].curResultIndex = -1;
				CoverPointRequest[i].coverPosition = Vector3.Zero;
				CoverPointRequest[i].coverDirection = Vector3.Zero;
				CoverPointRequest[i].targetPosition = e.TargetPosition;
				CoverPointRequest[i].targetDirection = e.TargetDirection;
				CoverPointRequest[i].requestOwner = e;
				return true;
			}
		}
		return false;
	}

	public static bool GetAttackPosition(CoverPointRequestCls e)
	{
		RayCastTestCount = 0;
		float num = 5800f * e.requestOwner.Weapon.CurrentWeapon.MaxRangeRatio;
		float num2 = num * num;
		float num3 = num * 0.35f * (num * 0.35f);
		float num4 = e.targetDirection.LengthSquared();
		if (!e.restrictDistance)
		{
			num4 = float.MaxValue;
			num2 = float.MaxValue;
			if (e.requestOwner.Weapon.CurrentWeapon.WepType != WeaponType.AlienGrenader)
			{
				num3 = 0f;
			}
		}
		for (int i = e.curSearchIndex; i < AttackPositions.Count; i++)
		{
			if (AttackPositions[i].OccupiedFlag)
			{
				continue;
			}
			for (int j = 0; j < 2; j++)
			{
				if (!AttackPositions[i].CoverPositions[j].IsValid)
				{
					continue;
				}
				vecToAP = e.targetPosition - AttackPositions[i].CoverPositions[j].Position;
				float num5 = vecToAP.LengthSquared();
				if (num5 > num2 || num5 < num3 || (e.moveCloser && num5 >= e.targetDirection.LengthSquared()))
				{
					continue;
				}
				float result = 0f;
				Vector3.Dot(ref AttackPositions[i].CoverPositions[j].Direction, ref vecToAP, out result);
				if (!(result > 0f))
				{
					continue;
				}
				float num6 = (AttackPositions[i].CoverPositions[j].Position - e.requestOwner.Position).LengthSquared();
				if (!(num6 < e.curDisSqr) || !(num6 < num4))
				{
					continue;
				}
				vecToAP.Normalize();
				Vector3.Dot(ref AttackPositions[i].CoverPositions[j].Direction, ref vecToAP, out result);
				if (result >= 0.7f)
				{
					RayCastTestCount++;
					segParams.OnlyWalkable = true;
					segParams.SegmentStart = AttackPositions[i].Position;
					segParams.SegmentStart.Y += 80f;
					segParams.SegmentDirection = e.targetPosition - segParams.SegmentStart;
					segParams.SegmentLength = segParams.SegmentDirection.Length();
					segParams.SegmentDirection.Normalize();
					segParams.SegmentEnd = segParams.SegmentStart + segParams.SegmentDirection * segParams.SegmentLength;
					segParams.PreComputeParameters();
					if (LevelOutside.RayCast(0, ref segParams, spawnSparks: false) == MaterialType.Undefined)
					{
						e.curDisSqr = num6;
						e.curResultIndex = i;
						e.coverPosition = AttackPositions[i].CoverPositions[j].Position;
						e.coverDirection = AttackPositions[i].CoverPositions[j].Direction;
					}
					e.curSearchIndex = i + 1;
					return false;
				}
			}
		}
		return true;
	}

	public static void SetOccupiedFlag(int index, bool e)
	{
		if (index >= 0 && index < AttackPositions.Count)
		{
			AttackPositions[index].OccupiedFlag = e;
		}
	}

	public static void ClearOccupiedFlag(int index)
	{
		if (index >= 0 && index < AttackPositions.Count)
		{
			AttackPositions[index].OccupiedFlag = false;
		}
	}

	public static void GetPopoutPosition(int index, ref Vector3 position)
	{
		if (index >= 0 && index < AttackPositions.Count)
		{
			position = AttackPositions[index].Position;
		}
	}

	public static void ResetAttackPoints()
	{
		for (int i = 0; i < 16; i++)
		{
			CoverPointRequest[i] = new CoverPointRequestCls();
			CoverPointRequest[i].curDisSqr = 4000000f;
			CoverPointRequest[i].moveCloser = false;
			CoverPointRequest[i].restrictDistance = false;
			CoverPointRequest[i].curSearchIndex = 0;
			CoverPointRequest[i].curResultIndex = 0;
			CoverPointRequest[i].requestOwner = null;
		}
		for (int j = 0; j < AttackPositions.Count; j++)
		{
			AttackPositions[j].OccupiedFlag = false;
		}
	}
}
