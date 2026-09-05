using Microsoft.Xna.Framework;

namespace Renderer;

public class RenderLight
{
	public Vector3 pos;

	public float falloff;

	public int range;

	public Color color;

	public RenderLight(Vector3 lightpos, float lightfalloff, int lightrange, Color lightColor)
	{
		pos = lightpos;
		falloff = lightfalloff;
		range = lightrange;
		color = lightColor;
	}

	public void Draw(TimeTracker gameTime)
	{
		SceneRenderer.AddLightToDraw(this);
	}
}
