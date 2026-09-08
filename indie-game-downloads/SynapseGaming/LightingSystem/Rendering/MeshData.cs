using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using p;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Represents geometry data that can be shared between multiple
/// scene objects (similar to xna Model).
///
/// Generally loaded through the xna content manager.
/// </summary>
public class MeshData : IDisposable
{
	private VertexBuffer _3A_0018;

	private IndexBuffer _3AL;

	private Effect _3A_0019;

	[CompilerGenerated]
	private Matrix _3A3;

	[CompilerGenerated]
	private bool _3A6;

	[CompilerGenerated]
	private int _3AD;

	[CompilerGenerated]
	private int _3A_0017;

	[CompilerGenerated]
	private int _3A_0003;

	[CompilerGenerated]
	private BoundingSphere _3Al;

	[CompilerGenerated]
	private BoundingBox _3At;

	/// <summary>
	/// Object space transform of the mesh.
	/// </summary>
	public Matrix MeshToObject
	{
		[CompilerGenerated]
		get
		{
			return _3A3;
		}
		[CompilerGenerated]
		set
		{
			_3A3 = value;
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
			return _3A6;
		}
		[CompilerGenerated]
		set
		{
			_3A6 = value;
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
			return _3AD;
		}
		[CompilerGenerated]
		set
		{
			_3AD = value;
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
			return _3A_0017;
		}
		[CompilerGenerated]
		set
		{
			_3A_0017 = value;
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
			return _3A_0003;
		}
		[CompilerGenerated]
		set
		{
			_3A_0003 = value;
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
			return _3Al;
		}
		[CompilerGenerated]
		set
		{
			_3Al = value;
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
			return _3At;
		}
		[CompilerGenerated]
		set
		{
			_3At = value;
		}
	}

	/// <summary>
	/// VertexBuffer that contains the mesh geometry.
	/// </summary>
	public VertexBuffer VertexBuffer
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// IndexBuffer that contains the mesh geometry.
	/// </summary>
	public IndexBuffer IndexBuffer
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
		}
	}

	/// <summary>
	/// Effect applied to the mesh during rendering.
	/// </summary>
	public Effect Effect
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
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
		p._0018._6_0006(ref _3A_0018);
		p._0018._6_0006(ref _3AL);
		p._0018._6_0006(ref _3A_0019);
	}
}
