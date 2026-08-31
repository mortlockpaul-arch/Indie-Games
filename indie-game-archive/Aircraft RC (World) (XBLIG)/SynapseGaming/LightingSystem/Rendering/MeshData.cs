using System;
using System.Runtime.CompilerServices;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Represents geometry data that can be shared between multiple
/// scene objects (similar to xna Model).
///
/// Generally loaded through the xna content manager.
/// </summary>
public class MeshData : IDisposable
{
	private VertexBuffer HCB;

	private IndexBuffer HC_0002;

	private Effect HC_0012;

	[CompilerGenerated]
	private Matrix HCH;

	[CompilerGenerated]
	private bool HC7;

	[CompilerGenerated]
	private int HC_0001;

	[CompilerGenerated]
	private int HCw;

	[CompilerGenerated]
	private int HCZ;

	[CompilerGenerated]
	private BoundingSphere HC_000F;

	[CompilerGenerated]
	private BoundingBox HCy;

	/// <summary>
	/// Object space transform of the mesh.
	/// </summary>
	public Matrix MeshToObject
	{
		[CompilerGenerated]
		get
		{
			return HCH;
		}
		[CompilerGenerated]
		set
		{
			HCH = value;
		}
	}

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public bool InfiniteBounds
	{
		[CompilerGenerated]
		get
		{
			return HC7;
		}
		[CompilerGenerated]
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Number of primitives in the mesh geometry.
	/// </summary>
	public int PrimitiveCount
	{
		[CompilerGenerated]
		get
		{
			return HC_0001;
		}
		[CompilerGenerated]
		set
		{
			HC_0001 = value;
		}
	}

	/// <summary>
	/// Number of vertices in the vertex buffer range required to draw the mesh.
	/// For instance, a quad rendering vertices at indices (2, 5, 6, 9) requires
	/// a vertex buffer range of 8 vertices (vertices 2 – 9 inclusive).
	/// </summary>
	public int VertexCount
	{
		[CompilerGenerated]
		get
		{
			return HCw;
		}
		[CompilerGenerated]
		set
		{
			HCw = value;
		}
	}

	/// <summary>
	/// Size in bytes of the elements in the vertex buffer.
	/// </summary>
	public int VertexStride
	{
		[CompilerGenerated]
		get
		{
			return HCZ;
		}
		[CompilerGenerated]
		set
		{
			HCZ = value;
		}
	}

	/// <summary>
	/// Object-space bounding area that completely contains the mesh.
	/// </summary>
	public BoundingSphere ObjectSpaceBoundingSphere
	{
		[CompilerGenerated]
		get
		{
			return HC_000F;
		}
		[CompilerGenerated]
		set
		{
			HC_000F = value;
		}
	}

	/// <summary>
	/// Object-space bounding area that completely contains the mesh.
	/// </summary>
	public BoundingBox ObjectSpaceBoundingBox
	{
		[CompilerGenerated]
		get
		{
			return HCy;
		}
		[CompilerGenerated]
		set
		{
			HCy = value;
		}
	}

	/// <summary>
	/// VertexBuffer that contains the mesh geometry.
	/// </summary>
	public VertexBuffer VertexBuffer
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// IndexBuffer that contains the mesh geometry.
	/// </summary>
	public IndexBuffer IndexBuffer
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// Effect applied to the mesh during rendering.
	/// </summary>
	public Effect Effect
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Creates a new MeshData instance.
	/// </summary>
	public MeshData()
	{
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		F.B._7_0004(ref HCB);
		F.B._7_0004(ref HC_0002);
		F.B._7_0004(ref HC_0012);
	}
}
