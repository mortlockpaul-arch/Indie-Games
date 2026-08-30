using GKEngine.Scenes;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class MaxModelPartRenderable : IRenderable
{
	private Matrix _render_matrix;

	public Scene scene;

	public Base3D anchor;

	public MaxModelPart part;

	public GUID guid;

	public MaxModelPartRenderable(Scene oScene, Base3D oAnchor, MaxModelPart oPart)
	{
		scene = oScene;
		anchor = oAnchor;
		part = oPart;
		guid = new GUID();
	}

	public void Render(GameTime oGameTime)
	{
		_render_matrix = anchor.matrix;
		part.Render(ref _render_matrix, scene.cameras.camera);
	}

	public void Dispose()
	{
		if (part != null)
		{
			part.Dispose();
			part = null;
		}
		scene = null;
		anchor = null;
		guid = null;
	}
}
