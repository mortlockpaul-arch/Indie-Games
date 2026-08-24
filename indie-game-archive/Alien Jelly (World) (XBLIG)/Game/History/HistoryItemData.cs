using Microsoft.Xna.Framework;

namespace Game.History;

public struct HistoryItemData(int pTime, Vector3 pPosition, Quaternion pRotation, Vector3 pVelocity, float pValue, float pYaw, float pPitch, float pRadius, int pIndex, bool pFlag, object pItem)
{
	public int time = pTime;

	public Vector3 position = pPosition;

	public Quaternion rotation = pRotation;

	public Vector3 velocity = pVelocity;

	public float value = pValue;

	public float yaw = pYaw;

	public float pitch = pPitch;

	public float radius = pRadius;

	public int index = pIndex;

	public bool flag = pFlag;

	public object item = pItem;
}
