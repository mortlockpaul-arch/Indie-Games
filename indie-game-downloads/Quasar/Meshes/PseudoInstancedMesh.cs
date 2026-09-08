using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.Meshes;

public class PseudoInstancedMesh : Mesh
{
	private int instanceNumber;

	private Mesh baseMesh;

	private Transform[] instanceData;

	public int InstanceNumber => instanceNumber;

	public PseudoInstancedMesh(Mesh baseMesh, int maxInstances)
	{
		this.baseMesh = baseMesh;
		instanceData = new Transform[maxInstances];
		for (int i = 0; i < maxInstances; i++)
		{
			instanceData[i] = new Transform();
		}
	}

	public bool AddInstance(ref Matrix data, bool checkCull)
	{
		if (instanceNumber >= instanceData.Length)
		{
			return true;
		}
		if (checkCull)
		{
			BoundingSphere sphere = ReplaceSphere(baseMesh.BoundingSphere, ref data);
			if (Scene.CurrentInstance != null && Scene.CurrentInstance.Camera.CheckCull(ref sphere))
			{
				return true;
			}
			if (SceneRenderData.CurrentRenderData != null && SceneRenderData.CurrentRenderData.Camera.CheckCull(ref sphere))
			{
				return true;
			}
		}
		instanceData[instanceNumber++].Matrix = data;
		return false;
	}

	protected BoundingSphere ReplaceSphere(BoundingSphere sphere, ref Matrix data)
	{
		float num = GameMath.VectorMaxValue(GameMath.MatrixScale(ref data));
		return new BoundingSphere(sphere.Center + data.Translation, sphere.Radius * num);
	}

	public bool AddInstance(Matrix data, bool checkCull)
	{
		return AddInstance(ref data, checkCull);
	}

	public void ClearInstances()
	{
		instanceNumber = 0;
	}

	public override void Render(Transform motion)
	{
		for (int i = 0; i < instanceNumber; i++)
		{
			instanceData[i].Parent = motion;
			baseMesh.Render(instanceData[i]);
		}
	}

	public override void RenderMaterial(Transform motion, int material)
	{
	}
}
