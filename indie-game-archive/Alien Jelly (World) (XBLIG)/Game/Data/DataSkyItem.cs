using Microsoft.Xna.Framework;

namespace Game.Data;

public class DataSkyItem
{
	public int type;

	public Vector3 position;

	public float scale;

	public float rotation;

	public string renderStack;

	public DataSkyItem()
	{
	}

	public DataSkyItem(int xType, Vector3 vPosition, float xScale, float xRotation, string xRenderStack)
	{
		type = xType;
		position = vPosition;
		scale = xScale;
		rotation = xRotation;
		renderStack = xRenderStack;
	}
}
