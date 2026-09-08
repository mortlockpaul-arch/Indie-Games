using Microsoft.Xna.Framework.Graphics;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.Meshes;

public class SkyboxMesh : Cube
{
	public SkyboxMesh(string texture)
		: this(CubeTextureManager.Textures[texture])
	{
	}

	public SkyboxMesh(TextureCube cube)
	{
		base.Materials[0].Textures.Add(cube);
		base.Shader = ShaderManager.Shaders["Skybox"];
	}
}
