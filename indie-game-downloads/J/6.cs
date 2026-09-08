using System.Runtime.CompilerServices;
using D;
using l;

namespace J;

internal class _6
{
	private l._7<_7> a5h = new l._7<_7>();

	[CompilerGenerated]
	private J._0006<D._0006> a5b;

	[CompilerGenerated]
	private J._0006<D.v> a56;

	[CompilerGenerated]
	private J._0006<D.Z> a5a;

	[CompilerGenerated]
	private J._0006<D._000F> a57;

	[CompilerGenerated]
	private J._0006<D.q> a5_0006;

	[CompilerGenerated]
	private J._0006<D.W> a5v;

	[CompilerGenerated]
	private J._0006<D._0002> a5B;

	[CompilerGenerated]
	private J._0006<D._000E> a5X;

	[CompilerGenerated]
	private J._0006<D.y> a5_0018;

	[CompilerGenerated]
	private J._0006<D.L> a5W;

	[CompilerGenerated]
	private J._0006<D.z> a5_0002;

	[CompilerGenerated]
	private J._0006<D.m> a5_000E;

	[CompilerGenerated]
	private J._0006<D.T> a5y;

	public J._0006<D._0006> BoxBox
	{
		[CompilerGenerated]
		get
		{
			return a5b;
		}
		[CompilerGenerated]
		private set
		{
			a5b = obj;
		}
	}

	public J._0006<D.v> BoxSphere
	{
		[CompilerGenerated]
		get
		{
			return a56;
		}
		[CompilerGenerated]
		private set
		{
			a56 = obj;
		}
	}

	public J._0006<D.Z> SphereSphere
	{
		[CompilerGenerated]
		get
		{
			return a5a;
		}
		[CompilerGenerated]
		private set
		{
			a5a = obj;
		}
	}

	public J._0006<D._000F> ConvexConvex
	{
		[CompilerGenerated]
		get
		{
			return a57;
		}
		[CompilerGenerated]
		private set
		{
			a57 = obj;
		}
	}

	public J._0006<D.q> TriangleConvex
	{
		[CompilerGenerated]
		get
		{
			return a5_0006;
		}
		[CompilerGenerated]
		private set
		{
			a5_0006 = obj;
		}
	}

	public J._0006<D.W> CompoundConvex
	{
		[CompilerGenerated]
		get
		{
			return a5v;
		}
		[CompilerGenerated]
		private set
		{
			a5v = obj;
		}
	}

	public J._0006<D._0002> CompoundCompound
	{
		[CompilerGenerated]
		get
		{
			return a5B;
		}
		[CompilerGenerated]
		private set
		{
			a5B = obj;
		}
	}

	public J._0006<D._000E> CompoundStaticMesh
	{
		[CompilerGenerated]
		get
		{
			return a5X;
		}
		[CompilerGenerated]
		private set
		{
			a5X = obj;
		}
	}

	public J._0006<D.y> CompoundTerrain
	{
		[CompilerGenerated]
		get
		{
			return a5_0018;
		}
		[CompilerGenerated]
		private set
		{
			a5_0018 = obj;
		}
	}

	public J._0006<D.L> StaticMeshConvex
	{
		[CompilerGenerated]
		get
		{
			return a5W;
		}
		[CompilerGenerated]
		private set
		{
			a5W = obj;
		}
	}

	public J._0006<D.z> StaticMeshSphere
	{
		[CompilerGenerated]
		get
		{
			return a5_0002;
		}
		[CompilerGenerated]
		private set
		{
			a5_0002 = obj;
		}
	}

	public J._0006<D.m> TerrainConvex
	{
		[CompilerGenerated]
		get
		{
			return a5_000E;
		}
		[CompilerGenerated]
		private set
		{
			a5_000E = obj;
		}
	}

	public J._0006<D.T> TerrainSphere
	{
		[CompilerGenerated]
		get
		{
			return a5y;
		}
		[CompilerGenerated]
		private set
		{
			a5y = obj;
		}
	}

	public l.X<_7> All => new l.X<_7>(a5h);

	public _6()
	{
		a5h.Add(BoxBox = new J._0006<D._0006>());
		a5h.Add(BoxSphere = new J._0006<D.v>());
		a5h.Add(SphereSphere = new J._0006<D.Z>());
		a5h.Add(ConvexConvex = new J._0006<D._000F>());
		a5h.Add(TriangleConvex = new J._0006<D.q>());
		a5h.Add(CompoundConvex = new J._0006<D.W>());
		a5h.Add(CompoundCompound = new J._0006<D._0002>());
		a5h.Add(CompoundStaticMesh = new J._0006<D._000E>());
		a5h.Add(CompoundTerrain = new J._0006<D.y>());
		a5h.Add(StaticMeshConvex = new J._0006<D.L>());
		a5h.Add(StaticMeshSphere = new J._0006<D.z>());
		a5h.Add(TerrainConvex = new J._0006<D.m>());
		a5h.Add(TerrainSphere = new J._0006<D.T>());
	}
}
