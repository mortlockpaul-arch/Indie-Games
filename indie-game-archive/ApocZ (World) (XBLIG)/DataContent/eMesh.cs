using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

public class eMesh
{
	[ContentSerializer]
	public BoundingSphere BoundSphere;

	[ContentSerializer]
	public string Name;

	[ContentSerializer]
	public string MeshType;

	[ContentSerializer]
	public object Tag;

	[ContentSerializer]
	public List<eMesh> Children;

	[ContentSerializer]
	public List<eMeshPart> MeshParts;
}
