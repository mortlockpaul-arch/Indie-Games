using Microsoft.Xna.Framework;

namespace Game.Data;

public class DataAtom
{
	public string definition;

	public string guid;

	public Vector3 point;

	public Vector4 data;

	public Quaternion rotation;

	public int state;

	public int[] properties;

	public string[] children;

	public DataKeyFrame[] focus;

	public DataAtom()
	{
	}

	public DataAtom(string xDefinition, string xGuid, Vector3 oPoint, Vector4 xData, Quaternion xRotation, int xState, int[] aProperties, string[] aChildren, DataKeyFrame[] aFocus)
	{
		definition = xDefinition;
		guid = xGuid;
		point = oPoint;
		data = xData;
		rotation = xRotation;
		state = xState;
		properties = aProperties;
		children = aChildren;
		focus = aFocus;
	}
}
