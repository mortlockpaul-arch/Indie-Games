using System.Collections.Generic;
using GKEngine.Entities;
using GKEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Scenes;

public class EntityStack
{
	public Scene scene;

	public string name;

	public Material.State renderState = Material.State.None;

	public bool sort;

	public Dictionary<string, Entity2D> stack2D = new Dictionary<string, Entity2D>();

	public Dictionary<string, IRenderable> stack3D = new Dictionary<string, IRenderable>();

	private int list2DLength;

	private int list3DLength;

	public List<IRenderable> list3D = new List<IRenderable>();

	public List<Entity2D> list2D = new List<Entity2D>();

	private int listCount;

	public List<PostProcess> postProcess = new List<PostProcess>();

	public int postProcessCount;

	public EntityStack(Scene oScene, Material.State oRenderState, string xName, bool xSort)
	{
		scene = oScene;
		name = xName;
		renderState = oRenderState;
		sort = xSort;
	}

	public void Render(GameTime oGameTime)
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		if (renderState != Material.State.None)
		{
			Material.RenderStates_Set(renderState);
		}
		if (sort)
		{
			SortDepths3D();
		}
		for (listCount = 0; listCount < list3DLength; listCount++)
		{
			list3D[listCount].Render(oGameTime);
		}
		if (postProcessCount > 0)
		{
			for (int i = 0; i < postProcessCount; i++)
			{
				if (postProcess[i].active)
				{
					postProcess[i].Execute(graphicsDevice, oGameTime);
				}
			}
		}
		for (listCount = 0; listCount < list2DLength; listCount++)
		{
			list2D[listCount].Render(oGameTime);
		}
	}

	public void Clear()
	{
		stack2D.Clear();
		list2D.Clear();
		list2DLength = list2D.Count;
		stack3D.Clear();
		list3D.Clear();
		list3DLength = list3D.Count;
		for (int i = 0; i < postProcess.Count; i++)
		{
			postProcess[i].Unload();
		}
		postProcess.Clear();
	}

	public int GetHighestDepth()
	{
		int result = 0;
		if (list2D.Count > 0)
		{
			result = list2D[list2D.Count - 1].depth;
		}
		return result;
	}

	public void Add(string xName, Entity2D oEntity)
	{
		if (stack2D.ContainsKey(xName))
		{
			list2D.Remove(oEntity);
			stack2D[xName] = oEntity;
			list2D.Add(oEntity);
			list2DLength = list2D.Count;
		}
		else if (!stack2D.ContainsValue(oEntity))
		{
			stack2D.Add(xName, oEntity);
			list2D.Add(oEntity);
			list2DLength = list2D.Count;
		}
		SortDepths();
	}

	public void Remove(string xName)
	{
		if (stack2D.ContainsKey(xName))
		{
			Entity2D item = stack2D[xName];
			stack2D.Remove(xName);
			list2D.Remove(item);
			list2DLength = list2D.Count;
			SortDepths();
		}
	}

	public void SortDepths()
	{
		list2D.Sort(CompUtils.Entity2D_Depth);
	}

	public void ReorderDepths()
	{
		SortDepths();
		for (int i = 0; i < list2D.Count; i++)
		{
			list2D[i].depth = i;
		}
	}

	public void Add(string xName, IRenderable oEntity)
	{
		if (stack3D.ContainsKey(xName))
		{
			list3D.Remove(oEntity);
			stack3D[xName] = oEntity;
			list3D.Add(oEntity);
			list3DLength = list3D.Count;
		}
		else
		{
			stack3D.Add(xName, oEntity);
			list3D.Add(oEntity);
			list3DLength = list3D.Count;
		}
	}

	public bool Remove(string xName, IRenderable oRemove)
	{
		bool flag = true;
		flag = stack3D.Remove(xName) && list3D.Remove(oRemove);
		list3DLength = list3D.Count;
		return flag;
	}

	public void SortDepths3D()
	{
		for (int i = 0; i < list3DLength; i++)
		{
			if (list3D[i] is Base3D)
			{
				(list3D[i] as Base3D).camDepth = Vector3.Distance(scene.cameras.camera._position, (list3D[i] as Base3D)._position);
			}
		}
		list3D.Sort(Compare_Depth_Cam);
	}

	public void Add(PostProcess oPost)
	{
		postProcess.Add(oPost);
		postProcessCount = postProcess.Count;
	}

	public void Remove(PostProcess oPost)
	{
		postProcess.Remove(oPost);
		postProcessCount = postProcess.Count;
	}

	private int Compare_Depth_Cam(IRenderable a, IRenderable b)
	{
		Base3D base3D = a as Base3D;
		Base3D base3D2 = b as Base3D;
		if (base3D == null)
		{
			if (base3D2 != null)
			{
				return -1;
			}
			return 0;
		}
		if (base3D2 == null)
		{
			return 1;
		}
		return base3D.camDepth.CompareTo(base3D2.camDepth);
	}
}
