using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Transparency helper class for sorting and rendering transparency nodes in batches.
/// </summary>
public class TransparencyRenderNodeSorter
{
	private class DA_0018 : IComparer<BaseTransparencyRenderNode>
	{
		public int Compare(BaseTransparencyRenderNode a, BaseTransparencyRenderNode b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			return b.SortIndex - a.SortIndex;
		}
	}

	private List<BaseTransparencyRenderNode> _3A_0018 = new List<BaseTransparencyRenderNode>();

	private List<BaseTransparencyRenderNode> _3AL = new List<BaseTransparencyRenderNode>();

	private static DA_0018 _3A_0019 = new DA_0018();

	/// <summary>
	/// Add a transparency node to the sorter.
	/// </summary>
	/// <param name="node"></param>
	public void Add(BaseTransparencyRenderNode node)
	{
		_3A_0018.Add(node);
	}

	/// <summary>
	/// Clear all transparency nodes from the sorter.
	/// </summary>
	public void Clear()
	{
		_3A_0018.Clear();
	}

	/// <summary>
	/// Sorts, batches, and renders all contained transparency nodes.
	/// </summary>
	/// <param name="scenestate">Current scene state.</param>
	/// <param name="allowbatching">Determines if transparency nodes are rendered in batches or individually.</param>
	public void RenderBatches(ISceneState scenestate, bool allowbatching)
	{
		_3A_0018.Sort(_3A_0019);
		int count = _3A_0018.Count;
		Type type = null;
		for (int i = 0; i < count; i++)
		{
			BaseTransparencyRenderNode baseTransparencyRenderNode = _3A_0018[i];
			Type type2 = baseTransparencyRenderNode.GetType();
			bool resetrenderstates = (object)type2 != type;
			_3AL.Clear();
			_3AL.Add(baseTransparencyRenderNode);
			if (allowbatching)
			{
				Effect effect = baseTransparencyRenderNode.Effect;
				for (int j = i + 1; j < count; j++)
				{
					BaseTransparencyRenderNode baseTransparencyRenderNode2 = _3A_0018[j];
					if ((object)type2 != baseTransparencyRenderNode2.GetType() || effect != baseTransparencyRenderNode2.Effect)
					{
						break;
					}
					_3AL.Add(baseTransparencyRenderNode2);
					i++;
				}
			}
			baseTransparencyRenderNode.RenderBatch(scenestate, _3AL, resetrenderstates);
			type = type2;
		}
	}
}
