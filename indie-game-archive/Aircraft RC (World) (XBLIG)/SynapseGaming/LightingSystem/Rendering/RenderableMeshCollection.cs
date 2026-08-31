using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Represents a collection of RenderableMesh objects.
/// </summary>
public class RenderableMeshCollection : ReadOnlyCollection<RenderableMesh>
{
	/// <summary>
	/// Creates a new RenderableMeshCollection instance.
	/// </summary>
	/// <param name="meshes">Source mesh list.</param>
	public RenderableMeshCollection(IList<RenderableMesh> meshes)
		: base(meshes)
	{
	}
}
