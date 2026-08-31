using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Models;

/// <summary>
/// Helper class for rendering raw XNA Models.
/// </summary>
public class ModelRenderHelper
{
	/// <summary>
	/// Renders an XNA ModelMesh using the currently assigned
	/// effect and render states (not the Model's).  Allows
	/// rendering XNA Models as raw geometry.
	/// </summary>
	/// <param name="device">Current graphics device.</param>
	/// <param name="mesh">ModelMesh to render.</param>
	public static void Render(GraphicsDevice device, ModelMesh mesh)
	{
		for (int i = 0; i < mesh.MeshParts.Count; i++)
		{
			ModelMeshPart modelMeshPart = mesh.MeshParts[i];
			device.SetVertexBuffer(modelMeshPart.VertexBuffer);
			device.Indices = modelMeshPart.IndexBuffer;
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, modelMeshPart.VertexOffset, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount);
		}
	}
}
