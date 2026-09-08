using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by objects that manage rendering, scene resources, and have a
/// containment volume. Commonly used for scenegraphs, oct-trees, and BSP-trees.
/// </summary>
public interface IWorldRenderableManager : IRenderableManager, IManager, IUnloadable
{
	/// <summary>
	/// The current containment volume for this object.
	/// </summary>
	BoundingBox WorldBoundingBox { get; }

	/// <summary>
	/// Enables automatic optimizations on the tree used to store contained objects. 
	/// Optimization occurs when a large number of objects fall outside of the tree bounds.
	/// </summary>
	bool AutoOptimize { get; set; }

	/// <summary>
	/// Determines if the tree used to store contained objects requires optimization.
	/// </summary>
	bool RequiresOptimization { get; }

	/// <summary>
	/// Resizes the tree used to store contained objects.
	/// </summary>
	/// <param name="worldboundingbox">The smallest bounding area that completely
	/// contains the scene. Helps the scenegraph build an optimal scene tree.</param>
	/// <param name="worldtreemaxdepth">Maximum depth for entries in the scene tree. Small
	/// scenes with few objects see better performance with shallow trees. Large complex
	/// scenes often need deeper trees.</param>
	void Resize(BoundingBox worldboundingbox, int worldtreemaxdepth);

	/// <summary>
	/// Optimizes the tree used to store contained objects.
	/// </summary>
	void Optimize();

	/// <summary>
	/// Optimizes the tree used to store contained objects using a fixed tree depth.
	/// </summary>
	/// <param name="worldtreemaxdepth">Fixed tree depth used to optimize the tree.</param>
	void Optimize(int worldtreemaxdepth);
}
