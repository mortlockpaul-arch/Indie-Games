using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Render.Sorters;

public class AlphaSorter : Sorter, IComparer<AlphaSortItem>
{
	private static AlphaSorter instance;

	private List<AlphaSortItem> items = new List<AlphaSortItem>(10);

	public static AlphaSorter Instance => instance;

	public override void BeginRender()
	{
		items.Clear();
	}

	public override void Add(Mesh mesh, Transform motion, int material)
	{
		if (material < 0)
		{
			material = 0;
		}
		List<Material> materials = mesh.Materials;
		if (materials == null)
		{
			return;
		}
		int count = materials.Count;
		if (count == 0)
		{
			return;
		}
		if (material >= count)
		{
			material = 0;
		}
		if (!mesh.Materials[material].HasAlpha)
		{
			mesh.RenderMaterial(motion, material);
			return;
		}
		Vector3 worldTranslation = motion.WorldTranslation;
		if (mesh.IsBoundingSphereValid)
		{
			worldTranslation += mesh.BoundingSphere.Center;
		}
		float distance = Vector3.Distance(SceneRenderData.CurrentRenderData.Camera.Transform.WorldTranslation, worldTranslation);
		items.Add(new AlphaSortItem(distance, mesh, motion, material, mesh.Materials[material].RenderPriority));
	}

	public override void EndRender()
	{
		items.Sort(this);
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			AlphaSortItem alphaSortItem = items[i];
			alphaSortItem.mesh.RenderMaterial(alphaSortItem.transform, alphaSortItem.material);
		}
	}

	public int Compare(AlphaSortItem x, AlphaSortItem y)
	{
		int priority = (int)y.priority;
		int num = priority.CompareTo((int)x.priority);
		if (num != 0)
		{
			return num;
		}
		num = y.distance.CompareTo(x.distance);
		if (num != 0)
		{
			return num;
		}
		num = x.mesh.GetHashCode().CompareTo(y.mesh.GetHashCode());
		if (num != 0)
		{
			return num;
		}
		return x.material.CompareTo(y.material);
	}

	static AlphaSorter()
	{
		instance = new AlphaSorter();
	}
}
