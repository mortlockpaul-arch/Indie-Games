using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct TriggerData
{
	public static bool TargetsActive;

	public static int TargetsHit;

	public bool Tiggered;

	public TriggerTypes eType;

	public OOBB oobb;

	public Vector3 direction;

	public Vector3 center;

	public int SegmentIndex;

	public string SegmentName;

	public TargetPracticeStruct[] targets;

	public bool ProcessTrigger()
	{
		if (eType == TriggerTypes.TargetPractice)
		{
			if (!Tiggered)
			{
				Tiggered = true;
				if (targets != null)
				{
					for (int i = 0; i < targets.Length; i++)
					{
						targets[i].TargetAngle = 0f;
						targets[i].Active = true;
					}
				}
			}
			return true;
		}
		return false;
	}

	public void AddTargets(Model e, ref Matrix[] t, string n)
	{
	}

	public void Reset()
	{
		SegmentIndex = -1;
		SegmentName = "null";
		Tiggered = false;
		TargetsHit = 0;
		eType = TriggerTypes.Undeclared;
		if (targets != null)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i].CurrentAngle = 0f;
				targets[i].TargetAngle = (float)Math.PI / 2f;
				targets[i].TargetOffset = Vector3.UnitY * -8f;
				targets[i].TargetTimer = (float)EndGameEngine.randGenerator.NextDouble() * 0.5f;
				targets[i].NumberHits = 0;
				targets[i].Active = false;
			}
		}
	}

	public void ReSpawn()
	{
		Tiggered = false;
		TargetsHit = 0;
		if (targets != null)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i].CurrentAngle = (float)Math.PI / 2f;
				targets[i].TargetAngle = (float)Math.PI / 2f;
				targets[i].TargetOffset = Vector3.UnitY * -8f;
				targets[i].TargetTimer = (float)EndGameEngine.randGenerator.NextDouble() * 0.5f;
				targets[i].NumberHits = 0;
				targets[i].Active = false;
			}
		}
	}

	public void Update(int qIndex, float eTimeMS)
	{
		if (eType != TriggerTypes.TargetPractice || !Tiggered || !TargetsActive || targets == null)
		{
			return;
		}
		for (int i = 0; i < targets.Length; i++)
		{
			targets[i].TargetTimer -= eTimeMS;
			if (targets[i].TargetTimer <= 0f)
			{
				targets[i].CurrentAngle = MathHelper.Lerp(targets[i].CurrentAngle, targets[i].TargetAngle, eTimeMS * 8f);
				targets[i].TargetOffset = Vector3.Lerp(targets[i].TargetOffset, Vector3.Zero, eTimeMS * 8f);
			}
		}
	}

	public void Draw(ref Matrix view, ref Matrix proj, ref Matrix textureProj)
	{
		if (!TargetsActive || LevelBaseMenu.gameMode != GameMode.CombatTraining || targets == null)
		{
			return;
		}
		for (int i = 0; i < targets.Length; i++)
		{
			if ((targets[i].TriMesh.Flags & GeometryFlags.Renderable) > GeometryFlags.Clear)
			{
				for (int j = 0; j < targets[i].model.MeshParts.Count; j++)
				{
					float currentAngle = targets[i].CurrentAngle;
					Vector3 targetOffset = targets[i].TargetOffset;
					eMeshPart eMeshPart2 = targets[i].model.MeshParts[j];
					eMeshPart2.Effect.GraphicsDevice.SetVertexBuffer(eMeshPart2.VertexBuffer, eMeshPart2.VertexOffset);
					eMeshPart2.Effect.GraphicsDevice.Indices = eMeshPart2.IndexBuffer;
					Vector3 zero = Vector3.Zero;
					zero.X = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition.X;
					zero.Y = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition.Y + 20f;
					zero.Z = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition.Z;
					((EffectParams)eMeshPart2.Tag).eyePosition.SetValue(zero);
					((EffectParams)eMeshPart2.Tag).matView.SetValue(view);
					((EffectParams)eMeshPart2.Tag).matTexProj.SetValue(textureProj);
					((EffectParams)eMeshPart2.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
					((EffectParams)eMeshPart2.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
					((EffectParams)eMeshPart2.Tag).vecMuzzleFlash.SetValue(particles.MuzzleFlash());
					Matrix transform = targets[i].transform;
					Vector3 translation = transform.Translation;
					transform.Translation = Vector3.Zero;
					transform = Matrix.CreateFromAxisAngle(Vector3.UnitX, currentAngle) * transform;
					Vector3.Transform(targets[i].TriMesh.oobb.Min, transform);
					transform.Translation = translation - targetOffset;
					((EffectParams)eMeshPart2.Tag).matViewProj.SetValue(transform * view * proj);
					eMeshPart2.Effect.CurrentTechnique.Passes[0].Apply();
					eMeshPart2.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, eMeshPart2.NumVertices, eMeshPart2.StartIndex, eMeshPart2.PrimitiveCount);
				}
			}
		}
	}

	public bool IntersectFPSCharacter(ref BoundingSphere e)
	{
		bool flag = false;
		if (LevelBaseMenu.gameMode == GameMode.CombatTraining && eType == TriggerTypes.TargetPractice && Tiggered && TargetsActive && targets != null)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i].Active)
				{
					flag = targets[i].PhysicsBox.IntersectSphere(ref e);
					if (flag)
					{
						return flag;
					}
				}
			}
		}
		return false;
	}

	public bool CollisionSphere(ref BoundingSphere e, ref CollisionStruct c)
	{
		if (TargetsActive && targets != null)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i].Active && (targets[i].TriMesh.Flags & GeometryFlags.Renderable) == 0)
				{
					for (int j = 0; j < targets[i].model.MeshParts.Count; j++)
					{
						_ = targets[i].CurrentAngle;
						_ = targets[i].TargetOffset;
						eMeshPart eMeshPart2 = targets[i].model.MeshParts[j];
						eMeshPart2.Effect.GraphicsDevice.SetVertexBuffer(eMeshPart2.VertexBuffer, eMeshPart2.VertexOffset);
						eMeshPart2.Effect.GraphicsDevice.Indices = eMeshPart2.IndexBuffer;
						_ = Vector3.Zero;
					}
				}
			}
		}
		return false;
	}

	public float RayCast(ref IntersectSegmentParams segParams, bool sqrRootResult)
	{
		float num = 1E+10f;
		if (TargetsActive && targets != null)
		{
			bool onlyWalkable = segParams.OnlyWalkable;
			segParams.OnlyWalkable = true;
			for (int i = 0; i < targets.Length; i++)
			{
				if (!targets[i].Active || targets[i].TriMesh.triangleMesh == null)
				{
					continue;
				}
				for (int j = 0; j < targets[i].TriMesh.triangleMesh.Length; j++)
				{
					if (MyMath.IntersectSegmentTriangle(ref segParams, ref targets[i].TriMesh.triangleMesh[j]))
					{
						float num2 = (segParams.hitPosition - segParams.SegmentStart).LengthSquared();
						if (num2 < num)
						{
							num = num2;
							segParams.TargetIndex = i;
							j = targets[i].TriMesh.triangleMesh.Length;
						}
					}
				}
			}
			segParams.OnlyWalkable = onlyWalkable;
		}
		return num;
	}

	public void ApplyHitOnTarget(int targetIndex)
	{
		if (targets != null && targetIndex >= 0 && targetIndex < targets.Length)
		{
			targets[targetIndex].NumberHits++;
			if (targets[targetIndex].NumberHits > 0)
			{
				TargetsHit++;
				targets[targetIndex].Active = false;
				targets[targetIndex].TargetAngle = (float)Math.PI / 2f;
			}
		}
	}
}
