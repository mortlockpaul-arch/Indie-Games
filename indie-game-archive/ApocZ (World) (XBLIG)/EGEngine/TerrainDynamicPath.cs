using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class TerrainDynamicPath : PropModelBase
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

	public unsafe override void Load(string n)
	{
		base.Load(n);
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
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < vertexCount; j++)
			{
				zero = Vector3.Transform(ptr[j].pos, matrix);
				zero = Vector3.Transform(zero, matScale);
				zero.Y = HeightMapPhysics.GetHeight(ref zero) + 4f;
				_ = Vector3.Zero;
				_ = Vector3.Zero;
				for (int k = -2; k < 3; k++)
				{
					for (int l = -2; l < 3; l++)
					{
					}
				}
			}
		}
		Vector3[] array2 = new Vector3[vertexCount];
		for (int m = 0; m < vertexCount; m++)
		{
			zero = Vector3.Transform(ptr[m].pos, matrix);
			zero = Vector3.Transform(zero, matScale);
			zero.Y = HeightMapPhysics.GetHeight(ref zero) + 2f;
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
		ShaderPass = 18;
		base.Draw(viewer, qIndex);
	}
}
