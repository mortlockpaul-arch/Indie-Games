using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace EGEngine;

public class Ragdoll
{
	public enum BoneNames
	{
		bone_pelvis,
		bone_right_thigh,
		bone_right_knee,
		bone_right_foot,
		bone_right_toe,
		bone_left_thigh,
		bone_left_knee,
		bone_left_foot,
		bone_left_toe,
		bone_spine0,
		bone_spine1,
		bone_spine2,
		bone_neck,
		bone_head,
		bone_right_arm,
		bone_right_elbow,
		bone_right_hand,
		bone_left_arm,
		bone_left_elbow,
		bone_left_hand,
		NumberBones
	}

	public struct SpringElement
	{
		public float RestLength;

		public float RestLengthSqr;

		public int Vertex0;

		public int Vertex1;
	}

	public struct ConstraintElement
	{
		public float RestLengthMin;

		public float RestLengthMax;

		public float RestLengthMinSqr;

		public float RestLengthMaxSqr;

		public int Vertex0;

		public int Vertex1;
	}

	public const int NumberOfVertices = 13;

	public const int NumberOfSprings = 18;

	public const int NumberOfConstraints = 11;

	public static bool UpdateToggle = false;

	public bool[] Render = new bool[2];

	public bool IsValid;

	public bool SetRagdoll;

	public bool UpdateVertices;

	public float SleepTimer;

	public int currentCharacterIndex;

	public Model currentCharacter;

	public DamegePacketType DamageType;

	public Vector3 DamageDirection = Vector3.Zero;

	public Vector3 Position;

	public Vector3 PelvisPosition;

	public SkinningData SkinData;

	public SpringElement[] Springs = new SpringElement[18];

	public ConstraintElement[] Constraints = new ConstraintElement[11];

	public float[] VerticesRadius = new float[13];

	public Vector3[] Vertices = new Vector3[13];

	public Vector3[] VerticesLast = new Vector3[13];

	public Vector3[] VerticesForces = new Vector3[13];

	public float[] VerticesFriction = new float[13];

	public Matrix RagdollWorldTransform;

	public Matrix RagdollWorldTransformSpawn;

	public Matrix[] RagdollBonePoseSpawn;

	public Matrix[] RagdollBonePoseSpawnLast;

	public Matrix[] RagdollSkinPose;

	public Matrix[] RagdollBonePose;

	public Matrix[] RagdollWorldPose;

	private static Matrix tmpMat = Matrix.Identity;

	private static Matrix tmpMatScale = Matrix.Identity;

	private static Vector3 tmpOffset = Vector3.Zero;

	private static Vector3 tmpDelta = Vector3.Zero;

	private static Vector3 tmpVertice = Vector3.Zero;

	private static Matrix tmpRagdollScale = Matrix.CreateScale(1.7f);

	private static BoundingSphere tmpRDSphere = default(BoundingSphere);

	private static Vector3 upPelvis = Vector3.Zero;

	private static Vector3 rightPelvis = Vector3.Zero;

	private static Vector3 forward = Vector3.Zero;

	private static Vector3 hitNormal = Vector3.Zero;

	private static Vector3 rightThigh = Vector3.Zero;

	private static Vector3 upKnee = Vector3.Zero;

	private static Vector3 forwardThigh = Vector3.Zero;

	private static Vector3 torsoRight = Vector3.Zero;

	private static Vector3 rightArm = Vector3.Zero;

	private static Vector3 leftArm = Vector3.Zero;

	private static Vector3 rightArmRight = Vector3.Zero;

	private static Vector3 rightElbowDown = Vector3.Zero;

	private static Vector3 rightArmForward = Vector3.Zero;

	private static Vector3 leftArmRight = Vector3.Zero;

	private static Vector3 leftElbowDown = Vector3.Zero;

	private static Vector3 leftArmForward = Vector3.Zero;

	private static Vector3 pelvisPos = Vector3.Zero;

	private static Vector3 pelvisOffset = Vector3.Zero;

	private static Matrix matWorldTrans = Matrix.Identity;

	private static Matrix mBoneTrans = Matrix.Identity;

	private static Vector3 tu = Vector3.Zero;

	private static Vector3 td = Vector3.Zero;

	private static Vector3 tf = Vector3.Zero;

	private static Vector3 tr = Vector3.Zero;

	private static Vector3 tl = Vector3.Zero;

	public Ragdoll()
	{
		Set();
	}

	public Ragdoll(SkinningData e)
	{
		SkinData = e;
		Set();
	}

	public void Set()
	{
		IsValid = false;
		SetRagdoll = false;
		for (int i = 0; i < 2; i++)
		{
			Render[i] = false;
		}
		ref Vector3 reference = ref Vertices[0];
		reference = new Vector3(-15f, 0f, 0f);
		ref Vector3 reference2 = ref Vertices[1];
		reference2 = new Vector3(15f, 0f, 0f);
		ref Vector3 reference3 = ref Vertices[2];
		reference3 = new Vector3(0f, 80f, 0f);
		Springs[0].Vertex0 = 0;
		Springs[0].Vertex1 = 1;
		Springs[1].Vertex0 = 0;
		Springs[1].Vertex1 = 2;
		Springs[2].Vertex0 = 1;
		Springs[2].Vertex1 = 2;
		ref Vector3 reference4 = ref Vertices[3];
		reference4 = new Vector3(-15f, -40f, 0f);
		ref Vector3 reference5 = ref Vertices[4];
		reference5 = new Vector3(-15f, -80f, 0f);
		ref Vector3 reference6 = ref Vertices[5];
		reference6 = new Vector3(15f, -40f, 0f);
		ref Vector3 reference7 = ref Vertices[6];
		reference7 = new Vector3(15f, -80f, 0f);
		Springs[3].Vertex0 = 0;
		Springs[3].Vertex1 = 3;
		Springs[4].Vertex0 = 3;
		Springs[4].Vertex1 = 4;
		Springs[5].Vertex0 = 1;
		Springs[5].Vertex1 = 5;
		Springs[6].Vertex0 = 5;
		Springs[6].Vertex1 = 6;
		ref Vector3 reference8 = ref Vertices[7];
		reference8 = new Vector3(-15f, 80f, 0f);
		ref Vector3 reference9 = ref Vertices[8];
		reference9 = new Vector3(15f, 80f, 0f);
		ref Vector3 reference10 = ref Vertices[9];
		reference10 = new Vector3(-30f, 80f, 0f);
		ref Vector3 reference11 = ref Vertices[10];
		reference11 = new Vector3(30f, 80f, 0f);
		Springs[7].Vertex0 = 11;
		Springs[7].Vertex1 = 7;
		Springs[8].Vertex0 = 7;
		Springs[8].Vertex1 = 9;
		Springs[9].Vertex0 = 12;
		Springs[9].Vertex1 = 8;
		Springs[10].Vertex0 = 8;
		Springs[10].Vertex1 = 10;
		ref Vector3 reference12 = ref Vertices[11];
		reference12 = new Vector3(-15f, 80f, 0f);
		ref Vector3 reference13 = ref Vertices[12];
		reference13 = new Vector3(15f, 80f, 0f);
		Springs[11].Vertex0 = 2;
		Springs[11].Vertex1 = 11;
		Springs[12].Vertex0 = 2;
		Springs[12].Vertex1 = 12;
		Springs[13].Vertex0 = 11;
		Springs[13].Vertex1 = 12;
		Springs[14].Vertex0 = 0;
		Springs[14].Vertex1 = 11;
		Springs[15].Vertex0 = 1;
		Springs[15].Vertex1 = 12;
		Springs[16].Vertex0 = 0;
		Springs[16].Vertex1 = 12;
		Springs[17].Vertex0 = 1;
		Springs[17].Vertex1 = 11;
		for (int j = 0; j < 18; j++)
		{
			Springs[j].RestLength = (Vertices[Springs[j].Vertex0] - Vertices[Springs[j].Vertex1]).Length();
			Springs[j].RestLengthSqr = Springs[j].RestLength * Springs[j].RestLength;
		}
		for (int k = 0; k < 13; k++)
		{
			ref Vector3 reference14 = ref VerticesLast[k];
			reference14 = Vertices[k];
			ref Vector3 reference15 = ref VerticesForces[k];
			reference15 = Vector3.UnitY * -1f;
			VerticesFriction[k] = 0f;
		}
	}

	public void SetSkinData(Model e, float scale)
	{
		currentCharacter = e;
		SkinData = ((SkinnedAnimationData)e.Tag).skinningData;
		RagdollSkinPose = new Matrix[SkinData.BindPose.Count];
		RagdollBonePose = new Matrix[SkinData.BindPose.Count];
		RagdollWorldPose = new Matrix[SkinData.BindPose.Count];
		RagdollBonePoseSpawn = new Matrix[SkinData.BindPose.Count];
		RagdollBonePoseSpawnLast = new Matrix[SkinData.BindPose.Count];
		ResetSkinData(e, 0);
		AnimationPlayer animationPlayer = new AnimationPlayer(SkinData);
		animationPlayer.StartClip(SkinData.AnimationClips["Take 001"]);
		animationPlayer.Update(new TimeSpan(1000L), relativeToCurrentTime: true, Matrix.Identity);
		Matrix[] boneTransforms = animationPlayer.GetBoneTransforms();
		for (int i = 0; i < boneTransforms.Length; i++)
		{
			ref Matrix reference = ref RagdollBonePose[i];
			reference = boneTransforms[i];
			ref Matrix reference2 = ref RagdollBonePoseSpawn[i];
			reference2 = boneTransforms[i];
			ref Matrix reference3 = ref RagdollBonePoseSpawnLast[i];
			reference3 = boneTransforms[i];
		}
		RagdollBonePose[0] *= Matrix.CreateScale(0.7f);
		ref Matrix reference4 = ref RagdollWorldPose[0];
		reference4 = RagdollBonePose[0];
		for (int j = 1; j < RagdollWorldPose.Length; j++)
		{
			int num = SkinData.SkeletonHierarchy[j];
			ref Matrix reference5 = ref RagdollWorldPose[j];
			reference5 = RagdollBonePose[j] * RagdollWorldPose[num];
		}
		ref Vector3 reference6 = ref Vertices[0];
		reference6 = RagdollWorldPose[1].Translation;
		ref Vector3 reference7 = ref Vertices[1];
		reference7 = RagdollWorldPose[5].Translation;
		ref Vector3 reference8 = ref Vertices[2];
		reference8 = RagdollWorldPose[10].Translation;
		PelvisPosition = (Vertices[0] + Vertices[1]) * 0.5f;
		ref Vector3 reference9 = ref Vertices[3];
		reference9 = RagdollWorldPose[2].Translation;
		ref Vector3 reference10 = ref Vertices[4];
		reference10 = RagdollWorldPose[3].Translation;
		ref Vector3 reference11 = ref Vertices[5];
		reference11 = RagdollWorldPose[6].Translation;
		ref Vector3 reference12 = ref Vertices[6];
		reference12 = RagdollWorldPose[7].Translation;
		ref Vector3 reference13 = ref Vertices[7];
		reference13 = RagdollWorldPose[15].Translation;
		ref Vector3 reference14 = ref Vertices[8];
		reference14 = RagdollWorldPose[18].Translation;
		ref Vector3 reference15 = ref Vertices[9];
		reference15 = RagdollWorldPose[16].Translation;
		ref Vector3 reference16 = ref Vertices[10];
		reference16 = RagdollWorldPose[19].Translation;
		ref Vector3 reference17 = ref Vertices[11];
		reference17 = RagdollWorldPose[14].Translation;
		ref Vector3 reference18 = ref Vertices[12];
		reference18 = RagdollWorldPose[17].Translation;
		VerticesRadius[0] = 12f;
		VerticesRadius[1] = 12f;
		VerticesRadius[2] = 12f;
		VerticesRadius[3] = 8f;
		VerticesRadius[4] = 6f;
		VerticesRadius[5] = 8f;
		VerticesRadius[6] = 6f;
		VerticesRadius[7] = 6f;
		VerticesRadius[8] = 6f;
		VerticesRadius[9] = 6f;
		VerticesRadius[10] = 6f;
		VerticesRadius[11] = 14f;
		VerticesRadius[12] = 14f;
		for (int k = 0; k < 13; k++)
		{
			ref Vector3 reference19 = ref VerticesLast[k];
			reference19 = Vertices[k];
			ref Vector3 reference20 = ref VerticesForces[k];
			reference20 = Vector3.UnitY * -1f;
		}
		RagdollWorldTransform = Matrix.CreateScale(scale);
		RagdollWorldTransformSpawn = Matrix.CreateScale(scale);
		for (int l = 0; l < 18; l++)
		{
			Springs[l].RestLength = (Vertices[Springs[l].Vertex0] - Vertices[Springs[l].Vertex1]).Length();
			Springs[l].RestLengthSqr = Springs[l].RestLength * Springs[l].RestLength;
		}
		Constraints[0].Vertex0 = 2;
		Constraints[0].Vertex1 = 3;
		Constraints[0].RestLengthMin = (Vertices[2] - Vertices[3]).Length() * 0.5f;
		Constraints[0].RestLengthMax = 1000f;
		Constraints[1].Vertex0 = 2;
		Constraints[1].Vertex1 = 5;
		Constraints[1].RestLengthMin = (Vertices[2] - Vertices[5]).Length() * 0.5f;
		Constraints[1].RestLengthMax = 1000f;
		Constraints[2].Vertex0 = 2;
		Constraints[2].Vertex1 = 4;
		Constraints[2].RestLengthMin = (Vertices[2] - Vertices[4]).Length() * 0.8f;
		Constraints[2].RestLengthMax = 1000f;
		Constraints[3].Vertex0 = 2;
		Constraints[3].Vertex1 = 6;
		Constraints[3].RestLengthMin = (Vertices[2] - Vertices[6]).Length() * 0.8f;
		Constraints[3].RestLengthMax = 1000f;
		Constraints[4].Vertex0 = 0;
		Constraints[4].Vertex1 = 5;
		Constraints[4].RestLengthMin = (Vertices[0] - Vertices[5]).Length() * 0.8f;
		Constraints[4].RestLengthMax = (Vertices[0] - Vertices[5]).Length() * 1.15f;
		Constraints[5].Vertex0 = 1;
		Constraints[5].Vertex1 = 3;
		Constraints[5].RestLengthMin = (Vertices[1] - Vertices[3]).Length() * 0.8f;
		Constraints[5].RestLengthMax = (Vertices[1] - Vertices[3]).Length() * 1.15f;
		Constraints[6].Vertex0 = 0;
		Constraints[6].Vertex1 = 7;
		Constraints[6].RestLengthMin = 0f;
		Constraints[6].RestLengthMax = (Vertices[0] - Vertices[11]).Length() * 1f;
		Constraints[7].Vertex0 = 1;
		Constraints[7].Vertex1 = 8;
		Constraints[7].RestLengthMin = 0f;
		Constraints[7].RestLengthMax = (Vertices[1] - Vertices[12]).Length() * 1f;
		Constraints[8].Vertex0 = 9;
		Constraints[8].Vertex1 = 11;
		Constraints[8].RestLengthMin = (Vertices[9] - Vertices[11]).Length() * 0.25f;
		Constraints[8].RestLengthMax = (Vertices[9] - Vertices[11]).Length() * 2f;
		Constraints[9].Vertex0 = 10;
		Constraints[9].Vertex1 = 12;
		Constraints[9].RestLengthMin = (Vertices[10] - Vertices[12]).Length() * 0.25f;
		Constraints[9].RestLengthMax = (Vertices[10] - Vertices[12]).Length() * 2f;
		Constraints[10].Vertex0 = 3;
		Constraints[10].Vertex1 = 5;
		Constraints[10].RestLengthMin = (Vertices[3] - Vertices[5]).Length() * 0.5f;
		Constraints[10].RestLengthMax = (Vertices[3] - Vertices[5]).Length() * 4f;
		for (int m = 0; m < 11; m++)
		{
			Constraints[m].RestLengthMinSqr = Constraints[m].RestLengthMin * Constraints[m].RestLengthMin;
			Constraints[m].RestLengthMaxSqr = Constraints[m].RestLengthMax * Constraints[m].RestLengthMax;
		}
		ref Matrix reference21 = ref RagdollWorldPose[0];
		reference21 = RagdollBonePose[0] * RagdollWorldTransform;
		for (int n = 1; n < RagdollWorldPose.Length; n++)
		{
			int num2 = SkinData.SkeletonHierarchy[n];
			ref Matrix reference22 = ref RagdollWorldPose[n];
			reference22 = RagdollBonePose[n] * RagdollWorldPose[num2];
		}
		for (int num3 = 0; num3 < RagdollWorldPose.Length; num3++)
		{
			ref Matrix reference23 = ref RagdollSkinPose[num3];
			reference23 = SkinData.InverseBindPose[num3] * RagdollWorldPose[num3];
		}
	}

	public void ResetSkinData(Model e, int i)
	{
		currentCharacter = e;
		currentCharacterIndex = i;
		SkinData = ((SkinnedAnimationData)e.Tag).skinningData;
	}

	public void Update()
	{
		if (UpdateVertices)
		{
			UpdateSpawn();
			UpdateVertices = false;
		}
		SleepTimer += EndGameEngine.fFIXED_TIME_STEP;
		if (SleepTimer > 14f)
		{
			return;
		}
		float num = 1f;
		for (int i = 0; i < 4; i++)
		{
			DamageDirection = Vector3.Lerp(DamageDirection, Vector3.Zero, 0.05f);
			if (DamageType == DamegePacketType.Grenade)
			{
				DamageDirection = Vector3.Lerp(DamageDirection, Vector3.Zero, 0.05f);
				ref Vector3 reference = ref VerticesForces[11];
				reference = DamageDirection;
				ref Vector3 reference2 = ref VerticesForces[12];
				reference2 = DamageDirection;
				ref Vector3 reference3 = ref VerticesForces[0];
				reference3 = DamageDirection;
				ref Vector3 reference4 = ref VerticesForces[1];
				reference4 = DamageDirection;
				ref Vector3 reference5 = ref VerticesForces[2];
				reference5 = DamageDirection;
			}
			else if (DamageType == DamegePacketType.Body)
			{
				DamageDirection = Vector3.Lerp(DamageDirection, Vector3.Zero, 0.07f);
				ref Vector3 reference6 = ref VerticesForces[0];
				reference6 = DamageDirection;
				ref Vector3 reference7 = ref VerticesForces[1];
				reference7 = DamageDirection;
				ref Vector3 reference8 = ref VerticesForces[2];
				reference8 = DamageDirection;
			}
			else if (DamageType == DamegePacketType.HeadShot)
			{
				DamageDirection = Vector3.Lerp(DamageDirection, Vector3.Zero, 0.07f);
				ref Vector3 reference9 = ref VerticesForces[11];
				reference9 = DamageDirection;
				ref Vector3 reference10 = ref VerticesForces[12];
				reference10 = DamageDirection;
			}
			else if (DamageType == DamegePacketType.Legs)
			{
				DamageDirection = Vector3.Lerp(DamageDirection, Vector3.Zero, 0.075f);
				ref Vector3 reference11 = ref VerticesForces[3];
				reference11 = DamageDirection;
				ref Vector3 reference12 = ref VerticesForces[4];
				reference12 = DamageDirection;
				ref Vector3 reference13 = ref VerticesForces[5];
				reference13 = DamageDirection;
				ref Vector3 reference14 = ref VerticesForces[6];
				reference14 = DamageDirection;
			}
			for (int j = 0; j < 13; j++)
			{
				tmpVertice = Vertices[j];
				Vertices[j] += (Vertices[j] - VerticesLast[j] + VerticesForces[j] * 0.01f * 0.01f) * VerticesFriction[j];
				ref Vector3 reference15 = ref VerticesLast[j];
				reference15 = tmpVertice;
				VerticesForces[j].X = 0f;
				VerticesForces[j].Y = -256f;
				VerticesForces[j].Z = 0f;
			}
			hitNormal = Vector3.Zero;
			if (i == 1)
			{
				tmpRDSphere.Radius = 90f;
				tmpRDSphere.Center = Vertices[0];
				LevelOutside.GetSphereIntersectList(ref tmpRDSphere);
				for (int k = 0; k < 13; k++)
				{
					tmpRDSphere.Radius = VerticesRadius[k];
					tmpRDSphere.Center = Vertices[k];
					float num2 = LevelOutside.RagdollSphereIntersectList(ref tmpRDSphere, ref hitNormal);
					if (num2 > 0f)
					{
						num2 = ((1f - num2 > 0f) ? (1f - num2) : 0.075f);
						num2 = ((num2 > 1f) ? 1f : num2);
						VerticesFriction[k] = num2;
					}
					else
					{
						VerticesFriction[k] = 1f;
					}
					if (tmpRDSphere.Center.Y < -1000f)
					{
						tmpRDSphere.Center.Y = -1000f;
					}
					ref Vector3 reference16 = ref Vertices[k];
					reference16 = tmpRDSphere.Center;
				}
				for (int l = 0; l < 13; l++)
				{
					float height = HeightMapPhysics.GetHeight(ref Vertices[l]);
					if (height > Vertices[l].Y - VerticesRadius[l])
					{
						VerticesFriction[l] = 0.1f;
						Vertices[l].Y = height + VerticesRadius[l];
					}
				}
			}
			upPelvis = Vector3.Zero;
			for (int m = 0; m < 3; m++)
			{
				upPelvis += Vertices[m];
			}
			upPelvis /= 3f;
			upPelvis = Vertices[2] - upPelvis;
			upPelvis.Normalize();
			rightPelvis = Vertices[1] - Vertices[0];
			rightPelvis.Normalize();
			forward = Vector3.Cross(upPelvis, rightPelvis);
			float num3 = 0f;
			for (int n = 0; n < 1; n++)
			{
				for (int num4 = 0; num4 < 18; num4++)
				{
					tmpDelta = Vertices[Springs[num4].Vertex0] - Vertices[Springs[num4].Vertex1];
					num3 = tmpDelta.X * tmpDelta.X + tmpDelta.Y * tmpDelta.Y + tmpDelta.Z * tmpDelta.Z;
					tmpDelta *= Springs[num4].RestLengthSqr / (num3 + Springs[num4].RestLengthSqr) - 0.5f;
					Vertices[Springs[num4].Vertex0] += tmpDelta;
					Vertices[Springs[num4].Vertex1] -= tmpDelta;
				}
			}
			for (int num5 = 0; num5 < 1; num5++)
			{
				for (int num6 = 0; num6 < 11; num6++)
				{
					tmpDelta = Vertices[Constraints[num6].Vertex0] - Vertices[Constraints[num6].Vertex1];
					num3 = tmpDelta.X * tmpDelta.X + tmpDelta.Y * tmpDelta.Y + tmpDelta.Z * tmpDelta.Z;
					if (num3 < Constraints[num6].RestLengthMinSqr)
					{
						tmpDelta *= Constraints[num6].RestLengthMinSqr / (num3 + Constraints[num6].RestLengthMinSqr) - 0.5f;
						tmpDelta *= num;
						Vertices[Constraints[num6].Vertex0] += tmpDelta;
						Vertices[Constraints[num6].Vertex1] -= tmpDelta;
					}
					else if (num3 > Constraints[num6].RestLengthMaxSqr)
					{
						tmpDelta *= Constraints[num6].RestLengthMaxSqr / (num3 + Constraints[num6].RestLengthMaxSqr) - 0.5f;
						tmpDelta *= num;
						Vertices[Constraints[num6].Vertex0] += tmpDelta;
						Vertices[Constraints[num6].Vertex1] -= tmpDelta;
					}
				}
			}
			float planeD = Vector3.Dot(forward, Vertices[2]);
			float num7 = DistanceToPlane(planeD, ref forward, ref Vertices[3]);
			float num8 = DistanceToPlane(planeD, ref forward, ref Vertices[5]);
			if (num7 > 0f)
			{
				Vertices[0] += forward * num7;
				Vertices[3] -= forward * num7;
			}
			if (num8 > 0f)
			{
				Vertices[1] += forward * num8;
				Vertices[5] -= forward * num8;
			}
			float result = 0f;
			rightThigh = RagdollWorldPose[1].Right;
			rightThigh.Normalize();
			upKnee = Vertices[3] - Vertices[4];
			upKnee.Normalize();
			Vector3.Dot(ref rightThigh, ref upKnee, out result);
			Vertices[3] -= rightThigh * result;
			Vertices[4] += rightThigh * result;
			forwardThigh = RagdollWorldPose[1].Forward;
			forwardThigh.Normalize();
			Vector3.Dot(ref forwardThigh, ref upKnee, out result);
			if (result > -0.4f)
			{
				result += 0.4f;
				Vertices[3] -= forwardThigh * result;
				Vertices[4] += forwardThigh * result;
			}
			rightThigh = RagdollWorldPose[5].Right;
			rightThigh.Normalize();
			upKnee = Vertices[5] - Vertices[6];
			upKnee.Normalize();
			Vector3.Dot(ref rightThigh, ref upKnee, out result);
			Vertices[5] -= rightThigh * result;
			Vertices[6] += rightThigh * result;
			forwardThigh = RagdollWorldPose[5].Forward;
			forwardThigh.Normalize();
			Vector3.Dot(ref forwardThigh, ref upKnee, out result);
			if (result > -0.4f)
			{
				result += 0.4f;
				Vertices[5] -= forwardThigh * result;
				Vertices[6] += forwardThigh * result;
			}
			torsoRight = RagdollWorldPose[11].Right;
			torsoRight.Normalize();
			rightArm = Vertices[7] - Vertices[11];
			rightArm.Normalize();
			float num9 = Vector3.Dot(rightArm, torsoRight);
			if (num9 > 0f)
			{
				Vertices[7] -= torsoRight * num9;
				Vertices[11] += torsoRight * num9;
			}
			leftArm = Vertices[8] - Vertices[12];
			leftArm.Normalize();
			float num10 = Vector3.Dot(leftArm, torsoRight);
			if (num10 <= 0f)
			{
				Vertices[8] -= torsoRight * num10;
				Vertices[12] += torsoRight * num10;
			}
			rightArmRight = RagdollWorldPose[14].Right;
			rightArmRight.Normalize();
			rightElbowDown = Vertices[9] - Vertices[7];
			rightElbowDown.Normalize();
			float num11 = Vector3.Dot(rightArmRight, rightElbowDown);
			Vertices[7] += rightArmRight * num11;
			Vertices[9] -= rightArmRight * num11;
			rightArmForward = RagdollWorldPose[14].Forward;
			rightArmForward.Normalize();
			num11 = Vector3.Dot(rightArmForward, rightElbowDown);
			if (num11 > 0f)
			{
				Vertices[7] += rightArmForward * num11;
				Vertices[9] -= rightArmForward * num11;
			}
			leftArmRight = RagdollWorldPose[17].Right;
			leftArmRight.Normalize();
			leftElbowDown = Vertices[10] - Vertices[8];
			leftElbowDown.Normalize();
			float num12 = Vector3.Dot(leftArmRight, leftElbowDown);
			Vertices[8] += leftArmRight * num12;
			Vertices[10] -= leftArmRight * num12;
			leftArmForward = RagdollWorldPose[17].Forward;
			leftArmForward.Normalize();
			num12 = Vector3.Dot(leftArmForward, leftElbowDown);
			if (num12 > 0f)
			{
				Vertices[8] += leftArmForward * num12;
				Vertices[10] -= leftArmForward * num12;
			}
			upPelvis = Vector3.Zero;
			for (int num13 = 0; num13 < 3; num13++)
			{
				upPelvis += Vertices[num13];
			}
			upPelvis /= 3f;
			upPelvis = Vertices[2] - upPelvis;
			upPelvis.Normalize();
			forward = Vector3.Cross(upPelvis, rightPelvis);
			ref Matrix reference17 = ref RagdollWorldPose[0];
			reference17 = RagdollBonePose[0] * RagdollWorldTransform;
			pelvisPos = (Vertices[1] + Vertices[0]) * 0.5f;
			pelvisOffset = RagdollWorldPose[0].Translation - pelvisPos;
			matWorldTrans = Matrix.Identity;
			matWorldTrans.Right = rightPelvis;
			matWorldTrans.Forward = forward;
			matWorldTrans.Up = upPelvis;
			matWorldTrans *= Matrix.CreateScale(1.8f);
			matWorldTrans.Translation = RagdollWorldPose[0].Translation - pelvisOffset;
			ref Matrix reference18 = ref RagdollWorldPose[0];
			reference18 = matWorldTrans;
		}
		for (int num14 = 1; num14 < RagdollWorldPose.Length; num14++)
		{
			int num15 = SkinData.SkeletonHierarchy[num14];
			switch (num14)
			{
			case 10:
			{
				ref Matrix reference36 = ref RagdollWorldPose[num14];
				reference36 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				tl = Vertices[Springs[13].Vertex0] - Vertices[Springs[13].Vertex1];
				tl.Normalize();
				tu = (Vertices[2] + Vertices[11] + Vertices[12]) / 3f;
				tu -= Vertices[2];
				tu.Normalize();
				tf = Vector3.Cross(tl, tu);
				tf.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Left = tl;
				mBoneTrans.Forward = tf;
				mBoneTrans.Up = tu;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference37 = ref RagdollWorldPose[num14];
				reference37 = mBoneTrans;
				break;
			}
			case 14:
			{
				ref Matrix reference34 = ref RagdollWorldPose[num14];
				reference34 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				td = Vertices[7] - RagdollWorldPose[num14].Translation;
				td.Normalize();
				tr = Vertices[1] - Vertices[7];
				tr.Normalize();
				tf = Vector3.Cross(td, tr);
				tf.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Left = tr * -1f;
				mBoneTrans.Forward = tf * -1f;
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference35 = ref RagdollWorldPose[num14];
				reference35 = mBoneTrans;
				break;
			}
			case 17:
			{
				ref Matrix reference32 = ref RagdollWorldPose[num14];
				reference32 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				td = Vertices[8] - RagdollWorldPose[num14].Translation;
				td.Normalize();
				tr = Vertices[0] - Vertices[8];
				tr.Normalize();
				tf = Vector3.Cross(td, tr);
				tf.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Left = tr * 1f;
				mBoneTrans.Backward = tf * -1f;
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference33 = ref RagdollWorldPose[num14];
				reference33 = mBoneTrans;
				break;
			}
			case 15:
			{
				ref Matrix reference30 = ref RagdollWorldPose[num14];
				reference30 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				td = Vertices[9] - Vertices[7];
				td.Normalize();
				tr = RagdollWorldPose[num15].Right;
				tr.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Forward = Vector3.Cross(td, tr) * -1f;
				mBoneTrans.Right = tr;
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference31 = ref RagdollWorldPose[num14];
				reference31 = mBoneTrans;
				break;
			}
			case 18:
			{
				ref Matrix reference28 = ref RagdollWorldPose[num14];
				reference28 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				td = Vertices[10] - Vertices[8];
				td.Normalize();
				tr = RagdollWorldPose[num15].Right;
				tr.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Forward = Vector3.Cross(td, tr) * -1f;
				mBoneTrans.Right = tr;
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference29 = ref RagdollWorldPose[num14];
				reference29 = mBoneTrans;
				break;
			}
			case 6:
			{
				ref Matrix reference26 = ref RagdollWorldPose[num14];
				reference26 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				tr = RagdollWorldPose[num14].Right;
				tr.Normalize();
				td = Vertices[6] - Vertices[5];
				td.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Right = tr;
				mBoneTrans.Forward = Vector3.Cross(tr, td);
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference27 = ref RagdollWorldPose[num14];
				reference27 = mBoneTrans;
				break;
			}
			case 2:
			{
				ref Matrix reference24 = ref RagdollWorldPose[num14];
				reference24 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				tr = RagdollWorldPose[num14].Right;
				tr.Normalize();
				td = Vertices[4] - Vertices[3];
				td.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Right = tr;
				mBoneTrans.Forward = Vector3.Cross(tr, td);
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference25 = ref RagdollWorldPose[num14];
				reference25 = mBoneTrans;
				break;
			}
			case 1:
			{
				ref Matrix reference22 = ref RagdollWorldPose[num14];
				reference22 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				td = Vertices[Springs[3].Vertex1] - RagdollWorldPose[num14].Translation;
				td.Normalize();
				tr = Vertices[0] - Vertices[1];
				tr.Normalize();
				tf = Vector3.Cross(td, tr);
				tf.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Left = Vector3.Cross(tf, td);
				mBoneTrans.Forward = tf;
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference23 = ref RagdollWorldPose[num14];
				reference23 = mBoneTrans;
				break;
			}
			case 5:
			{
				ref Matrix reference20 = ref RagdollWorldPose[num14];
				reference20 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				td = Vertices[Springs[5].Vertex1] - RagdollWorldPose[num14].Translation;
				td.Normalize();
				tr = Vertices[0] - Vertices[1];
				tr.Normalize();
				tf = Vector3.Cross(td, tr);
				tf.Normalize();
				mBoneTrans = Matrix.Identity;
				mBoneTrans.Left = Vector3.Cross(tf, td);
				mBoneTrans.Forward = tf;
				mBoneTrans.Down = td;
				mBoneTrans *= Matrix.CreateScale(1.8f);
				mBoneTrans.Translation = RagdollWorldPose[num14].Translation;
				ref Matrix reference21 = ref RagdollWorldPose[num14];
				reference21 = mBoneTrans;
				break;
			}
			default:
			{
				ref Matrix reference19 = ref RagdollWorldPose[num14];
				reference19 = RagdollBonePose[num14] * RagdollWorldPose[num15];
				break;
			}
			}
		}
		for (int num16 = 0; num16 < RagdollWorldPose.Length; num16++)
		{
			ref Matrix reference38 = ref RagdollSkinPose[num16];
			reference38 = SkinData.InverseBindPose[num16] * RagdollWorldPose[num16];
		}
	}

	public void Draw(int qIndex)
	{
	}

	public void Spawn(Matrix world, Matrix[] pose, Matrix[] poseLast)
	{
		RagdollWorldTransformSpawn = world;
		for (int i = 0; i < pose.Length; i++)
		{
			ref Matrix reference = ref RagdollBonePoseSpawn[i];
			reference = pose[i];
		}
		for (int j = 0; j < poseLast.Length; j++)
		{
			ref Matrix reference2 = ref RagdollBonePoseSpawnLast[j];
			reference2 = poseLast[j];
		}
		IsValid = true;
		SetRagdoll = false;
		UpdateVertices = true;
	}

	public void UpdateSpawn()
	{
		for (int i = 0; i < RagdollBonePoseSpawn.Length; i++)
		{
			if (i != 7 && i != 19 && i != 8 && i != 12 && i != 3 && i != 16 && i != 4 && i != 9 && i != 11)
			{
				ref Matrix reference = ref RagdollBonePose[i];
				reference = RagdollBonePoseSpawn[i];
			}
		}
		Position = RagdollWorldTransformSpawn.Translation;
		RagdollWorldTransform = RagdollWorldTransformSpawn;
		ref Matrix reference2 = ref RagdollWorldPose[0];
		reference2 = RagdollBonePoseSpawnLast[0] * RagdollWorldTransform;
		for (int j = 1; j < RagdollWorldPose.Length; j++)
		{
			int num = SkinData.SkeletonHierarchy[j];
			ref Matrix reference3 = ref RagdollWorldPose[j];
			reference3 = RagdollBonePoseSpawnLast[j] * RagdollWorldPose[num];
		}
		Vector3 right = RagdollWorldPose[0].Right;
		Vector3 up = RagdollWorldPose[0].Up;
		Vector3 vector = RagdollWorldPose[0].Forward;
		right.Normalize();
		up.Normalize();
		vector.Normalize();
		ref Vector3 reference4 = ref VerticesLast[0];
		reference4 = RagdollWorldPose[0].Translation + right * -12f;
		ref Vector3 reference5 = ref VerticesLast[1];
		reference5 = RagdollWorldPose[0].Translation + right * 12f;
		ref Vector3 reference6 = ref VerticesLast[2];
		reference6 = RagdollWorldPose[10].Translation;
		ref Vector3 reference7 = ref VerticesLast[3];
		reference7 = RagdollWorldPose[2].Translation;
		ref Vector3 reference8 = ref VerticesLast[4];
		reference8 = RagdollWorldPose[3].Translation;
		ref Vector3 reference9 = ref VerticesLast[5];
		reference9 = RagdollWorldPose[6].Translation;
		ref Vector3 reference10 = ref VerticesLast[6];
		reference10 = RagdollWorldPose[7].Translation;
		ref Vector3 reference11 = ref VerticesLast[7];
		reference11 = RagdollWorldPose[15].Translation;
		ref Vector3 reference12 = ref VerticesLast[8];
		reference12 = RagdollWorldPose[18].Translation;
		ref Vector3 reference13 = ref VerticesLast[9];
		reference13 = RagdollWorldPose[16].Translation;
		ref Vector3 reference14 = ref VerticesLast[10];
		reference14 = RagdollWorldPose[19].Translation;
		ref Vector3 reference15 = ref VerticesLast[11];
		reference15 = RagdollWorldPose[14].Translation;
		ref Vector3 reference16 = ref VerticesLast[12];
		reference16 = RagdollWorldPose[17].Translation;
		ref Matrix reference17 = ref RagdollWorldPose[0];
		reference17 = RagdollBonePose[0] * RagdollWorldTransform;
		for (int k = 1; k < RagdollWorldPose.Length; k++)
		{
			int num2 = SkinData.SkeletonHierarchy[k];
			ref Matrix reference18 = ref RagdollWorldPose[k];
			reference18 = RagdollBonePose[k] * RagdollWorldPose[num2];
		}
		right = RagdollWorldPose[0].Right;
		up = RagdollWorldPose[0].Up;
		vector = RagdollWorldPose[0].Forward;
		right.Normalize();
		up.Normalize();
		vector.Normalize();
		ref Vector3 reference19 = ref Vertices[0];
		reference19 = RagdollWorldPose[0].Translation + right * -12f;
		ref Vector3 reference20 = ref Vertices[1];
		reference20 = RagdollWorldPose[0].Translation + right * 12f;
		ref Vector3 reference21 = ref Vertices[2];
		reference21 = RagdollWorldPose[10].Translation;
		PelvisPosition = RagdollWorldPose[0].Translation;
		ref Vector3 reference22 = ref Vertices[3];
		reference22 = RagdollWorldPose[2].Translation;
		ref Vector3 reference23 = ref Vertices[4];
		reference23 = RagdollWorldPose[3].Translation;
		ref Vector3 reference24 = ref Vertices[5];
		reference24 = RagdollWorldPose[6].Translation;
		ref Vector3 reference25 = ref Vertices[6];
		reference25 = RagdollWorldPose[7].Translation;
		ref Vector3 reference26 = ref Vertices[7];
		reference26 = RagdollWorldPose[15].Translation;
		ref Vector3 reference27 = ref Vertices[8];
		reference27 = RagdollWorldPose[18].Translation;
		ref Vector3 reference28 = ref Vertices[9];
		reference28 = RagdollWorldPose[16].Translation;
		ref Vector3 reference29 = ref Vertices[10];
		reference29 = RagdollWorldPose[19].Translation;
		ref Vector3 reference30 = ref Vertices[11];
		reference30 = RagdollWorldPose[14].Translation;
		ref Vector3 reference31 = ref Vertices[12];
		reference31 = RagdollWorldPose[17].Translation;
		for (int l = 0; l < RagdollWorldPose.Length; l++)
		{
			ref Matrix reference32 = ref RagdollSkinPose[l];
			reference32 = SkinData.InverseBindPose[l] * RagdollWorldPose[l];
		}
		for (int m = 0; m < 13; m++)
		{
			ref Vector3 reference33 = ref VerticesForces[m];
			reference33 = Vector3.UnitY * -128f;
			VerticesFriction[m] = 0.1f;
		}
		IsValid = true;
		SetRagdoll = false;
		SleepTimer = 0f;
	}

	private float DistanceToPlane(float planeD, ref Vector3 planeN, ref Vector3 p)
	{
		float result = 0f;
		Vector3.Dot(ref p, ref planeN, out result);
		return result - planeD;
	}
}
