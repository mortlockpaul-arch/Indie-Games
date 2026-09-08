using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class ParallaxMesh : UserVertexMesh<VertexPosition>
{
	public const string PARALLAX_SHADER = "Parallax";

	private bool dirty;

	private Vector2 size;

	private List<ParallaxBackground> backgrounds = new List<ParallaxBackground>();

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
			dirty = true;
		}
	}

	public Vector2 BackgroundSize
	{
		set
		{
			base.FirstMaterial.SetVector4Parameter(0, new Vector4(value, 0f, 0f));
		}
	}

	private void initializeBuffer()
	{
		initMesh(4);
		fillBuffer();
	}

	private void fillBuffer()
	{
		Vector2 vector = size / 2f;
		verticesBuffer[0].Position = new Vector3(0f - vector.X, vector.Y, 0f);
		verticesBuffer[1].Position = new Vector3(vector.X, vector.Y, 0f);
		verticesBuffer[2].Position = new Vector3(0f - vector.X, 0f - vector.Y, 0f);
		verticesBuffer[3].Position = new Vector3(vector.X, 0f - vector.Y, 0f);
		updateBoundingSphere(4);
		dirty = false;
	}

	public void addParallax(ParallaxBackground data)
	{
		backgrounds.Add(data);
		updateMaterials();
	}

	private void updateMaterials()
	{
		for (int i = base.FirstMaterial.Vector4ArrayParameterCount; i < backgrounds.Count; i++)
		{
			Vector4[] array = new Vector4[3];
			ParallaxBackground parallaxBackground = backgrounds[i];
			ref Vector4 reference = ref array[0];
			reference = new Vector4(parallaxBackground.scale, parallaxBackground.offset.X, parallaxBackground.offset.Y);
			ref Vector4 reference2 = ref array[1];
			reference2 = new Vector4(parallaxBackground.minUV, parallaxBackground.maxUV.X, parallaxBackground.maxUV.Y);
			ref Vector4 reference3 = ref array[2];
			reference3 = new Vector4(parallaxBackground.speed, parallaxBackground.texture.Width, parallaxBackground.texture.Height);
			base.FirstMaterial.AddVector4ArrayParameter(array);
			base.FirstMaterial.Textures.Add(parallaxBackground.texture);
		}
	}

	public ParallaxMesh(Vector2 size, Texture2D backgroundTexture)
	{
		this.size = size;
		initializeBuffer();
		Material material = new Material();
		shader = ShaderManager.Shaders["Parallax"];
		material.AddVector4Parameter(new Vector4(backgroundTexture.Width, backgroundTexture.Height, 0f, 0f));
		material.Textures.Add(backgroundTexture);
		materials.Add(material);
	}

	public override void Render(Transform motion)
	{
		if (backgrounds.Count != 0)
		{
			base.FirstMaterial.Technique = "Parallax" + backgrounds.Count;
			if (dirty)
			{
				fillBuffer();
			}
			base.Render(motion);
		}
	}
}
