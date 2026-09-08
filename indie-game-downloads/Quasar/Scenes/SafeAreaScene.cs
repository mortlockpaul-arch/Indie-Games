using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Meshes;

namespace Quasar.Scenes;

public class SafeAreaScene : Scene2D
{
	private const float SAFE_X = 0.8f;

	private const float SAFE_Y = 0.8f;

	public SafeAreaScene()
	{
		float num = 0.099999994f * Engine.GUIWidth;
		float num2 = 0.099999994f * Engine.GUIHeight;
		_ = Engine.Device.Viewport.TitleSafeArea;
		RenderItem renderItem = new RenderItem();
		renderItem.addMesh(new Sized2DRectangleMesh(new Vector2(num, Engine.GUIHeight), GameMath.RGBAToVector(0, 0, 0, 80))
		{
			Offset = new Vector2((0f - (Engine.GUIWidth - num)) * 0.5f, 0f)
		});
		renderItem.addMesh(new Sized2DRectangleMesh(new Vector2(num, Engine.GUIHeight), GameMath.RGBAToVector(0, 0, 0, 80))
		{
			Offset = new Vector2((Engine.GUIWidth - num) * 0.5f, 0f)
		});
		renderItem.addMesh(new Sized2DRectangleMesh(new Vector2(Engine.GUIWidth - 2f * num, num2), GameMath.RGBAToVector(0, 0, 0, 80))
		{
			Offset = new Vector2(0f, (Engine.GUIHeight - num2) * 0.5f)
		});
		renderItem.addMesh(new Sized2DRectangleMesh(new Vector2(Engine.GUIWidth - 2f * num, num2), GameMath.RGBAToVector(0, 0, 0, 80))
		{
			Offset = new Vector2(0f, (0f - (Engine.GUIHeight - num2)) * 0.5f)
		});
		Add(renderItem);
	}
}
