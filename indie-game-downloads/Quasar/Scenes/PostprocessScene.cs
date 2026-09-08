using Microsoft.Xna.Framework.Graphics;
using Quasar.Elements.Cameras;
using Quasar.Items;

namespace Quasar.Scenes;

public class PostprocessScene : Scene
{
	private Postprocess postprocess;

	public Postprocess PostProcess => postprocess;

	public PostprocessScene()
	{
		base.Camera = new PostprocessCamera();
	}

	public PostprocessScene(Texture texture, string shader)
		: this()
	{
		postprocess = new Postprocess(texture, shader);
		Add(postprocess);
	}

	public PostprocessScene(Texture texture)
		: this()
	{
		postprocess = new Postprocess(texture);
		Add(postprocess);
	}

	public PostprocessScene(Texture texture, Shader shader)
		: this()
	{
		postprocess = new Postprocess(texture, shader);
		Add(postprocess);
	}
}
