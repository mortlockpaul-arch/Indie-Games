using System.Runtime.InteropServices;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class TerrainStreams : PropModelBase
{
	public struct tmpVertStrcut
	{
		public Vector3 pos;

		private int pad0;

		private int pad1;

		private int pad2;

		private int pad3;

		private int pad4;
	}

	private Vector3 tmpPos = Vector3.Zero;

	private Vector3 tmpNorm = Vector3.Zero;

	private Matrix matScale = Matrix.CreateScale(10f);

	private Vector4 uvOffset = Vector4.Zero;

	public unsafe override void Load(string n)
	{
		base.Load("models\\props\\stream00");
		PropModelBase.drawMesh = propModel.Meshes[0];
		PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[0];
		int vertexStride = PropModelBase.drawMeshPart.VertexBuffer.VertexDeclaration.VertexStride;
		int vertexCount = PropModelBase.drawMeshPart.VertexBuffer.VertexCount;
		Matrix matrix = propTransforms[PropModelBase.drawMesh.ParentBone.Index];
		Matrix matrix2 = Matrix.Invert(matrix);
		byte[] array = new byte[vertexCount * vertexStride];
		PropModelBase.drawMeshPart.VertexBuffer.GetData(array);
		tmpVertStrcut* ptr = (tmpVertStrcut*)(void*)GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
		Vector3 zero = Vector3.Zero;
		for (int i = 0; i < 4; i++)
		{
			float num = float.MaxValue;
			for (int j = 0; j < vertexCount; j++)
			{
				zero = Vector3.Transform(ptr[j].pos, matrix);
				zero = Vector3.Transform(zero, matScale);
				zero.Y = HeightMapPhysics.GetHeight(ref zero) + 4f;
				float num2 = 512f;
				Vector3 zero2 = Vector3.Zero;
				Vector3 zero3 = Vector3.Zero;
				for (int k = -2; k < 3; k++)
				{
					for (int l = -2; l < 3; l++)
					{
						zero3 = zero;
						zero3.X += (float)k * num2;
						zero3.Z += (float)l * num2;
						zero2 = zero3 - zero;
						zero2.Y = 0f;
						float num3 = 1f - zero2.Length() / (num2 * 3f);
						if (zero.Y > num)
						{
							zero.Y = num;
						}
						if (zero.Y < num)
						{
							num = zero.Y;
						}
						HeightMapPhysics.SetHeightMap(ref zero3, zero.Y, num3 * 0.2f);
						if (num3 > 0.8f)
						{
							HeightMapPhysics.SetAlphaMap(ref zero3, 16777215u);
						}
						if (num3 > 0.7f)
						{
							HeightMapPhysics.SetTreeMap(ref zero3, 0);
						}
					}
				}
			}
		}
		Vector3[] array2 = new Vector3[vertexCount];
		for (int m = 0; m < vertexCount; m++)
		{
			zero = Vector3.Transform(ptr[m].pos, matrix);
			zero = Vector3.Transform(zero, matScale);
			zero.Y = HeightMapPhysics.GetHeight(ref zero) + 8f;
			array2[m] = zero;
			ptr[m].pos = Vector3.Transform(zero, matrix2);
		}
		PropModelBase.drawMeshPart.VertexBuffer.SetData(array);
	}

	public void Update(float eTime, int qIndex, PlayerBase playerRef)
	{
		ref Matrix reference = ref matWorld[qIndex];
		reference = Matrix.Identity;
		tmpPos.X = 0f - playerRef.vecHeadPosition[qIndex].X;
		tmpPos.Z = 0f - playerRef.vecHeadPosition[qIndex].Z;
		matWorld[qIndex].Translation = tmpPos;
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
	}

	public override void DrawAlpha(PlayerBase viewer, int qIndex)
	{
		ShaderPass = 20;
		PropModelBase.matViewProj = viewer.mDataQueue[qIndex].view * viewer.mDataQueue[qIndex].projection;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			PropModelBase.drawMesh = propModel.Meshes[i];
			for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
			{
				PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[j];
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(PropModelBase.drawMeshPart.VertexBuffer, PropModelBase.drawMeshPart.VertexOffset);
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.Indices = PropModelBase.drawMeshPart.IndexBuffer;
				PropModelBase.drawMeshPart.Effect.Parameters["DepthTexture"].SetValue(LevelBaseMenu.DepthRenderTarget);
				Vector4 zero = Vector4.Zero;
				zero.X = LevelOutside.SunPosition.X - viewer.vecHeadPosition[qIndex].X;
				zero.Y = LevelOutside.SunPosition.Y;
				zero.Z = LevelOutside.SunPosition.Z - viewer.vecHeadPosition[qIndex].Z;
				zero.W = 1f;
				PropModelBase.drawMeshPart.Effect.Parameters["vecSunPosition"].SetValue(zero);
				Vector3 value = Vector3.Transform(-viewer.mDataQueue[qIndex].view.Translation, Matrix.Transpose(viewer.mDataQueue[qIndex].view));
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).eyePosition.SetValue(value);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matViewProj.SetValue(PropModelBase.matViewProj);
				uvOffset.Y -= 0.01f;
				uvOffset.Z += 0.001f;
				uvOffset.W -= 0.0022f;
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).vecUVOffset.SetValue(uvOffset);
				PropModelBase.drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PropModelBase.drawMeshPart.NumVertices, PropModelBase.drawMeshPart.StartIndex, PropModelBase.drawMeshPart.PrimitiveCount);
			}
		}
	}
}
