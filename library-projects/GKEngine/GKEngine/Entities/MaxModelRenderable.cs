using GKEngine.Scenes;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class MaxModelRenderable : IRenderable
{
	private Matrix _render_matrix;

	public Scene scene;

	public MaxModel model;

	public GUID guid;

	public EntityStack renderStack;

	public MaxModelRenderable(Scene oScene, MaxModel oModel)
	{
		scene = oScene;
		model = oModel;
		guid = new GUID();
	}

	public void Init(Base3D oParent, string xRenderStack)
	{
		model.Build(oParent);
		renderStack = scene.RenderStacks_FromName(xRenderStack);
		renderStack.Add(guid.value, this);
	}

	public void Render(GameTime oGameTime)
	{
		_render_matrix = model.parent.matrix;
		model.Render(scene.cameras.camera);
	}

	public void Dispose()
	{
		if (renderStack != null)
		{
			renderStack.Remove(guid.value, this);
		}
		if (model != null)
		{
			model.Dispose();
			model = null;
		}
		scene = null;
		guid = null;
	}
}
