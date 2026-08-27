using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class MeshTools
{
	public unsafe static Vector3[] GetPositionsFromeMesh(eMesh mesh, VertexType vertType)
	{
		if (mesh.MeshParts.Count == 0)
		{
			return new Vector3[1] { Vector3.Zero };
		}
		int vertexStride = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
		byte[] array = new byte[mesh.MeshParts[0].NumVertices * vertexStride];
		mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride, array, 0, mesh.MeshParts[0].NumVertices * vertexStride, 1);
		Vector3[] array2 = new Vector3[mesh.MeshParts[0].NumVertices];
		for (int i = 0; i < mesh.MeshParts[0].NumVertices; i++)
		{
			fixed (byte* ptr = &array[i * vertexStride])
			{
				ref Vector3 reference = ref array2[i];
				reference = *(Vector3*)ptr;
			}
		}
		return array2;
	}

	public static Vector3[] GetPositionsFromMesh(ModelMesh mesh, VertexType vertType)
	{
		switch (vertType)
		{
		case VertexType.PosNormTexTan:
		{
			int vertexStride5 = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			VS_PosNormTexTanCompressed[] array9 = new VS_PosNormTexTanCompressed[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride5, array9, 0, mesh.MeshParts[0].NumVertices, vertexStride5);
			Vector3[] array10 = new Vector3[mesh.MeshParts[0].NumVertices];
			for (int m = 0; m < mesh.MeshParts[0].NumVertices; m++)
			{
				ref Vector3 reference4 = ref array10[m];
				reference4 = array9[m].position;
			}
			return array10;
		}
		case VertexType.Unknown:
		{
			int vertexStride3 = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			float[] array5 = new float[mesh.MeshParts[0].NumVertices * (vertexStride3 / 4)];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride3, array5, 0, mesh.MeshParts[0].NumVertices, vertexStride3);
			int num = 0;
			Vector3[] array6 = new Vector3[mesh.MeshParts[0].NumVertices];
			for (int k = 0; k < mesh.MeshParts[0].NumVertices; k++)
			{
				array6[k].X = array5[num];
				array6[k].Y = array5[num + 1];
				array6[k].Z = array5[num + 2];
				num = k * (vertexStride3 / 4);
			}
			return array6;
		}
		case VertexType.Basic:
		{
			int vertexStride6 = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			VS_Struct[] array11 = new VS_Struct[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride6, array11, 0, mesh.MeshParts[0].NumVertices, vertexStride6);
			Vector3[] array12 = new Vector3[mesh.MeshParts[0].NumVertices];
			for (int n = 0; n < mesh.MeshParts[0].NumVertices; n++)
			{
				ref Vector3 reference5 = ref array12[n];
				reference5 = array11[n].position;
			}
			return array12;
		}
		case VertexType.Position:
		{
			int vertexStride2 = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			Vector3[] array3 = new Vector3[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride2, array3, 0, mesh.MeshParts[0].NumVertices, vertexStride2);
			Vector3[] array4 = new Vector3[mesh.MeshParts[0].NumVertices];
			for (int j = 0; j < mesh.MeshParts[0].NumVertices; j++)
			{
				ref Vector3 reference2 = ref array4[j];
				reference2 = array3[j];
			}
			return array4;
		}
		case VertexType.Physics:
		{
			int vertexStride4 = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			VS_PhysicsStruct[] array7 = new VS_PhysicsStruct[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride4, array7, 0, mesh.MeshParts[0].NumVertices, vertexStride4);
			Vector3[] array8 = new Vector3[mesh.MeshParts[0].NumVertices];
			for (int l = 0; l < mesh.MeshParts[0].NumVertices; l++)
			{
				ref Vector3 reference3 = ref array8[l];
				reference3 = array7[l].position;
			}
			return array8;
		}
		case VertexType.BakedLight:
		{
			int vertexStride = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			VS_BakedLightStruct[] array = new VS_BakedLightStruct[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride, array, 0, mesh.MeshParts[0].NumVertices, vertexStride);
			Vector3[] array2 = new Vector3[mesh.MeshParts[0].NumVertices];
			for (int i = 0; i < mesh.MeshParts[0].NumVertices; i++)
			{
				ref Vector3 reference = ref array2[i];
				reference = array[i].position;
			}
			return array2;
		}
		default:
			return null;
		}
	}

	public static Color GetColorFromMesh(ModelMesh mesh, VertexType vertType)
	{
		switch (vertType)
		{
		case VertexType.Basic:
			return new Color(1, 1, 1, 1);
		case VertexType.BakedLight:
		{
			int vertexStride = mesh.MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;
			VS_BakedLightStruct[] data = new VS_BakedLightStruct[mesh.MeshParts[0].NumVertices];
			mesh.MeshParts[0].VertexBuffer.GetData(mesh.MeshParts[0].VertexOffset * vertexStride, data, 0, mesh.MeshParts[0].NumVertices, vertexStride);
			return new Color(1, 1, 1, 1);
		}
		default:
			return new Color(1, 1, 1, 1);
		}
	}
}
