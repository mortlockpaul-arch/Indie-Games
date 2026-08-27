using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class BotPhysics
{
	public List<BotPhysicsPart> parts = new List<BotPhysicsPart>();

	private static int[] physicsBones = new int[11]
	{
		9, 1, 5, 2, 6, 11, 17, 14, 15, 18,
		13
	};

	private static int[] physicsBoneDamage = new int[11]
	{
		35, 30, 30, 25, 25, 40, 25, 25, 20, 20,
		200
	};

	private static Ray tmpRayCast = default(Ray);

	private static Vector3 rcTmpVec = Vector3.Zero;

	private static Matrix tmpRCWorldInverse;

	private static Matrix tmpWorld;

	private static Matrix tmpScale = Matrix.CreateScale(0.7f);

	private static Matrix tmpPartScale = Matrix.Identity;

	private static Matrix tmpRCWorld = Matrix.Identity;

	private static BotPhysicsPart tmpPart;

	public static bool LastHitWasHeadShot = false;

	public static int LastHitBodyPart = 0;

	public BotPhysics()
	{
	}

	public BotPhysics(Model physModel)
	{
		Set(physModel);
	}

	public void Set(Model physModel)
	{
		Matrix[] array = new Matrix[physModel.Bones.Count];
		physModel.CopyAbsoluteBoneTransformsTo(array);
		foreach (ModelMesh mesh in physModel.Meshes)
		{
			BotPhysicsPart item = default(BotPhysicsPart);
			item.name = mesh.Name;
			item.transform = array[mesh.ParentBone.Index];
			item.inverseTransform = Matrix.Invert(array[mesh.ParentBone.Index]);
			item.oobb = new OOBB(MeshTools.GetPositionsFromMesh(mesh, VertexType.Basic), item.transform);
			item.mesh = mesh;
			parts.Add(item);
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
}
