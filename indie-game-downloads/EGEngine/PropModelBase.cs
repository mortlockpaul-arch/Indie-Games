using System;
using System.Collections.ObjectModel;
using System.IO;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropModel;

namespace EGEngine;

public class PropModelBase
{
	public enum UserObjectTypes
	{
		RenderNoShadowZeroY = 12,
		NumberOf
	}

	public struct tmpVertCollisionStrcut
	{
		public Vector3 pos;
	}

	private static int ModelMaterialDebugCount;

	private static ushort uid;

	public static Vector3 ObjectSpaceUpVector;

	public Matrix[] matWorld;

	public int LODIndex;

	public int ShaderPass;

	public Model propModel;

	public bool[] InFrustum;

	public Matrix[] propTransforms;

	public int numPropBoundingSphere;

	public BoundingSphere[] propBoundingSphere;

	private static Vector4 vecUVOffset;

	protected bool Loaded;

	private static Vector3 UnitNegY;

	public static float RayCastDist;

	public static bool TestRayCast;

	public static bool RayCastCollision;

	private static Vector3 p;

	private static Vector3 q;

	private static Vector3 s;

	private static Vector3 p1;

	private static Vector3 p2;

	private static Vector3 p3;

	private static Vector3 e1;

	private static Vector3 e2;

	private static Vector3 norm;

	private static Vector3 ab;

	private static Vector3 ac;

	private static Vector3 ap;

	private static Vector3 bp;

	private static Vector3 cp;

	private static BoundingSphere tmpSphere;

	private static Vector3 SphereColSphereCenter;

	private static Vector3 SphereColClosestPoint;

	private static Vector3 SphereColp1;

	private static Vector3 SphereColp2;

	private static Vector3 SphereColp3;

	private static Vector3 SphereCole1;

	private static Vector3 SphereCole2;

	protected static Vector3 collisionPos;

	protected static Vector3 collisionDir;

	protected static Vector3 eyePosition;

	protected static Matrix tmpMatWorld;

	protected static Matrix matViewProj;

	protected static ModelMesh drawMesh;

	protected static ModelMeshPart drawMeshPart;

	private static Vector3 tmpVecZeroY;

	private Matrix playerView;

	private Matrix playerProj;

	public static ushort UniqueId
	{
		get
		{
			uid++;
			return uid;
		}
		set
		{
		}
	}

	public virtual void Load(string n)
	{
		Load(n, 1f);
	}

	public virtual void Load(string n, float scale)
	{
		if (Loaded)
		{
			return;
		}
		Loaded = true;
		ref Matrix reference = ref matWorld[0];
		reference = Matrix.Identity;
		ref Matrix reference2 = ref matWorld[1];
		reference2 = Matrix.Identity;
		propModel = EndGameEngine.GameAssetMgr.Load<Model>(n);
		if (ModelMaterialDebugCount < 60)
		{
			foreach (ModelMesh mesh in propModel.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					if (ModelMaterialDebugCount++ >= 60)
					{
						break;
					}
					string text = string.Join(", ", Array.ConvertAll(meshPart.VertexBuffer.VertexDeclaration.GetVertexElements(), (VertexElement e) => e.ToString()));
					File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-prop-materials.log"), "asset=" + n + " mesh=" + mesh.Name + " effect=" + meshPart.Effect.Name + " stride=" + meshPart.VertexBuffer.VertexDeclaration.VertexStride + " declaration=[" + text + "]" + Environment.NewLine);
					for (int num = 0; num < meshPart.Effect.Parameters.Count; num++)
					{
						EffectParameter effectParameter = meshPart.Effect.Parameters[num];
						if (effectParameter.ParameterType == EffectParameterType.Texture || effectParameter.ParameterType == EffectParameterType.Texture2D)
						{
							Texture2D valueTexture2D = effectParameter.GetValueTexture2D();
							if (valueTexture2D != null)
							{
								File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-prop-materials.log"), "  texture=" + effectParameter.Name + " size=" + valueTexture2D.Width + "x" + valueTexture2D.Height + Environment.NewLine);
							}
						}
					}
				}
			}
		}
		propTransforms = new Matrix[((ReadOnlyCollection<ModelBone>)propModel.Bones).Count];
		propModel.CopyAbsoluteBoneTransformsTo(propTransforms);
		for (int num2 = 0; num2 < propTransforms.Length; num2++)
		{
			ref Matrix reference3 = ref propTransforms[num2];
			reference3 = propTransforms[num2] * Matrix.CreateScale(scale);
		}
		for (int num3 = 0; num3 < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; num3++)
		{
			ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[num3];
			modelMesh.Tag = new MeshAttributesParams();
			for (int num4 = 0; num4 < ((ReadOnlyCollection<ModelMeshPart>)modelMesh.MeshParts).Count; num4++)
			{
				((ReadOnlyCollection<ModelMeshPart>)modelMesh.MeshParts)[num4].Tag = new PropEffectParams(((ReadOnlyCollection<ModelMeshPart>)modelMesh.MeshParts)[num4].Effect);
				if (modelMesh.Name.Contains("_LOD_"))
				{
					LODIndex = num3;
				}
			}
		}
		numPropBoundingSphere = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count;
		propBoundingSphere = new BoundingSphere[numPropBoundingSphere];
		for (int num5 = 0; num5 < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; num5++)
		{
			ref BoundingSphere reference4 = ref propBoundingSphere[num5];
			reference4 = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[num5].BoundingSphere;
			Vector3 center = Vector3.Transform(propBoundingSphere[num5].Center, propTransforms[((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[num5].ParentBone.Index]);
			Vector3 vector = Vector3.Transform(Vector3.UnitX * propBoundingSphere[num5].Radius, propTransforms[((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[num5].ParentBone.Index]);
			propBoundingSphere[num5].Center = center;
			propBoundingSphere[num5].Radius = vector.Length();
		}
	}

	public virtual bool SphereCollision(ref BoundingSphere sphere, int qIndex, ref bool onWalkable)
	{
		bool result = false;
		CollisionData collisionData = ((MeshUserData)propModel.Tag).collisionData;
		float num = collisionData.bSphere.Radius + sphere.Radius;
		num *= num;
		float num2 = (sphere.Center - collisionData.bSphere.Center).LengthSquared();
		if (TestRayCast)
		{
			num += RayCastDist * RayCastDist;
		}
		if (num2 <= num)
		{
			float num3 = float.MaxValue;
			float num4 = sphere.Radius * sphere.Radius;
			SphereColSphereCenter = sphere.Center;
			SphereColClosestPoint = Vector3.Zero;
			tmpSphere.Center = sphere.Center;
			tmpSphere.Radius = sphere.Radius + RayCastDist;
			if (collisionData.bBox.Intersects(tmpSphere))
			{
				int num5 = collisionData.indices.Length;
				for (int i = 0; i < num5; i += 3)
				{
					int num6 = collisionData.indices[i];
					int num7 = collisionData.indices[i + 1];
					int num8 = collisionData.indices[i + 2];
					p1 = collisionData.vertices[num6];
					p2 = collisionData.vertices[num7];
					p3 = collisionData.vertices[num8];
					float num9 = float.MaxValue;
					UnitNegY = ObjectSpaceUpVector * -1f;
					bool flag = false;
					num9 = 1000000f;
					e1 = p2 - p1;
					e2 = p3 - p1;
					p = Vector3.Cross(UnitNegY, e2);
					float num10 = Vector3.Dot(e1, p);
					if (Math.Abs(num10) >= 1E-06f)
					{
						float num11 = 1f / num10;
						s = SphereColSphereCenter - p1;
						float num12 = num11 * Vector3.Dot(s, p);
						if (num12 >= 0f && num12 <= 1f)
						{
							q = Vector3.Cross(s, e1);
							float num13 = num11 * Vector3.Dot(UnitNegY, q);
							if (num13 >= 0f && num13 + num12 <= 1f)
							{
								num9 = num11 * Vector3.Dot(e2, q);
								flag = num9 > 0f;
							}
						}
					}
					if ((TestRayCast & flag) && num9 < RayCastDist)
					{
						result = true;
						RayCastCollision = true;
						SphereColSphereCenter += ObjectSpaceUpVector * (RayCastDist - num9);
						e1 = p2 - p1;
						e2 = p3 - p1;
						Vector3.Cross(ref e2, ref e1, out norm);
						norm.Normalize();
						Vector3.Dot(norm, ObjectSpaceUpVector);
						onWalkable = true;
					}
					bool flag2 = false;
					e1 = p2 - p1;
					e2 = p3 - p1;
					norm = Vector3.Cross(e2, e1);
					norm.Normalize();
					s = p1 - SphereColSphereCenter;
					float num14 = Vector3.Dot(norm, s);
					if (num14 * num14 <= num4)
					{
						MyMath.ClosestPTPointTriangle(ref SphereColSphereCenter, ref p1, ref p2, ref p3, ref SphereColClosestPoint);
						SphereColClosestPoint.X = SphereColSphereCenter.X - SphereColClosestPoint.X;
						SphereColClosestPoint.Y = SphereColSphereCenter.Y - SphereColClosestPoint.Y;
						SphereColClosestPoint.Z = SphereColSphereCenter.Z - SphereColClosestPoint.Z;
						num3 = SphereColClosestPoint.LengthSquared();
						flag2 = num3 <= num4;
					}
					if (flag2 && SphereColClosestPoint.LengthSquared() > 1f)
					{
						SphereColClosestPoint.Normalize();
						float num15 = sphere.Radius - (float)Math.Sqrt(num3);
						SphereColSphereCenter += SphereColClosestPoint * num15;
						result = true;
						if (Vector3.Dot(SphereColClosestPoint, ObjectSpaceUpVector) > 0.8f)
						{
							onWalkable = true;
						}
					}
				}
				sphere.Center = SphereColSphereCenter;
			}
		}
		return result;
	}

	public virtual bool RayCast(ref Ray ray, int qIndex, ref Vector3 hitPosition, ref Vector3 hitNormal, ref float hitDistance)
	{
		bool result = false;
		CollisionData collisionData = ((MeshUserData)propModel.Tag).collisionData;
		if (collisionData.bSphere.Intersects(ray).HasValue && collisionData.bBox.Intersects(ray).HasValue)
		{
			int num = collisionData.indices.Length;
			for (int i = 0; i < num; i += 3)
			{
				int num2 = collisionData.indices[i];
				int num3 = collisionData.indices[i + 1];
				int num4 = collisionData.indices[i + 2];
				SphereColp1 = collisionData.vertices[num2];
				SphereColp2 = collisionData.vertices[num3];
				SphereColp3 = collisionData.vertices[num4];
				float lineParameter = float.MaxValue;
				if (MyMath.IntersectRayTriangle(ref ray.Position, ref ray.Direction, ref SphereColp1, ref SphereColp2, ref SphereColp3, ref lineParameter) && lineParameter < hitDistance)
				{
					result = true;
					hitDistance = lineParameter;
					hitPosition = ray.Position + ray.Direction * hitDistance;
					SphereCole1 = SphereColp2 - SphereColp1;
					SphereCole2 = SphereColp3 - SphereColp1;
					hitNormal = Vector3.Cross(SphereCole2, SphereCole1);
				}
			}
		}
		return result;
	}

	public void TransformBoundingSphere(Matrix m)
	{
		for (int i = 0; i < numPropBoundingSphere; i++)
		{
			propBoundingSphere[i].Center = Vector3.Transform(propBoundingSphere[i].Center, m);
			m.Translation = Vector3.Zero;
			propBoundingSphere[i].Radius = Vector3.Transform(Vector3.UnitX * propBoundingSphere[i].Radius, m).Length();
		}
	}

	public void AdjustTerrainWithContourData()
	{
	}

	public bool CollisionPointBoundingSphere(ref Vector3 p)
	{
		bool result = false;
		for (int i = 0; i < numPropBoundingSphere; i++)
		{
			float num = propBoundingSphere[i].Radius * propBoundingSphere[i].Radius;
			collisionPos = propBoundingSphere[i].Center;
			collisionDir = p - collisionPos;
			float num2 = collisionDir.LengthSquared();
			if (num2 < num)
			{
				result = true;
				p += collisionDir * (1f - num2 / num);
			}
		}
		return result;
	}

	public BoundingSphere GetBoundingSphere()
	{
		BoundingSphere boundingSphere = propBoundingSphere[0];
		for (int i = 1; i < numPropBoundingSphere; i++)
		{
			boundingSphere = BoundingSphere.CreateMerged(boundingSphere, propBoundingSphere[i]);
		}
		return boundingSphere;
	}

	public virtual void Update(float eTime, int qIndex)
	{
	}

	public virtual void Draw(PlayerBase viewer, int qIndex)
	{
		matViewProj = viewer.mDataQueue[qIndex].view * viewer.mDataQueue[qIndex].projection;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
		{
			drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
			if (((MeshAttributesParams)drawMesh.Tag).ObjectType == EnumObjectTypes.Render)
			{
				for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; j++)
				{
					drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
					drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
					((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(propTransforms[drawMesh.ParentBone.Index] * matWorld[qIndex]);
					((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(matViewProj);
					drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
					drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
		}
	}

	public virtual void DrawCameraSpace(PlayerBase viewer, int qIndex, float lod)
	{
		tmpMatWorld = matWorld[qIndex];
		eyePosition = tmpMatWorld.Translation;
		eyePosition.X -= viewer.vecHeadPosition[qIndex].X;
		eyePosition.Z -= viewer.vecHeadPosition[qIndex].Z;
		tmpMatWorld.Translation = eyePosition;
		EnumObjectTypes objectType;
		if (LODIndex == -1 || lod < 36000000f)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
			{
				drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
				if (i == LODIndex || drawMesh.Name == "LightGlare")
				{
					continue;
				}
				objectType = ((MeshAttributesParams)drawMesh.Tag).ObjectType;
				if (objectType != EnumObjectTypes.Render && objectType != EnumObjectTypes.RenderCastNoShadow && objectType != EnumObjectTypes.NumberOf)
				{
					continue;
				}
				if (objectType == EnumObjectTypes.NumberOf)
				{
					tmpVecZeroY = eyePosition;
					tmpVecZeroY.Y = 0f;
					tmpMatWorld.Translation = tmpVecZeroY;
				}
				else
				{
					tmpMatWorld.Translation = eyePosition;
				}
				for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; j++)
				{
					drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
					if (((MeshAttributesParams)drawMesh.Tag).Culling == EnumCullingTypes.CullNone)
					{
						drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
					}
					else
					{
						drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
					}
					drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
					((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpMatWorld);
					((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
					int primitiveCount = drawMeshPart.PrimitiveCount;
					if (((MeshAttributesParams)drawMesh.Tag).Opacity == EnumOpacityTypes.AlphaTest)
					{
						drawMeshPart.Effect.CurrentTechnique.Passes[18].Apply();
						drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount);
					}
					else
					{
						drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
						drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount);
					}
				}
			}
			return;
		}
		drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[LODIndex];
		objectType = ((MeshAttributesParams)drawMesh.Tag).ObjectType;
		if (objectType != EnumObjectTypes.Render && objectType != EnumObjectTypes.RenderCastNoShadow && objectType != EnumObjectTypes.NumberOf)
		{
			return;
		}
		if (objectType == EnumObjectTypes.NumberOf)
		{
			tmpVecZeroY = eyePosition;
			tmpVecZeroY.Y = 0f;
			tmpMatWorld.Translation = tmpVecZeroY;
		}
		else
		{
			tmpMatWorld.Translation = eyePosition;
		}
		for (int k = 0; k < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; k++)
		{
			drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[k];
			if (((MeshAttributesParams)drawMesh.Tag).Culling == EnumCullingTypes.CullNone)
			{
				drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			}
			else
			{
				drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			}
			drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
			drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
			((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
			((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpMatWorld);
			((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
			int primitiveCount2 = drawMeshPart.PrimitiveCount;
			if (((MeshAttributesParams)drawMesh.Tag).Opacity == EnumOpacityTypes.AlphaTest)
			{
				drawMeshPart.Effect.CurrentTechnique.Passes[18].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount2);
			}
			else
			{
				drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount2);
			}
		}
	}

	public virtual void DrawCameraSpaceAlpha(PlayerBase viewer, int qIndex, float lod)
	{
		tmpMatWorld = matWorld[qIndex];
		eyePosition = tmpMatWorld.Translation;
		eyePosition.X -= viewer.vecHeadPosition[qIndex].X;
		eyePosition.Z -= viewer.vecHeadPosition[qIndex].Z;
		tmpMatWorld.Translation = eyePosition;
		EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Additive;
		EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
		EnumObjectTypes objectType;
		if (LODIndex == -1 || lod < 36000000f)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
			{
				if (i == LODIndex)
				{
					continue;
				}
				drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
				if (drawMesh.Name != "LightGlare")
				{
					continue;
				}
				objectType = ((MeshAttributesParams)drawMesh.Tag).ObjectType;
				if (objectType == EnumObjectTypes.Render)
				{
					tmpMatWorld.Translation = eyePosition;
					for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; j++)
					{
						drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
						drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
						drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
						((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpMatWorld);
						((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
						int primitiveCount = drawMeshPart.PrimitiveCount;
						drawMeshPart.Effect.CurrentTechnique.Passes[19].Apply();
						drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount);
					}
				}
			}
			return;
		}
		drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[LODIndex];
		objectType = ((MeshAttributesParams)drawMesh.Tag).ObjectType;
		if (objectType != EnumObjectTypes.Render && objectType != EnumObjectTypes.RenderCastNoShadow && objectType != EnumObjectTypes.NumberOf)
		{
			return;
		}
		if (objectType == EnumObjectTypes.NumberOf)
		{
			tmpVecZeroY = eyePosition;
			tmpVecZeroY.Y = 0f;
			tmpMatWorld.Translation = tmpVecZeroY;
		}
		else
		{
			tmpMatWorld.Translation = eyePosition;
		}
		for (int k = 0; k < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; k++)
		{
			drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[k];
			if (((MeshAttributesParams)drawMesh.Tag).Culling == EnumCullingTypes.CullNone)
			{
				drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			}
			else
			{
				drawMeshPart.Effect.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			}
			drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
			drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
			((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
			((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(tmpMatWorld);
			((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
			int primitiveCount2 = drawMeshPart.PrimitiveCount;
			if (((MeshAttributesParams)drawMesh.Tag).Opacity == EnumOpacityTypes.AlphaTest)
			{
				drawMeshPart.Effect.CurrentTechnique.Passes[18].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount2);
			}
			else
			{
				drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, primitiveCount2);
			}
		}
	}

	public virtual void DrawShadowMap(PlayerBase viewer, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex, bool lod)
	{
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCW;
		tmpMatWorld = matWorld[qIndex];
		eyePosition = tmpMatWorld.Translation;
		eyePosition.X -= viewer.vecHeadPosition[qIndex].X;
		eyePosition.Z -= viewer.vecHeadPosition[qIndex].Z;
		tmpMatWorld.Translation = eyePosition;
		graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
		{
			drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
			if (i == LODIndex || drawMesh.Name == "LightGlare" || ((MeshAttributesParams)drawMesh.Tag).ObjectType != EnumObjectTypes.Render)
			{
				continue;
			}
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; j++)
			{
				ModelMeshPart modelMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
				if (((MeshAttributesParams)drawMesh.Tag).ObjectType == EnumObjectTypes.Render)
				{
					Effect effect = modelMeshPart.Effect;
					effect.GraphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer, modelMeshPart.VertexOffset);
					effect.GraphicsDevice.Indices = modelMeshPart.IndexBuffer;
					((PropEffectParams)modelMeshPart.Tag).eyePosition.SetValue(lightPos);
					((PropEffectParams)modelMeshPart.Tag).matLightViewProj.SetValue(LightViewProj);
					((PropEffectParams)modelMeshPart.Tag).matWorld.SetValue(tmpMatWorld);
					if (((MeshAttributesParams)drawMesh.Tag).Opacity == EnumOpacityTypes.AlphaTest)
					{
						effect.CurrentTechnique.Passes[21].Apply();
					}
					else
					{
						effect.CurrentTechnique.Passes[21].Apply();
					}
					int primitiveCount = modelMeshPart.PrimitiveCount;
					effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, primitiveCount);
				}
			}
		}
	}

	public virtual void Draw(ref Matrix matVP, ref Vector3 eyePos, int qIndex)
	{
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
		{
			drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts).Count; j++)
			{
				drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
				drawMeshPart.Effect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
				drawMeshPart.Effect.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePos);
				((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(propTransforms[drawMesh.ParentBone.Index] * matWorld[qIndex]);
				((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(matVP);
				((PropEffectParams)drawMeshPart.Tag).vecUVOffset.SetValue(Vector4.Zero);
				drawMeshPart.Effect.CurrentTechnique.Passes[11].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public virtual void DrawAlpha(PlayerBase viewer, int qIndex)
	{
	}

	public virtual void DrawPost(PlayerBase e, int qIndex)
	{
	}

	public virtual void DrawPostAlphaTest(PlayerBase viewer, int qIndex)
	{
		matViewProj = viewer.mDataQueue[qIndex].view * viewer.mDataQueue[qIndex].projection;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
		{
			drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
			for (int j = 0; j < 1; j++)
			{
				drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
				drawMeshPart.Effect.GraphicsDevice.BlendState = BlendState.Opaque;
				drawMeshPart.Effect.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				eyePosition = Vector3.Transform(-viewer.mDataQueue[qIndex].view.Translation, Matrix.Transpose(viewer.mDataQueue[qIndex].view));
				((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
				((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(propTransforms[drawMesh.ParentBone.Index] * matWorld[qIndex]);
				((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(matViewProj);
				drawMeshPart.Effect.CurrentTechnique.Passes[13].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public virtual void DrawPostAlphaBlend(PlayerBase viewer, int qIndex)
	{
		matViewProj = viewer.mDataQueue[qIndex].view * viewer.mDataQueue[qIndex].projection;
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)propModel.Meshes).Count; i++)
		{
			drawMesh = ((ReadOnlyCollection<ModelMesh>)propModel.Meshes)[i];
			for (int j = 0; j < 1; j++)
			{
				drawMeshPart = ((ReadOnlyCollection<ModelMeshPart>)drawMesh.MeshParts)[j];
				drawMeshPart.Effect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
				drawMeshPart.Effect.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				eyePosition = Vector3.Transform(-viewer.mDataQueue[qIndex].view.Translation, Matrix.Transpose(viewer.mDataQueue[qIndex].view));
				((PropEffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
				((PropEffectParams)drawMeshPart.Tag).matWorld.SetValue(propTransforms[drawMesh.ParentBone.Index] * matWorld[qIndex]);
				((PropEffectParams)drawMeshPart.Tag).matViewProj.SetValue(matViewProj);
				vecUVOffset.X -= 0.0001f;
				vecUVOffset.Y -= 0.05f;
				drawMeshPart.Effect.Parameters["vecUVOffset"].SetValue(vecUVOffset);
				drawMeshPart.Effect.Parameters["DepthTexture"].SetValue(LevelBaseMenu.DepthRenderTarget);
				drawMeshPart.Effect.CurrentTechnique.Passes[14].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public PropModelBase()
	{
		matWorld = new Matrix[2];
		LODIndex = -1;
		InFrustum = new bool[2];
		playerView = Matrix.Identity;
		playerProj = Matrix.Identity;
	}

	static PropModelBase()
	{
		uid = 1;
		ObjectSpaceUpVector = Vector3.UnitZ;
		vecUVOffset = Vector4.Zero;
		UnitNegY = -Vector3.UnitY;
		RayCastDist = 48f;
		TestRayCast = false;
		RayCastCollision = false;
		p = Vector3.Zero;
		q = Vector3.Zero;
		s = Vector3.Zero;
		p1 = Vector3.Zero;
		p2 = Vector3.Zero;
		p3 = Vector3.Zero;
		e1 = Vector3.Zero;
		e2 = Vector3.Zero;
		norm = Vector3.Zero;
		ab = Vector3.Zero;
		ac = Vector3.Zero;
		ap = Vector3.Zero;
		bp = Vector3.Zero;
		cp = Vector3.Zero;
		tmpSphere = default(BoundingSphere);
		SphereColSphereCenter = Vector3.Zero;
		SphereColClosestPoint = Vector3.Zero;
		SphereColp1 = Vector3.Zero;
		SphereColp2 = Vector3.Zero;
		SphereColp3 = Vector3.Zero;
		SphereCole1 = Vector3.Zero;
		SphereCole2 = Vector3.Zero;
		collisionPos = Vector3.Zero;
		collisionDir = Vector3.Zero;
		eyePosition = Vector3.Zero;
		tmpVecZeroY = Vector3.Zero;
	}
}
