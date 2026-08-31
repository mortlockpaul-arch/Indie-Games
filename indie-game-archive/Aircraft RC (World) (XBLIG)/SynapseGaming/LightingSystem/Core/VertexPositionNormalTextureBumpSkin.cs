using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Describes a SunBurn compatible vertex format structure that contains position, normal data, one set of texture coordinates,
/// tangent space information used in bump / specular mapping, and skinning information.
/// </summary>
public struct VertexPositionNormalTextureBumpSkin : IVertexType
{
	/// <summary>
	/// The vertex position.
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// The vertex normal.
	/// </summary>
	public Vector3 Normal;

	/// <summary>
	/// The texture coordinates.
	/// </summary>
	public Vector2 TextureCoordinate;

	/// <summary>
	/// Tangent space tangent element used in bump / specular mapping.
	/// </summary>
	public Vector3 Tangent;

	/// <summary>
	/// Tangent space binormal element used in bump / specular mapping.
	/// </summary>
	public Vector3 Binormal;

	/// <summary>
	/// Index used during skinning to lookup the meshToObject transform from a bone
	/// transform array given to an effect or render manager for rendering.
	/// </summary>
	public byte BoneIndex3;

	/// <summary>
	/// Index used during skinning to lookup the meshToObject transform from a bone
	/// transform array given to an effect or render manager for rendering.
	/// </summary>
	public byte BoneIndex2;

	/// <summary>
	/// Index used during skinning to lookup the meshToObject transform from a bone
	/// transform array given to an effect or render manager for rendering.
	/// </summary>
	public byte BoneIndex1;

	/// <summary>
	/// Index used during skinning to lookup the meshToObject transform from a bone
	/// transform array given to an effect or render manager for rendering.
	/// </summary>
	public byte BoneIndex0;

	/// <summary>
	/// Weights used to blend between the transforms assigned via bone indices 0 - 3.
	/// </summary>
	public Vector4 BoneWeights;

	/// <summary>
	/// An array of vertex elements describing this vertex.
	/// </summary>
	public static readonly VertexElement[] VertexElements;

	/// <summary>
	/// Vertex declaration, which defines per-vertex data.
	/// </summary>
	public static readonly VertexDeclaration VertexDeclaration;

	/// <summary>
	/// Gets the size of this structure.
	/// </summary>
	public static int SizeInBytes => 76;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	static VertexPositionNormalTextureBumpSkin()
	{
		VertexElements = new VertexElement[7]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
			new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
			new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
			new VertexElement(56, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
			new VertexElement(60, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0)
		};
		VertexDeclaration = new VertexDeclaration(VertexElements)
		{
			Name = "VertexPositionNormalTextureBumpSkin.VertexDeclaration"
		};
	}

	/// <summary>
	/// Generates tangent space data (used for bump and specular mapping) from the provided vertex information.
	///
	/// The vertex position, normal, and texture uv need to be set with valid info prior to calling this method.
	/// </summary>
	/// <param name="indices">Indices that describe a list of triangles to generate tangent space
	/// information for.  WARNING: this method requires triangle lists (not fans or strips).</param>
	/// <param name="vertices">Array of vertices.</param>
	public static void BuildTangentSpaceDataForTriangleList(short[] indices, VertexPositionNormalTextureBumpSkin[] vertices)
	{
		for (int i = 0; i < indices.Length; i += 3)
		{
			int num = indices[i];
			int num2 = indices[i + 1];
			int num3 = indices[i + 2];
			Vector2 textureCoordinate = vertices[num].TextureCoordinate;
			Vector2 textureCoordinate2 = vertices[num2].TextureCoordinate;
			Vector2 textureCoordinate3 = vertices[num3].TextureCoordinate;
			float num4 = textureCoordinate2.X - textureCoordinate.X;
			float num5 = textureCoordinate3.X - textureCoordinate.X;
			float num6 = textureCoordinate2.Y - textureCoordinate.Y;
			float num7 = textureCoordinate3.Y - textureCoordinate.Y;
			float num8 = num4 * num7 - num5 * num6;
			if (num8 != 0f)
			{
				num8 = 1f / num8;
				Vector3 position = vertices[num].Position;
				Vector3 position2 = vertices[num2].Position;
				Vector3 position3 = vertices[num3].Position;
				float num9 = position2.X - position.X;
				float num10 = position3.X - position.X;
				float num11 = position2.Y - position.Y;
				float num12 = position3.Y - position.Y;
				float num13 = position2.Z - position.Z;
				float num14 = position3.Z - position.Z;
				Vector3 vector = new Vector3((num7 * num9 - num6 * num10) * num8, (num7 * num11 - num6 * num12) * num8, (num7 * num13 - num6 * num14) * num8);
				Vector3 vector2 = new Vector3((num4 * num10 - num5 * num9) * num8, (num4 * num12 - num5 * num11) * num8, (num4 * num14 - num5 * num13) * num8);
				vertices[num].Tangent += vector;
				vertices[num2].Tangent += vector;
				vertices[num3].Tangent += vector;
				vertices[num].Binormal += vector2;
				vertices[num2].Binormal += vector2;
				vertices[num3].Binormal += vector2;
			}
		}
		for (int j = 0; j < vertices.Length; j++)
		{
			Vector3 tangent = vertices[j].Tangent;
			if (!(tangent == Vector3.Zero))
			{
				vertices[j].Tangent = Vector3.Normalize(tangent);
				vertices[j].Binormal = Vector3.Normalize(vertices[j].Binormal);
			}
		}
	}
}
