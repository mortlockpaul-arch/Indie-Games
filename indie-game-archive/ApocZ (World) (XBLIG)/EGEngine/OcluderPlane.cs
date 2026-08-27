using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public struct OcluderPlane
{
	private eTriangleMesh OcluderMesh;

	private List<GeometryMesh> OcluderMeshObjects;

	private static ContainmentType tmpCont;

	private static BoundingBox tmpBBox = default(BoundingBox);

	public void Initialize(eTriangleMesh oc)
	{
		OcluderMesh = oc;
		OcluderMeshObjects = new List<GeometryMesh>();
	}

	public bool TestOclusion(ref Vector3 objPos)
	{
		tmpBBox.Min = OcluderMesh.oobb.Min;
		tmpBBox.Max = OcluderMesh.oobb.Max;
		tmpCont = tmpBBox.Contains(objPos);
		if (tmpCont == ContainmentType.Contains)
		{
			return true;
		}
		return false;
	}

	public void CalculateOcluder(PlayerBase playerRef)
	{
	}

	public void AddMeshObject(GeometryMesh e)
	{
		OcluderMeshObjects.Add(e);
	}

	public void ToggleRender(bool e, int qIndx, int pIndx)
	{
		for (int i = 0; i < OcluderMeshObjects.Count; i++)
		{
			OcluderMeshObjects[i].Render[qIndx][pIndx] = e;
			OcluderMeshObjects[i].RenderLOD[qIndx][pIndx] = e;
		}
	}
}
