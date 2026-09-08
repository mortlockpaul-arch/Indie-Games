using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public struct TriangleBatch
{
	public IndexedTriangle[] Triangles;

	public VertexDataBuffer Vertices;

	public TriangleBatch Clone()
	{
		TriangleBatch result = default(TriangleBatch);
		int num = Triangles.Length;
		result.Triangles = new IndexedTriangle[num];
		Triangles.CopyTo(result.Triangles, 0);
		result.Vertices = Vertices.Clone();
		return result;
	}

	public int GetMemoryUsage()
	{
		return Triangles.Length * 12 + (Vertices.ColorChannelCount * 4 + Vertices.TextureChannelCount * 8 + 48) * Vertices.Positions.Length;
	}
}
