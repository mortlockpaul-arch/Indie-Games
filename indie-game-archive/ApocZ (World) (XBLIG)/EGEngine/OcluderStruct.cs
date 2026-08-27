using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public struct OcluderStruct
{
	private bool Valid;

	private eTriangleMesh ocluderMesh;

	private OcluderPlane[] OcluderPlanes;

	private static ContainmentType tmpCont;

	private static BoundingBox tmpBBox = default(BoundingBox);

	public void Initialize(eMesh mesh, LevelModel root)
	{
		eTriangleMesh eTriangleMesh2 = (eTriangleMesh)mesh.Tag;
		Valid = false;
		ocluderMesh = eTriangleMesh2;
		int num = 0;
		OcluderPlanes = new OcluderPlane[mesh.Children.Count];
		for (int i = 0; i < mesh.Children.Count; i++)
		{
			eMesh eMesh2 = mesh.Children[i];
			OcluderPlanes[num] = default(OcluderPlane);
			OcluderPlanes[num].Initialize((eTriangleMesh)eMesh2.Tag);
			num++;
		}
	}

	public bool TestOclusion(ref Vector3 objPos)
	{
		if (!Valid)
		{
			return false;
		}
		for (int i = 0; i < OcluderPlanes.Length; i++)
		{
			if (OcluderPlanes[i].TestOclusion(ref objPos))
			{
				return true;
			}
		}
		return false;
	}

	public void AddOcclusionReference(GeometryMesh e)
	{
		eTriangleMesh eTriangleMesh2 = (eTriangleMesh)e.Mesh.Tag;
		for (int i = 0; i < OcluderPlanes.Length; i++)
		{
			if (OcluderPlanes[i].TestOclusion(ref eTriangleMesh2.oobb.center))
			{
				OcluderPlanes[i].AddMeshObject(e);
			}
		}
	}

	public void ToggleOcclusionGeometry(int qIndx, int pIndx)
	{
		if (Valid)
		{
			for (int i = 0; i < OcluderPlanes.Length; i++)
			{
				OcluderPlanes[i].ToggleRender(e: false, qIndx, pIndx);
			}
		}
	}

	public void CalculateOcluder(PlayerBase playerRef)
	{
		Valid = false;
		tmpBBox.Min = ocluderMesh.oobb.Min;
		tmpBBox.Max = ocluderMesh.oobb.Max;
		tmpCont = tmpBBox.Contains(playerRef.vecPosition);
		if (tmpCont == ContainmentType.Contains)
		{
			Valid = true;
		}
	}
}
