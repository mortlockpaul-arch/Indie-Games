using Microsoft.Xna.Framework;

namespace Game.Data;

public class DataSkyRing
{
	public int type;

	public float radius;

	public Vector3 axis;

	public Vector3 position;

	public float rotation;

	public float height;

	public float speed;

	public string renderStack;

	public DataSkyRing()
	{
	}

	public DataSkyRing(int xType, float xRadius, Vector3 vAxis, Vector3 vPosition, float xRotation, float xHeight, float xSpeed, string xRenderStack)
	{
		type = xType;
		radius = xRadius;
		axis = vAxis;
		position = vPosition;
		rotation = xRotation;
		height = xHeight;
		speed = xSpeed;
		renderStack = xRenderStack;
	}
}
