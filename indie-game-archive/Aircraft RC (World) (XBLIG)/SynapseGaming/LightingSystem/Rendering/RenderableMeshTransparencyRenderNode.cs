using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Provides transparency sorting support for renderable meshes.
/// </summary>
public class RenderableMeshTransparencyRenderNode : BaseTransparencyRenderNode
{
	private static List<RenderableMesh> HCB = new List<RenderableMesh>();

	[CompilerGenerated]
	private RenderableMesh HC_0002;

	[CompilerGenerated]
	private BaseRenderManager HC_0012;

	/// <summary>
	/// Mesh rendered by this transparency node.
	/// </summary>
	public RenderableMesh RenderableMesh
	{
		[CompilerGenerated]
		get
		{
			return HC_0002;
		}
		[CompilerGenerated]
		protected set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// The render manager used to render the node batches.
	/// </summary>
	public BaseRenderManager RenderManager
	{
		[CompilerGenerated]
		get
		{
			return HC_0012;
		}
		[CompilerGenerated]
		protected set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Effect used by this transparency node.
	/// </summary>
	public override Effect Effect => RenderableMesh.HC6;

	/// <summary>
	/// Sets up the transparency node for sorting, batching, and rendering.
	/// </summary>
	/// <param name="rendermanager">The render manager used to render the node batches.</param>
	/// <param name="scenestate">Current scene state.</param>
	/// <param name="mesh">Mesh rendered by this transparency node.</param>
	public void Build(BaseRenderManager rendermanager, ISceneState scenestate, RenderableMesh mesh)
	{
		RenderManager = rendermanager;
		RenderableMesh = mesh;
		BoundingBox meshBoundingBox = mesh.MeshBoundingBox;
		Vector3 position = (meshBoundingBox.Max + meshBoundingBox.Min) * 0.5f;
		Vector3.Transform(ref position, ref mesh.HCD, out var result);
		Build(scenestate, result);
	}

	/// <summary>
	/// Render all transparency nodes of a specific type in a single batch.
	/// </summary>
	/// <param name="scenestate">Current scene state.</param>
	/// <param name="nodes">All transparency nodes rendered during this call.</param>
	/// <param name="resetrenderstates">Determines if render states need to be set because the
	/// previous render call was to a different BaseTransparencyRenderNode type.</param>
	public override void RenderBatch(ISceneState scenestate, List<BaseTransparencyRenderNode> nodes, bool resetrenderstates)
	{
		HCB.Clear();
		foreach (BaseTransparencyRenderNode node in nodes)
		{
			if (node is RenderableMeshTransparencyRenderNode renderableMeshTransparencyRenderNode)
			{
				HCB.Add(renderableMeshTransparencyRenderNode.RenderableMesh);
			}
		}
		RenderManager._0017dO_0016P(HCB, resetrenderstates);
	}
}
