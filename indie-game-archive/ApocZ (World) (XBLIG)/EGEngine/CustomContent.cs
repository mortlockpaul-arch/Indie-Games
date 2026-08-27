using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class CustomContent
{
	public string name;

	public string textureName;

	public int renderType;

	public Color baseColor;

	public Texture2D DiffuseMap;

	public Texture2D NormalMap;

	public Matrix transform;

	public object oobb;

	public object triangleMesh;

	public void SetPhysics(ModelMesh mesh, Matrix t, VertexType v)
	{
		transform = t;
		Vector3[] positionsFromMesh = MeshTools.GetPositionsFromMesh(mesh, v);
		oobb = new OOBB(positionsFromMesh, t);
	}
}
