using Microsoft.Xna.Framework;

namespace Game.Data;

public class DataKeyFrame
{
	public Vector3 position;

	public Quaternion rotation;

	public DataKeyFrame()
	{
	}

	public DataKeyFrame(Vector3 oPosition, Quaternion oRotation)
	{
		position = oPosition;
		rotation = oRotation;
	}
}
