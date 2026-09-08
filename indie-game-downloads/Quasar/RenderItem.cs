using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar;

public class RenderItem : Element
{
	protected List<Mesh> meshes = new List<Mesh>(1);

	public List<Mesh> Meshes => meshes;

	public Mesh Mesh
	{
		get
		{
			if (meshes.Count > 0)
			{
				return meshes[0];
			}
			return null;
		}
	}

	public Vector3 Diffuse
	{
		get
		{
			if (meshes.Count > 0)
			{
				return Mesh.Diffuse;
			}
			return Vector3.One;
		}
		set
		{
			if (meshes.Count > 0)
			{
				Mesh.Diffuse = value;
			}
		}
	}

	public Vector3 Ambient
	{
		get
		{
			if (meshes.Count > 0)
			{
				return Mesh.Ambient;
			}
			return Vector3.One;
		}
		set
		{
			if (meshes.Count > 0)
			{
				Mesh.Ambient = value;
			}
		}
	}

	public float Alpha
	{
		get
		{
			if (meshes.Count > 0)
			{
				return Mesh.Alpha;
			}
			return 1f;
		}
		set
		{
			if (meshes.Count > 0)
			{
				Mesh.Alpha = value;
			}
		}
	}

	public RenderItem()
	{
		DebugColor = Color.Red;
	}

	public RenderItem(Mesh m)
		: this()
	{
		addMesh(m);
	}

	public void addMesh(Mesh m)
	{
		meshes.Add(m);
	}

	public void removeMesh(Mesh m)
	{
		meshes.Remove(m);
	}

	public void clearMeshes()
	{
		meshes.Clear();
	}

	protected override void DoRender()
	{
		CompatHooks.MarkRenderItem(this);
		int count = meshes.Count;
		for (int i = 0; i < count; i++)
		{
			Mesh mesh = meshes[i];
			CompatHooks.RenderMeshSafely(mesh, transform, this);
		}
	}

	public override void Dispose()
	{
		if (meshes != null)
		{
			meshes.Clear();
			meshes = null;
		}
		base.Dispose();
	}
}
