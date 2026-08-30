using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;

namespace GKEngine.Lights;

public class Light : Base3D
{
	public enum LightTypes
	{
		Directional,
		Point,
		Spot
	}

	public LightTypes type;

	public Vector3 color;

	public bool active;

	protected Scene scene;

	public Light(Scene oScene)
	{
		scene = oScene;
		type = LightTypes.Directional;
		color = new Vector3(1f, 1f, 1f);
		active = true;
	}

	public void SetColor(byte xR, byte xG, byte xB)
	{
		color.X = (float)(int)xR / 255f;
		color.Y = (float)(int)xG / 255f;
		color.Z = (float)(int)xB / 255f;
	}
}
