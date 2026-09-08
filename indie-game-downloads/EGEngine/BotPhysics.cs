using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class BotPhysics
{
	public List<BotPhysicsPart> parts = new List<BotPhysicsPart>();

	private static int[] physicsBones;

	private static int[] physicsBoneDamage;

	private static Ray tmpRayCast;

	private static Vector3 rcTmpVec;

	private static Matrix tmpRCWorldInverse;

	private static Matrix tmpWorld;

	private static Matrix tmpScale;

	private static Matrix tmpPartScale;

	private static Matrix tmpRCWorld;

	private static BotPhysicsPart tmpPart;

	public static bool LastHitWasHeadShot;

	public static int LastHitBodyPart;

	public BotPhysics()
	{
	}

	public BotPhysics(Model physModel)
	{
		Set(physModel);
	}

	public void Set(Model physModel)
	{
		Matrix[] array = new Matrix[((ReadOnlyCollection<ModelBone>)physModel.Bones).Count];
		physModel.CopyAbsoluteBoneTransformsTo(array);
		ModelMeshCollection.Enumerator enumerator = physModel.Meshes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ModelMesh current = enumerator.Current;
				BotPhysicsPart item = default(BotPhysicsPart);
				item.name = current.Name;
				item.transform = array[current.ParentBone.Index];
				item.inverseTransform = Matrix.Invert(array[current.ParentBone.Index]);
				item.oobb = new OOBB(MeshTools.GetPositionsFromMesh(current, VertexType.Basic), item.transform);
				item.mesh = current;
				parts.Add(item);
			}
		}
		finally
		{
			enumerator.Dispose();
		}
	}

	public int RayCast(ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPosition, ref Matrix worldTran, Matrix[] skinnedPose, float scaling)
	{
		for (int i = 0; i < parts.Count; i++)
		{
			tmpPart = parts[i];
			Matrix.Multiply(ref tmpPart.transform, ref skinnedPose[physicsBones[i]], out tmpRCWorld);
			tmpRCWorld *= worldTran;
			Matrix.Invert(ref tmpRCWorld, out tmpRCWorldInverse);
			Vector3.Transform(ref origin, ref tmpRCWorldInverse, out tmpRayCast.Position);
			tmpRCWorldInverse.Translation = Vector3.Zero;
			Vector3.Transform(ref direction, ref tmpRCWorldInverse, out tmpRayCast.Direction);
			float? num = tmpPart.oobb.CollisionRayInverted(ref tmpRayCast, scaling);
			if (num.HasValue)
			{
				LastHitBodyPart = i;
				hitPosition = origin + direction * num.Value;
				if (i == 10)
				{
					LastHitWasHeadShot = true;
				}
				else
				{
					LastHitWasHeadShot = false;
				}
				return physicsBoneDamage[i];
			}
		}
		return 0;
	}

	static BotPhysics()
	{
		physicsBones = new int[11]
		{
			9, 1, 5, 2, 6, 11, 17, 14, 15, 18,
			13
		};
		physicsBoneDamage = new int[11]
		{
			35, 30, 30, 25, 25, 40, 25, 25, 20, 20,
			200
		};
		tmpRayCast = default(Ray);
		rcTmpVec = Vector3.Zero;
		tmpScale = Matrix.CreateScale(0.7f);
		tmpPartScale = Matrix.Identity;
		tmpRCWorld = Matrix.Identity;
		LastHitWasHeadShot = false;
		LastHitBodyPart = 0;
	}
}
