using Microsoft.Xna.Framework.Graphics;
using Quasar.Meshes;
using Quasar.Shaders;

namespace Quasar.Items;

public class Postprocess : RenderItem
{
	public Material Material => meshes[0].Materials[0];

	public Postprocess(Texture source)
		: this(source, ShaderManager.Shaders["PassThrough"])
	{
	}

	public Postprocess(Texture source, Shader shader)
	{
		addMesh(new PostprocessMesh(source)
		{
			Shader = shader
		});
	}

	public Postprocess(Texture source, string shaderFile)
		: this(source, ShaderManager.Shaders[shaderFile])
	{
	}
}
