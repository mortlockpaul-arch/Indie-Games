using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Render;
using Quasar.Shaders;

namespace Quasar;

public abstract class Mesh : IDisposable
{
	public const string MESH_FOLDER = "Models/";

	public Color DebugColor = Color.Cyan;

	protected List<Material> materials = new List<Material>(1);

	protected Shader shader;

	protected BoundingSphere boundingSphere = new BoundingSphere(Vector3.Zero, 0f);

	protected bool boundingSphereIgnoreTransform;

	public List<Material> Materials => materials;

	public Material FirstMaterial => materials[0];

	public Vector3 Diffuse
	{
		get
		{
			return materials[0].Diffuse;
		}
		set
		{
			materials[0].Diffuse = value;
		}
	}

	public Vector3 Ambient
	{
		get
		{
			return materials[0].Ambient;
		}
		set
		{
			materials[0].Ambient = value;
		}
	}

	public float Alpha
	{
		get
		{
			return materials[0].Alpha;
		}
		set
		{
			materials[0].Alpha = value;
		}
	}

	public Texture Texture
	{
		get
		{
			return materials[0].Texture;
		}
		set
		{
			materials[0].Texture = value;
		}
	}

	public Shader Shader
	{
		get
		{
			return shader;
		}
		set
		{
			shader = value;
		}
	}

	public BoundingSphere BoundingSphere => boundingSphere;

	public bool IsBoundingSphereValid => boundingSphere.Radius != -1f;

	public abstract void Render(Transform motion);

	public abstract void RenderMaterial(Transform motion, int material);

	protected Shader PrepareShader(Shader shader, Transform motion, Material material)
	{
		if (SceneRenderData.CurrentRenderData.HasForcedTechnique && shader.HasTechnique(SceneRenderData.CurrentRenderData.ForcedTechnique))
		{
			shader.BeginRender(motion, material, SceneRenderData.CurrentRenderData.ForcedTechnique);
			return SceneRenderData.CurrentRenderData.ForcedShader;
		}
		if (SceneRenderData.CurrentRenderData.HasForcedShader)
		{
			SceneRenderData.CurrentRenderData.ForcedShader.BeginRender(motion, material);
			return SceneRenderData.CurrentRenderData.ForcedShader;
		}
		shader.BeginRender(motion, material);
		return shader;
	}

	public Mesh(Shader shader)
	{
		boundingSphere.Radius = -1f;
		this.shader = shader;
	}

	public void ClearMaterials()
	{
		materials.Clear();
	}

	public Mesh()
	{
		shader = ShaderManager.Shaders["Base"];
	}

	public virtual void Dispose()
	{
		shader = null;
		if (materials != null)
		{
			materials.Clear();
		}
		materials = null;
		GC.SuppressFinalize(this);
	}

	~Mesh()
	{
		Dispose();
	}

	protected virtual Mesh DoClone()
	{
		return null;
	}

	public Mesh Clone()
	{
		Mesh mesh = DoClone();
		if (mesh != null)
		{
			CopyMeshData(mesh);
		}
		return mesh;
	}

	protected virtual void CopyMeshData(Mesh mesh)
	{
		mesh.shader = shader;
		mesh.boundingSphere = boundingSphere;
		mesh.ClearMaterials();
		foreach (Material material in materials)
		{
			mesh.Materials.Add(material.Clone());
		}
	}
}
