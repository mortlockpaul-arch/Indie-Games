using System;
using System.Collections.Generic;
using _000E;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using N;
using P;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Rendering;
using r;

namespace x;

internal class _7 : ICollisionEntity, h, IDisposable
{
	private struct _00065h
	{
		public int LastCollisionId;

		public ICollisionMaterial Material;

		public _00065h(ICollisionMaterial material)
		{
			if (material != null)
			{
				LastCollisionId = material.CollisionId;
			}
			else
			{
				LastCollisionId = 0;
			}
			Material = material;
		}
	}

	private int a5h;

	private int a5b;

	private r.a a56;

	private ICollisionObject a5a;

	private Dictionary<r.h, _00065h> a57 = new Dictionary<r.h, _00065h>();

	private Dictionary<r.h, _00065h> a5_0006 = new Dictionary<r.h, _00065h>();

	public float Distance => 0f;

	public Vector3 Normal => Vector3.Zero;

	public ICollisionObject Object => a5a;

	public _7(ICollisionObject obj)
	{
		a5a = obj;
	}

	public void Build(r.a space)
	{
		Dispose();
		a56 = space;
		SyncToPhysicsEntity();
	}

	public void GetSpaceObjects(List<r.h> spaceobjects)
	{
		foreach (KeyValuePair<r.h, _00065h> item in a57)
		{
			spaceobjects.Add(item.Key);
		}
	}

	public virtual void ReaddSpaceObjectsToSpace(r.a space)
	{
		a57.Clear();
		foreach (KeyValuePair<r.h, _00065h> item in a5_0006)
		{
			r.h key = item.Key;
			if (key.Space != null)
			{
				key.Space.Remove(key);
			}
			space.Add(key);
			a57.Add(key, item.Value);
		}
		a5_0006.Clear();
	}

	public virtual void RemoveSpaceObjectsFromSpace()
	{
		a5_0006.Clear();
		foreach (KeyValuePair<r.h, _00065h> item in a57)
		{
			r.h key = item.Key;
			if (key.Space != null)
			{
				key.Space.Remove(key);
			}
			a5_0006.Add(key, item.Value);
		}
		a57.Clear();
	}

	public void Dispose()
	{
		RemoveSpaceObjectsFromSpace();
	}

	public void ApplyWorldForce(ref Vector3 worldforce)
	{
	}

	public void ApplyWorldForce(ref Vector3 worldposition, ref Vector3 worldforce)
	{
	}

	public void RemoveForces()
	{
	}

	private void bR(RenderableMesh P_0)
	{
		ICollisionMaterial collisionMaterial = P_0.Effect as ICollisionMaterial;
		GeometryData meshData = GeometryExtractionHelper.GetMeshData(P_0);
		Vector3[] array = meshData.Vertices.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = Vector3.Transform(array[i], P_0.World);
		}
		P._7 obj = new P._7(array, meshData.Indices.ToArray());
		if (collisionMaterial != null)
		{
			obj.Material.Bounciness = 1f - collisionMaterial.Elasticity;
			obj.Material.KineticFriction = collisionMaterial.Friction;
			obj.Material.StaticFriction = collisionMaterial.Friction;
		}
		obj.Tag = this;
		a56.Add(obj);
		a57.Add(obj, new _00065h(collisionMaterial));
	}

	private void b_0003(RenderableMesh P_0, BaseTerrainEffect P_1)
	{
		Texture2D heightMapTexture = P_1.HeightMapTexture;
		if (heightMapTexture == null)
		{
			return;
		}
		float num = 1f / P_1.Tiling;
		float num2 = (float)P_1.MeshSegments * num * 2f + 1f;
		int num3 = (int)num2;
		float num4 = num2 / (float)num3;
		int width = heightMapTexture.Width;
		int height = heightMapTexture.Height;
		float[,] array = new float[num3, num3];
		HalfSingle[] array2 = new HalfSingle[heightMapTexture.Height * heightMapTexture.Width];
		heightMapTexture.GetData(array2);
		float num5 = 1f / ((float)num3 - 1f);
		for (int i = 0; i < num3; i++)
		{
			for (int j = 0; j < num3; j++)
			{
				float num6 = (float)j * num5 + 0.0001f;
				float num7 = (float)i * num5 + 0.0001f;
				int num8 = (int)(num6 * (float)width) % width;
				int num9 = (int)(num7 * (float)height) % height;
				int num10 = num8 + num9 * width;
				array[j, i] = array2[num10].ToSingle();
			}
		}
		_000E.v shape = new _000E.v(array, _000E.B.BottomLeftUpperRight);
		N.h worldTransform = default(N.h);
		Matrix matrix = new Matrix
		{
			M11 = 1f,
			M23 = 1f,
			M32 = 1f,
			M44 = 1f
		};
		float num11 = 0.5f / (float)P_1.MeshSegments * num4;
		Matrix matrix2 = Matrix.CreateScale(num11, P_1.HeightScale, num11) * matrix * Matrix.CreateTranslation(-0.5f * num, -0.5f * num, 0f);
		int tileRepeatCount = P_1.TileRepeatCount;
		float tileWidth = P_1.GetTileWidth();
		int num12 = (int)Math.Ceiling((float)tileRepeatCount * 0.5f) - 1;
		for (int k = 0; k < tileRepeatCount; k++)
		{
			for (int m = 0; m < tileRepeatCount; m++)
			{
				Matrix matrix3 = Matrix.CreateTranslation((float)(m - num12) * tileWidth, (float)(k - num12) * tileWidth, 0f);
				worldTransform.Matrix = matrix2 * matrix3 * P_0.World;
				P._0006 obj = new P._0006(shape, worldTransform);
				if (P_0.Effect is ICollisionMaterial collisionMaterial)
				{
					obj.Material.Bounciness = 1f - collisionMaterial.Elasticity;
					obj.Material.KineticFriction = collisionMaterial.Friction;
					obj.Material.StaticFriction = collisionMaterial.Friction;
				}
				obj.Tag = this;
				a56.Add(obj);
				a57.Add(obj, new _00065h(P_1));
			}
		}
	}

	public virtual bool CheckSceneObjectChanged()
	{
		foreach (KeyValuePair<r.h, _00065h> item in a57)
		{
			_00065h value = item.Value;
			if (value.Material != null && value.Material.CollisionId != value.LastCollisionId)
			{
				return true;
			}
		}
		if (a5a.MoveId == a5h)
		{
			return a5a.CollisionId != a5b;
		}
		return true;
	}

	public void SyncToPhysicsEntity()
	{
		Dispose();
		if (!(a5a is ISceneObject sceneObject))
		{
			return;
		}
		foreach (RenderableMesh renderableMesh in sceneObject.RenderableMeshes)
		{
			if (renderableMesh.Effect is BaseTerrainEffect baseTerrainEffect)
			{
				b_0003(renderableMesh, baseTerrainEffect);
			}
			else
			{
				bR(renderableMesh);
			}
		}
		a5h = a5a.MoveId;
		a5b = a5a.CollisionId;
	}

	public void SyncToSceneObject()
	{
		a5h = a5a.MoveId;
	}
}
