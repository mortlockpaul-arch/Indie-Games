using Quasar.Elements.Cameras;

namespace Quasar.Scenes;

public class Scene2D : Scene
{
	public Camera2D Camera2D => (Camera2D)base.Camera;

	public Scene2D()
	{
		base.Camera = new StaticCamera2D();
	}
}
