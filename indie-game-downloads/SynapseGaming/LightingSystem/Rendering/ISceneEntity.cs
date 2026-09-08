using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Interface used for scene entities, basic named and movable objects that exist in the scene.
/// </summary>
public interface ISceneEntity : IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject
{
	/// <summary>
	/// Determines the bounds used in object culling and collision.
	/// </summary>
	HullType HullType { get; set; }

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	BoundingSphere ObjectBoundingSphere { get; }

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	BoundingBox ObjectBoundingBox { get; }

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	BoundingSphere WorldBoundingSphere { get; }

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	/// </summary>
	/// <param name="world">World space transform of the object.</param>
	/// <param name="worldtoobj">Inverse world space transform of the object.</param>
	void SetWorldAndWorldToObject(Matrix world, Matrix worldtoobj);

	/// <summary>
	/// Implements a custom rendering pass. The pass occurs after scene rendering completes, but before post processing.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	void RenderCustomPass(ISceneState scenestate);
}
