using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides access to geometry data extracted from renderable meshes and vertex / index buffers.
/// </summary>
public class GeometryData
{
	/// <summary>
	/// List of the geometry's vertices.
	/// </summary>
	public List<Vector3> Vertices = new List<Vector3>(256);

	/// <summary>
	/// List of the geometry's indices.
	/// </summary>
	public List<int> Indices = new List<int>(256);
}
