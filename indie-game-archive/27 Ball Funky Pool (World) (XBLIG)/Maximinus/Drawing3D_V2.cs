using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Drawing3D_V2
{
	public struct BoundingBoxTransformable
	{
		public readonly BoundingBox OriginalBox;

		private readonly Vector3[] OriginalData;

		public readonly Matrix OriginalTransform;

		public readonly List<Vector3> TransformedData;

		public BoundingBoxTransformable(ModelMesh mesh)
		{
			OriginalBox = (BoundingBox)mesh.Tag;
			OriginalData = OriginalBox.GetCorners();
			OriginalTransform = mesh.ParentBone.Transform;
			TransformedData = new List<Vector3>(OriginalData.Length);
			Vector3[] originalData = OriginalData;
			foreach (Vector3 position in originalData)
			{
				TransformedData.Add(Vector3.Transform(position, OriginalTransform));
			}
		}

		public VertexPositionNormalColor[] DrawingData(bool showDiagonals)
		{
			List<VertexPositionNormalColor> list = new List<VertexPositionNormalColor>();
			for (int i = 0; i < OriginalData.Length; i++)
			{
				Vector3 vector = OriginalData[i];
				for (int j = 0; j < OriginalData.Length; j++)
				{
					Vector3 vector2 = OriginalData[j];
					if (i == j)
					{
						continue;
					}
					bool num;
					if (!showDiagonals)
					{
						if ((vector.X != vector2.X || vector.Y != vector2.Y) && (vector.X != vector2.X || vector.Z != vector2.Z))
						{
							if (vector.Z != vector2.Z)
							{
								continue;
							}
							num = vector.Y == vector2.Y;
							goto IL_00de;
						}
					}
					else if (vector.X != vector2.X && vector.Y != vector2.Y)
					{
						num = vector.Z == vector2.Z;
						goto IL_00de;
					}
					goto IL_00e4;
					IL_00e4:
					list.Add(new VertexPositionNormalColor(TransformedData[i], Vector3.Zero));
					list.Add(new VertexPositionNormalColor(TransformedData[j], Vector3.Zero));
					continue;
					IL_00de:
					if (!num)
					{
						continue;
					}
					goto IL_00e4;
				}
			}
			return list.ToArray();
		}
	}

	private static DynamicVertexBuffer instanceVertexBuffer = null;

	private static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0), new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1), new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2), new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3));

	public static BasicEffect NewDefaultEffect
	{
		get
		{
			BasicEffect basicEffect = new BasicEffect(MaximinusGame.Draw2D.Device);
			basicEffect.PreferPerPixelLighting = true;
			basicEffect.EnableDefaultLighting();
			return basicEffect;
		}
	}

	public static BasicEffect NewEffectNoLighting
	{
		get
		{
			BasicEffect basicEffect = new BasicEffect(MaximinusGame.Draw2D.Device);
			basicEffect.PreferPerPixelLighting = true;
			return basicEffect;
		}
	}

	public static void DrawModelHWInstances(Model model, Matrix[] instances)
	{
		DrawModelHWInstances(model, instances, null);
	}

	public static void DrawModelHWInstances(Model model, Matrix[] instances, Texture2D overridenTex)
	{
		if (instances.Length == 0)
		{
			return;
		}
		Drawing2D.PrepareFor3D(MaximinusGame.Draw2D.Device);
		MaximinusGame.Draw2D.Device.RasterizerState = RasterizerState.CullNone;
		if (instanceVertexBuffer == null || instances.Length > instanceVertexBuffer.VertexCount)
		{
			if (instanceVertexBuffer != null)
			{
				instanceVertexBuffer.Dispose();
			}
			instanceVertexBuffer = new DynamicVertexBuffer(MaximinusGame.Draw2D.Device, instanceVertexDeclaration, instances.Length, BufferUsage.WriteOnly);
		}
		instanceVertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				MaximinusGame.Draw2D.Device.SetVertexBuffers(new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0), new VertexBufferBinding(instanceVertexBuffer, 0, 1));
				MaximinusGame.Draw2D.Device.Indices = meshPart.IndexBuffer;
				Effect effect = meshPart.Effect;
				effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];
				effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
				effect.Parameters["View"].SetValue(MaximinusGame.Instance.Camera.View);
				effect.Parameters["Projection"].SetValue(MaximinusGame.Instance.Camera.Proj);
				if (overridenTex != null)
				{
					effect.Parameters["Texture"].SetValue(overridenTex);
				}
				foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					MaximinusGame.Draw2D.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount, instances.Length);
				}
			}
		}
	}

	public static void DrawModel(Model m, Matrix world)
	{
		DrawModel(m, world, useDefaultLighting: true);
	}

	public static void DrawModel(Model model, Matrix world, bool useDefaultLighting)
	{
		foreach (ModelMesh mesh in model.Meshes)
		{
			DrawModelMesh(mesh, mesh.ParentBone.Transform * world, useDefaultLighting);
		}
	}

	public static void DrawModelMesh(ModelMesh mesh, Matrix transform, bool useDefaultLighting)
	{
		foreach (BasicEffect effect in mesh.Effects)
		{
			ApplyEffect(effect, transform, useDefaultLighting);
		}
		mesh.Draw();
	}

	public static void ApplyEffect(BasicEffect effect, Matrix transform, bool useDefaultLighting)
	{
		effect.World = transform;
		effect.View = MaximinusGame.Instance.Camera.View;
		effect.Projection = MaximinusGame.Instance.Camera.Proj;
		effect.PreferPerPixelLighting = true;
		if (useDefaultLighting)
		{
			effect.EnableDefaultLighting();
		}
	}
}
