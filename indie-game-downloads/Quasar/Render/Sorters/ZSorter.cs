using System.Collections.Generic;

namespace Quasar.Render.Sorters;

public class ZSorter : Sorter, IComparer<ZSortItem>
{
	private static ZSorter instance;

	private List<ZSortItem> items = new List<ZSortItem>(10);

	public static ZSorter Instance => instance;

	public override void BeginRender()
	{
		items.Clear();
	}

	public override void Add(Mesh mesh, Transform motion, int material)
	{
		if (!mesh.Materials[material].HasAlpha)
		{
			mesh.RenderMaterial(motion, material);
			return;
		}
		_ = motion.WorldTranslation;
		float num = motion.WorldTranslation.Z;
		if (mesh.IsBoundingSphereValid)
		{
			num += mesh.BoundingSphere.Center.Z;
		}
		items.Add(new ZSortItem(num, mesh, motion, material, mesh.Materials[material].RenderPriority));
	}

	public override void EndRender()
	{
		items.Sort(this);
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			ZSortItem zSortItem = items[i];
			zSortItem.mesh.RenderMaterial(zSortItem.transform, zSortItem.material);
		}
	}

	public int Compare(ZSortItem x, ZSortItem y)
	{
		int priority = (int)y.priority;
		int num = priority.CompareTo((int)x.priority);
		num = x.yPos.CompareTo(y.yPos);
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

	static ZSorter()
	{
		instance = new ZSorter();
	}
}
