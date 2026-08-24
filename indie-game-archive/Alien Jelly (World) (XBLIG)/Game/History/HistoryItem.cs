using Microsoft.Xna.Framework;

namespace Game.History;

public class HistoryItem
{
	public enum Action
	{
		Nothing = -1,
		Move,
		Physics,
		Flip,
		Death,
		Exit,
		SetQBit,
		Property,
		Lasering,
		LaserCharging,
		Flag
	}

	public Action action;

	public IReversible subject;

	public HistoryItemData start;

	public HistoryItemData end;

	public HistoryItem()
	{
		action = Action.Nothing;
		subject = null;
		start = new HistoryItemData(-1, Vector3.Zero, Quaternion.Identity, Vector3.Zero, 0f, 0f, 0f, 0f, 0, pFlag: false, null);
		end = new HistoryItemData(-1, Vector3.Zero, Quaternion.Identity, Vector3.Zero, 0f, 0f, 0f, 0f, 0, pFlag: false, null);
	}

	public void Copy(ref HistoryItem oItem)
	{
		action = oItem.action;
		subject = oItem.subject;
		Data_Copy(ref oItem.start, ref start);
		Data_Copy(ref oItem.end, ref end);
	}

	public void Clear()
	{
		action = Action.Nothing;
		subject = null;
		Data_Clear(ref start);
		Data_Clear(ref end);
	}

	public void Data_Copy(ref HistoryItemData oFrom, ref HistoryItemData oTo)
	{
		oTo.time = oFrom.time;
		oTo.position = oFrom.position;
		oTo.rotation = oFrom.rotation;
		oTo.velocity = oFrom.velocity;
		oTo.value = oFrom.value;
		oTo.yaw = oFrom.yaw;
		oTo.pitch = oFrom.pitch;
		oTo.radius = oFrom.radius;
		oTo.item = oFrom.item;
		oTo.index = oFrom.index;
		oTo.flag = oFrom.flag;
	}

	public void Data_Clear(ref HistoryItemData oItem)
	{
		oItem.time = -1;
		oItem.position = Vector3.Zero;
		oItem.rotation = Quaternion.Identity;
		oItem.velocity = Vector3.Zero;
		oItem.value = 0f;
		oItem.pitch = 0f;
		oItem.yaw = 0f;
		oItem.radius = 0f;
		oItem.item = null;
		oItem.index = -1;
		oItem.flag = false;
	}
}
