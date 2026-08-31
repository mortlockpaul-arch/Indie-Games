using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Abstract class that provides base support for transparency sorting.
/// </summary>
public abstract class BaseTransparencyRenderNode
{
	[CompilerGenerated]
	private int HCB;

	/// <summary>
	/// Determines the transparency sorting value of the node, where 0 is closest to the
	/// camera / viewer and larger values are increasingly further away.
	/// </summary>
	public int SortIndex
	{
		[CompilerGenerated]
		get
		{
			return HCB;
		}
		[CompilerGenerated]
		protected set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Effect used by this transparency node.
	/// </summary>
	public abstract Effect Effect { get; }

	/// <summary>
	/// Calculates the transparency SortIndex based on the object center.
	/// </summary>
	/// <param name="scenestate">Current scene state.</param>
	/// <param name="objectcenter">Object center.</param>
	protected void Build(ISceneState scenestate, Vector3 objectcenter)
	{
		float visibleDistance = scenestate.Environment.VisibleDistance;
		if (!(visibleDistance <= 0f))
		{
			Vector3 value = scenestate.ViewToWorld.Translation;
			Vector3.DistanceSquared(ref value, ref objectcenter, out var result);
			SortIndex = (int)(result / (visibleDistance * visibleDistance) * 65535f);
		}
	}

	/// <summary>
	/// Render all transparency nodes of a specific type in a single batch.
	/// </summary>
	/// <param name="scenestate">Current scene state.</param>
	/// <param name="nodes">All transparency nodes rendered during this call.</param>
	/// <param name="resetrenderstates">Determines if render states need to be set because the
	/// previous render call was to a different BaseTransparencyRenderNode type.</param>
	public abstract void RenderBatch(ISceneState scenestate, List<BaseTransparencyRenderNode> nodes, bool resetrenderstates);
}
