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
	private class _0001CB : IComparer<BaseTransparencyRenderNode>
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

	private List<BaseTransparencyRenderNode> HCB = new List<BaseTransparencyRenderNode>();

	private List<BaseTransparencyRenderNode> HC_0002 = new List<BaseTransparencyRenderNode>();

	private static _0001CB HC_0012 = new _0001CB();

	/// <summary>
	/// Add a transparency node to the sorter.
	/// </summary>
	/// <param name="node"></param>
	public void Add(BaseTransparencyRenderNode node)
	{
		HCB.Add(node);
	}

	/// <summary>
	/// Clear all transparency nodes from the sorter.
	/// </summary>
	public void Clear()
	{
		HCB.Clear();
	}

	/// <summary>
	/// Sorts, batches, and renders all contained transparency nodes.
	/// </summary>
	/// <param name="scenestate">Current scene state.</param>
	/// <param name="allowbatching">Determines if transparency nodes are rendered in batches or individually.</param>
	public void RenderBatches(ISceneState scenestate, bool allowbatching)
	{
		HCB.Sort(HC_0012);
		int count = HCB.Count;
		Type type = null;
		for (int i = 0; i < count; i++)
		{
			BaseTransparencyRenderNode baseTransparencyRenderNode = HCB[i];
			Type type2 = baseTransparencyRenderNode.GetType();
			bool resetrenderstates = (object)type2 != type;
			HC_0002.Clear();
			HC_0002.Add(baseTransparencyRenderNode);
			if (allowbatching)
			{
				Effect effect = baseTransparencyRenderNode.Effect;
				for (int j = i + 1; j < count; j++)
				{
					BaseTransparencyRenderNode baseTransparencyRenderNode2 = HCB[j];
					if ((object)type2 != baseTransparencyRenderNode2.GetType() || effect != baseTransparencyRenderNode2.Effect)
					{
						break;
					}
					HC_0002.Add(baseTransparencyRenderNode2);
					i++;
				}
			}
			baseTransparencyRenderNode.RenderBatch(scenestate, HC_0002, resetrenderstates);
			type = type2;
		}
	}
}
