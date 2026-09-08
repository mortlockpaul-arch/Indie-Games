namespace EGEngine;

public class InventoryItemCls
{
	public ushort desc;

	public object item;

	public ushort ItemType
	{
		get
		{
			return (ushort)(desc & 0xFF);
		}
		set
		{
		}
	}
}
